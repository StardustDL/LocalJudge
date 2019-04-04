using StarOJ.Core.Problems;
using System;

namespace StarOJ.Server.API.Models
{
    public class TestCaseData
    {
        public TestCaseMetadata Metadata { get; set; }

        public string Input { get; set; }

        public string Output { get; set; }
    }
}
