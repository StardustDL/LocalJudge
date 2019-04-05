using StarOJ.Core.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace StarOJ.Core.Problems
{
    public class TestCaseMetadata : IHasId<string>
    {
        public const long MaximumMemoryLimit = MemoryValueHelper.TB, MinimumMemoryLimit = MemoryValueHelper.MB;

        [Required]
        public string Id { get; set; }

        [Required]
        [DataType(DataType.Duration)]
        public TimeSpan TimeLimit { get; set; }

        [Required]
        [Range(MinimumMemoryLimit, MaximumMemoryLimit)]
        public long MemoryLimit { get; set; }
    }
}
