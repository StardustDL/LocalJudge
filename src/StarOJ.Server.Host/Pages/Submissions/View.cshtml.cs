using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using StarOJ.Server.Host.APIClients;
using StarOJ.Server.Host.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StarOJ.Server.Host.Pages.Submissions
{
    public class ViewModel : PageModel
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly IAuthorizationService _authorizationService;

        public ViewModel(IHttpClientFactory clientFactory, IAuthorizationService authorizationService)
        {
            this.clientFactory = clientFactory;
            _authorizationService = authorizationService;
        }

        public SubmissionModel Submission { get; set; }

        [BindProperty]
        public SubmissionPostModel PostData { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var httpclient = clientFactory.CreateClient();
            var client = new SubmissionsClient(httpclient);
            try
            {
                var metadata = await client.GetAsync(id);
                Submission = await SubmissionModel.GetAsync(metadata, httpclient);
            }
            catch
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDownloadCodeAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var httpclient = clientFactory.CreateClient();
            var client = new SubmissionsClient(httpclient);
            try
            {
                var bytes = Encoding.UTF8.GetBytes((await client.GetAsync(PostData.Id)).Code);
                return File(bytes, "text/plain", $"{PostData.Id}.{ProgrammingLanguageHelper.Extends[PostData.Language]}");
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if((await _authorizationService.AuthorizeAsync(User,Authorizations.Administrator)).Succeeded == false)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var httpclient = clientFactory.CreateClient();
            var client = new SubmissionsClient(httpclient);
            try
            {
                await client.DeleteAsync(PostData.Id);
                return RedirectToPage("./Index");
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostRejudgeAsync()
        {
            if ((await _authorizationService.AuthorizeAsync(User, Authorizations.Administrator)).Succeeded == false)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var httpclient = clientFactory.CreateClient();
            var client = new SubmissionsClient(httpclient);
            try
            {
                await client.RejudgeAsync(PostData.Id);
                return RedirectToPage(new { id = PostData.Id });
            }
            catch
            {
                return NotFound();
            }
        }


    }
}