using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using StarOJ.Server.API.Clients;
using StarOJ.Server.Host.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StarOJ.Core.Judgers;
using StarOJ.Core.Helpers;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using StarOJ.Server.Host.TagHelpers;

namespace StarOJ.Server.Host.Pages.Submissions
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly IAuthorizationService _authorizationService;

        public List<SelectListItem> QueryLanguages { get; private set; }

        public List<SelectListItem> QueryJudgeStates { get; private set; }

        public IndexModel(IHttpClientFactory clientFactory, IAuthorizationService authorizationService, IHtmlLocalizer<JudgeStateTagHelper> judgeStateLocalizer)
        {
            this.clientFactory = clientFactory;
            _authorizationService = authorizationService;

            QueryLanguages = new List<SelectListItem>
            {
                new SelectListItem("", "")
            };
            foreach (ProgrammingLanguage v in Enum.GetValues(typeof(ProgrammingLanguage)))
            {
                QueryLanguages.Add(new SelectListItem(ProgrammingLanguageHelper.DisplayNames[v], Enum.GetName(typeof(ProgrammingLanguage), v)));
            }

            QueryJudgeStates = new List<SelectListItem>
            {
                new SelectListItem("", "")
            };
            foreach (JudgeState v in Enum.GetValues(typeof(JudgeState)))
            {
                QueryJudgeStates.Add(new SelectListItem(judgeStateLocalizer[JudgeStateHelper.DisplayNames[v]].Value, Enum.GetName(typeof(JudgeState), v)));
            }
        }

        public async Task<bool> GetModifyAuthorization()
        {
            return (await _authorizationService.AuthorizeAsync(User, Authorizations.Administrator)).Succeeded;
        }

        public IList<SubmissionModel> Submissions { get; set; }

        [BindProperty]
        public SubmissionPostModel PostData { get; set; }

        public async Task OnGetAsync()
        {
            var httpclient = clientFactory.CreateClient();
            var client = new SubmissionsClient(httpclient);
            try
            {
                var ms = (await client.GetAllAsync()).ToList();
                ms.Sort((x, y) => y.Time.CompareTo(x.Time));
                var ss = new List<SubmissionModel>();
                foreach (var v in ms)
                {
                    ss.Add(await SubmissionModel.GetAsync(v, httpclient));
                }
                Submissions = ss;
            }
            catch
            {
                Submissions = Array.Empty<SubmissionModel>();
            }
        }

        public async Task<IActionResult> OnPostQueryAsync()
        {
            try
            {
                var httpclient = clientFactory.CreateClient();
                var client = new SubmissionsClient(httpclient);
                ProgrammingLanguage? lang = null;
                if (!string.IsNullOrEmpty(PostData.QueryLanguage))
                    lang = Enum.Parse<ProgrammingLanguage>(PostData.QueryLanguage);
                JudgeState? state = null;
                if (!string.IsNullOrEmpty(PostData.QueryJudgeState))
                    state = Enum.Parse<JudgeState>(PostData.QueryJudgeState);

                var ms = (await client.QueryAsync(PostData.Id, PostData.ProblemId, PostData.UserId, lang, state)).ToArray();
                var ss = new List<SubmissionModel>();
                foreach (var v in ms)
                {
                    ss.Add(await SubmissionModel.GetAsync(v, httpclient));
                }
                Submissions = ss;
            }
            catch
            {
                Submissions = Array.Empty<SubmissionModel>();
            }
            return Page();
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
            var httpclient = clientFactory.CreateClient();
            var client = new SubmissionsClient(httpclient);
            try
            {
                await client.DeleteAsync(PostData.Id);
                return RedirectToPage();
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
            var httpclient = clientFactory.CreateClient();
            var client = new SubmissionsClient(httpclient);
            try
            {
                await client.RejudgeAsync(PostData.Id);
                return RedirectToPage();
            }
            catch
            {
                return NotFound();
            }
        }
    }
}