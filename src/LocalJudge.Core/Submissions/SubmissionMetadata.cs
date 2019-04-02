using LocalJudge.Core.Judgers;
using System;

namespace LocalJudge.Core.Submissions
{
    public class SubmissionMetadata : IHasId<string>
    {
        public string Id { get; set; }

        public string ProblemId { get; set; }

        public ProgrammingLanguage Language { get; set; }

        public DateTimeOffset Time { get; set; }

        public uint CodeLength { get; set; }

        public string CodePath { get; set; }
    }
}
