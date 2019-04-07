using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarOJ.Core.Helpers;
using StarOJ.Core.Judgers;
using StarOJ.Core.Submissions;
using StarOJ.Data.Provider;
using StarOJ.Server.API.Models;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

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

        void SendJudgeRequest(string id)
        {
            using (NamedPipeClientStream pipe = new NamedPipeClientStream(".", PipeStreamName, PipeDirection.Out))
            {
                pipe.Connect(10 * 1000);// Wait for 10s
                byte[] buffer = Encoding.UTF8.GetBytes(id);
                pipe.Write(buffer, 0, buffer.Length);
                pipe.WaitForPipeDrain();
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<SubmissionMetadata>> Submit([FromBody] SubmitData data)
        {
            if (await _workspace.Problems.Has(data.ProblemId) == false)
                return NotFound();

            if (await _workspace.Users.Has(data.UserId) == false)
                return NotFound();

            SubmissionMetadata meta = new SubmissionMetadata
            {
                Id = Guid.NewGuid().ToString(),
                ProblemId = data.ProblemId,
                UserId = data.UserId,
                Language = data.Language,
                Time = DateTimeOffset.Now,
            };
            ISubmissionProvider sub = await _workspace.Submissions.Create(meta);
            if (sub == null) return Forbid();
            try
            {
                if (data.CodeFile != null)
                {
                    using (System.IO.Stream s = data.CodeFile.OpenReadStream())
                    {
                        meta.CodeLength = (uint)s.Length;
                        await sub.SetCode(s);
                    }
                }
                else
                {
                    meta.CodeLength = (uint)Encoding.UTF8.GetByteCount(data.Code);
                    using (System.IO.Stream ms = TextIO.ToStream(data.Code ?? ""))
                        await sub.SetCode(ms);
                }
                await sub.SetMetadata(meta);
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
            IEnumerable<ISubmissionProvider> all = await _workspace.Submissions.GetAll();
            List<SubmissionMetadata> ans = new List<SubmissionMetadata>();
            foreach (ISubmissionProvider v in all)
            {
                ans.Add(await v.GetMetadata());
            }
            return Ok(ans);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubmissionMetadata>> Get(string id)
        {
            SubmissionMetadata res = await (await _workspace.Submissions.Get(id))?.GetMetadata();
            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpGet("query")]
        public async Task<ActionResult<IEnumerable<SubmissionMetadata>>> Query(string id, string problemId, string userId, ProgrammingLanguage? language, JudgeState? state)
        {
            IEnumerable<ISubmissionProvider> all = await _workspace.Submissions.Query(id, problemId, userId, language, state);
            List<SubmissionMetadata> ans = new List<SubmissionMetadata>();
            foreach (ISubmissionProvider v in all)
            {
                ans.Add(await v.GetMetadata());
            }
            return ans;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<ActionResult> Rejudge(string id)
        {
            ISubmissionProvider res = await _workspace.Submissions.Get(id);
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
        public async Task<ActionResult<SubmissionResult>> GetResult(string id)
        {
            SubmissionResult res = await (await _workspace.Submissions.Get(id))?.GetResult();
            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpPut("{id}/result")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<ActionResult> SetResult(string id, [FromBody] SubmissionResult value)
        {
            ISubmissionProvider res = await _workspace.Submissions.Get(id);
            if (res != null)
            {
                await res.SetResult(value);
                return Accepted();
            }
            else
                return NotFound();
        }

        [HttpGet("{id}/code")]
        public async Task<ActionResult> GetCode(string id)
        {
            ISubmissionProvider res = await _workspace.Submissions.Get(id);
            if (res == null) return NotFound();
            System.IO.Stream code = await res.GetCode();
            ProgrammingLanguage lang = (await res.GetMetadata()).Language;
            return File(code, "text/plain", $"{id}.{ProgrammingLanguageHelper.Extends[lang]}");
        }

        [HttpDelete("{id}")]
        public Task Delete(string id)
        {
            return _workspace.Submissions.Delete(id);
        }
    }
}