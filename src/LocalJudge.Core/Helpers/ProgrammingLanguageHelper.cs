using LocalJudge.Core.Judgers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocalJudge.Core.Helpers
{
    public class ProgrammingLanguageHelper
    {
        public static readonly Dictionary<ProgrammingLanguage, string> Extends = new Dictionary<ProgrammingLanguage, string>
        {
            [ProgrammingLanguage.C] = "c",
            [ProgrammingLanguage.Cpp] = "cpp",
            [ProgrammingLanguage.CSharp] = "cs",
            [ProgrammingLanguage.Java] = "java",
            [ProgrammingLanguage.Python] = "py",
            [ProgrammingLanguage.Rust] = "rs",
        };
    }
}
