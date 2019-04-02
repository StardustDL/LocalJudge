using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LocalJudge.Server.Host.APIClients;

namespace LocalJudge.Server.Host.Pages.Submissions
{
    public class SubmissionModel
    {
        public SubmissionMetadata Metadata { get; set; }

        public SubmissionResult Result { get; set; }

        public ProblemMetadata Problem { get; set; }

        public static async Task<SubmissionModel> GetAsync(SubmissionMetadata metadata, HttpClient client)
        {
            var res = new SubmissionModel
            {
                Metadata = metadata
            };
            try
            {
                var scli = new SubmissionsClient(client);
                res.Result = await scli.GetResultAsync(metadata.Id);
            }
            catch
            {
                res.Result = new SubmissionResult
                {
                    State = JudgeState.Pending
                };
            }
            try
            {
                var pcli = new ProblemsClient(client);
                res.Problem = await pcli.GetAsync(metadata.ProblemId);
            }
            catch
            {
                res.Problem = new ProblemMetadata
                {
                    Id = res.Metadata.ProblemId,
                    Name = $"Not found: {res.Metadata.ProblemId}"
                };
            }
            return res;
        }
    }
}