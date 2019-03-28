using LocalJudge.Core;
using LocalJudge.Core.Judgers;
using LocalJudge.Core.Submissions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace LocalJudger.Server.Judger
{
    class Program
    {
        const string PipeStreamName = "LocalJudger.Server.Judger";
        static Workspace Workspace { get; set; }

        static WorkspaceConfig WSConfig { get; set; }

        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(@"G:\Program\LocalJudge\temp\test");
            Workspace = new Workspace(Directory.GetCurrentDirectory());
            WSConfig = Workspace.GetConfig();

            Console.WriteLine("Judger started.");

            ThreadPool.SetMaxThreads(1, 1);
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
            }
        }

        static void JudgeSubmission(string id)
        {
            SubmissionResult result = new SubmissionResult
            {
                State = JudgeState.Judging
            };

            var submission = Workspace.Submissions.Get(id);
            var submdata = submission.GetMetadata();
            var problem = Workspace.Problems.Get(submdata.ProblemID);

            if (problem != null)
            {
                if (WSConfig.Languages.ContainsKey(submdata.Language))
                {
                    var lang = WSConfig.Languages[submdata.Language];

                    CompileResult compileResult = null;

                    if (lang.CompileCommand != null)
                    {
                        compileResult = Compiler.Compile(lang.CompileCommand, submission.Code, Path.Join(submission.Root, "compiled"), lang.CompileTimeLimit, lang.CompileMemoryLimit);
                        if (compileResult.State != CompileState.Compiled)
                        {
                            result.Issues.Add(new Issue(IssueLevel.Error, "Compiled error with internal error: " + compileResult.State.ToString()));
                            result.Issues.AddRange(compileResult.Issues);
                            result.State = JudgeState.CompileError;
                        }
                    }

                    if (result.State == JudgeState.Judging)
                    {
                        Dictionary<string, string> vars = new Dictionary<string, string>()
                        {
                            [LocalJudge.Core.Judgers.Judger.V_CodeFile] = submission.Code,
                        };
                        if (compileResult != null)
                            vars.Add(LocalJudge.Core.Judgers.Judger.V_CompileOutput, compileResult.OutputPath);

                        var comparer = new LocalJudge.Core.Judgers.Comparers.LineByLine();

                        foreach (var item in problem.GetSamples())
                        {
                            var casemdata = item.GetMetadata();
                            var res = LocalJudge.Core.Judgers.Judger.Judge(casemdata.ID, lang.RunCommand.Resolve(vars), casemdata.TimeLimit, casemdata.MemoryLimit, File.OpenText(item.Input), File.OpenText(item.Output), comparer);
                            result.Samples.Add(res);
                        }
                        foreach (var item in problem.GetTests())
                        {
                            var casemdata = item.GetMetadata();
                            var res = LocalJudge.Core.Judgers.Judger.Judge(casemdata.ID, lang.RunCommand.Resolve(vars), casemdata.TimeLimit, casemdata.MemoryLimit, File.OpenText(item.Input), File.OpenText(item.Output), comparer);
                            result.Tests.Add(res);
                        }
                    }
                }
                else
                {
                    result.Issues.Add(new Issue(IssueLevel.Error, "No support for language " + submdata.Language.ToString()));
                }
            }
            else
            {
                result.Issues.Add(new Issue(IssueLevel.Error, "No problem " + submdata.ProblemID));
            }

            if (result.State == JudgeState.Judging)
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
                else result.State = JudgeState.Accept;
            }

            submission.SaveResult(result);
        }
    }
}
