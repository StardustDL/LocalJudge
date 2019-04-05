using StarOJ.Core.Judgers;
using System;
using System.ComponentModel.DataAnnotations;

namespace StarOJ.Core.Submissions
{
    public class SubmissionMetadata : IHasId<string>
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string ProblemId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public ProgrammingLanguage Language { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTimeOffset Time { get; set; }

        public uint CodeLength { get; set; }

        public string Code { get; set; }
    }
}
