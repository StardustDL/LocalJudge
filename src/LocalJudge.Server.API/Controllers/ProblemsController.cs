using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LocalJudge.Core;
using LocalJudge.Core.Problems;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace LocalJudge.Server.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProblemsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<ProblemMetadata>> GetAll()
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

        [HttpGet("{id}/samples/{tid}/input/{num}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<DataPreview> GetSampleInput(string id, string tid, int num)
        {
            var res = Program.Workspace.Problems.Get(id)?.GetSample(tid);
            if (res != null)
                return Ok(res.GetInput(num));
            else
                return NotFound();
        }

        [HttpGet("{id}/samples/{tid}/inputfile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<byte[]> GetSampleInputFile(string id, string tid)
        {
            var res = Program.Workspace.Problems.Get(id)?.GetSample(tid);
            if (res != null)
            {
                return Ok(System.IO.File.ReadAllBytes(res.Input));
            }
            else
                return NotFound();
        }

        [HttpGet("{id}/samples/{tid}/output/{num}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<DataPreview> GetSampleOutput(string id, string tid, int num)
        {
            var res = Program.Workspace.Problems.Get(id)?.GetSample(tid);
            if (res != null)
                return Ok(res.GetOutput(num));
            else
                return NotFound();
        }

        [HttpGet("{id}/samples/{tid}/outputfile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<byte[]> GetSampleOutputFile(string id, string tid)
        {
            var res = Program.Workspace.Problems.Get(id)?.GetSample(tid);
            if (res != null)
            {
                return Ok(System.IO.File.ReadAllBytes(res.Output));
            }
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

        [HttpGet("{id}/tests/{tid}/input/{num}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<DataPreview> GetTestInput(string id, string tid, int num)
        {
            var res = Program.Workspace.Problems.Get(id)?.GetTest(tid);
            if (res != null)
                return Ok(res.GetInput(num));
            else
                return NotFound();
        }

        [HttpGet("{id}/tests/{tid}/inputfile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<byte[]> GetTestInputFile(string id, string tid)
        {
            var res = Program.Workspace.Problems.Get(id)?.GetTest(tid);
            if (res != null)
            {
                return Ok(System.IO.File.ReadAllBytes(res.Input));
            }
            else
                return NotFound();
        }

        [HttpGet("{id}/tests/{tid}/output/{num}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<DataPreview> GetTestOutput(string id, string tid, int num)
        {
            var res = Program.Workspace.Problems.Get(id)?.GetTest(tid);
            if (res != null)
                return Ok(res.GetOutput(num));
            else
                return NotFound();
        }

        [HttpGet("{id}/tests/{tid}/outputfile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<byte[]> GetTestOutputFile(string id, string tid)
        {
            var res = Program.Workspace.Problems.Get(id)?.GetTest(tid);
            if (res != null)
            {
                return Ok(System.IO.File.ReadAllBytes(res.Output));
            }
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
                return Created($"problems/{id}", res.GetMetadata());
            else
                return Conflict();
        }
    }
}