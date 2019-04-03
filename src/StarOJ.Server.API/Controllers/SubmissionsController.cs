using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarOJ.Core;
using StarOJ.Core.Helpers;
using StarOJ.Core.Judgers;
using StarOJ.Core.Submissions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StarOJ.Server.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        const string PipeStreamName = "StarOJ.Server.Judger";
        private readonly IWorkspace _workspace;

        public SubmissionsController(IWorkspace workspace)
        {
            _workspace = workspace;
        }

        public class SubmitData
        {
            public string ProblemId { get; set; }

            public string UserId { get; set; }

            public string Code { get; set; }

            public ProgrammingLanguage Language { get; set; }
        }

        // TODO
        static string GetCodePath(ProgrammingLanguage lang)
        {
            switch (lang)
            {
                case ProgrammingLanguage.Java:
                    return $"Main.java";
                default:
                    return $"code.{ProgrammingLanguageHelper.Extends[lang]}";
            }
        }

        void SendJudgeRequest(string id)
        {
            using (var pipe = new NamedPipeClientStream(".", PipeStreamName, PipeDirection.Out))
            {
                pipe.Connect(10 * 1000);// Wait for 10s
                var buffer = Encoding.UTF8.GetBytes(id);
                pipe.Write(buffer, 0, buffer.Length);
                pipe.WaitForPipeDrain();
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<SubmissionMetadata>> Submit([FromBody] SubmitData data)
        {
            if (await _workspace.Problems.Has(data.ProblemId) == false)
                return NotFound();

            if (await _workspace.Users.Has(data.UserId) == false)
                return NotFound();

            if (data.Code == null) data.Code = string.Empty;
            var meta = new SubmissionMetadata
            {
                Id = Guid.NewGuid().ToString(),
                ProblemId = data.ProblemId,
                UserId = data.UserId,
                Language = data.Language,
                Time = DateTimeOffset.Now,
                CodeLength = (uint)Encoding.UTF8.GetByteCount(data.Code),
                Code = data.Code,
            };
            var sub = await _workspace.Submissions.Create(meta);
            if (sub == null) return Forbid();
            try
            {
                SendJudgeRequest(sub.Id);
                return Created($"submissions/{meta.Id}", await sub.GetMetadata());
            }
            catch
            {
                await _workspace.Submissions.Delete(sub.Id);
                return Forbid();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubmissionMetadata>>> GetAll()
        {
            var all = await _workspace.Submissions.GetAll();
            List<SubmissionMetadata> ans = new List<SubmissionMetadata>();
            foreach (var v in all)
            {
                ans.Add(await v.GetMetadata());
            }
            return Ok(ans);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<SubmissionMetadata>> Get(string id)
        {
            var res = await (await _workspace.Submissions.Get(id))?.GetMetadata();
            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Rejudge(string id)
        {
            var res = await _workspace.Submissions.Get(id);
            if (res != null)
            {
                await res.SetResult(null);
                try
                {
                    SendJudgeRequest(res.Id);
                    return Accepted();
                }
                catch
                {
                    return Forbid();
                }
            }
            else
                return NotFound();
        }

        [HttpGet("{id}/result")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<SubmissionResult>> GetResult(string id)
        {
            var res = await(await _workspace.Submissions.Get(id))?.GetResult();
            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpDelete("{id}")]
        public Task Delete(string id)
        {
            return _workspace.Submissions.Delete(id);
        }
    }
}