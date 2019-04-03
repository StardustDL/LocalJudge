using StarOJ.Core;
using StarOJ.Core.Judgers;
using StarOJ.Core.Submissions;
using StarOJ.Data.Provider.FileSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace StarOJr.Server.Judger.FileSystem
{
    class Program
    {
        const string PipeStreamName = "StarOJr.Server.Judger";

        static Workspace Workspace { get; set; }

        static WorkspaceConfig WSConfig { get; set; }

        static void Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                Description = "Judger for StarOJ."
            };

            rootCommand.AddOption(new Option(new string[] { "--dir", "-d" }, "The path of working directory.", new Argument<string>("")));
            rootCommand.Handler = CommandHandler.Create((string dir) =>
            {
                if (!string.IsNullOrEmpty(dir))
                    Directory.SetCurrentDirectory(dir);
            });

            // Parse the incoming args and invoke the handler
            int cmdExitCode = rootCommand.InvokeAsync(args).Result;

            if (cmdExitCode != 0) return;

            Workspace = new Workspace(Directory.GetCurrentDirectory());
            WSConfig = Workspace.GetConfig();

            Console.WriteLine("Judger started.");
            Console.WriteLine($"Working directory: {Workspace.Root}");

            const int C_MaxThread = 1;
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
            Console.WriteLine($"Judging submission: {id}");

            SubmissionResult result = new SubmissionResult
            {
                State = JudgeState.Pending
            };

            var submission = Workspace.Submissions.Get(id) as SubmissionProvider;
            var submdata = submission.GetMetadata();
            var problem = Workspace.Problems.Get(submdata.ProblemId);
            string codePath = Path.Join(submission.Root, submdata.CodeName);

            if (problem != null)
            {
                if (WSConfig.Languages.ContainsKey(submdata.Language))
                {
                    var lang = WSConfig.Languages[submdata.Language];

                    CompileResult compileResult = null;

                    if (lang.CompileCommand != null)
                    {
                        result.State = JudgeState.Compiling;
                        submission.SetResult(result);
                        string outputPath = Path.Join(submission.Root, Path.GetFileNameWithoutExtension(codePath));

                        // java Main.java -> Main.class
                        // gcc/g++ code.c -> code.(out/exe)
                        // csc code.cs -> code.exe

                        compileResult = Compiler.Compile(lang.CompileCommand, submission.Root, codePath, outputPath, lang.CompileTimeLimit, lang.CompileMemoryLimit);
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
                        submission.SetResult(result);

                        Dictionary<string, string> vars = new Dictionary<string, string>()
                        {
                            [StarOJ.Core.Judgers.Judger.V_CodeFile] = codePath,
                        };
                        if (compileResult != null)
                            vars.Add(StarOJ.Core.Judgers.Judger.V_CompileOutput, compileResult.OutputPath);

                        var comparer = new StarOJ.Core.Judgers.Comparers.LineByLine();

                        foreach (TestCaseProvider item in problem.GetSamples())
                        {
                            var casemdata = item.GetMetadata();
                            JudgeResult res = null;
                            using (var input = File.OpenText(item.Input))
                            using (var output = File.OpenText(item.Output))
                                res = StarOJ.Core.Judgers.Judger.Judge(casemdata.Id, lang.RunCommand.Resolve(vars), submission.Root, casemdata.TimeLimit, casemdata.MemoryLimit, input, output, comparer);
                            result.Samples.Add(res);
                        }
                        foreach (TestCaseProvider item in problem.GetTests())
                        {
                            var casemdata = item.GetMetadata();
                            JudgeResult res = null;
                            using (var input = File.OpenText(item.Input))
                            using (var output = File.OpenText(item.Output))
                                res = StarOJ.Core.Judgers.Judger.Judge(casemdata.Id, lang.RunCommand.Resolve(vars), submission.Root, casemdata.TimeLimit, casemdata.MemoryLimit, input, output, comparer);
                            result.Tests.Add(res);
                        }

                        result.State = JudgeState.Pending;
                    }
                }
                else
                {
                    result.Issues.Add(new Issue(IssueLevel.Error, "No support for language " + submdata.Language.ToString()));
                    result.State = JudgeState.SystemError;
                }
            }
            else
            {
                result.Issues.Add(new Issue(IssueLevel.Error, "No problem " + submdata.ProblemId));
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

            submission.SetResult(result);

            Console.WriteLine($"Judged submission: {id}");
        }
    }
}
