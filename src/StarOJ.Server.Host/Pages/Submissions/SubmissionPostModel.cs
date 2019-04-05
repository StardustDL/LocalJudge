using StarOJ.Server.Host.APIClients;
using System.ComponentModel.DataAnnotations;

namespace StarOJ.Server.Host.Pages.Submissions
{
    // For code download
    public class SubmissionPostModel
    {
        [Required]
        public string ProblemId { get; set; }

        [Required]
        public string Id { get; set; }

        [Required]
        public ProgrammingLanguage Language { get; set; }
    }
}