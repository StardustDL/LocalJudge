using LocalJudge.Server.Host.APIClients;
using System.ComponentModel.DataAnnotations;

namespace LocalJudge.Server.Host.Pages.Submissions
{
    public class SubmissionPostModel
    {
        public string ProblemId { get; set; }

        public string Id { get; set; }

        public ProgrammingLanguage Language { get; set; }
    }
}