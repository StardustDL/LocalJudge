using Microsoft.AspNetCore.Localization;

namespace LocalJudge.Server.Host.Pages.Admin
{
    public enum SettingsItemType
    {
        Command,
        Language,
    }

    public class SettingsItem
    {
        public SettingsItemType Type { get; set; }

        public string Command { get; set; }

        public string Language { get; set; }
    }
}