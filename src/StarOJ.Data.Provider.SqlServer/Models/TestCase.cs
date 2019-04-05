using System;

namespace StarOJ.Data.Provider.SqlServer.Models
{
    public class TestCase
    {
        public int Id { get; set; }

        public int ProblemId { get; set; }

        public TimeSpan TimeLimit { get; set; }

        public long MemoryLimit { get; set; }

        public bool IsSample { get; set; }
    }
}
