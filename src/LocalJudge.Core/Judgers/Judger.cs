using LocalJudge.Core.Judgers.Comparers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LocalJudge.Core.Judgers
{
    public enum JudgeState
    {
        Pending,
        Judging,
        Accept,
        WrongAnswer,
        TimeLimitExceeded,
        MemoryLimitExceeded,
        RuntimeError,
        SystemError,
    }

    public class JudgeResult
    {
        public JudgeState State { get; set; }

        public List<Issue> Issues { get; set; }
    }

    public static class Judger
    {
        public static JudgeResult Judge(string name, string[] executor, TimeSpan timelimit, long memoryLimit, StreamReader input, StreamReader output, IJudgeComparer comparer)
        {
            JudgeResult res = new JudgeResult
            {
                State = JudgeState.Pending,
                Issues = new List<Issue>()
            };
            try
            {
                using (var runner = new Runner(new System.Diagnostics.ProcessStartInfo(executor[0], string.Join(" ", executor.Skip(1))))
                {
                    TimeLimit = timelimit,
                    MemoryLimit = memoryLimit,
                    Input = input,
                })
                {
                    runner.Run();
                    switch (runner.State)
                    {
                        case RunnerState.Ended:
                            {
                                if (runner.ExitCode != 0)
                                {
                                    res.Issues.Add(new Issue(IssueLevel.Error, $"Runtime error for {name}: exited with {runner.ExitCode}."));
                                    res.State = JudgeState.RuntimeError;
                                    break;
                                }
                                var expected = output;
                                var real = new StringReader(runner.Output);
                                var diff = comparer.Compare(expected, real).ToArray();
                                if (diff.Length != 0)
                                {
                                    res.Issues.AddRange(diff);
                                    res.State = JudgeState.WrongAnswer;
                                }
                                else
                                {
                                    // TODO:
                                    if (timelimit.TotalSeconds / runner.RunningTime.TotalSeconds < 2)
                                        res.Issues.Add(new Issue(IssueLevel.Warning, $"The time limit is too small for {name}. It used {runner.RunningTime.TotalSeconds} seconds"));
                                    if ((double)memoryLimit / runner.MaximumMemory < 2)
                                        res.Issues.Add(new Issue(IssueLevel.Warning, $"The memory limit is too small for {name}. It used {runner.MaximumMemory} bytes"));
                                    res.State = JudgeState.Accept;
                                }
                                break;
                            }
                        case RunnerState.OutOfMemory:
                            {
                                var message = $"Used {runner.MaximumMemory} bytes, limit {memoryLimit} bytes.";
                                res.State = JudgeState.MemoryLimitExceeded;
                                break;
                            }
                        case RunnerState.OutOfTime:
                            {
                                var message = $"Used {runner.RunningTime.TotalSeconds} seconds, limit {timelimit.TotalSeconds} seconds.";
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
