using StarOJ.Core.Judgers;
using StarOJ.Server.API.Clients;
using System.ComponentModel.DataAnnotations;

namespace StarOJ.Server.Host.Pages.Submissions
{
    public class SubmissionPostModel
    {
        public string ProblemId { get; set; }

        [Required]
        public string Id { get; set; }

        public ProgrammingLanguage Language { get; set; }
    }
}