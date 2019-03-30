using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LocalJudge.Server.Host.APIClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LocalJudge.Server.Host.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public string APIWorkspace
        {
            get; set;
        }

        public bool IsConnected
        {
            get; set;
        }

        [BindProperty]
        public string Command { get; set; }

        public async Task OnGetAsync()
        {
            var httpclient = clientFactory.CreateClient();
            var client = new AdminClient(httpclient);
            try
            {
                APIWorkspace = await client.GetRootDirectoryAsync();
                IsConnected = true;
            }
            catch
            {
                APIWorkspace = "";
                IsConnected = false;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var httpclient = clientFactory.CreateClient();
            var client = new AdminClient(httpclient);
            switch (Command)
            {
                case "init":
                    await client.InitializeAsync();
                    return Redirect("/Admin");
                case "seed":
                    await client.SeedDataAsync();
                    return Redirect("/Admin");
            }
            return BadRequest();
        }
    }
}