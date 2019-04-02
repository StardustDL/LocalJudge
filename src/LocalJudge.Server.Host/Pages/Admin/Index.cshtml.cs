using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LocalJudge.Server.Host.APIClients;
using LocalJudge.Server.Host.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LocalJudge.Server.Host.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public IndexModel(IHttpClientFactory clientFactory, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager, IAuthorizationService authorizationService)
        {
            this.clientFactory = clientFactory;
            _signInManager = signInManager;
            _authorizationService = authorizationService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public string APIWorkspace
        {
            get; set;
        }

        public bool IsConnected
        {
            get; set;
        }

        public bool EnableAdmin
        {
            get; set;
        }

        [BindProperty]
        public SettingsPostModel PostData { get; set; }

        public async Task<bool> CanAdmin()
        {
            var httpclient = clientFactory.CreateClient();
            var wclient = new WorkspaceClient(httpclient);
            try
            {
                if (await wclient.GetHasInitializedAsync())
                {
                    if (!_signInManager.IsSignedIn(User)) return false;
                    if ((await _authorizationService.AuthorizeAsync(User, Authorizations.Administrator)).Succeeded)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

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
                EnableAdmin = await CanAdmin();
                IsConnected = true;
            }
            catch
            {
                APIWorkspace = "";
                IsConnected = false;
                EnableAdmin = false;
            }
        }

        public async Task<IActionResult> OnPostInitializeAsync()
        {
            if((await CanAdmin()) == false)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var httpclient = clientFactory.CreateClient();
            var client = new AdminClient(httpclient);
            await client.InitializeAsync();
            string adminId = Guid.NewGuid().ToString();
            await _userManager.CreateAsync(new User { Id = adminId, Email = "admin@localhost", Name = "Admin" }, "admin");
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