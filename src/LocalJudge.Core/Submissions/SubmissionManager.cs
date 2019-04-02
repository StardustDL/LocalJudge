using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LocalJudge.Core.Submissions
{
    public class SubmissionManager : IPathItemManager<SubmissionPath>
    {
        public string Root { get; private set; }

        public IEnumerable<SubmissionPath> GetAll() => Directory.GetDirectories(Root).Select(path => new SubmissionPath(path));

        public bool Has(string name)
        {
            string path = Path.Combine(Root, name);
            return Directory.Exists(path);
        }

        public SubmissionPath Get(string id)
        {
            string path = Path.Combine(Root, id);
            return Directory.Exists(path) ? new SubmissionPath(path) : null;
        }

        public SubmissionPath Create(string id, SubmissionMetadata metadata = null, string code = "")
        {
            string path = Path.Combine(Root, id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            return SubmissionPath.Initialize(path, metadata, code);
        }

        public SubmissionPath Create(string id) => Create(id, null, null);

        public void Delete(string id)
        {
            string path = Path.Combine(Root, id);
            Directory.Delete(path, true);
        }

        public SubmissionManager(string root)
        {
            Root = root;
        }
    }
}
