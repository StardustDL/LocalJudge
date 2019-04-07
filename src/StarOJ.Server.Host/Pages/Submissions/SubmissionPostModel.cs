using StarOJ.Core.Judgers;

namespace StarOJ.Server.Host.Pages.Submissions
{
    public class SubmissionPostModel
    {
        public string ProblemId { get; set; }

        public string Id { get; set; }

        public ProgrammingLanguage Language { get; set; }

        public string QueryJudgeState { get; set; }

        public string QueryLanguage { get; set; }

        public string UserId { get; set; }
    }
}