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
using System.Text;
using StarOJ.Core.Helpers;

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

        private static async Task UpdateIOData(TestCaseData data, ITestCaseProvider item)
        {
            if (data.InputFile != null)
            {
                using (var s = data.InputFile.OpenReadStream())
                    await item.SetInput(s);
            }
            else
            {
                using (var ms = TextIO.ToStream(data.Input))
                    await item.SetInput(ms);
            }
            if (data.OutputFile != null)
            {
                using (var s = data.OutputFile.OpenReadStream())
                    await item.SetOutput(s);
            }
            else
            {
                using (var ms = TextIO.ToStream(data.Output))
                    await item.SetOutput(ms);
            }
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
        public async Task<ActionResult<ProblemMetadata>> Get(string id)
        {
            var res = await (await _workspace.Problems.Get(id))?.GetMetadata();
            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

        [HttpGet("{id}/description")]
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
        public async Task<ActionResult> UpdateDescription(string id, [FromBody] ProblemDescription data)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null)
                return NotFound();

            await res.SetDescription(data);
            return Accepted();
        }

        [HttpGet("{id}/samples")]
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<TestCaseMetadata>> CreateSample(string id, [FromBody] TestCaseData data)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return NotFound();
            var item = await res.Samples.Create(data.Metadata);
            await UpdateIOData(data, item);
            return Created($"problems/{res.Id}/samples/{item.Id}", await item.GetMetadata());
        }

        [HttpGet("{id}/samples/{tid}")]
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
        public async Task<ActionResult<TestCaseMetadata>> UpdateSample(string id, string tid, [FromBody] TestCaseData data)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return NotFound();
            var item = await res.Samples.Get(tid);
            if (item == null) return NotFound();
            await item.SetMetadata(data.Metadata);
            await UpdateIOData(data, item);
            return Accepted();
        }

        [HttpGet("{id}/samples/{tid}/input/{num}/preview")]
        public async Task<ActionResult<DataPreview>> GetSampleInputPreview(string id, string tid, int num)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return NotFound();
            var item = await res.Samples.Get(tid);
            if (item == null) return NotFound();
            return Ok(await item.GetInputPreview(num));
        }

        [HttpGet("{id}/samples/{tid}/input")]
        public async Task<ActionResult> GetSampleInput(string id, string tid)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return NotFound();
            var item = await res.Samples.Get(tid);
            if (item == null) return NotFound();
            return File(await item.GetInput(), "text/plain", $"{tid}.in");
        }

        [HttpGet("{id}/samples/{tid}/output/{num}/preview")]
        public async Task<ActionResult<DataPreview>> GetSampleOutputPreview(string id, string tid, int num)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return NotFound();
            var item = await res.Samples.Get(tid);
            if (item == null) return NotFound();
            return Ok(await item.GetOutputPreview(num));
        }

        [HttpGet("{id}/samples/{tid}/output")]
        public async Task<ActionResult> GetSampleOutput(string id, string tid)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return NotFound();
            var item = await res.Samples.Get(tid);
            if (item == null) return NotFound();
            return File(await item.GetOutput(), "text/plain", $"{tid}.out");
        }

        [HttpGet("{id}/tests")]
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<TestCaseMetadata>> CreateTest(string id, [FromBody] TestCaseData data)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return NotFound();
            var item = await res.Tests.Create(data.Metadata);
            await UpdateIOData(data, item);
            return Created($"problems/{res.Id}/tests/{item.Id}", await item.GetMetadata());
        }

        [HttpGet("{id}/tests/{tid}")]
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
        public async Task<ActionResult<TestCaseMetadata>> UpdateTest(string id, string tid, [FromBody] TestCaseData data)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return NotFound();
            var item = await res.Tests.Get(tid);
            if (item == null) return NotFound();
            await item.SetMetadata(data.Metadata);
            await UpdateIOData(data, item);
            return Accepted();
        }

        [HttpGet("{id}/tests/{tid}/input/{num}/preview")]
        public async Task<ActionResult<DataPreview>> GetTestInputPreview(string id, string tid, int num)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return NotFound();
            var item = await res.Tests.Get(tid);
            if (item == null) return NotFound();
            return Ok(await item.GetInputPreview(num));
        }

        [HttpGet("{id}/tests/{tid}/input")]
        public async Task<ActionResult> GetTestInput(string id, string tid)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return NotFound();
            var item = await res.Tests.Get(tid);
            if (item == null) return NotFound();
            return File(await item.GetInput(), "text/plain", $"{tid}.in");
        }

        [HttpGet("{id}/tests/{tid}/output/{num}/preview")]
        public async Task<ActionResult<DataPreview>> GetTestOutputPreview(string id, string tid, int num)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return NotFound();
            var item = await res.Tests.Get(tid);
            if (item == null) return NotFound();
            return Ok(await item.GetOutputPreview(num));
        }

        [HttpGet("{id}/tests/{tid}/output")]
        public async Task<ActionResult> GetTestOutput(string id, string tid)
        {
            var res = await _workspace.Problems.Get(id);
            if (res == null) return NotFound();
            var item = await res.Tests.Get(tid);
            if (item == null) return NotFound();
            return File(await item.GetOutput(), "text/plain", $"{tid}.out");
        }

        [HttpDelete("{id}")]
        public Task Delete(string id)
        {
            return _workspace.Problems.Delete(id);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ProblemMetadata>> Create([FromBody] ProblemData data)
        {
            var res = await _workspace.Problems.Create(data.Metadata);

            await res.SetDescription(data.Description);

            if (data.Samples != null)
                foreach (var v in data.Samples)
                {
                    var item = await res.Samples.Create(v.Metadata);
                    await UpdateIOData(v, item);
                }

            if (data.Tests != null)
                foreach (var v in data.Tests)
                {
                    var item = await res.Tests.Create(v.Metadata);
                    await UpdateIOData(v, item);
                }

            return Created($"problems/{res.Id}", await res.GetMetadata());
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
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