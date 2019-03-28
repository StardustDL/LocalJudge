using LocalJudge.Core.Judgers;
using System.Collections.Generic;

namespace LocalJudge.Core
{
    public class WorkspaceConfig
    {
        public Dictionary<ProgrammingLanguage, LanguageConfig> Languages { get; set; }
    }
}
