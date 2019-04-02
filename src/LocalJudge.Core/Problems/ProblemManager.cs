using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LocalJudge.Core.Problems
{
    public class ProblemManager : IPathItemManager<ProblemPath>
    {
        public string Root { get; private set; }

        public IEnumerable<ProblemPath> GetAll() => Directory.GetDirectories(Root).Select(path => new ProblemPath(path));

        public bool Has(string id) => Directory.Exists(Path.Combine(Root, id));

        public ProblemPath Get(string id)
        {
            string path = Path.Combine(Root, id);
            return Directory.Exists(path) ? new ProblemPath(path) : null;
        }

        public ProblemPath Create(string id, ProblemMetadata metadata = null, ProblemDescription description = null)
        {
            string path = Path.Combine(Root, id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            return ProblemPath.Initialize(path, metadata, description);
        }

        public ProblemPath Create(string id) => Create(id, null, null);

        public void Delete(string id) => Directory.Delete(Path.Combine(Root, id), true);

        public ProblemManager(string root) => Root = root;
    }
}
