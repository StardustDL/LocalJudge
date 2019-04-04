using StarOJ.Core.Problems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.FileSystem
{
    public class TestCaseListProvider : ITestCaseListProvider
    {
        public string Root { get; private set; }

        public async Task<ITestCaseProvider> Create(TestCaseMetadata metadata)
        {
            metadata.Id = Guid.NewGuid().ToString();
            string path = Path.Combine(Root, metadata.Id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            return await TestCaseProvider.Initialize(path, metadata);
        }

        public async Task<ITestCaseProvider> Create()
        {
            string id = Guid.NewGuid().ToString();
            string path = Path.Combine(Root, id);
            if (Directory.Exists(path)) return null;
            Directory.CreateDirectory(path);
            return await TestCaseProvider.Initialize(path);
        }

        public Task Delete(string id)
        {
            string path = Path.Combine(Root, id);
            Directory.Delete(path, true);
            return Task.CompletedTask;
        }

        public Task<ITestCaseProvider> Get(string id)
        {
            string path = Path.Combine(Root, id);
            return Task.FromResult(Directory.Exists(path) ? (ITestCaseProvider)new TestCaseProvider(path) : null);
        }

        public Task<IEnumerable<ITestCaseProvider>> GetAll() => Task.FromResult(Directory.GetDirectories(Root).Select(path => (ITestCaseProvider)new TestCaseProvider(path)));

        public Task<bool> Has(string id) => Task.FromResult(Directory.Exists(Path.Combine(Root, id)));

        public Task Clear()
        {
            foreach (var v in Directory.GetDirectories(Root))
                Directory.Delete(v, true);
            return Task.CompletedTask;
        }

        public TestCaseListProvider(string root)
        {
            Root = root;
        }
    }
}
