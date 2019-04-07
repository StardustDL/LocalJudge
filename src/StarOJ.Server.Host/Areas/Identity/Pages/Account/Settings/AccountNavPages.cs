using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace StarOJ.Server.Host.Areas.Identity.Pages.Account.Settings
{
    public static class AccountSettingsNavPages
    {
        public static string Index => "Index";

        public static string ChangePassword => "ChangePassword";

        public static string IndexNavClass(ViewContext viewContext)
        {
            return PageNavClass(viewContext, Index);
        }

        public static string ChangePasswordNavClass(ViewContext viewContext)
        {
            return PageNavClass(viewContext, ChangePassword);
        }

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            string activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}