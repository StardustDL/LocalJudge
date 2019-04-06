using Microsoft.Extensions.DependencyInjection;
using StarOJ.Core;
using StarOJ.Core.Helpers;
using StarOJ.Core.Judgers;
using StarOJ.Core.Submissions;
using StarOJ.Server.API.Clients;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StarOJ.Server.Judger
{
    class Program
    {
        const string PipeStreamName = "StarOJ.Server.Judger";

        const int C_MaxThread = 5;

        static string ConfigPath { get; set; }

        static string TempDirectory { get; set; }

        static Dictionary<ProgrammingLanguage, JudgerLangConfig> Configs;

        static IHttpClientFactory HttpClientFactory = null;

        public static string ApiServer
        {
            get => API.Clients.BaseClient.Url;
            set => API.Clients.BaseClient.Url = value;
        }

        static void Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                Description = "Judger for StarOJ."
            };

            rootCommand.AddOption(new Option(new string[] { "--api-server", "-s" }, "The server of StarOJ API.", new Argument<string>("https://localhost:5001")));
            rootCommand.AddOption(new Option(new string[] { "--dir", "-d" }, "The path of temp directory.", new Argument<string>("")));
            rootCommand.AddOption(new Option(new string[] { "--config", "-c" }, "The path of judger config.", new Argument<string>("")));
            rootCommand.Handler = CommandHandler.Create((string apiServer, string dir, string config) =>
            {
                ApiServer = apiServer;
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.SetCurrentDirectory(dir);
                }
                ConfigPath = config;
            });

            int cmdExitCode = rootCommand.InvokeAsync(args).Result;

            if (cmdExitCode != 0) return;

            TempDirectory = Directory.GetCurrentDirectory();

            if (string.IsNullOrEmpty(ConfigPath))
            {
                Configs = new Dictionary<ProgrammingLanguage, JudgerLangConfig>();
                foreach (var v in StarOJ.Core.Judgers.Judger.DefaultLangConfigs)
                {
                    Configs.Add(v.Key, v.Value);
                }
            }
            else
            {
                Configs = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<ProgrammingLanguage, JudgerLangConfig>>(TextIO.ReadAllInUTF8(ConfigPath));
            }

            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddHttpClient();
                var services = serviceCollection.BuildServiceProvider();
                HttpClientFactory = services.GetService<IHttpClientFactory>();
            }

            Console.WriteLine("Judger started.");
            Console.WriteLine($"Workingspace: {TempDirectory}");
            Console.WriteLine($"Config path: {(string.IsNullOrEmpty(ConfigPath) ? "Use default" : ConfigPath)}");

            ThreadPool.SetMaxThreads(C_MaxThread, C_MaxThread);
            Console.WriteLine($"Maximum threads: {C_MaxThread}");

            var buffer = new byte[1 << 15];
            string recieved = "";
            while (true)
            {
                using (var pipe = new NamedPipeServerStream(PipeStreamName, PipeDirection.In))
                {
                    pipe.WaitForConnection();
                    var number = pipe.Read(buffer);
                    recieved = Encoding.UTF8.GetString(buffer, 0, number);
                }
                Console.WriteLine($"Recieved submission: {recieved}");
                ThreadPool.QueueUserWorkItem(JudgeSubmission, recieved, false);
                Console.WriteLine($"Queued submission: {recieved}");
            }
        }

        static void JudgeSubmission(string id)
        {
            try
            {
                JudgeSubmissionAsync(id).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Judging Failed for submission: {id}.");
                Console.WriteLine(ex.ToString());
            }
        }

        static string GetCodePath(ProgrammingLanguage lang)
        {
            switch (lang)
            {
                case ProgrammingLanguage.Java:
                    return $"Main.java";
                default:
                    return $"code.{ProgrammingLanguageHelper.Extends[lang]}";
            }
        }

        static async Task JudgeSubmissionAsync(string id)
        {
            Console.WriteLine($"Judging submission: {id}");

            var httpclient = HttpClientFactory.CreateClient();
            var scli = new SubmissionsClient(httpclient);
            var pcli = new ProblemsClient(httpclient);

            SubmissionResult result = new SubmissionResult
            {
                State = JudgeState.Pending,
                Issues = new List<Issue>(),
                Samples = new List<JudgeResult>(),
                Tests = new List<JudgeResult>(),
            };
            var submission = await scli.GetAsync(id);

            try
            {

                var problem = await pcli.GetAsync(submission.ProblemId);
                string rootDir = Path.Join(TempDirectory, submission.Id);
                if (!Directory.Exists(rootDir)) Directory.CreateDirectory(rootDir);
                string codePath = Path.Join(rootDir, GetCodePath(submission.Language));

                if (problem != null)
                {
                    if (Configs.ContainsKey(submission.Language))
                    {
                        var lang = Configs[submission.Language];

                        var code = await scli.GetCodeAsync(submission.Id);

                        using (var fs = File.Open(codePath, FileMode.Create))
                            await code.Stream.CopyToAsync(fs);

                        CompileResult compileResult = null;

                        if (lang.CompileCommand != null)
                        {
                            result.State = JudgeState.Compiling;
                            await scli.SetResultAsync(submission.Id, result);
                            string outputPath = Path.Join(rootDir, Path.GetFileNameWithoutExtension(codePath));

                            // java Main.java -> Main.class
                            // gcc/g++ code.c -> code.(out/exe)
                            // csc code.cs -> code.exe

                            compileResult = await Compiler.Compile(lang.CompileCommand, rootDir, codePath, outputPath, lang.CompileTimeLimit, lang.CompileMemoryLimit);
                            if (compileResult.State != CompileState.Compiled)
                            {
                                result.Issues.Add(new Issue(IssueLevel.Error, "Compiled error with internal error: " + compileResult.State.ToString()));
                                result.Issues.AddRange(compileResult.Issues);
                                result.State = JudgeState.CompileError;
                            }

                            result.State = JudgeState.Pending;
                        }

                        if (result.State == JudgeState.Pending)
                        {
                            result.State = JudgeState.Judging;
                            await scli.SetResultAsync(submission.Id, result);

                            Dictionary<string, string> vars = new Dictionary<string, string>()
                            {
                                [Core.Judgers.Judger.V_CodeFile] = codePath,
                            };
                            if (compileResult != null)
                                vars.Add(Core.Judgers.Judger.V_CompileOutput, compileResult.OutputPath);

                            var comparer = new Core.Judgers.Comparers.LineByLine();

                            var samples = await pcli.GetSamplesAsync(problem.Id);
                            foreach (var item in samples)
                            {
                                var casemdata = await pcli.GetSampleAsync(problem.Id, item.Id);
                                JudgeResult res = null;
                                using (var fin = await pcli.GetSampleInputAsync(problem.Id, item.Id))
                                using (var fout = await pcli.GetSampleOutputAsync(problem.Id, item.Id))
                                using (var input = new StreamReader(fin.Stream))
                                using (var output = new StreamReader(fout.Stream))
                                    res = await Core.Judgers.Judger.Judge(casemdata.Id, lang.RunCommand.Resolve(vars), rootDir, casemdata.TimeLimit, casemdata.MemoryLimit, input, output, comparer);
                                result.Samples.Add(res);
                            }

                            var tests = await pcli.GetTestsAsync(problem.Id);
                            foreach (var item in tests)
                            {
                                var casemdata = await pcli.GetTestAsync(problem.Id, item.Id);
                                JudgeResult res = null;
                                using (var fin = await pcli.GetTestInputAsync(problem.Id, item.Id))
                                using (var fout = await pcli.GetTestOutputAsync(problem.Id, item.Id))
                                using (var input = new StreamReader(fin.Stream))
                                using (var output = new StreamReader(fout.Stream))
                                    res = await Core.Judgers.Judger.Judge(casemdata.Id, lang.RunCommand.Resolve(vars), rootDir, casemdata.TimeLimit, casemdata.MemoryLimit, input, output, comparer);
                                result.Tests.Add(res);
                            }

                            result.State = JudgeState.Pending;
                        }
                    }
                    else
                    {
                        result.Issues.Add(new Issue(IssueLevel.Error, "No support for language " + submission.Language.ToString()));
                        result.State = JudgeState.SystemError;
                    }
                }
                else
                {
                    result.Issues.Add(new Issue(IssueLevel.Error, "No problem " + submission.ProblemId));
                    result.State = JudgeState.SystemError;
                }

                if (result.State == JudgeState.Pending)
                {
                    Dictionary<JudgeState, uint> cnt = new Dictionary<JudgeState, uint>
                    {
                        [JudgeState.SystemError] = 0,
                        [JudgeState.MemoryLimitExceeded] = 0,
                        [JudgeState.TimeLimitExceeded] = 0,
                        [JudgeState.WrongAnswer] = 0,
                        [JudgeState.RuntimeError] = 0,
                    };
                    foreach (var v in result.Samples) if (cnt.ContainsKey(v.State)) cnt[v.State]++;
                    foreach (var v in result.Tests) if (cnt.ContainsKey(v.State)) cnt[v.State]++;
                    if (cnt[JudgeState.SystemError] > 0) result.State = JudgeState.SystemError;
                    else if (cnt[JudgeState.RuntimeError] > 0) result.State = JudgeState.RuntimeError;
                    else if (cnt[JudgeState.MemoryLimitExceeded] > 0) result.State = JudgeState.MemoryLimitExceeded;
                    else if (cnt[JudgeState.TimeLimitExceeded] > 0) result.State = JudgeState.TimeLimitExceeded;
                    else if (cnt[JudgeState.WrongAnswer] > 0) result.State = JudgeState.WrongAnswer;
                    else result.State = JudgeState.Accepted;

                    TimeSpan totalTime = TimeSpan.FromTicks(0);
                    long maxMemory = 0;
                    int totalCase = 0;
                    int acceptCase = 0;
                    if (result.Samples != null)
                        foreach (var v in result.Samples)
                        {
                            totalTime += v.Time;
                            maxMemory = Math.Max(maxMemory, v.Memory);
                            totalCase++;
                            if (v.State == JudgeState.Accepted) acceptCase++;
                        }
                    if (result.Tests != null)
                        foreach (var v in result.Tests)
                        {
                            totalTime += v.Time;
                            maxMemory = Math.Max(maxMemory, v.Memory);
                            totalCase++;
                            if (v.State == JudgeState.Accepted) acceptCase++;
                        }

                    result.TotalTime = totalTime;
                    result.MaximumMemory = maxMemory;
                    result.TotalCase = totalCase;
                    result.AcceptedCase = acceptCase;
                }
            }
            catch (Exception ex)
            {
                result.State = JudgeState.SystemError;
                result.Issues.Add(new Issue(IssueLevel.Error, ex.ToString()));
            }

            result.HasIssue = result.Issues.Count + result.Samples.Sum(x => x.Issues?.Count ?? 0) + result.Tests.Sum(x => x.Issues?.Count ?? 0) > 0;
            await scli.SetResultAsync(submission.Id, result);

            Console.WriteLine($"Judged submission: {id}");
        }
    }
}
