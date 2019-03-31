using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LocalJudge.Server.Host.APIClients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
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
        public SettingsPostModel PostData { get; set; }

        public async Task OnGetAsync()
        {
            PostData = new SettingsPostModel
            {
                Language = HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.UICulture.Name
            };

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

        public async Task<IActionResult> OnPostInitializeAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var httpclient = clientFactory.CreateClient();
            var client = new AdminClient(httpclient);
            await client.InitializeAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSeedAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var httpclient = clientFactory.CreateClient();
            var client = new AdminClient(httpclient);
            await client.SeedDataAsync();
            return RedirectToPage();
        }

        public IActionResult OnPostChangeLanguage()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(PostData.Language)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return RedirectToPage();
        }
    }
}