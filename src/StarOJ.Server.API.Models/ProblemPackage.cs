using System;
using System.Collections.Generic;
using System.Text;

namespace StarOJ.Core.Problems
{
    public class ProblemPackage
    {
        public class TestCasePackage
        {
            public TestCaseMetadata Metadata { get; set; }

            public string Input { get; set; }

            public string Output { get; set; }
        }

        public ProblemMetadata Metadata { get; set; }

        public ProblemDescription Description { get; set; }

        public IList<TestCasePackage> Samples { get; set; }

        public IList<TestCasePackage> Tests { get; set; }
    }
}
