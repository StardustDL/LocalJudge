using StarOJ.Core.Helpers;
using System;

namespace StarOJ.Core.Judgers
{
    public class LanguageConfig
    {
        public TimeSpan CompileTimeLimit { get; set; }

        public long CompileMemoryLimit { get; set; }

        public Command CompileCommand { get; set; }

        public Command RunCommand { get; set; }
    }
}
