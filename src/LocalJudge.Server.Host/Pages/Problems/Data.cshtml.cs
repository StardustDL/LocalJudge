using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LocalJudge.Server.Host.APIClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LocalJudge.Server.Host.Pages.Problems
{
    public class DataModel : PageModel
    {
        const int MaxPreviewBytes = 128;

        public class TestCasePreviewModel
        {
            public TestCaseMetadata Metadata { get; set; }

            public DataPreview Input { get; set; }

            public DataPreview Output { get; set; }
        }

        public class TestCaseDownloadModel
        {
            public string ProblemId { get; set; }

            public string TestCaseId { get; set; }
        }

        private readonly IHttpClientFactory clientFactory;

        public DataModel(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public ProblemModel Problem { get; set; }

        public IList<TestCasePreviewModel> SamplePreview { get; set; }

        public IList<TestCasePreviewModel> TestPreview { get; set; }

        [BindProperty]
        public TestCaseDownloadModel PostData { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
            try
            {
                var metadata = await client.GetAsync(id);
                Problem = await ProblemModel.GetAsync(metadata, httpclient, false, true);
            }
            catch
            {
                return NotFound();
            }

            {
                var ls = new List<TestCasePreviewModel>();
                foreach (var item in Problem.Samples)
                {
                    ls.Add(new TestCasePreviewModel
                    {
                        Metadata = item,
                        Input = await client.GetSampleInputPreviewAsync(Problem.Metadata.Id, item.Id, MaxPreviewBytes),
                        Output = await client.GetSampleOutputPreviewAsync(Problem.Metadata.Id, item.Id, MaxPreviewBytes),
                    });
                }
                SamplePreview = ls;
            }

            {
                var ls = new List<TestCasePreviewModel>();
                foreach (var item in Problem.Tests)
                {
                    ls.Add(new TestCasePreviewModel
                    {
                        Metadata = item,
                        Input = await client.GetTestInputPreviewAsync(Problem.Metadata.Id, item.Id, MaxPreviewBytes),
                        Output = await client.GetTestOutputPreviewAsync(Problem.Metadata.Id, item.Id, MaxPreviewBytes),
                    });
                }
                TestPreview = ls;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSampleInputFileAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
            try
            {
                var bytes = Encoding.UTF8.GetBytes(await client.GetSampleInputAsync(PostData.ProblemId, PostData.TestCaseId));
                return File(bytes, "text/plain", $"sample{PostData.TestCaseId}.in");
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostSampleOutputFileAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
            try
            {
                var bytes = Encoding.UTF8.GetBytes(await client.GetSampleOutputAsync(PostData.ProblemId, PostData.TestCaseId));
                return File(bytes, "text/plain", $"sample{PostData.TestCaseId}.out");
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostTestInputFileAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
            try
            {
                var bytes = Encoding.UTF8.GetBytes(await client.GetTestInputAsync(PostData.ProblemId, PostData.TestCaseId));
                return File(bytes, "text/plain", $"test{PostData.TestCaseId}.in");
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostTestOutputFileAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
            try
            {
                var bytes = Encoding.UTF8.GetBytes(await client.GetTestOutputAsync(PostData.ProblemId, PostData.TestCaseId));
                return File(bytes, "text/plain", $"test{PostData.TestCaseId}.out");
            }
            catch
            {
                return NotFound();
            }
        }
    }
}