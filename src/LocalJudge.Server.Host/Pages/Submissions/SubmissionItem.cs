using System.Net.Http;
using System.Threading.Tasks;
using LocalJudge.Server.Host.APIClients;

namespace LocalJudge.Server.Host.Pages.Submissions
{
    public class SubmissionItem
    {
        public SubmissionMetadata Metadata { get; set; }

        public SubmissionResult Result { get; set; }

        public ProblemMetadata Problem { get; set; }

        public static async Task<SubmissionItem> Get(SubmissionMetadata metadata,HttpClient client)
        {
            var res = new SubmissionItem
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
                res.Problem = await pcli.GetAsync(metadata.ProblemID);
            }
            catch
            {
                res.Problem = new ProblemMetadata
                {
                    Id = res.Metadata.ProblemID,
                    Name = $"ID: {res.Metadata.ProblemID}"
                };
            }
            return res;
        }
    }
}