using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LocalJudge.Server.Host.APIClients;

namespace LocalJudge.Server.Host.Pages.Submissions
{
    public class SubmissionItem
    {
        public SubmissionMetadata Metadata { get; set; }

        public SubmissionResult Result { get; set; }

        public TimeSpan TotalTime { get; set; }

        public long MaximumMemory { get; set; }

        public int TotalCase { get; set; }

        public int AcceptedCase { get; set; }

        public ProblemMetadata Problem { get; set; }

        public static async Task<SubmissionItem> Get(SubmissionMetadata metadata, HttpClient client)
        {
            var res = new SubmissionItem
            {
                Metadata = metadata,
                TotalTime = TimeSpan.FromTicks(0),
                MaximumMemory = 0,
                TotalCase = 0,
                AcceptedCase = 0,
            };
            try
            {
                var scli = new SubmissionsClient(client);
                res.Result = await scli.GetResultAsync(metadata.Id);
                if (res.Result.Samples != null)
                    foreach (var v in res.Result.Samples)
                    {
                        res.TotalTime += v.Time;
                        res.MaximumMemory = Math.Max(res.MaximumMemory, v.Memory);
                        res.TotalCase++;
                        if (v.State == JudgeState.Accepted) res.AcceptedCase++;
                    }
                if (res.Result.Tests != null)
                    foreach (var v in res.Result.Tests)
                    {
                        res.TotalTime += v.Time;
                        res.MaximumMemory = Math.Max(res.MaximumMemory, v.Memory);
                        res.TotalCase++;
                        if (v.State == JudgeState.Accepted) res.AcceptedCase++;
                    }
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