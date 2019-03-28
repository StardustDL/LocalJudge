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
        CompileError,
        SystemError,
    }
}
