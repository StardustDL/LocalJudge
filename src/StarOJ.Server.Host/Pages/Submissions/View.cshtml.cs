using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StarOJ.Core.Helpers;
using StarOJ.Server.API.Clients;
using StarOJ.Server.Host.Helpers;
using System.Net.Http;
using System.Threading.Tasks;

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

        public async Task<bool> GetModifyAuthorization()
        {
            return (await _authorizationService.AuthorizeAsync(User, Authorizations.Administrator)).Succeeded;
        }

        public SubmissionModel Submission { get; set; }

        [BindProperty]
        public SubmissionPostModel PostData { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            HttpClient httpclient = clientFactory.CreateClient();
            SubmissionsClient client = new SubmissionsClient(httpclient);
            try
            {
                Core.Submissions.SubmissionMetadata metadata = await client.GetAsync(id);
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
            HttpClient httpclient = clientFactory.CreateClient();
            SubmissionsClient client = new SubmissionsClient(httpclient);
            try
            {
                FileResponse file = await client.GetCodeAsync(PostData.Id);
                return File(file.Stream, "text/plain", $"{PostData.Id}.{ProgrammingLanguageHelper.Extends[PostData.Language]}");
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (await GetModifyAuthorization() == false)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            HttpClient httpclient = clientFactory.CreateClient();
            SubmissionsClient client = new SubmissionsClient(httpclient);
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
            if (await GetModifyAuthorization() == false)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            HttpClient httpclient = clientFactory.CreateClient();
            SubmissionsClient client = new SubmissionsClient(httpclient);
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