using System;

namespace LocalJudge.Core.Problems
{
    public class TestCaseMetadata
    {
        public string ID { get; set; }

        public TimeSpan TimeLimit { get; set; }

        public long MemoryLimit { get; set; }
    }
}
