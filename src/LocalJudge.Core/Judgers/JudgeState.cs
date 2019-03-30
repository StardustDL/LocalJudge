namespace LocalJudge.Core.Judgers
{
    public enum JudgeState
    {
        Pending,
        Judging,
        Accepted,
        WrongAnswer,
        TimeLimitExceeded,
        MemoryLimitExceeded,
        RuntimeError,
        CompileError,
        SystemError,
    }
}
