using StarOJ.Core.Problems;
using System;
using System.ComponentModel.DataAnnotations;

namespace StarOJ.Server.API.Models
{
    public class TestCaseData
    {
        public TestCaseMetadata Metadata { get; set; }

        [DataType(DataType.MultilineText)]
        public string Input { get; set; }

        [DataType(DataType.MultilineText)]
        public string Output { get; set; }
    }
}
