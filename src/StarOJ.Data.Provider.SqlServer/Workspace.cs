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
            if (config == null)
            {
                return Task.FromResult(new WorkspaceConfig
                {
                    SupportLanguages = new List<ProgrammingLanguage>()
                });
            }
            else
            {
                return Task.FromResult(JsonConvert.DeserializeObject<WorkspaceConfig>(config.Config));
            }
        }

        public async Task Initialize()
        {
            var langs = new List<ProgrammingLanguage>();
            foreach (var v in Judger.DefaultLangConfigs)
                langs.Add(v.Key);
            WorkspaceConfig config = new WorkspaceConfig
            {
                SupportLanguages = langs
            };
            _context.WorkspaceInfos.RemoveRange(_context.WorkspaceInfos.ToArray());
            await _context.SaveChangesAsync();
            WorkspaceInfo empty = new WorkspaceInfo
            {
                Config = JsonConvert.SerializeObject(config)
            };
            _context.WorkspaceInfos.Add(empty);
            await _context.SaveChangesAsync();
        }

        public async Task Clear()
        {
            _context.WorkspaceInfos.RemoveRange(_context.WorkspaceInfos.ToArray());
            _context.Submissions.RemoveRange(_context.Submissions.ToArray());
            _context.Problems.RemoveRange(_context.Problems.ToArray());
            _context.Users.RemoveRange(_context.Users.ToArray());
            _context.Roles.RemoveRange(_context.Roles.ToArray());
            _context.Samples.RemoveRange(_context.Samples.ToArray());
            _context.Tests.RemoveRange(_context.Tests.ToArray());
            await _context.SaveChangesAsync();
        }

        public Workspace(OJContext context)
        {
            _context = context;
            Problems = new ProblemListProvider(this, _context);
            Submissions = new SubmissionListProvider(this, _context);
            Users = new UserListProvider(this, _context);
            Roles = new RoleListProvider(this, _context);
        }
    }
}
