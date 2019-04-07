using StarOJ.Core.Judgers;
using System.Collections.Generic;

namespace StarOJ.Core.Helpers
{
    public static class ProgrammingLanguageHelper
    {
        public static readonly IReadOnlyDictionary<ProgrammingLanguage, string> Extends = new Dictionary<ProgrammingLanguage, string>
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

        public static readonly IReadOnlyDictionary<ProgrammingLanguage, string> DisplayNames = new Dictionary<ProgrammingLanguage, string>
        {
            [ProgrammingLanguage.C] = "C",
            [ProgrammingLanguage.Cpp] = "C++",
            [ProgrammingLanguage.CSharp] = "C#",
            [ProgrammingLanguage.VisualBasic] = "Visual Basic",
            [ProgrammingLanguage.Java] = "Java",
            [ProgrammingLanguage.Python] = "Python",
            [ProgrammingLanguage.Rust] = "Rust",
            [ProgrammingLanguage.Go] = "Go",
            [ProgrammingLanguage.Haskell] = "Haskell",
            [ProgrammingLanguage.Javascript] = "Javascript",
            [ProgrammingLanguage.Kotlin] = "Kotlin",
            [ProgrammingLanguage.Php] = "PHP",
            [ProgrammingLanguage.Ruby] = "Ruby",
            [ProgrammingLanguage.Scala] = "Scala",
        };
    }

    public static class JudgeStateHelper
    {
        public static readonly IReadOnlyDictionary<JudgeState, string> DisplayNames = new Dictionary<JudgeState, string>
        {
            [JudgeState.Accepted] = "Accepted",
            [JudgeState.CompileError] = "Compile Error",
            [JudgeState.Compiling] = "Compiling",
            [JudgeState.Judging] = "Judging",
            [JudgeState.MemoryLimitExceeded] = "Memory Limit Exceeded",
            [JudgeState.Pending] = "Pending",
            [JudgeState.RuntimeError] = "Runtime Error",
            [JudgeState.SystemError] = "System Error",
            [JudgeState.TimeLimitExceeded] = "Time Limit Exceeded",
            [JudgeState.WrongAnswer] = "Wrong Answer",
        };
    }
}
