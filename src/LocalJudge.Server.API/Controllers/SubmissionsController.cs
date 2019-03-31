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
            public string ProblemID { get; set; }

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
            if (Program.Workspace.Problems.Has(data.ProblemID) == false)
                return NotFound();

            if (data.Code == null) data.Code = String.Empty;
            var meta = new SubmissionMetadata
            {
                ProblemID = data.ProblemID,
                Language = data.Language,
                Time = DateTimeOffset.Now,
                CodeLength = (uint)Encoding.UTF8.GetByteCount(data.Code),
                CodePath = GetCodePath(data.Language),
            };
            var sub = Program.Workspace.Submissions.Create(Guid.NewGuid().ToString(), meta);
            if (sub == null) return Forbid();
            try
            {
                TextIO.WriteAllInUTF8(sub.GetCodePath(), data.Code);
                SendJudgeRequest(sub.ID);
                return Created($"submissions/{meta.ID}", meta);
            }
            catch
            {
                Program.Workspace.Submissions.Delete(sub.ID);
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
                System.IO.File.Delete(res.Result);
                try
                {
                    SendJudgeRequest(res.ID);
                    return Accepted();
                }
                catch
                {
                    Program.Workspace.Submissions.Delete(res.ID);
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

        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            Program.Workspace.Submissions.Delete(id);
        }
    }
}