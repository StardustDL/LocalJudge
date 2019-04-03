using LocalJudge.Core.Problems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LocalJudge.Data.Provider.FileSystem
{
    public class ProblemListProvider : IProblemListProvider
    {
        public string Root { get; private set; }

        public IProblemProvider Create(ProblemMetadata metadata)
        {
            string path = Path.Combine(Root, metadata.Id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            return ProblemProvider.Initialize(path, metadata, null);
        }

        public IProblemProvider Create(string id)
        {
            string path = Path.Combine(Root, id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            return ProblemProvider.Initialize(path, null, null);
        }

        public void Delete(string id) => Directory.Delete(Path.Combine(Root, id), true);

        public IProblemProvider Get(string id)
        {
            string path = Path.Combine(Root, id);
            return Directory.Exists(path) ? new ProblemProvider(path) : null;
        }

        public IEnumerable<IProblemProvider> GetAll() => Directory.GetDirectories(Root).Select(path => new ProblemProvider(path));

        public bool Has(string id) => Directory.Exists(Path.Combine(Root, id));

        public ProblemListProvider(string root)
        {
            Root = root;
        }
    }
}
