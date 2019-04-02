using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalJudge.Core.Helpers;
using LocalJudge.Core.Judgers;
using LocalJudge.Core.Submissions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocalJudge.Server.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        const string PipeStreamName = "LocalJudger.Server.Judger";

        public class SubmitData
        {
            public string ProblemId { get; set; }

            public string UserId { get; set; }

            public string Code { get; set; }

            public ProgrammingLanguage Language { get; set; }
        }

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
        public ActionResult<SubmissionMetadata> Submit([FromBody] SubmitData data)
        {
            if (Program.Workspace.Problems.Has(data.ProblemId) == false)
                return NotFound();

            if (Program.Workspace.Users.Has(data.UserId) == false)
                return NotFound();

            if (data.Code == null) data.Code = string.Empty;
            var meta = new SubmissionMetadata
            {
                ProblemId = data.ProblemId,
                UserId = data.UserId,
                Language = data.Language,
                Time = DateTimeOffset.Now,
                CodeLength = (uint)Encoding.UTF8.GetByteCount(data.Code),
                CodePath = GetCodePath(data.Language),
            };
            var sub = Program.Workspace.Submissions.Create(Guid.NewGuid().ToString(), meta);
            if (sub == null) return Forbid();
            try
            {
                sub.SaveCode(data.Code);
                SendJudgeRequest(sub.Id);
                return Created($"submissions/{meta.Id}", sub.GetMetadata());
            }
            catch
            {
                Program.Workspace.Submissions.Delete(sub.Id);
                return Forbid();
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<SubmissionMetadata>> GetAll()
        {
            return Ok(Program.Workspace.Submissions.GetAll().Select(item => item.GetMetadata()));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<SubmissionMetadata> Get(string id)
        {
            var res = Program.Workspace.Submissions.Get(id)?.GetMetadata();
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
        public ActionResult Rejudge(string id)
        {
            var res = Program.Workspace.Submissions.Get(id);
            if (res != null)
            {
                res.ClearResult();
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
        public ActionResult<SubmissionResult> GetResult(string id)
        {
            var res = Program.Workspace.Submissions.Get(id)?.GetResult();
            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpGet("{id}/code")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<string> GetCode(string id)
        {
            var res = Program.Workspace.Submissions.Get(id)?.GetCode();
            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpGet("{id}/codefile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<byte[]> GetCodeFile(string id)
        {
            var res = Program.Workspace.Submissions.Get(id)?.GetCodePath();
            if (res != null)
                return Ok(System.IO.File.ReadAllBytes(res));
            else
                return NotFound();
        }

        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            Program.Workspace.Submissions.Delete(id);
        }
    }
}