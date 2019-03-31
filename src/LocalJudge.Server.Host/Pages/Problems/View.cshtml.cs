using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LocalJudge.Server.Host.APIClients;
using Markdig;
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

        public MarkdownPipelineBuilder MarkdownBuilder { get; private set; }

        public ViewModel(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
            MarkdownBuilder = new MarkdownPipelineBuilder().UseAdvancedExtensions();
        }

        [BindProperty]
        public ProblemItemOperation PostData { get; set; }

        public ProblemMetadata Metadata { get; set; }

        public ProblemDescription Description { get; set; }

        public IList<TestCaseData> SampleData { get; set; }

        public IList<TestCaseMetadata> Samples { get; set; }

        public IList<TestCaseMetadata> Tests { get; set; }

        public string LanguageConfig { get; set; }

        public bool EnableCode { get; set; }

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

            try
            {
                Samples = await client.GetSamplesAsync(id);
            }
            catch
            {
                Samples = Array.Empty<TestCaseMetadata>();
            }

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

            try
            {
                Tests = await client.GetTestsAsync(id);
            }
            catch
            {
                Tests = Array.Empty<TestCaseMetadata>();
            }

            {
                StringBuilder res = new StringBuilder();
                var wclient = new WorkspaceClient(httpclient);
                IList<ProgrammingLanguage> langs;
                try
                {
                    langs = await wclient.GetSupportLanguagesAsync();
                }
                catch
                {
                    langs = Array.Empty<ProgrammingLanguage>();
                }
                if (langs.Count == 0)
                {
                    EnableCode = false;
                }
                else
                {
                    EnableCode = true;
                    foreach (var item in langs)
                    {
                        var editorId = Helper.GetEditorLanguage(item);
                        if (editorId == "plaintext") continue;

                        res.Append("{editorID: \"" + editorId + "\", ");
                        switch (item)
                        {
                            // editorID for editor, lang for enum, show for selector in html
                            case ProgrammingLanguage.C:
                                res.Append("lang: \"C\", show: \"C\"},");
                                break;
                            case ProgrammingLanguage.Cpp:
                                res.Append("lang: \"Cpp\", show: \"C++\"},");
                                break;
                            case ProgrammingLanguage.Java:
                                res.Append("lang: \"Java\", show: \"Java\"},");
                                break;
                            case ProgrammingLanguage.Python:
                                res.Append("lang: \"Python\", show: \"Python\"},");
                                break;
                            case ProgrammingLanguage.CSharp:
                                res.Append("lang: \"CSharp\", show: \"C#\"},");
                                break;
                            case ProgrammingLanguage.Rust:
                                res.Append("lang: \"Rust\", show: \"Rust\"},");
                                break;
                        }
                    }
                }
                LanguageConfig = res.ToString();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var httpclient = clientFactory.CreateClient();
            try
            {
                switch (PostData.Type)
                {
                    case ProblemItemOperationType.Submit:
                        {
                            var client = new SubmissionsClient(httpclient);
                            if (!ModelState.IsValid)
                            {
                                return BadRequest();
                            }
                            try
                            {
                                var meta = await client.SubmitAsync(PostData.SubmitData);
                                return Redirect($"/Submissions/View?id={meta.Id}");
                            }
                            catch
                            {
                                return NotFound();
                            }
                        }
                    case ProblemItemOperationType.Delete:
                        {
                            var client = new ProblemsClient(httpclient);
                            await client.DeleteAsync(PostData.ID);
                            return Redirect($"/Problems/Index");
                        }
                }
                return BadRequest();
            }
            catch
            {
                return NotFound();
            }
        }

    }
}