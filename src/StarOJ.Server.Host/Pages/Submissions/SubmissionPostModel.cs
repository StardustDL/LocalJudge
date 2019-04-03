using StarOJ.Server.Host.APIClients;
using System.ComponentModel.DataAnnotations;

namespace StarOJ.Server.Host.Pages.Submissions
{
    public class SubmissionPostModel
    {
        public string ProblemId { get; set; }

        public string Id { get; set; }

        public ProgrammingLanguage Language { get; set; }
    }
}