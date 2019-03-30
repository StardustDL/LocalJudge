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

        public SubmissionItem Submission { get; set; }

        public string Code { get; set; }

        [BindProperty]
        public SubmissionItemOperation PostData { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var httpclient = clientFactory.CreateClient();
            var client = new SubmissionsClient(httpclient);
            try
            {
                var metadata = await client.GetAsync(id);
                Submission = await SubmissionItem.Get(metadata, httpclient);
                Code = await client.GetCodeAsync(id);
            }
            catch
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var httpclient = clientFactory.CreateClient();
            var client = new SubmissionsClient(httpclient);
            try
            {
                switch (PostData.Type)
                {
                    case SubmissionItemOperationType.Rejudge:
                        await client.RejudgeAsync(PostData.ID);
                        return Redirect($"/Submissions/View?id={PostData.ID}");
                    case SubmissionItemOperationType.Delete:
                        await client.DeleteAsync(PostData.ID);
                        return Redirect($"/Submissions/Index");
                }
                return BadRequest();                
            }
            catch
            {
                return NotFound();
            }
        }
    }
}