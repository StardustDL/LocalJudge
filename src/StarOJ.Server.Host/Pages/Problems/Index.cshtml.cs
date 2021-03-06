﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StarOJ.Server.API.Clients;
using StarOJ.Server.Host.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<bool> GetModifyAuthorization()
        {
            return (await _authorizationService.AuthorizeAsync(User, Authorizations.Administrator)).Succeeded;
        }

        public IList<ProblemModel> Problems { get; set; }

        [BindProperty]
        public ProblemPostModel PostData { get; set; }

        public async Task OnGetAsync()
        {
            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            IList<Core.Problems.ProblemMetadata> ms = await client.GetAllAsync();
            List<ProblemModel> ss = new List<ProblemModel>();
            foreach (Core.Problems.ProblemMetadata v in ms)
            {
                ss.Add(await ProblemModel.GetAsync(v, httpclient, false, false));
            }
            Problems = ss;
        }

        public async Task<IActionResult> OnPostQueryAsync()
        {
            try
            {
                HttpClient httpclient = clientFactory.CreateClient();
                ProblemsClient client = new ProblemsClient(httpclient);
                List<Core.Problems.ProblemMetadata> ms = (await client.QueryAsync(PostData.QueryId, PostData.QueryUserId, PostData.QueryName, PostData.QuerySource)).ToList();
                List<ProblemModel> ss = new List<ProblemModel>();
                foreach (Core.Problems.ProblemMetadata v in ms)
                {
                    ss.Add(await ProblemModel.GetAsync(v, httpclient, false, false));
                }
                Problems = ss;
            }
            catch
            {
                Problems = Array.Empty<ProblemModel>();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (!await GetModifyAuthorization())
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            try
            {
                await client.DeleteAsync(PostData.Metadata.Id);
                return RedirectToPage();
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostExportAsync()
        {
            if (!await GetModifyAuthorization())
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            try
            {
                Core.Problems.ProblemPackage package = await client.ExportAsync(PostData.Metadata.Id);
                string str = Newtonsoft.Json.JsonConvert.SerializeObject(package);
                return File(Encoding.UTF8.GetBytes(str), "text/plain", $"{PostData.Metadata.Id}.json");
            }
            catch
            {
                return NotFound();
            }
        }
    }
}