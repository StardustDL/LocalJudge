﻿using System;
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public ActionResult<SubmissionMetadata> Submit([FromBody] SubmitData data)
        {
            var meta = new SubmissionMetadata
            {
                ProblemID = data.ProblemID,
                Language = data.Language,
            };
            var sub = Program.Workspace.Submissions.Create(Guid.NewGuid().ToString(), meta);
            if (sub == null) return Forbid();
            try
            {
                TextIO.WriteAllInUTF8(sub.Code, data.Code);
                using (var pipe = new NamedPipeClientStream(".", PipeStreamName, PipeDirection.Out))
                {
                    pipe.Connect(10 * 1000);// Wait for 10s
                    var buffer = Encoding.UTF8.GetBytes(sub.ID);
                    pipe.Write(buffer, 0, buffer.Length);
                    pipe.WaitForPipeDrain();
                }
                return Created($"submissions/{meta.ID}", meta);
            }
            catch
            {
                Program.Workspace.Submissions.Delete(sub.ID);
                return Forbid();
            }
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
        public ActionResult<SubmissionResult> GetCode(string id)
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