using LocalJudge.Core.Helpers;
using LocalJudge.Core.Judgers.Comparers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LocalJudge.Core.Judgers
{
    public static class Judger
    {
        public const string V_CodeFile = "{codefile}", V_CompileOutput = "{compiled}";

        public static JudgeResult Judge(string name, Command executor,string workingDirectory, TimeSpan timeLimit, long memoryLimit, TextReader input, TextReader output, IJudgeComparer comparer)
        {
            JudgeResult res = new JudgeResult
            {
                Id = name,
                State = JudgeState.Pending,
                Issues = new List<Issue>()
            };
            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo(executor.Name, string.Join(" ", executor.Arguments))
                {
                    WorkingDirectory = workingDirectory
                };
                using (var runner = new Runner(startInfo)
                {
                    TimeLimit = timeLimit,
                    MemoryLimit = memoryLimit,
                    Input = input,
                })
                {
                    res.State = JudgeState.Judging;
                    runner.Run();
                    res.Time = runner.RunningTime;
                    res.Memory = runner.MaximumMemory;
                    if (!string.IsNullOrEmpty(runner.Error))
                        res.Issues.Add(new Issue(IssueLevel.Warning, "Error output: " + runner.Error));
                    switch (runner.State)
                    {
                        case RunnerState.Ended:
                            {
                                if (runner.ExitCode != 0)
                                {
                                    res.Issues.Add(new Issue(IssueLevel.Error, $"exited with {runner.ExitCode}."));
                                    res.State = JudgeState.RuntimeError;
                                    break;
                                }
                                var expected = output;
                                var real = new StringReader(runner.Output ?? "");
                                var diff = comparer.Compare(expected, real).ToArray();
                                if (diff.Length != 0)
                                {
                                    res.Issues.AddRange(diff);
                                    res.State = JudgeState.WrongAnswer;
                                }
                                else
                                {
                                    res.State = JudgeState.Accepted;
                                }
                                break;
                            }
                        case RunnerState.OutOfMemory:
                            {
                                var message = $"Used {runner.MaximumMemory} bytes, limit {memoryLimit} bytes.";
                                res.Issues.Add(new Issue(IssueLevel.Error, $"Memory limit exceeded for {name}. {message}"));
                                res.State = JudgeState.MemoryLimitExceeded;
                                break;
                            }
                        case RunnerState.OutOfTime:
                            {
                                var message = $"Used {runner.RunningTime.TotalSeconds} seconds, limit {timeLimit.TotalSeconds} seconds.";
                                res.Issues.Add(new Issue(IssueLevel.Error, $"Time limit exceeded for {name}. {message}"));
                                res.State = JudgeState.TimeLimitExceeded;
                                break;
                            }
                        default:
                            throw new Exception("The program doesn't stop.");
                    }
                }
            }
            catch (Exception ex)
            {
                res.Issues.Add(new Issue(IssueLevel.Error, $"System error for {name} with {ex.ToString()}."));
                res.State = JudgeState.SystemError;
            }
            return res;
        }
    }
}
