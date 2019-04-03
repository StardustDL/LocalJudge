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

        public WorkspaceConfig GetConfig() => Newtonsoft.Json.JsonConvert.DeserializeObject<WorkspaceConfig>(TextIO.ReadAllInUTF8(Profile));

        public void Initialize()
        {
            Directory.CreateDirectory(_problems.Root);
            Directory.CreateDirectory(_submissions.Root);
            Directory.CreateDirectory(_users.Root);
            Directory.CreateDirectory(_roles.Root);
            WorkspaceConfig config = new WorkspaceConfig
            {
                Languages = new Dictionary<ProgrammingLanguage, LanguageConfig>
                {
                    [ProgrammingLanguage.C] = new LanguageConfig
                    {
                        CompileMemoryLimit = 1024 * 1024 * 1024,
                        CompileTimeLimit = TimeSpan.FromSeconds(20),
                        CompileCommand = new Command
                        {
                            Name = "gcc",
                            Arguments = new string[]
                            {
                                Compiler.V_CodeFile,
                                "-o",
                                Compiler.V_Output
                            }
                        },
                        RunCommand = new Command
                        {
                            Name = Judger.V_CompileOutput,
                        }
                    },
                    [ProgrammingLanguage.Cpp] = new LanguageConfig
                    {
                        CompileMemoryLimit = 1024 * 1024 * 1024,
                        CompileTimeLimit = TimeSpan.FromSeconds(20),
                        CompileCommand = new Command
                        {
                            Name = "g++",
                            Arguments = new string[]
                            {
                                Compiler.V_CodeFile,
                                "-o",
                                Compiler.V_Output
                            }
                        },
                        RunCommand = new Command
                        {
                            Name = Judger.V_CompileOutput,
                        }
                    },
                    [ProgrammingLanguage.Java] = new LanguageConfig
                    {
                        CompileMemoryLimit = 1024 * 1024 * 1024,
                        CompileTimeLimit = TimeSpan.FromSeconds(20),
                        CompileCommand = new Command
                        {
                            Name = "javac",
                            Arguments = new string[]
                            {
                                Compiler.V_CodeFile
                            }
                        },
                        RunCommand = new Command
                        {
                            Name = "java",
                            Arguments = new string[]
                            {
                                "Main"
                            },
                        }
                    },
                    [ProgrammingLanguage.CSharp] = new LanguageConfig
                    {
                        CompileMemoryLimit = 1024 * 1024 * 1024,
                        CompileTimeLimit = TimeSpan.FromSeconds(20),
                        CompileCommand = new Command
                        {
                            Name = "csc",
                            Arguments = new string[]
                            {
                                Compiler.V_CodeFile
                            }
                        },
                        RunCommand = new Command
                        {
                            Name = Judger.V_CompileOutput,
                        }
                    },
                    [ProgrammingLanguage.VisualBasic] = new LanguageConfig
                    {
                        CompileMemoryLimit = 1024 * 1024 * 1024,
                        CompileTimeLimit = TimeSpan.FromSeconds(20),
                        CompileCommand = new Command
                        {
                            Name = "vbc",
                            Arguments = new string[]
                            {
                                Compiler.V_CodeFile
                            }
                        },
                        RunCommand = new Command
                        {
                            Name = Judger.V_CompileOutput,
                        }
                    },
                    [ProgrammingLanguage.Python] = new LanguageConfig
                    {
                        CompileCommand = null,
                        RunCommand = new Command
                        {
                            Name = "python",
                            Arguments = new string[]
                            {
                                Judger.V_CodeFile
                            },
                        }
                    },
                    [ProgrammingLanguage.Rust] = new LanguageConfig
                    {
                        CompileMemoryLimit = 1024 * 1024 * 1024,
                        CompileTimeLimit = TimeSpan.FromSeconds(20),
                        CompileCommand = new Command
                        {
                            Name = "rustc",
                            Arguments = new string[]
                            {
                                Compiler.V_CodeFile,
                                "-o",
                                Compiler.V_Output
                            }
                        },
                        RunCommand = new Command
                        {
                            Name = Judger.V_CompileOutput,
                        }
                    },
                    [ProgrammingLanguage.Php] = new LanguageConfig
                    {
                        CompileMemoryLimit = 1024 * 1024 * 1024,
                        CompileTimeLimit = TimeSpan.FromSeconds(20),
                        CompileCommand = null,
                        RunCommand = new Command
                        {
                            Name = "php",
                            Arguments = new string[]
                            {
                                Judger.V_CodeFile,
                            }
                        }
                    },
                    [ProgrammingLanguage.Go] = new LanguageConfig
                    {
                        CompileMemoryLimit = 1024 * 1024 * 1024,
                        CompileTimeLimit = TimeSpan.FromSeconds(20),
                        CompileCommand = new Command
                        {
                            Name = "go",
                            Arguments = new string[]
                            {
                                "build",
                                Compiler.V_CodeFile
                            }
                        },
                        RunCommand = new Command
                        {
                            Name = Judger.V_CompileOutput
                        }
                    },
                    [ProgrammingLanguage.Haskell] = new LanguageConfig
                    {
                        CompileMemoryLimit = 1024 * 1024 * 1024,
                        CompileTimeLimit = TimeSpan.FromSeconds(20),
                        CompileCommand = new Command
                        {
                            Name = "ghc",
                            Arguments = new string[]
                            {
                                Compiler.V_CodeFile,
                                "-o",
                                Compiler.V_Output
                            }
                        },
                        RunCommand = new Command
                        {
                            Name = Judger.V_CompileOutput
                        }
                    },
                    [ProgrammingLanguage.Javascript] = new LanguageConfig
                    {
                        CompileMemoryLimit = 1024 * 1024 * 1024,
                        CompileTimeLimit = TimeSpan.FromSeconds(20),
                        CompileCommand = null,
                        RunCommand = new Command
                        {
                            Name = "node",
                            Arguments = new string[]
                            {
                                Judger.V_CodeFile
                            }
                        }
                    },
                    [ProgrammingLanguage.Java] = new LanguageConfig
                    {
                        CompileMemoryLimit = 1024 * 1024 * 1024,
                        CompileTimeLimit = TimeSpan.FromSeconds(20),
                        CompileCommand = new Command
                        {
                            Name = "kotlinc",
                            Arguments = new string[]
                            {
                                Compiler.V_CodeFile
                            }
                        },
                        RunCommand = new Command
                        {
                            Name = "kotlin",
                            Arguments = new string[]
                            {
                                "Main"
                            },
                        }
                    },
                    [ProgrammingLanguage.Scala] = new LanguageConfig
                    {
                        CompileMemoryLimit = 1024 * 1024 * 1024,
                        CompileTimeLimit = TimeSpan.FromSeconds(20),
                        CompileCommand = null,
                        RunCommand = new Command
                        {
                            Name = "scala",
                            Arguments = new string[]
                            {
                                Judger.V_CodeFile
                            }
                        }
                    },
                    [ProgrammingLanguage.Ruby] = new LanguageConfig
                    {
                        CompileCommand = null,
                        RunCommand = new Command
                        {
                            Name = "ruby",
                            Arguments = new string[]
                            {
                                Judger.V_CodeFile
                            },
                        }
                    },
                }
            };
            TextIO.WriteAllInUTF8(Profile, Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented));
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
