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

namespace StarOJ.Server.Host.Pages.Problems
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly IAuthorizationService _authorizationService;

        public IndexModel(IHttpClientFactory clientFactory, IAuthorizationService authorizationService)
        {
            this.clientFactory = clientFactory;
            _authorizationService = authorizationService;
        }

        public IList<ProblemModel> Problems { get; set; }

        [BindProperty]
        public ProblemPostModel PostData { get; set; }

        public async Task OnGetAsync()
        {
            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
            var ms = await client.GetAllAsync();
            var ss = new List<ProblemModel>();
            foreach (var v in ms)
            {
                ss.Add(await ProblemModel.GetAsync(v, httpclient, false, false));
            }
            Problems = ss;
        }

        public async Task<IActionResult> OnPostDeleteAsync()
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
            var client = new ProblemsClient(httpclient);
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
    }
}