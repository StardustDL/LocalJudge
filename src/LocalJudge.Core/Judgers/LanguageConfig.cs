using LocalJudge.Core.Helpers;
using System;

namespace LocalJudge.Core.Judgers
{
    public class LanguageConfig
    {
        public TimeSpan CompileTimeLimit { get; set; }

        public long CompileMemoryLimit { get; set; }

        public Command CompileCommand { get; set; }

        public Command RunCommand { get; set; }
    }
}
