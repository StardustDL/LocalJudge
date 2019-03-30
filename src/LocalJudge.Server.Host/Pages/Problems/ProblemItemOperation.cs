using LocalJudge.Server.Host.APIClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalJudge.Server.Host.Pages.Problems
{
    public enum ProblemItemOperationType
    {
        Submit,
        Delete,
    }

    public class ProblemItemOperation
    {
        public ProblemItemOperationType Type { get; set; }

        public string ID { get; set; }

        public SubmitData SubmitData { get; set; }
    }
}
