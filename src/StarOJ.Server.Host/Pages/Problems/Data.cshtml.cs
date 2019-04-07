using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StarOJ.Core;
using StarOJ.Core.Helpers;
using StarOJ.Core.Problems;
using StarOJ.Server.API.Clients;
using StarOJ.Server.API.Models;
using StarOJ.Server.Host.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;

namespace StarOJ.Server.Host.Pages.Problems
{
    public class DataModel : PageModel
    {
        const int MaxPreviewBytes = 128;

        public class TestCasePreviewModel
        {
            public TestCaseMetadata Metadata { get; set; }

            public DataPreview Input { get; set; }

            public DataPreview Output { get; set; }

            public bool IsNew { get; set; }
        }

        public class TestCaseModel
        {
            [Required]
            public string ProblemId { get; set; }

            [Required]
            public string TestCaseId { get; set; }

            public TestCaseData TestCase { get; set; }
        }

        private readonly IHttpClientFactory clientFactory;
        private readonly IAuthorizationService _authorizationService;

        public DataModel(IHttpClientFactory clientFactory, IAuthorizationService authorizationService)
        {
            this.clientFactory = clientFactory;
            _authorizationService = authorizationService;
        }

        public ProblemModel Problem { get; set; }

        public string ShowId { get; set; }

        public IList<TestCasePreviewModel> SamplePreview { get; set; }

        public IList<TestCasePreviewModel> TestPreview { get; set; }

        [BindProperty]
        public TestCaseModel PostData { get; set; }

        async Task<bool> GetData(string id)
        {
            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            try
            {
                ProblemMetadata metadata = await client.GetAsync(id);
                Problem = await ProblemModel.GetAsync(metadata, httpclient, false, true);
            }
            catch
            {
                return false;
            }

            TestCasePreviewModel emptyTestCase = new TestCasePreviewModel
            {
                Metadata = new TestCaseMetadata
                {
                    Id = "empty",
                    TimeLimit = TimeSpan.FromSeconds(1),
                    MemoryLimit = 128 * MemoryValueHelper.MB,
                },
                IsNew = true,
            };

            {
                List<TestCasePreviewModel> ls = new List<TestCasePreviewModel>();
                foreach (TestCaseMetadata item in Problem.Samples)
                {
                    ls.Add(new TestCasePreviewModel
                    {
                        Metadata = item,
                        Input = await client.GetSampleInputPreviewAsync(Problem.Metadata.Id, item.Id, MaxPreviewBytes),
                        Output = await client.GetSampleOutputPreviewAsync(Problem.Metadata.Id, item.Id, MaxPreviewBytes),
                        IsNew = false,
                    });
                }
                ls.Add(emptyTestCase);
                SamplePreview = ls;
            }

            {
                List<TestCasePreviewModel> ls = new List<TestCasePreviewModel>();
                foreach (TestCaseMetadata item in Problem.Tests)
                {
                    ls.Add(new TestCasePreviewModel
                    {
                        Metadata = item,
                        Input = await client.GetTestInputPreviewAsync(Problem.Metadata.Id, item.Id, MaxPreviewBytes),
                        Output = await client.GetTestOutputPreviewAsync(Problem.Metadata.Id, item.Id, MaxPreviewBytes),
                    });
                }
                ls.Add(emptyTestCase);
                TestPreview = ls;
            }

            return true;
        }

        public async Task<IActionResult> OnGetAsync(string id, string showId)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            ShowId = showId;

            if (await GetData(id))
                return Page();
            else
                return NotFound();
        }

        public async Task<IActionResult> OnPostSampleInputFileAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            try
            {
                FileResponse file = await client.GetSampleInputAsync(PostData.ProblemId, PostData.TestCaseId);
                return File(file.Stream, "text/plain", $"{PostData.TestCaseId}.in");
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

            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            try
            {
                FileResponse file = await client.GetSampleOutputAsync(PostData.ProblemId, PostData.TestCaseId);
                return File(file.Stream, "text/plain", $"{PostData.TestCaseId}.out");
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

            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            try
            {
                FileResponse file = await client.GetTestInputAsync(PostData.ProblemId, PostData.TestCaseId);
                return File(file.Stream, "text/plain", $"{PostData.TestCaseId}.in");
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

            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            try
            {
                FileResponse file = await client.GetTestOutputAsync(PostData.ProblemId, PostData.TestCaseId);
                return File(file.Stream, "text/plain", $"test{PostData.TestCaseId}.out");
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<bool> GetModifyAuthorization()
        {
            return (await _authorizationService.AuthorizeAsync(User, Authorizations.Administrator)).Succeeded;
        }

        public async Task<IActionResult> OnPostDeleteSampleAsync()
        {
            if ((await GetModifyAuthorization()) == false)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            try
            {
                await client.DeleteSampleAsync(PostData.ProblemId, PostData.TestCaseId);
                return RedirectToPage(new { id = PostData.ProblemId });
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostDeleteTestAsync()
        {
            if ((await GetModifyAuthorization()) == false)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            try
            {
                await client.DeleteTestAsync(PostData.ProblemId, PostData.TestCaseId);
                return RedirectToPage(new { id = PostData.ProblemId });
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostEditTestAsync()
        {
            if ((await GetModifyAuthorization()) == false)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                ShowId = PostData.TestCaseId;
                if (await GetData(PostData.ProblemId))
                    return Page();
                else
                    return NotFound();
            }

            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            try
            {
                await client.UpdateTestAsync(PostData.ProblemId, PostData.TestCaseId, PostData.TestCase);
                return RedirectToPage(new { id = PostData.ProblemId, showId = PostData.TestCaseId });
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostEditSampleAsync()
        {
            if ((await GetModifyAuthorization()) == false)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                ShowId = PostData.TestCaseId;
                if (await GetData(PostData.ProblemId))
                    return Page();
                else
                    return NotFound();
            }

            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            try
            {
                await client.UpdateSampleAsync(PostData.ProblemId, PostData.TestCaseId, PostData.TestCase);
                return RedirectToPage(new { id = PostData.ProblemId, showId = PostData.TestCaseId });
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostCreateTestAsync()
        {
            if ((await GetModifyAuthorization()) == false)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                ShowId = PostData.TestCaseId;
                if (await GetData(PostData.ProblemId))
                    return Page();
                else
                    return NotFound();
            }

            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            try
            {
                TestCaseMetadata tmeta = await client.CreateTestAsync(PostData.ProblemId, PostData.TestCase);
                return RedirectToPage(new { id = PostData.ProblemId, showId = tmeta.Id });
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostCreateSampleAsync()
        {
            if ((await GetModifyAuthorization()) == false)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                ShowId = PostData.TestCaseId;
                if (await GetData(PostData.ProblemId))
                    return Page();
                else
                    return NotFound();
            }

            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            try
            {
                TestCaseMetadata tmeta = await client.CreateSampleAsync(PostData.ProblemId, PostData.TestCase);
                return RedirectToPage(new { id = PostData.ProblemId, showId = tmeta.Id });
            }
            catch
            {
                return NotFound();
            }
        }
    }
}