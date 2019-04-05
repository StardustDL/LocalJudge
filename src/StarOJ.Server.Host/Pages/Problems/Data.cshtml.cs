using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using StarOJ.Server.API.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using StarOJ.Server.Host.Helpers;
using System.ComponentModel.DataAnnotations;
using StarOJ.Core;
using StarOJ.Core.Problems;
using StarOJ.Server.API.Models;

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
            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
            try
            {
                var metadata = await client.GetAsync(id);
                Problem = await ProblemModel.GetAsync(metadata, httpclient, false, true);
            }
            catch
            {
                return false;
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

            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
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

            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
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

            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
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

            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
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

            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
            try
            {
                var tmeta = await client.CreateTestAsync(PostData.ProblemId, PostData.TestCase);
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

            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
            try
            {
                var tmeta = await client.CreateSampleAsync(PostData.ProblemId, PostData.TestCase);
                return RedirectToPage(new { id = PostData.ProblemId, showId = tmeta.Id });
            }
            catch
            {
                return NotFound();
            }
        }
    }
}