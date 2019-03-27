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
            var workspace = new Workspace(Program.Workspace);
            return Ok(workspace.Problems.GetAll().Select(item => item.GetMetadata()));
        }

        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<ProblemMetadata> Get(string name)
        {
            var workspace = new Workspace(Program.Workspace);
            var res = workspace.Problems.Get(name)?.GetMetadata();
            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpGet("{name}/description")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<ProblemDescription> GetDescription(string name)
        {
            var workspace = new Workspace(Program.Workspace);
            var res = workspace.Problems.Get(name);
            if (res != null)
                return Ok(res.GetDescription());
            else
                return NotFound();
        }

        [HttpGet("{name}/samples")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<TestCaseMetadata>> GetSamples(string name)
        {
            var workspace = new Workspace(Program.Workspace);
            var res = workspace.Problems.Get(name);
            if (res != null)
                return Ok(res.GetSamples().Select(item => item.GetMetadata()));
            else
                return NotFound();
        }

        [HttpGet("{name}/samples/{sname}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<TestCaseMetadata> GetSample(string name, string sname)
        {
            var workspace = new Workspace(Program.Workspace);
            var res = workspace.Problems.Get(name)?.GetSample(sname);
            if (res != null)
                return Ok(res.GetMetadata());
            else
                return NotFound();
        }

        [HttpGet("{name}/samples/{sname}/input")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<TestCaseMetadata> GetSampleInput(string name, string sname)
        {
            var workspace = new Workspace(Program.Workspace);
            var res = workspace.Problems.Get(name)?.GetSample(sname);
            if (res != null)
                return Ok(res.GetInput());
            else
                return NotFound();
        }

        [HttpGet("{name}/samples/{sname}/output")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<string> GetSampleOutput(string name, string sname)
        {
            var workspace = new Workspace(Program.Workspace);
            var res = workspace.Problems.Get(name)?.GetSample(sname);
            if (res != null)
                return Ok(res.GetOutput());
            else
                return NotFound();
        }

        [HttpGet("{name}/tests")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<IEnumerable<TestCaseMetadata>> GetTests(string name)
        {
            var workspace = new Workspace(Program.Workspace);
            var res = workspace.Problems.Get(name);
            if (res != null)
                return Ok(res.GetTests().Select(item => item.GetMetadata()));
            else
                return NotFound();
        }

        [HttpGet("{name}/tests/{tname}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<TestCaseMetadata> GetTest(string name, string tname)
        {
            var workspace = new Workspace(Program.Workspace);
            var res = workspace.Problems.Get(name)?.GetTest(tname);
            if (res != null)
                return Ok(res.GetMetadata());
            else
                return NotFound();
        }

        [HttpGet("{name}/tests/{tname}/input")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<string> GetTestInput(string name, string tname)
        {
            var workspace = new Workspace(Program.Workspace);
            var res = workspace.Problems.Get(name)?.GetTest(tname);
            if (res != null)
                return Ok(res.GetInput());
            else
                return NotFound();
        }

        [HttpGet("{name}/tests/{tname}/output")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult<string> GetTestOutput(string name, string tname)
        {
            var workspace = new Workspace(Program.Workspace);
            var res = workspace.Problems.Get(name)?.GetTest(tname);
            if (res != null)
                return Ok(res.GetOutput());
            else
                return NotFound();
        }

        [HttpDelete("{name}")]
        public void Delete(string name)
        {
            var workspace = new Workspace(Program.Workspace);
            workspace.Problems.Delete(name);
        }

        [HttpPut("{name}")]
        public void Create(string name)
        {
            var workspace = new Workspace(Program.Workspace);
            workspace.Problems.Create(name);
        }
    }
}