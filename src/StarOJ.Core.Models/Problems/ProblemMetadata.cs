using System.ComponentModel.DataAnnotations;

namespace StarOJ.Core.Problems
{
    public class ProblemMetadata
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string UserId { get; set; }

        public string Source { get; set; }
    }
}
