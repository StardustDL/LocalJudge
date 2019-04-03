using StarOJ.Core.Problems;
using StarOJ.Data.Provider.SqlServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.SqlServer
{
    public class ProblemProvider : IProblemProvider
    {
        private readonly OJContext _context;
        private readonly Problem _problem;

        public ProblemProvider(OJContext context, Problem problem)
        {
            _context = context;
            _problem = problem;
        }

        public string Id => _problem.Id;

        public Task<ProblemDescription> GetDescription()
        {
            return Task.FromResult(new ProblemDescription
            {
                Description = _problem.Description,
                Input = _problem.Input,
                Output = _problem.Output,
                Hint = _problem.Hint,
            });
        }

        public Task<ProblemMetadata> GetMetadata()
        {
            return Task.FromResult(new ProblemMetadata
            {
                Author = _problem.Author,
                Id = _problem.Id,
                Name = _problem.Name,
                Source = _problem.Source
            });
        }

        public async Task<ITestCaseProvider> GetSample(string id)
        {
            var test = await _context.Samples.FindAsync(id);
            if (test == null || test.ProblemId != Id)
                return null;
            else
                return new TestCaseProvider(_context, test);
        }

        public Task<IEnumerable<ITestCaseProvider>> GetSamples()
        {
            var test = (from x in _context.Samples where x.ProblemId == Id select x).ToList();
            List<ITestCaseProvider> res = new List<ITestCaseProvider>();
            foreach (var v in test)
            {
                res.Add(new TestCaseProvider(_context, v));
            }
            return Task.FromResult((IEnumerable<ITestCaseProvider>)res);
        }

        public async Task<ITestCaseProvider> GetTest(string id)
        {
            var test = await _context.Tests.FindAsync(id);
            if (test == null || test.ProblemId != Id)
                return null;
            else
                return new TestCaseProvider(_context, test);
        }

        public Task<IEnumerable<ITestCaseProvider>> GetTests()
        {
            var test = (from x in _context.Tests where x.ProblemId == Id select x).ToList();
            List<ITestCaseProvider> res = new List<ITestCaseProvider>();
            foreach (var v in test)
            {
                res.Add(new TestCaseProvider(_context, v));
            }
            return Task.FromResult((IEnumerable<ITestCaseProvider>)res);
        }

        public async Task SetMetadata(ProblemMetadata value)
        {
            _problem.Source = value.Source;
            _problem.Author = value.Author;
            _problem.Name = value.Name;
            await _context.SaveChangesAsync();
        }
    }
}
