using LocalJudge.Server.Host.APIClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalJudge.Server.Host.Pages
{
    public class Helper
    {

        public static string GetIssueColor(IssueLevel state)
        {
            switch (state)
            {
                case IssueLevel.Info:
                    return "blue";
                case IssueLevel.Error:
                    return "red";
                case IssueLevel.Warning:
                    return "orange";
            }
            return "black";
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
            }
            return "plaintext";
        }
    }
}
