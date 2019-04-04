using System;

namespace StarOJ.Data.Provider.SqlServer.Models
{
    public class TestCase
    {
        public int Id { get; set; }

        public int ProblemId { get; set; }

        public TimeSpan TimeLimit { get; set; }

        public long MemoryLimit { get; set; }

        public string Input { get; set; }

        public string Output { get; set; }
    }

    public class SampleCase
    {
        public int Id { get; set; }

        public int ProblemId { get; set; }

        public TimeSpan TimeLimit { get; set; }

        public long MemoryLimit { get; set; }

        public string Input { get; set; }

        public string Output { get; set; }
    }
}
