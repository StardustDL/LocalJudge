using Newtonsoft.Json;
using StarOJ.Core;
using StarOJ.Core.Identity;
using StarOJ.Core.Judgers;
using StarOJ.Core.Problems;
using StarOJ.Core.Submissions;
using StarOJ.Data.Provider.SqlServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.SqlServer
{
    public class Workspace : IWorkspace
    {
        private readonly OJContext _context;

        public IProblemListProvider Problems { get; private set; }

        public ISubmissionListProvider Submissions { get; private set; }

        public IUserListProvider Users { get; private set; }

        public IRoleListProvider Roles { get; private set; }

        public bool HasInitialized { get => _context.WorkspaceInfos.FirstOrDefault() != null; }

        public Task<WorkspaceConfig> GetConfig()
        {
            var config = _context.WorkspaceInfos.FirstOrDefault();
            if(config == null)
            {
                return Task.FromResult(new WorkspaceConfig
                {
                    Languages = new Dictionary<Core.Judgers.ProgrammingLanguage, Core.Judgers.LanguageConfig>()
                });
            }
            else
            {
                return Task.FromResult(JsonConvert.DeserializeObject<WorkspaceConfig>(config.Config));
            }
        }

        public async Task Initialize()
        {
            var langs = new Dictionary<ProgrammingLanguage, LanguageConfig>();
            foreach (var v in WorkspaceConfig.DefaultLanguages)
                langs.Add(v.Key, v.Value);
            WorkspaceConfig config = new WorkspaceConfig
            {
                Languages = langs
            };
            _context.WorkspaceInfos.RemoveRange(_context.WorkspaceInfos.ToArray());
            await _context.SaveChangesAsync();
            _context.WorkspaceInfos.Add(new WorkspaceInfo
            {
                Config = JsonConvert.SerializeObject(config)
            });
            await _context.SaveChangesAsync();
        }

        public Workspace(OJContext context)
        {
            _context = context;
        }
    }
}
