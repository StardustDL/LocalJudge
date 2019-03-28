using System;
using System.Collections.Generic;

namespace LocalJudge.Core.Judgers
{
    public class JudgeResult
    {
        public string ID { get; set; }

        public JudgeState State { get; set; }

        public TimeSpan Time { get; set; }

        // Byte
        public long Memory { get; set; }

        public List<Issue> Issues { get; set; }
    }
}
