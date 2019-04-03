using StarOJ.Core.Judgers;
using System;

namespace StarOJ.Data.Provider.SqlServer.Models
{
    public class Submission
    {
        #region Metadata

        public string Id { get; set; }

        public string ProblemId { get; set; }

        public string UserId { get; set; }

        public ProgrammingLanguage Language { get; set; }

        public DateTimeOffset Time { get; set; }

        public uint CodeLength { get; set; }

        public string Code { get; set; }

        #endregion

        #region Result

        public JudgeState State { get; set; }

        public string SampleResults { get; set; }

        public string TestResults { get; set; }

        public string Issues { get; set; }

        public bool HasIssue { get; set; }

        public TimeSpan? TotalTime { get; set; }

        public long? MaximumMemory { get; set; }

        public int? TotalCase { get; set; }

        public int? AcceptedCase { get; set; }

        #endregion
    }
}
