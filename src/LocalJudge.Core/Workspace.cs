using LocalJudge.Core.Helpers;
using LocalJudge.Core.Identity;
using LocalJudge.Core.Judgers;
using LocalJudge.Core.Problems;
using LocalJudge.Core.Submissions;
using System;
using System.IO;

namespace LocalJudge.Core
{
    public class Workspace
    {
        public const string PD_Problem = "problems", PD_Submission = "submissions", PD_User = "users",PD_Role="roles", PF_Profile = "profile.json";

        public string Root { get; private set; }

        public string Profile { get; private set; }

        public WorkspaceConfig GetConfig() => Newtonsoft.Json.JsonConvert.DeserializeObject<WorkspaceConfig>(TextIO.ReadAllInUTF8(Profile));

        public ProblemManager Problems { get; private set; }

        public SubmissionManager Submissions { get; private set; }

        public UserManager Users { get; private set; }

        public RoleManager Roles { get; private set; }

        public bool HasInitialized { get => File.Exists(Profile); }

        public Workspace(string root)
        {
            Root = root;
            Problems = new ProblemManager(Path.Combine(Root, PD_Problem));
            Submissions = new SubmissionManager(Path.Combine(Root, PD_Submission));
            Users = new UserManager(Path.Combine(Root, PD_User));
            Roles = new RoleManager(Path.Combine(Root, PD_Role));
            Profile = Path.Combine(Root, PF_Profile);
        }

        public static Workspace Initialize(string root)
        {
            var res = new Workspace(root);
            Directory.CreateDirectory(res.Problems.Root);
            Directory.CreateDirectory(res.Submissions.Root);
            Directory.CreateDirectory(res.Users.Root);
            Directory.CreateDirectory(res.Roles.Root);
            WorkspaceConfig config = new WorkspaceConfig
            {
                Languages = new System.Collections.Generic.Dictionary<ProgrammingLanguage, LanguageConfig>
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
            TextIO.WriteAllInUTF8(res.Profile, Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented));
            return res;
        }
    }
}
