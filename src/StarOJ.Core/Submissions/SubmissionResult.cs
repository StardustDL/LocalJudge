using StarOJ.Core.Judgers;
using System;
using System.Collections.Generic;

namespace StarOJ.Core.Submissions
{
    public class SubmissionResult
    {
        public JudgeState State { get; set; }

        public List<JudgeResult> Samples { get; set; }

        public List<JudgeResult> Tests { get; set; }

        public List<Issue> Issues { get; set; }

        public bool HasIssue { get; set; }

        public TimeSpan? TotalTime { get; set; }

        public long? MaximumMemory { get; set; }

        public int? TotalCase { get; set; }

        public int? AcceptedCase { get; set; }
    }
}
