using Microsoft.AspNetCore.Identity;
using StarOJ.Core;
using StarOJ.Core.Identity;
using StarOJ.Core.Judgers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StarOJ.Server.Host.Pages
{
    public class Helper
    {
        public static async Task<string> TryGetUserEmail(ClaimsPrincipal user, UserManager<UserMetadata> manager)
        {
            try
            {
                UserMetadata ru = await manager.GetUserAsync(user);
                return await manager.GetEmailAsync(ru);
            }
            catch
            {
                return "";
            }
        }

        public static string GetIssueClass(IssueLevel state)
        {
            switch (state)
            {
                case IssueLevel.Info:
                    return "info";
                case IssueLevel.Error:
                    return "danger";
                case IssueLevel.Warning:
                    return "warning";
            }
            return "secondary";
        }

        public static string GetEditorLanguage(ProgrammingLanguage lang)
        {
            switch (lang)
            {
                case ProgrammingLanguage.C:
                    return "c";
                case ProgrammingLanguage.Cpp:
                    return "cpp";
                case ProgrammingLanguage.Java:
                    return "java";
                case ProgrammingLanguage.Python:
                    return "python";
                case ProgrammingLanguage.CSharp:
                    return "csharp";
                case ProgrammingLanguage.Rust:
                    return "rust";
                case ProgrammingLanguage.VisualBasic:
                    return "vb";
                case ProgrammingLanguage.Go:
                    return "go";
                case ProgrammingLanguage.Haskell:
                    return "haskell";
                case ProgrammingLanguage.Javascript:
                    return "javascript";
                case ProgrammingLanguage.Kotlin:
                    return "kotlin";
                case ProgrammingLanguage.Php:
                    return "php";
                case ProgrammingLanguage.Ruby:
                    return "ruby";
                case ProgrammingLanguage.Scala:
                    return "scala";
            }
            return "plaintext";
        }
    }
}
