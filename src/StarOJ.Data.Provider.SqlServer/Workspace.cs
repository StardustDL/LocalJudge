using Newtonsoft.Json;
using StarOJ.Core.Identity;
using StarOJ.Core.Judgers;
using StarOJ.Core.Problems;
using StarOJ.Core.Submissions;
using StarOJ.Data.Provider.SqlServer.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.SqlServer
{
    public class Workspace : IWorkspace
    {
        public const string PD_Tests = "tests", PD_Submissions = "submissions";

        private readonly OJContext _context;

        public string FileStoreRoot { get; private set; }

        public string TestCaseStoreRoot { get; private set; }

        public string SubmissionStoreRoot { get; private set; }

        public IProblemListProvider Problems { get; private set; }

        public ISubmissionListProvider Submissions { get; private set; }

        public IUserListProvider Users { get; private set; }

        public IRoleListProvider Roles { get; private set; }

        public bool HasInitialized => _context.WorkspaceInfos.FirstOrDefault() != null;

        public IStatisticProvider Statistics { get; private set; }

        public Task<WorkspaceConfig> GetConfig()
        {
            WorkspaceInfo config = _context.WorkspaceInfos.FirstOrDefault();
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
            if (!Directory.Exists(TestCaseStoreRoot))
                Directory.CreateDirectory(TestCaseStoreRoot);
            if (!Directory.Exists(SubmissionStoreRoot))
                Directory.CreateDirectory(SubmissionStoreRoot);

            List<ProgrammingLanguage> langs = new List<ProgrammingLanguage>();
            foreach (KeyValuePair<ProgrammingLanguage, JudgerLangConfig> v in Judger.DefaultLangConfigs)
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
            _context.Tests.RemoveRange(_context.Tests.ToArray());
            await _context.SaveChangesAsync();
            foreach (string v in Directory.GetDirectories(FileStoreRoot))
                Directory.Delete(v, true);
        }

        public Workspace(OJContext context, WorkspaceStartup startup)
        {
            _context = context;
            FileStoreRoot = startup.FileStoreRoot;
            TestCaseStoreRoot = Path.Join(FileStoreRoot, PD_Tests);
            SubmissionStoreRoot = Path.Join(FileStoreRoot, PD_Submissions);

            Problems = new ProblemListProvider(this, _context);
            Submissions = new SubmissionListProvider(this, _context);
            Users = new UserListProvider(this, _context);
            Roles = new RoleListProvider(this, _context);
            Statistics = new StatisticProvider(this, _context);
        }
    }
}
