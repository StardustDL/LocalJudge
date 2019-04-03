using StarOJ.Server.Host.APIClients;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StarOJ.Server.Host.Pages.Problems
{
    public class ProblemPostModel
    {
        public string Id { get; set; }

        public SubmitData SubmitData { get; set; }
    }
}
