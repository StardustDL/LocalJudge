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
        public ProblemPostModel PostData { get; set; }

        public ProblemModel Problem { get; set; }

        public IList<TestCaseData> SampleData { get; set; }

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
                var metadata = await client.GetAsync(id);
                Problem = await ProblemModel.GetAsync(metadata, httpclient, true, true);
            }
            catch
            {
                return NotFound();
            }

            List<TestCaseData> samples = new List<TestCaseData>();
            foreach (var s in Problem.Samples)
            {
                var input = await client.GetSampleInputAsync(id, s.Id, int.MaxValue);
                var output = await client.GetSampleOutputAsync(id, s.Id, int.MaxValue);
                TestCaseData td = new TestCaseData
                {
                    Metadata = s,
                    Input = input.Content,
                    Output = output.Content,
                };
                samples.Add(td);
            }
            SampleData = samples;

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

                        res.Append("{editorId: \"" + editorId + "\", ");
                        switch (item)
                        {
                            // editorId for editor, lang for enum, show for selector in html
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
                            case ProgrammingLanguage.VisualBasic:
                                res.Append("lang: \"VisualBasic\", show: \"Visual Basic\"},");
                                break;
                            case ProgrammingLanguage.Go:
                                res.Append("lang: \"Go\", show: \"Go\"},");
                                break;
                            case ProgrammingLanguage.Haskell:
                                res.Append("lang: \"Haskell\", show: \"Haskell\"},");
                                break;
                            case ProgrammingLanguage.Javascript:
                                res.Append("lang: \"Javascript\", show: \"Javascript\"},");
                                break;
                            case ProgrammingLanguage.Kotlin:
                                res.Append("lang: \"Kotlin\", show: \"Kotlin\"},");
                                break;
                            case ProgrammingLanguage.Php:
                                res.Append("lang: \"Php\", show: \"PHP\"},");
                                break;
                            case ProgrammingLanguage.Ruby:
                                res.Append("lang: \"Ruby\", show: \"Ruby\"},");
                                break;
                            case ProgrammingLanguage.Scala:
                                res.Append("lang: \"Scala\", show: \"Scala\"},");
                                break;
                        }
                    }
                }
                LanguageConfig = res.ToString();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var httpclient = clientFactory.CreateClient();
            var client = new ProblemsClient(httpclient);
            try
            {
                await client.DeleteAsync(PostData.Id);
                return RedirectToPage("/Problems/Index");
            }
            catch
            {
                return NotFound();
            }
        }

        public IActionResult OnPostDataAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return RedirectToPage("/Problems/Data", new { id = PostData.Id });
        }

        public async Task<IActionResult> OnPostSubmitAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var httpclient = clientFactory.CreateClient();
            var client = new SubmissionsClient(httpclient);
            try
            {
                var meta = await client.SubmitAsync(PostData.SubmitData);
                return RedirectToPage("/Submissions/View", new { id = meta.Id });
            }
            catch
            {
                return NotFound();
            }
        }

    }
}