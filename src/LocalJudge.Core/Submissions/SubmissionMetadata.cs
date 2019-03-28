using LocalJudge.Core.Judgers;
using System;

namespace LocalJudge.Core.Submissions
{
    public class SubmissionMetadata
    {
        public string ID { get; set; }

        public string ProblemID { get; set; }

        public ProgrammingLanguage Language { get; set; }

        public DateTimeOffset Time { get; set; }
    }
}
