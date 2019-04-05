using Microsoft.AspNetCore.Http;
using StarOJ.Core.Problems;
using System;
using System.ComponentModel.DataAnnotations;

namespace StarOJ.Server.API.Models
{
    public class TestCaseData
    {
        public TestCaseMetadata Metadata { get; set; }

        // file is high-priority

        [DataType(DataType.MultilineText)]
        public string Input { get; set; }

        [DataType(DataType.MultilineText)]
        public string Output { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile InputFile { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile OutputFile { get; set; }
    }
}
