using StarOJ.Core.Helpers;
using StarOJ.Core.Judgers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarOJ.Core
{
    public class WorkspaceConfig
    {
        public static IReadOnlyDictionary<ProgrammingLanguage, LanguageConfig> DefaultLanguages = new Dictionary<ProgrammingLanguage, LanguageConfig>
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
        };

        public Dictionary<ProgrammingLanguage, LanguageConfig> Languages { get; set; }

        public IEnumerable<ProgrammingLanguage> GetSupportLanguages() => from l in Languages.Keys select l;
    }
}
