using StarOJ.Core.Judgers;
using System.Collections.Generic;

namespace StarOJ.Core.Identity
{
    public class UserStatistics
    {
        public int SubmissionCount { get; set; }

        public Dictionary<ProgrammingLanguage, int> SubmissionLanguageCount { get; set; }

        public Dictionary<JudgeState, int> SubmissionStateCount { get; set; }

        public HashSet<string> ProblemAcceptedId { get; set; }
    }
}
