using StarOJ.Core.Identity;
using StarOJ.Core.Problems;
using StarOJ.Server.API.Clients;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace StarOJ.Server.Host.Pages.Problems
{
    public class ProblemModel
    {
        public ProblemMetadata Metadata { get; set; }

        public ProblemDescription Description { get; set; }

        public IList<TestCaseMetadata> Samples { get; set; }

        public IList<TestCaseMetadata> Tests { get; set; }

        public UserMetadata User { get; set; }

        public ProblemStatistics Statistics { get; set; }

        public string AcceptedRate
        {
            get
            {
                if (Statistics == null || Statistics.SubmissionCount == 0) return "N/A";
                double res = (double)Statistics.SubmissionAcceptedCount / Statistics.SubmissionCount;
                return string.Format("{0:f1}%", res * 100);
            }
        }

        public static async Task<ProblemModel> GetAsync(ProblemMetadata metadata, HttpClient client, bool loadDescription, bool loadData)
        {
            ProblemModel res = new ProblemModel
            {
                Metadata = metadata,
            };

            ProblemsClient pcli = new ProblemsClient(client);

            {
                try
                {
                    StatisticsClient stcli = new StatisticsClient(client);
                    res.Statistics = await stcli.GetProblemAsync(metadata.Id);
                }
                catch
                {
                    res.Statistics = null;
                }
            }

            if (loadDescription)
            {
                try
                {
                    res.Description = await pcli.GetDescriptionAsync(metadata.Id);
                }
                catch { }
            }

            if (loadData)
            {
                try
                {
                    res.Samples = await pcli.GetSamplesAsync(metadata.Id);
                }
                catch
                {
                    res.Samples = Array.Empty<TestCaseMetadata>();
                }

                try
                {
                    res.Tests = await pcli.GetTestsAsync(metadata.Id);
                }
                catch
                {
                    res.Tests = Array.Empty<TestCaseMetadata>();
                }
            }

            {
                UsersClient ucli = new UsersClient(client);
                try
                {
                    res.User = await ucli.GetAsync(metadata.UserId);
                }
                catch
                {
                    res.User = null;
                }
            }

            return res;
        }
    }
}