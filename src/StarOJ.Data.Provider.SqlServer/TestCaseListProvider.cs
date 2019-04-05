using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StarOJ.Data.Provider.SqlServer.Models;
using System.Linq;
using StarOJ.Core.Problems;
using System.IO;

namespace StarOJ.Data.Provider.SqlServer
{
    public class TestCaseListProvider : ITestCaseListProvider
    {
        private readonly Workspace _workspace;
        private readonly OJContext _context;
        private readonly int _problemId;
        private readonly bool _isSample;

        public TestCaseListProvider(Workspace workspace, OJContext context, int problemId, bool isSample)
        {
            _workspace = workspace;
            _context = context;
            _problemId = problemId;
            _isSample = isSample;
        }

        public async Task Clear()
        {
            var tests = (from x in _context.Tests where x.ProblemId == _problemId && x.IsSample == _isSample select x).ToArray();
            _context.Tests.RemoveRange(tests);
            await _context.SaveChangesAsync();
        }

        public async Task<ITestCaseProvider> Create(TestCaseMetadata metadata)
        {
            TestCase empty = new TestCase { ProblemId = _problemId, IsSample = _isSample };
            _context.Tests.Add(empty);
            await _context.SaveChangesAsync();
            var res = new TestCaseProvider(_workspace, _context, empty);
            Directory.CreateDirectory(res.GetRoot());
            await res.SetMetadata(metadata);
            return res;
        }

        public Task<ITestCaseProvider> Create()
        {
            return Create(new TestCaseMetadata { MemoryLimit = 128 * 1024 * 1024, TimeLimit = TimeSpan.FromSeconds(1) });
        }

        public async Task Delete(string id)
        {
            int _id = int.Parse(id);
            var item = await _context.Tests.FindAsync(_id);
            if (item != null && item.ProblemId == _problemId && item.IsSample == _isSample)
            {
                var prov = new TestCaseProvider(_workspace, _context, item);
                Directory.Delete(prov.GetRoot(), true);
                _context.Tests.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ITestCaseProvider> Get(string id)
        {
            int _id = int.Parse(id);
            var item = await _context.Tests.FindAsync(_id);
            if (item != null && item.ProblemId == _problemId && item.IsSample == _isSample)
            {
                return new TestCaseProvider(_workspace, _context, item); 
            }
            else
            {
                return null;
            }
        }

        public Task<IEnumerable<ITestCaseProvider>> GetAll()
        {
            var test = (from x in _context.Tests where x.ProblemId == _problemId && x.IsSample == _isSample select x).ToList();
            List<ITestCaseProvider> res = new List<ITestCaseProvider>();
            foreach (var v in test)
                res.Add(new TestCaseProvider(_workspace, _context, v));
            return Task.FromResult((IEnumerable<ITestCaseProvider>)res);
        }

        public async Task<bool> Has(string id)
        {
            int _id = int.Parse(id);
            var item = await _context.Tests.FindAsync(_id);
            if (item != null && item.ProblemId == _problemId && item.IsSample == _isSample)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
