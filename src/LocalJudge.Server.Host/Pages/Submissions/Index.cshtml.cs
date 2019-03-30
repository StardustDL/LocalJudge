﻿using System;
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

        [BindProperty]
        public SubmissionItemOperation PostData { get; set; }

        public async Task OnGetAsync()
        {
            var httpclient = clientFactory.CreateClient();
            var client = new SubmissionsClient(httpclient);
            var ms = (await client.GetAllAsync()).ToList();
            ms.Sort((x, y) => y.Time.CompareTo(x.Time));
            var ss = new List<SubmissionItem>();
            foreach (var v in ms)
            {
                ss.Add(await SubmissionItem.Get(v, httpclient));
            }
            Submissions = ss;
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
                        return Redirect($"/Submissions/Index");
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