using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StarOJ.Data.Provider.SqlServer.Models;
using System.Linq;
using StarOJ.Core.Problems;

namespace StarOJ.Data.Provider.SqlServer
{
    public class SampleCaseListProvider : ITestCaseListProvider
    {
        private readonly Workspace _workspace;
        private readonly OJContext _context;
        private readonly int _problemId;

        public SampleCaseListProvider(Workspace workspace, OJContext context, int problemId)
        {
            _workspace = workspace;
            _context = context;
            _problemId = problemId;
        }

        public async Task Clear()
        {
            var tests = (from x in _context.Samples where x.ProblemId == _problemId select x).ToArray();
            _context.Samples.RemoveRange(tests);
            await _context.SaveChangesAsync();
        }

        public async Task<ITestCaseProvider> Create(TestCaseMetadata metadata)
        {
            SampleCase empty = new SampleCase { ProblemId = _problemId };
            _context.Samples.Add(empty);
            await _context.SaveChangesAsync();
            var res = new SampleCaseProvider(_workspace, _context, empty);
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
            var item = await _context.Samples.FindAsync(_id);
            if (item != null && item.ProblemId == _problemId)
            {
                _context.Samples.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ITestCaseProvider> Get(string id)
        {
            int _id = int.Parse(id);
            var item = await _context.Samples.FindAsync(_id);
            if (item == null || item.ProblemId != _problemId)
            {
                return null;
            }
            else
            {
                return new SampleCaseProvider(_workspace, _context, item);
            }
        }

        public Task<IEnumerable<ITestCaseProvider>> GetAll()
        {
            var test = (from x in _context.Samples where x.ProblemId == _problemId select x).ToList();
            List<ITestCaseProvider> res = new List<ITestCaseProvider>();
            foreach (var v in test)
            {
                res.Add(new SampleCaseProvider(_workspace, _context, v));
            }
            return Task.FromResult((IEnumerable<ITestCaseProvider>)res);
        }

        public async Task<bool> Has(string id)
        {
            int _id = int.Parse(id);
            var item = await _context.Samples.FindAsync(_id);
            if (item != null && item.ProblemId == _problemId)
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
