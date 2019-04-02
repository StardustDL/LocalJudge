using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LocalJudge.Server.Host.APIClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LocalJudge.Server.Host.Pages.Problems
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public IList<ProblemMetadata> Problems { get; set; }

        [BindProperty]
        public ProblemPostModel PostData { get; set; }

        public async Task OnGetAsync()
        {
            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
            Problems = await client.GetAllAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
            try
            {
                await client.DeleteAsync(PostData.Id);
                return RedirectToPage();
            }
            catch
            {
                return NotFound();
            }
        }
    }
}