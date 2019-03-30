using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalJudge.Server.Host.APIClients
{
    public abstract class BaseClient
    {
        public string BaseUrl { get => Program.ApiServer; }
    }
}
