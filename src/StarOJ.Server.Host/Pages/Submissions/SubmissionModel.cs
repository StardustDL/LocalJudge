using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using StarOJ.Core.Identity;
using StarOJ.Core.Judgers;
using StarOJ.Core.Problems;
using StarOJ.Core.Submissions;
using StarOJ.Server.API.Clients;

namespace StarOJ.Server.Host.Pages.Submissions
{
    public class SubmissionModel
    {
        public SubmissionMetadata Metadata { get; set; }

        public SubmissionResult Result { get; set; }

        public ProblemMetadata Problem { get; set; }

        public UserMetadata User { get; set; }

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
            try
            {
                var ucli = new UsersClient(client);
                res.User = await ucli.GetAsync(metadata.UserId);
            }
            catch
            {
                res.User = new UserMetadata
                {
                    Id = res.Metadata.UserId,
                    Name = $"Not found: {res.Metadata.UserId}"
                };
            }

            return res;
        }
    }
}