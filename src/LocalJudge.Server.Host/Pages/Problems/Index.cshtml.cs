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
        public ProblemItemOperation PostData { get; set; }

        public async Task OnGetAsync()
        {
            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
            Problems = await client.GetAllAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
            try
            {
                switch (PostData.Type)
                {
                    case ProblemItemOperationType.Delete:
                        await client.DeleteAsync(PostData.ID);
                        return Redirect($"/Problems/Index");
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