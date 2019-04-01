using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LocalJudge.Server.Host.APIClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LocalJudge.Server.Host.Pages.Submissions
{
    public class ViewModel : PageModel
    {
        private readonly IHttpClientFactory clientFactory;

        public ViewModel(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public SubmissionModel Submission { get; set; }

        public string Code { get; set; }

        [BindProperty]
        public SubmissionPostModel PostData { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var httpclient = clientFactory.CreateClient();
            var client = new SubmissionsClient(httpclient);
            try
            {
                var metadata = await client.GetAsync(id);
                Submission = await SubmissionModel.GetAsync(metadata, httpclient);
                Code = await client.GetCodeAsync(id);
            }
            catch
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var httpclient = clientFactory.CreateClient();
            var client = new SubmissionsClient(httpclient);
            try
            {
                await client.DeleteAsync(PostData.ID);
                return RedirectToPage("/Submissions/Index");
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostRejudgeAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var httpclient = clientFactory.CreateClient();
            var client = new SubmissionsClient(httpclient);
            try
            {
                await client.RejudgeAsync(PostData.ID);
                return RedirectToPage();
            }
            catch
            {
                return NotFound();
            }
        }

        public IActionResult OnPostDataAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return RedirectToPage("/Problems/Data", new { id = PostData.ProblemID });
        }
    }
}