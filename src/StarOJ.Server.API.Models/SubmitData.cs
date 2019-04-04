using StarOJ.Core.Judgers;

namespace StarOJ.Server.API.Models
{
    public class SubmitData
    {
        public string ProblemId { get; set; }

        public string UserId { get; set; }

        public string Code { get; set; }

        public ProgrammingLanguage Language { get; set; }
    }
}
