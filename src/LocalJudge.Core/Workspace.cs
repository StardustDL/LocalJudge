using LocalJudge.Core.Helpers;
using LocalJudge.Core.Judgers;
using LocalJudge.Core.Problems;
using LocalJudge.Core.Submissions;
using System;
using System.IO;

namespace LocalJudge.Core
{
    public class Workspace
    {
        public const string PD_Problem = "problems", PD_Submission = "submissions", PF_Profile = "profile.json";

        public string Root { get; private set; }

        public string Profile { get; private set; }

        public WorkspaceConfig GetConfig() => Newtonsoft.Json.JsonConvert.DeserializeObject<WorkspaceConfig>(TextIO.ReadAllInUTF8(Profile));

        public ProblemManager Problems { get; private set; }

        public SubmissionManager Submissions { get; private set; }

        public Workspace(string root)
        {
            Root = root;
            Problems = new ProblemManager(Path.Combine(Root, PD_Problem));
            Submissions = new SubmissionManager(Path.Combine(Root, PD_Submission));
            Profile = Path.Combine(Root, PF_Profile);
        }

        public static Workspace Initialize(string root)
        {
            var res = new Workspace(root);
            Directory.CreateDirectory(res.Problems.Root);
            Directory.CreateDirectory(res.Submissions.Root);
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
                }
            };
            TextIO.WriteAllInUTF8(res.Profile, Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented));
            return res;
        }
    }
}
