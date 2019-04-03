using StarOJ.Core.Problems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.FileSystem
{
    public class ProblemListProvider : IProblemListProvider
    {
        public string Root { get; private set; }

        public async Task<IProblemProvider> Create(ProblemMetadata metadata)
        {
            metadata.Id = Guid.NewGuid().ToString();
            string path = Path.Combine(Root, metadata.Id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            return await ProblemProvider.Initialize(path, metadata, null);
        }

        public async Task<IProblemProvider> Create()
        {
            string id = Guid.NewGuid().ToString();
            string path = Path.Combine(Root, id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            return await ProblemProvider.Initialize(path);
        }

        public Task Delete(string id)
        {
            Directory.Delete(Path.Combine(Root, id), true);
            return Task.CompletedTask;
        }

        public Task<IProblemProvider> Get(string id)
        {
            string path = Path.Combine(Root, id);
            return Task.FromResult(Directory.Exists(path) ? (IProblemProvider)new ProblemProvider(path) : null);
        }

        public Task<IEnumerable<IProblemProvider>> GetAll() => Task.FromResult(Directory.GetDirectories(Root).Select(path => (IProblemProvider)new ProblemProvider(path)));

        public Task<bool> Has(string id) => Task.FromResult(Directory.Exists(Path.Combine(Root, id)));

        public ProblemListProvider(string root)
        {
            Root = root;
        }
    }
}
