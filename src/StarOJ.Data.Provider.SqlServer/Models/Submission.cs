using StarOJ.Core.Judgers;
using System;

namespace StarOJ.Data.Provider.SqlServer.Models
{
    public class Submission
    {
        #region Metadata

        public int Id { get; set; }

        public int ProblemId { get; set; }

        public int UserId { get; set; }

        public ProgrammingLanguage Language { get; set; }

        public DateTimeOffset Time { get; set; }

        public uint CodeLength { get; set; }

        #endregion

        #region Result

        public JudgeState State { get; set; }

        public string SampleResults { get; set; }

        public string TestResults { get; set; }

        public string Issues { get; set; }

        public bool HasIssue { get; set; }

        // Ticks
        public long? MaximumTime { get; set; }

        public long? MaximumMemory { get; set; }

        public int? TotalCase { get; set; }

        public int? AcceptedCase { get; set; }

        #endregion
    }
}
