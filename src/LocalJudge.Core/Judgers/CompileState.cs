﻿namespace LocalJudge.Core.Judgers
{
    public enum CompileState
    {
        Pending,
        Compiling,
        Compiled,
        TimeLimitExceeded,
        MemoryLimitExceeded,
        RuntimeError,
        SystemError,
    }
}
