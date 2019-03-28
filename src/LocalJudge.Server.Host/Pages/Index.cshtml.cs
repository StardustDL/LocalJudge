using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LocalJudge.Server.Host.Pages
{
    public class IndexModel : PageModel
    {


        public string APIWorkspace
        {
            get
            {
                using (var hc = new System.Net.Http.HttpClient())
                {
                    var client = new APIClients.AdminClient(hc);
                    return client.GetRootDirectoryAsync().Result;
                }
            }
        }

        public void OnGet()
        {

        }
    }
}
