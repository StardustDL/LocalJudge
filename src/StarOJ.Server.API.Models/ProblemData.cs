using StarOJ.Core.Problems;
using System.Collections.Generic;

namespace StarOJ.Server.API.Models
{
    public class ProblemData
    {
        public ProblemMetadata Metadata { get; set; }

        public ProblemDescription Description { get; set; }

        public IList<TestCaseData> Tests { get; set; }

        public IList<TestCaseData> Samples { get; set; }
    }
}
