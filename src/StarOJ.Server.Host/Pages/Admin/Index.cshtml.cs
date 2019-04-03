using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using StarOJ.Server.Host.APIClients;
using StarOJ.Server.Host.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StarOJ.Server.Host.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly SignInManager<UserMetadata> _signInManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<UserMetadata> _userManager;
        private readonly RoleManager<RoleMetadata> _roleManager;

        public IndexModel(IHttpClientFactory clientFactory, UserManager<UserMetadata> userManager, SignInManager<UserMetadata> signInManager, RoleManager<RoleMetadata> roleManager, IAuthorizationService authorizationService)
        {
            this.clientFactory = clientFactory;
            _signInManager = signInManager;
            _authorizationService = authorizationService;
            _userManager = userManager;
            _roleManager = roleManager;
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

            try
            {
                EnableAdmin = await CanAdmin();
                IsConnected = true;
            }
            catch
            {
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
            await _userManager.CreateAsync(new UserMetadata { Id = adminId, Email = "admin@localhost", Name = "Admin" }, "admin");
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