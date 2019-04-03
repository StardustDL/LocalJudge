using System.Collections.Generic;
using System.Threading.Tasks;
using StarOJ.Core.Problems;
using StarOJ.Data.Provider.SqlServer.Models;

namespace StarOJ.Data.Provider.SqlServer
{
    public class ProblemListProvider : IProblemListProvider
    {
        private readonly OJContext _context;

        public ProblemListProvider(OJContext context)
        {
            _context = context;
        }

        public async Task<IProblemProvider> Create(ProblemMetadata metadata)
        {
            Problem empty = new Problem();
            _context.Add(empty);
            await _context.SaveChangesAsync();
            var res = new ProblemProvider(_context, empty);
            await res.SetMetadata(metadata);
            return res;
        }

        public Task<IProblemProvider> Create()
        {
            return Create(new ProblemMetadata
            {
                Name = "Untitled"
            });
        }

        public async Task Delete(string id)
        {
            var item = await _context.Problems.FindAsync(id);
            if (item != null)
            {
                _context.Problems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IProblemProvider> Get(string id)
        {
            var item = await _context.Problems.FindAsync(id);
            if (item == null)
            {
                return null;
            }
            else
            {
                return new ProblemProvider(_context, item);
            }
        }

        public Task<IEnumerable<IProblemProvider>> GetAll()
        {
            List<IProblemProvider> res = new List<IProblemProvider>();
            foreach (var v in _context.Problems)
            {
                res.Add(new ProblemProvider(_context, v));
            }
            return Task.FromResult((IEnumerable<IProblemProvider>)res);
        }

        public async Task<bool> Has(string id)
        {
            return await _context.Problems.FindAsync(id) != null;
        }
    }
    
}
