using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace StarOJ.Server.Host.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHtmlLocalizer<IndexModel> _localizer;

        public IndexModel(IHtmlLocalizer<IndexModel> localizer)
        {
            _localizer = localizer;
        }

        public void OnGet()
        {
            
        }
    }
}
