using System;

namespace LocalJudge.Core.Problems
{
    public class TestCaseMetadata : IHasId<string>
    {
        public string Id { get; set; }

        public TimeSpan TimeLimit { get; set; }

        public long MemoryLimit { get; set; }
    }
}
