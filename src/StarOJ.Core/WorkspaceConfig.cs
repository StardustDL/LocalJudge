using StarOJ.Core.Judgers;
using System.Collections.Generic;
using System.Linq;

namespace StarOJ.Core
{
    public class WorkspaceConfig
    {
        public Dictionary<ProgrammingLanguage, LanguageConfig> Languages { get; set; }

        public IEnumerable<ProgrammingLanguage> GetSupportLanguages() => from l in Languages.Keys select l;
    }
}
