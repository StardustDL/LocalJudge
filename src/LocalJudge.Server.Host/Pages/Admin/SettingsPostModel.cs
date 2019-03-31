using Microsoft.AspNetCore.Localization;
using System.ComponentModel.DataAnnotations;

namespace LocalJudge.Server.Host.Pages.Admin
{
    public class SettingsPostModel
    {
        public string Language { get; set; }
    }
}