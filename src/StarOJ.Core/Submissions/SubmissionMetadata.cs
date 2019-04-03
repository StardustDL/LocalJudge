using StarOJ.Core.Judgers;
using System;

namespace StarOJ.Core.Submissions
{
    public class SubmissionMetadata : IHasId<string>
    {
        public string Id { get; set; }

        public string ProblemId { get; set; }

        public string UserId { get; set; }

        public ProgrammingLanguage Language { get; set; }

        public DateTimeOffset Time { get; set; }

        public uint CodeLength { get; set; }

        public string CodeName { get; set; }
    }
}
