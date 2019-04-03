using StarOJ.Core.Submissions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StarOJ.Data.Provider.FileSystem
{
    public class SubmissionListProvider : ISubmissionListProvider
    {
        public string Root { get; private set; }

        public ISubmissionProvider Create(SubmissionMetadata metadata)
        {
            string path = Path.Combine(Root, metadata.Id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            return SubmissionProvider.Initialize(path, metadata);
        }

        public ISubmissionProvider Create(string id)
        {
            string path = Path.Combine(Root, id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            return SubmissionProvider.Initialize(path);
        }

        public void Delete(string id)
        {
            string path = Path.Combine(Root, id);
            Directory.Delete(path, true);
        }

        public ISubmissionProvider Get(string id)
        {
            string path = Path.Combine(Root, id);
            return Directory.Exists(path) ? new SubmissionProvider(path) : null;
        }

        public IEnumerable<ISubmissionProvider> GetAll() => Directory.GetDirectories(Root).Select(path => new SubmissionProvider(path));

        public bool Has(string id) => Directory.Exists(Path.Combine(Root, id));

        public SubmissionListProvider(string root)
        {
            Root = root;
        }
    }
}
