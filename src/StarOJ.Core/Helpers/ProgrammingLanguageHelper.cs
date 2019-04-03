using StarOJ.Core.Judgers;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarOJ.Core.Helpers
{
    public class ProgrammingLanguageHelper
    {
        public static readonly Dictionary<ProgrammingLanguage, string> Extends = new Dictionary<ProgrammingLanguage, string>
        {
            [ProgrammingLanguage.C] = "c",
            [ProgrammingLanguage.Cpp] = "cpp",
            [ProgrammingLanguage.CSharp] = "cs",
            [ProgrammingLanguage.VisualBasic] = "vb",
            [ProgrammingLanguage.Java] = "java",
            [ProgrammingLanguage.Python] = "py",
            [ProgrammingLanguage.Rust] = "rs",
            [ProgrammingLanguage.Go] = "go",
            [ProgrammingLanguage.Haskell] = "hs",
            [ProgrammingLanguage.Javascript] = "js",
            [ProgrammingLanguage.Kotlin] = "kt",
            [ProgrammingLanguage.Php] = "php",
            [ProgrammingLanguage.Ruby] = "rb",
            [ProgrammingLanguage.Scala] = "scala",
        };
    }
}
