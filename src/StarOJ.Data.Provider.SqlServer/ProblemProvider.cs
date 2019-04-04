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
        private readonly Workspace _workspace;
        private readonly OJContext _context;
        private readonly Problem _problem;

        public ProblemProvider(Workspace workspace, OJContext context, Problem problem)
        {
            _workspace = workspace;
            _context = context;
            _problem = problem;
            Samples = new SampleCaseListProvider(_workspace, _context, _problem.Id);
            Tests = new TestCaseListProvider(_workspace, _context, _problem.Id);
        }

        public string Id => _problem.Id.ToString();

        public ITestCaseListProvider Samples { get; private set; }

        public ITestCaseListProvider Tests { get; private set; }

        public async Task DeleteSample(string id)
        {
            int _id = int.Parse(id);
            var test = await _context.Samples.FindAsync(_id);
            if (test == null || test.ProblemId != _problem.Id)
                return;
            _context.Samples.Remove(test);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTest(string id)
        {
            int _id = int.Parse(id);
            var test = await _context.Tests.FindAsync(_id);
            if (test == null || test.ProblemId != _problem.Id)
                return;
            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTests()
        {
            var tests = (from x in _context.Tests where x.ProblemId == _problem.Id select x).ToArray();
            _context.Tests.RemoveRange(tests);
            await _context.SaveChangesAsync();
        }

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
                UserId = _problem.UserId.ToString(),
                Id = _problem.Id.ToString(),
                Name = _problem.Name,
                Source = _problem.Source
            });
        }

        public async Task SetDescription(ProblemDescription value)
        {
            _problem.Description = value.Description;
            _problem.Input = value.Input;
            _problem.Output = value.Output;
            _problem.Hint = value.Hint;
            await _context.SaveChangesAsync();
        }

        public async Task SetMetadata(ProblemMetadata value)
        {
            _problem.Source = value.Source;
            _problem.UserId = int.Parse(value.UserId);
            _problem.Name = value.Name;
            await _context.SaveChangesAsync();
        }
    }
}
