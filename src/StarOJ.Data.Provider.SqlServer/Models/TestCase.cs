using System;

namespace StarOJ.Data.Provider.SqlServer.Models
{
    public class TestCase
    {
        public string Id { get; set; }

        public string ProblemId { get; set; }

        public TimeSpan TimeLimit { get; set; }

        public long MemoryLimit { get; set; }

        public string Input { get; set; }

        public string Output { get; set; }
    }
}
