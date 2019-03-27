using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LocalJudge.Core.Problems
{
    public class ProblemManager
    {
        public string Root { get; private set; }

        public IEnumerable<ProblemPath> GetAll() => Directory.GetDirectories(Root).Select(path => new ProblemPath(path));

        public bool Has(string name)
        {
            string path = Path.Combine(Root, name);
            return Directory.Exists(path);
        }

        public ProblemPath Get(string name)
        {
            string path = Path.Combine(Root, name);
            return Directory.Exists(path) ? new ProblemPath(path) : null;
        }

        public ProblemPath Create(string id, ProblemMetadata metadata = null, ProblemDescription description = null)
        {
            string path = Path.Combine(Root, id);
            Directory.CreateDirectory(path);
            return ProblemPath.Initialize(path, metadata, description);
        }

        public void Delete(string name)
        {
            string path = Path.Combine(Root, name);
            Directory.Delete(path, true);
        }

        public ProblemManager(string root)
        {
            Root = root;
        }
    }
}
