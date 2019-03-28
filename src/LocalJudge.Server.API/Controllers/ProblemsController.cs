using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocalJudge.Core;
using LocalJudge.Core.Problems;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocalJudge.Server.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProblemsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<ProblemMetadata>> Get()
        {
            return Ok(Program.Workspace.Problems.GetAll().Select(item => item.GetMetadata()));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<ProblemMetadata> Get(string id)
        {
            var res = Program.Workspace.Problems.Get(id)?.GetMetadata();
            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpGet("{id}/description")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<ProblemDescription> GetDescription(string id)
        {
            var res = Program.Workspace.Problems.Get(id);
            if (res != null)
                return Ok(res.GetDescription());
            else
                return NotFound();
        }

        [HttpGet("{id}/samples")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<TestCaseMetadata>> GetSamples(string id)
        {
            var res = Program.Workspace.Problems.Get(id);
            if (res != null)
                return Ok(res.GetSamples().Select(item => item.GetMetadata()));
            else
                return NotFound();
        }

        [HttpGet("{id}/samples/{tid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<TestCaseMetadata> GetSample(string id, string tid)
        {
            var res = Program.Workspace.Problems.Get(id)?.GetSample(tid);
            if (res != null)
                return Ok(res.GetMetadata());
            else
                return NotFound();
        }

        [HttpGet("{id}/samples/{tid}/input")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<TestCaseMetadata> GetSampleInput(string id, string tid)
        {
            var res = Program.Workspace.Problems.Get(id)?.GetSample(tid);
            if (res != null)
                return Ok(res.GetInput());
            else
                return NotFound();
        }

        [HttpGet("{id}/samples/{tid}/output")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<string> GetSampleOutput(string id, string tid)
        {
            var res = Program.Workspace.Problems.Get(id)?.GetSample(tid);
            if (res != null)
                return Ok(res.GetOutput());
            else
                return NotFound();
        }

        [HttpGet("{id}/tests")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<TestCaseMetadata>> GetTests(string id)
        {
            var res = Program.Workspace.Problems.Get(id);
            if (res != null)
                return Ok(res.GetTests().Select(item => item.GetMetadata()));
            else
                return NotFound();
        }

        [HttpGet("{id}/tests/{tid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<TestCaseMetadata> GetTest(string id, string tid)
        {
            var res = Program.Workspace.Problems.Get(id)?.GetTest(tid);
            if (res != null)
                return Ok(res.GetMetadata());
            else
                return NotFound();
        }

        [HttpGet("{id}/tests/{tid}/input")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<string> GetTestInput(string id, string tid)
        {
            var res = Program.Workspace.Problems.Get(id)?.GetTest(tid);
            if (res != null)
                return Ok(res.GetInput());
            else
                return NotFound();
        }

        [HttpGet("{id}/tests/{tid}/output")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<string> GetTestOutput(string id, string tid)
        {
            var res = Program.Workspace.Problems.Get(id)?.GetTest(tid);
            if (res != null)
                return Ok(res.GetOutput());
            else
                return NotFound();
        }

        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            Program.Workspace.Problems.Delete(id);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesDefaultResponseType]
        public ActionResult<ProblemMetadata> Create(string id)
        {
            var res = Program.Workspace.Problems.Create(id);
            if (res != null)
                return Created($"problems/{id}", res);
            else
                return Conflict();
        }
    }
}