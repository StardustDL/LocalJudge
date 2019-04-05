using StarOJ.Core.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace StarOJ.Core.Judgers
{
    public class JudgerLangConfig
    {
        [DataType(DataType.Duration)]
        public TimeSpan CompileTimeLimit { get; set; }

        public long CompileMemoryLimit { get; set; }

        public Command CompileCommand { get; set; }

        public Command RunCommand { get; set; }
    }
}
