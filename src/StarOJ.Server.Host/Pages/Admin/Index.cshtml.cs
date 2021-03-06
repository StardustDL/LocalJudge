﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StarOJ.Core.Identity;
using StarOJ.Server.API.Clients;
using StarOJ.Server.Host.Helpers;
using System;
using System.Net.Http;
using System.Threading.Tasks;

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
            HttpClient httpclient = clientFactory.CreateClient();
            WorkspaceClient wclient = new WorkspaceClient(httpclient);
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
            if ((await CanAdmin()) == false)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            HttpClient httpclient = clientFactory.CreateClient();
            WorkspaceClient client = new WorkspaceClient(httpclient);
            await client.InitializeAsync();
            UserMetadata rawUser = new UserMetadata { Email = "admin@localhost", Name = "Admin" };
            await _userManager.CreateAsync(rawUser, "admin");

            {
                UsersClient ucli = new UsersClient(httpclient);
                UserMetadata user = await ucli.GetByNameAsync(rawUser.NormalizedName);
                ProblemsClient pcli = new ProblemsClient(httpclient);
                await pcli.CreateAsync(Helpers.Problems.GetAPlusB(user.Id));
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostClearAsync()
        {
            if ((await CanAdmin()) == false)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            HttpClient httpclient = clientFactory.CreateClient();
            WorkspaceClient client = new WorkspaceClient(httpclient);
            await client.ClearAsync();

            await _signInManager.SignOutAsync();

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