using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StarOJ.Core.Identity;
using StarOJ.Core.Problems;
using StarOJ.Server.API.Clients;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace StarOJ.Server.Host.Pages.Problems
{
    [Authorize]
    [RequestSizeLimit(104857600)] // 100MB
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<UserMetadata> _userManager;

        public bool IsNew { get; set; }

        public ProblemModel Problem { get; set; }

        [BindProperty]
        public ProblemPostModel PostData { get; set; }

        public EditModel(IHttpClientFactory clientFactory, UserManager<UserMetadata> userManager, IAuthorizationService authorizationService)
        {
            this.clientFactory = clientFactory;
            _authorizationService = authorizationService;
            _userManager = userManager;
        }

        async Task<bool> GetData(string id)
        {
            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            try
            {
                ProblemMetadata metadata = await client.GetAsync(id);
                Problem = await ProblemModel.GetAsync(metadata, httpclient, true, false);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                IsNew = true;
                Problem = new ProblemModel
                {
                    Metadata = new Core.Problems.ProblemMetadata
                    {
                        Name = "Untitled",
                    },
                    Description = new Core.Problems.ProblemDescription()
                };
                PostData = new ProblemPostModel
                {
                    Description = Problem.Description
                };
                return Page();
            }
            else
            {
                IsNew = false;
                if (await GetData(id))
                {
                    PostData = new ProblemPostModel
                    {
                        Description = Problem.Description
                    };
                    return Page();
                }
                else
                    return NotFound();
            }
        }

        async Task<bool> CheckModel(bool isImport)
        {
            if (ModelState.IsValid == false) return false;
            if (!isImport)
            {
                try
                {
                    UserMetadata user = await _userManager.GetUserAsync(User);
                    if (await _userManager.GetUserIdAsync(user) != PostData.Metadata.UserId)
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                if (PostData.ImportFile == null)
                    return false;
            }
            return true;
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!await CheckModel(false))
            {
                if (await GetData(PostData.Metadata.Id))
                    return Page();
                else
                    return NotFound();
            }

            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            try
            {
                await client.UpdateMetadataAsync(PostData.Metadata.Id, PostData.Metadata);
                await client.UpdateDescriptionAsync(PostData.Metadata.Id, PostData.Description);
                return RedirectToPage(new { id = PostData.Metadata.Id });
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!await CheckModel(false))
            {
                if (await GetData(PostData.Metadata.Id))
                    return Page();
                else
                    return NotFound();
            }

            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            try
            {
                ProblemMetadata pro = await client.CreateAsync(new API.Models.ProblemData
                {
                    Description = PostData.Description,
                    Metadata = PostData.Metadata,
                });
                return RedirectToPage(new { id = pro.Id });
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (!await CheckModel(false))
            {
                return BadRequest();
            }
            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            try
            {
                await client.DeleteAsync(PostData.Metadata.Id);
                return RedirectToPage("/Problems/Index");
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostImportAsync()
        {
            if (!await CheckModel(true))
            {
                return BadRequest();
            }

            HttpClient httpclient = clientFactory.CreateClient();
            ProblemsClient client = new ProblemsClient(httpclient);
            try
            {
                string sp = "";
                using (Stream st = PostData.ImportFile.OpenReadStream())
                using (StreamReader sr = new StreamReader(st))
                    sp = await sr.ReadToEndAsync();
                ProblemPackage package = Newtonsoft.Json.JsonConvert.DeserializeObject<ProblemPackage>(sp);
                package.Metadata.UserId = PostData.Metadata.UserId;
                ProblemMetadata pro = await client.ImportAsync(package);
                return RedirectToPage(new { id = pro.Id });
            }
            catch
            {
                return NotFound();
            }
        }
    }
}