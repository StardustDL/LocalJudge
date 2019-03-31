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
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public IList<SubmissionModel> Submissions { get; set; }

        [BindProperty]
        public SubmissionPostModel PostData { get; set; }

        public async Task OnGetAsync()
        {
            var httpclient = clientFactory.CreateClient();
            var client = new SubmissionsClient(httpclient);
            var ms = (await client.GetAllAsync()).ToList();
            ms.Sort((x, y) => y.Time.CompareTo(x.Time));
            var ss = new List<SubmissionModel>();
            foreach (var v in ms)
            {
                ss.Add(await SubmissionModel.Get(v, httpclient));
            }
            Submissions = ss;
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
                return RedirectToPage();
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
    }
}