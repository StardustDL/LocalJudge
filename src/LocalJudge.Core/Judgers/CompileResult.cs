using System;
using System.Collections.Generic;

namespace LocalJudge.Core.Judgers
{
    public class CompileResult
    {
        public CompileState State { get; set; }

        public TimeSpan Time { get; set; }

        // Byte
        public long Memory { get; set; }

        public List<Issue> Issues { get; set; }

        public string OutputPath { get; set; }
    }
}
