using StarOJ.Core;
using StarOJ.Core.Helpers;
using StarOJ.Core.Identity;
using StarOJ.Core.Judgers;
using StarOJ.Core.Problems;
using StarOJ.Core.Submissions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.FileSystem
{
    public class Workspace : IWorkspace
    {
        public const string PD_Problem = "problems", PD_Submission = "submissions", PD_User = "users", PD_Role = "roles", PF_Profile = "profile.json";

        private readonly ProblemListProvider _problems;
        private readonly SubmissionListProvider _submissions;
        private readonly UserListProvider _users;
        private readonly RoleListProvider _roles;

        public string Root { get; private set; }

        public string Profile { get; private set; }

        public IProblemListProvider Problems => _problems;

        public ISubmissionListProvider Submissions => _submissions;

        public IUserListProvider Users => _users;

        public IRoleListProvider Roles => _roles;

        public bool HasInitialized => File.Exists(Profile);

        public async Task<WorkspaceConfig> GetConfig() => Newtonsoft.Json.JsonConvert.DeserializeObject<WorkspaceConfig>(await TextIO.ReadAllInUTF8Async(Profile));

        public async Task Initialize()
        {
            Directory.CreateDirectory(_problems.Root);
            Directory.CreateDirectory(_submissions.Root);
            Directory.CreateDirectory(_users.Root);
            Directory.CreateDirectory(_roles.Root);
            var langs = new List<ProgrammingLanguage>();
            foreach (var v in Judger.DefaultLangConfigs)
                langs.Add(v.Key);
            WorkspaceConfig config = new WorkspaceConfig
            {
                SupportLanguages = langs
            };
            await TextIO.WriteAllInUTF8Async(Profile, Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented));
        }

        public Task Clear()
        {
            File.Delete(Profile);
            return Task.CompletedTask;
        }

        public Workspace(string root)
        {
            Root = root;
            _problems = new ProblemListProvider(Path.Combine(Root, PD_Problem));
            _submissions = new SubmissionListProvider(Path.Combine(Root, PD_Submission));
            _users = new UserListProvider(Path.Combine(Root, PD_User));
            _roles = new RoleListProvider(Path.Combine(Root, PD_Role));
            Profile = Path.Combine(Root, PF_Profile);
        }
    }
}
