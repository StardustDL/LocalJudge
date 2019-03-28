using LocalJudge.Core.Judgers;
using System;
using System.Collections.Generic;

namespace LocalJudge.Core.Submissions
{
    public class SubmissionResult
    {
        public JudgeState State { get; set; }

        public List<JudgeResult> Samples { get; private set; } = new List<JudgeResult>();

        public List<JudgeResult> Tests { get; private set; } = new List<JudgeResult>();

        public List<Issue> Issues { get; private set; } = new List<Issue>();
    }
}
