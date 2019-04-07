using Microsoft.AspNetCore.Http;
using StarOJ.Core.Problems;
using StarOJ.Server.API.Models;

namespace StarOJ.Server.Host.Pages.Problems
{
    public class ProblemPostModel
    {
        public string QueryId { get; set; }

        public string QueryUserId { get; set; }

        public string QueryName { get; set; }

        public string QuerySource { get; set; }

        public SubmitData SubmitData { get; set; }

        public ProblemDescription Description { get; set; }

        public ProblemMetadata Metadata { get; set; }

        public IFormFile ImportFile { get; set; }
    }
}
