using StarOJ.Core.Problems;
using StarOJ.Server.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarOJ.Server.Host.Helpers
{
    public static class Problems
    {
        public static ProblemData GetAPlusB(string userId)
        {
            Random rnd = new Random();

            List<TestCaseData> tests = new List<TestCaseData>();
            for (int i = 0; i < 10; i++)
            {
                int a = rnd.Next(0, 1001), b = rnd.Next(0, 1001);
                tests.Add(new TestCaseData
                {
                    Metadata = new TestCaseMetadata
                    {
                        TimeLimit = TimeSpan.FromSeconds(1),
                        MemoryLimit = 32 * 1024 * 1024,
                    },
                    Input = $"{a} {b}",
                    Output = $"{a + b}"
                });
            }
            return new ProblemData
            {
                Metadata = new ProblemMetadata
                {
                    UserId = userId,
                    Source = "Original",
                    Name = "A+B Problem",
                },
                Description = new ProblemDescription
                {
                    Description = @"Calculate $a+b$.",
                    Hint = @"$0\le a, b\le 10^3$",
                    Input = @"One line: $a$ and $b$",
                    Output = @"The value of $a+b$.",
                },
                Samples = new TestCaseData[] { tests.First() },
                Tests = tests.Skip(1).ToArray(),
            };
        }
    }
}
