using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LocalJudge.Server.Host.APIClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LocalJudge.Server.Host.Pages.Problems
{
    public class ViewModel : PageModel
    {
        public class TestCaseData
        {
            public TestCaseMetadata Metadata { get; set; }

            public double TimeLimit { get => Metadata.TimeLimit.TotalSeconds; }

            public long MemoryLimit { get => Metadata.MemoryLimit / 1024 / 1024; }

            public string Input { get; set; }

            public string Output { get; set; }
        }

        private readonly IHttpClientFactory clientFactory;

        public ViewModel(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        [BindProperty]
        public SubmitData SubmitData { get; set; }

        public ProblemMetadata Metadata { get; set; }

        public ProblemDescription Description { get; set; }

        public IList<TestCaseData> SampleData { get; set; }

        public IList<TestCaseMetadata> Samples { get; set; }

        public IList<TestCaseMetadata> Tests { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
            try
            {
                Metadata = await client.GetAsync(id);
                Description = await client.GetDescriptionAsync(id);
            }
            catch
            {
                return NotFound();
            }

            Samples = await client.GetSamplesAsync(id);
            List<TestCaseData> samples = new List<TestCaseData>();
            foreach (var s in Samples)
            {
                TestCaseData td = new TestCaseData
                {
                    Metadata = s,
                    Input = await client.GetSampleInputAsync(id, s.Id),
                    Output = await client.GetSampleOutputAsync(id, s.Id),
                };
                samples.Add(td);
            }
            SampleData = samples;

            Tests = await client.GetTestsAsync(id);

            SubmitData = new SubmitData
            {
                Code = "",
                Language = ProgrammingLanguage.Cpp,
                ProblemID = Metadata.Id,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var httpclient = clientFactory.CreateClient();
            var client = new SubmissionsClient(httpclient);
            try
            {
                var meta = await client.SubmitAsync(SubmitData);
                return Redirect($"../Submissions/View?id={meta.Id}");
            }
            catch
            {
                return NotFound();
            }
        }
    }
}