using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using StarOJ.Server.API.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using StarOJ.Core.Identity;

namespace StarOJ.Server.Host.Areas.Identity.Pages
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<UserMetadata> _signInManager;
        private readonly IHttpClientFactory _clientFactory;
        private readonly UserManager<UserMetadata> _userManager;
        // private readonly ILogger<RegisterModel> _logger;
        // private readonly IEmailSender _emailSender;

        public RegisterModel(IHttpClientFactory clientFactory,
            UserManager<UserMetadata> userManager,
            SignInManager<UserMetadata> signInManager)
        {
            _clientFactory = clientFactory;
            _userManager = userManager;
            _signInManager = signInManager;
            // _logger = logger;
            // _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                UserMetadata user = new UserMetadata { Name = Input.UserName, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    // _logger.LogInformation("User created a new account with password.");

                    /* var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."); */

                    try
                    {
                        var httpclient = _clientFactory.CreateClient();
                        var client = new UsersClient(httpclient);
                        user = await client.GetByNameAsync(user.Name);
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    catch(Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                    
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
