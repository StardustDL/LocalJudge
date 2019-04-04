using StarOJ.Core.Submissions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.FileSystem
{
    public class SubmissionListProvider : ISubmissionListProvider
    {
        public string Root { get; private set; }

        public async Task<ISubmissionProvider> Create(SubmissionMetadata metadata)
        {
            metadata.Id = Guid.NewGuid().ToString();
            string path = Path.Combine(Root, metadata.Id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            return await SubmissionProvider.Initialize(path, metadata);
        }

        public async Task<ISubmissionProvider> Create()
        {
            string id = Guid.NewGuid().ToString();
            string path = Path.Combine(Root, id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            return await SubmissionProvider.Initialize(path);
        }

        public Task Delete(string id)
        {
            string path = Path.Combine(Root, id);
            Directory.Delete(path, true);
            return Task.CompletedTask;
        }

        public Task<ISubmissionProvider> Get(string id)
        {
            string path = Path.Combine(Root, id);
            return Task.FromResult(Directory.Exists(path) ? (ISubmissionProvider)new SubmissionProvider(path) : null);
        }

        public Task<IEnumerable<ISubmissionProvider>> GetAll() => Task.FromResult(Directory.GetDirectories(Root).Select(path => (ISubmissionProvider)new SubmissionProvider(path)));

        public Task<bool> Has(string id) => Task.FromResult(Directory.Exists(Path.Combine(Root, id)));

        public Task Clear()
        {
            foreach (var v in Directory.GetDirectories(Root))
                Directory.Delete(v, true);
            return Task.CompletedTask;
        }

        public SubmissionListProvider(string root)
        {
            Root = root;
        }
    }
}
