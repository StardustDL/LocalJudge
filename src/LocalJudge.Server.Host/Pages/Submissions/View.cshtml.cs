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

        public SubmissionMetadata Metadata { get; set; }

        public string Code { get; set; }

        public SubmissionResult Result { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var httpclient = clientFactory.CreateClient();
            var client = new SubmissionsClient(httpclient);
            try
            {
                Metadata = await client.GetAsync(id);
                Code = await client.GetCodeAsync(id);
            }
            catch
            {
                return NotFound();
            }

            try
            {
                Result = await client.GetResultAsync(id);
            }
            catch
            {
                Result = null;
            }

            return Page();
        }
    }
}