using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StarOJ.Core.Identity;
using StarOJ.Server.API.Clients;
using System.Net.Http;
using System.Threading.Tasks;

namespace StarOJ.Server.Host.Areas.Identity.Pages.Account
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<UserMetadata> _userManager;
        private readonly IHttpClientFactory _clientFactory;

        // private readonly IEmailSender _emailSender;

        public IndexModel(IHttpClientFactory clientFactory, UserManager<UserMetadata> userManager)
        {
            _userManager = userManager;
            _clientFactory = clientFactory;
        }

        public UserMetadata CurrentUser { get; set; }

        public UserStatistics Statistics { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                CurrentUser = await _userManager.GetUserAsync(User);
            }
            else
            {
                HttpClient httpclient = _clientFactory.CreateClient();
                UsersClient client = new UsersClient(httpclient);
                try
                {
                    CurrentUser = await client.GetAsync(id);
                }
                catch
                {
                    CurrentUser = null;
                }

                try
                {
                    StatisticsClient sscli = new StatisticsClient(httpclient);
                    Statistics = await sscli.GetUserAsync(id);
                }
                catch
                {
                    Statistics = null;
                }
            }

            if (CurrentUser == null)
            {
                return NotFound($"Unable to load user with ID '{id ?? _userManager.GetUserId(User)}'.");
            }

            return Page();
        }
    }
}
