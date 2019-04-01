using System.ComponentModel.DataAnnotations;

namespace LocalJudge.Server.Host.Pages.Submissions
{
    public class SubmissionPostModel
    {
        public string ProblemID { get; set; }

        public string ID { get; set; }
    }
}