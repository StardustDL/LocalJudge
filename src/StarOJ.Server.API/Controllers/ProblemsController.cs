using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StarOJ.Core;
using StarOJ.Core.Problems;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using StarOJ.Server.API.Models;

namespace StarOJ.Server.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProblemsController : ControllerBase
    {
        private readonly IWorkspace _workspace;

        public ProblemsController(IWorkspace workspace)
        {
            _workspace = workspace;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProblemMetadata>>> GetAll()
        {
            var all = await _workspace.Problems.GetAll();
            List<ProblemMetadata> ans = new List<ProblemMetadata>();
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
        public async Task<ActionResult<ProblemMetadata>> Get(string id)
        {
            var res = await (await _workspace.Problems.Get(id))?.GetMetadata();
            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpGet("{id}/description")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ProblemDescription>> GetDescription(string id)
        {
            var res = await _workspace.Problems.Get(id);
            if (res != null)
                return Ok(await res.GetDescription());
            else
                return NotFound();
        }

        [HttpPut("{id}/description")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> UpdateDescription(string id, [FromBody] ProblemDescription data)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null)
                return NotFound();

            await res.SetDescription(data);
            return Accepted();
        }

        [HttpGet("{id}/samples")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<TestCaseMetadata>>> GetSamples(string id)
        {
            var res = await _workspace.Problems.Get(id);
            if (res != null)
            {
                var all = await res.Samples.GetAll();
                List<TestCaseMetadata> ans = new List<TestCaseMetadata>();
                foreach (var v in all)
                {
                    ans.Add(await v.GetMetadata());
                }
                return Ok(ans);
            }
            else
                return NotFound();
        }

        [HttpPut("{id}/samples/clear")]
        public async Task ClearSample(string id)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return;
            await res.Samples.Clear();
        }

        [HttpPost("{id}/samples")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<TestCaseMetadata>> AddSample(string id, [FromBody] TestCaseData data)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return NotFound();
            var item = await res.Samples.Create(data.Metadata);
            await item.SetInput(data.Input);
            await item.SetOutput(data.Output);
            return Accepted();
        }

        [HttpGet("{id}/samples/{tid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<TestCaseMetadata>> GetSample(string id, string tid)
        {
            var res = await _workspace.Problems.Get(id);
            if (res != null)
                return Ok(await (await res.Samples.Get(tid)).GetMetadata());
            else
                return NotFound();
        }

        [HttpDelete("{id}/samples/{tid}")]
        public async Task DeleteSample(string id, string tid)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return;
            await res.Samples.Delete(tid);
        }

        [HttpPut("{id}/samples/{tid}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<TestCaseMetadata>> UpdateSample(string id, string tid, [FromBody] TestCaseData data)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return NotFound();
            var item = await res.Samples.Get(tid);
            if (item == null) return NotFound();
            await item.SetMetadata(data.Metadata);
            await item.SetInput(data.Input);
            await item.SetOutput(data.Output);
            return Accepted();
        }

        [HttpGet("{id}/samples/{tid}/input/{num}/preview")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<DataPreview>> GetSampleInputPreview(string id, string tid, int num)
        {
            var res = await _workspace.Problems.Get(id);
            if (res != null)
                return Ok(await (await res.Samples.Get(tid)).GetInputPreview(num));
            else
                return NotFound();
        }

        [HttpGet("{id}/samples/{tid}/input")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<string>> GetSampleInput(string id, string tid)
        {
            var res = await _workspace.Problems.Get(id);
            if (res != null)
            {
                return Ok(await (await res.Samples.Get(tid)).GetInput());
            }
            else
                return NotFound();
        }

        [HttpGet("{id}/samples/{tid}/output/{num}/preview")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<DataPreview>> GetSampleOutputPreview(string id, string tid, int num)
        {
            var res = await _workspace.Problems.Get(id);
            if (res != null)
                return Ok(await (await res.Samples.Get(tid)).GetOutputPreview(num));
            else
                return NotFound();
        }

        [HttpGet("{id}/samples/{tid}/output")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<string>> GetSampleOutput(string id, string tid)
        {
            var res = await _workspace.Problems.Get(id);
            if (res != null)
            {
                return Ok(await (await res.Samples.Get(tid)).GetOutput());
            }
            else
                return NotFound();
        }

        [HttpGet("{id}/tests")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<TestCaseMetadata>>> GetTests(string id)
        {
            var res = await _workspace.Problems.Get(id);
            if (res != null)
            {
                var all = await res.Tests.GetAll();
                List<TestCaseMetadata> ans = new List<TestCaseMetadata>();
                foreach (var v in all)
                {
                    ans.Add(await v.GetMetadata());
                }
                return Ok(ans);
            }
            else
                return NotFound();
        }

        [HttpPut("{id}/tests/clear")]
        public async Task ClearTest(string id)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return;
            await res.Tests.Clear();
        }

        [HttpPost("{id}/tests")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<TestCaseMetadata>> AddTest(string id, [FromBody] TestCaseData data)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return NotFound();
            var item = await res.Tests.Create(data.Metadata);
            await item.SetInput(data.Input);
            await item.SetOutput(data.Output);
            return Accepted();
        }

        [HttpGet("{id}/tests/{tid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<TestCaseMetadata>> GetTest(string id, string tid)
        {
            var res = await _workspace.Problems.Get(id);
            if (res != null)
                return Ok(await (await res.Tests.Get(tid)).GetMetadata());
            else
                return NotFound();
        }

        [HttpDelete("{id}/tests/{tid}")]
        public async Task DeleteTest(string id, string tid)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return;
            await res.Tests.Delete(tid);
        }

        [HttpPut("{id}/tests/{tid}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<TestCaseMetadata>> UpdateTest(string id, string tid, [FromBody] TestCaseData data)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return NotFound();
            var item = await res.Tests.Get(tid);
            if (item == null) return NotFound();
            await item.SetMetadata(data.Metadata);
            await item.SetInput(data.Input);
            await item.SetOutput(data.Output);
            return Accepted();
        }

        [HttpGet("{id}/tests/{tid}/input/{num}/preview")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<DataPreview>> GetTestInputPreview(string id, string tid, int num)
        {
            var res = await _workspace.Problems.Get(id);
            if (res != null)
                return Ok(await (await res.Tests.Get(tid)).GetInputPreview(num));
            else
                return NotFound();
        }

        [HttpGet("{id}/tests/{tid}/input")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<string>> GetTestInput(string id, string tid)
        {
            var res = await _workspace.Problems.Get(id);
            if (res != null)
            {
                return Ok(await (await res.Tests.Get(tid)).GetInput());
            }
            else
                return NotFound();
        }

        [HttpGet("{id}/tests/{tid}/output/{num}/preview")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<DataPreview>> GetTestOutputPreview(string id, string tid, int num)
        {
            var res = await _workspace.Problems.Get(id);
            if (res != null)
                return Ok(await (await res.Tests.Get(tid)).GetOutputPreview(num));
            else
                return NotFound();
        }

        [HttpGet("{id}/tests/{tid}/output")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<string>> GetTestOutput(string id, string tid)
        {
            var res = await _workspace.Problems.Get(id);
            if (res != null)
            {
                return Ok(await (await res.Tests.Get(tid)).GetOutput());
            }
            else
                return NotFound();
        }

        [HttpDelete("{id}")]
        public Task Delete(string id)
        {
            return _workspace.Problems.Delete(id);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ProblemMetadata>> Create([FromBody] ProblemData data)
        {
            var res = await _workspace.Problems.Create(data.Metadata);

            await res.SetDescription(data.Description);
            foreach (var v in data.Samples)
            {
                var item = await res.Samples.Create(v.Metadata);
                await item.SetInput(v.Input);
                await item.SetOutput(v.Output);
            }

            foreach (var v in data.Tests)
            {
                var item = await res.Tests.Create(v.Metadata);
                await item.SetInput(v.Input);
                await item.SetOutput(v.Output);
            }

            return Created($"problems/{res.Id}", await res.GetMetadata());
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> UpdateMetadata(string id, [FromBody] ProblemMetadata data)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null)
                return NotFound();

            await res.SetMetadata(data);
            return Accepted();
        }
    }
}