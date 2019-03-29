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

        public IList<SubmissionItem> Submissions { get; set; }

        public async Task OnGetAsync()
        {
            var httpclient = clientFactory.CreateClient();
            var client = new SubmissionsClient(httpclient);
            var ms = (await client.GetAllAsync()).ToList();
            ms.Sort((x, y) => y.Time.CompareTo(x.Time));
            var ss = new List<SubmissionItem>();
            foreach(var v in ms)
            {
                ss.Add(await SubmissionItem.Get(v, httpclient));
            }
            Submissions = ss;
        }
    }
}