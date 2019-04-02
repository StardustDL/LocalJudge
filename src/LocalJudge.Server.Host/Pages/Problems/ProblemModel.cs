using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LocalJudge.Server.Host.APIClients;

namespace LocalJudge.Server.Host.Pages.Problems
{
    public class ProblemModel
    {
        public ProblemMetadata Metadata { get; set; }

        public ProblemDescription Description { get; set; }

        public IList<TestCaseMetadata> Samples { get; set; }

        public IList<TestCaseMetadata> Tests { get; set; }

        public static async Task<ProblemModel> GetAsync(ProblemMetadata metadata, HttpClient client, bool loadDescription, bool loadData)
        {
            var res = new ProblemModel
            {
                Metadata = metadata,
            };

            var pcli = new ProblemsClient(client);

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

            return res;
        }
    }
}