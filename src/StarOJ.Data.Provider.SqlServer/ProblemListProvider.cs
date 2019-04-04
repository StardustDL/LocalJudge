using System.Collections.Generic;
using System.Threading.Tasks;
using StarOJ.Core.Problems;
using StarOJ.Data.Provider.SqlServer.Models;
using System.Linq;

namespace StarOJ.Data.Provider.SqlServer
{
    public class ProblemListProvider : IProblemListProvider
    {
        private readonly Workspace _workspace;
        private readonly OJContext _context;

        public ProblemListProvider(Workspace workspace, OJContext context)
        {
            _workspace = workspace;
            _context = context;
        }

        public async Task Clear()
        {
            var ls = _context.Problems.ToArray();
            foreach (var v in ls)
                await Delete(v.Id.ToString());
        }

        public async Task<IProblemProvider> Create(ProblemMetadata metadata)
        {
            Problem empty = new Problem();
            _context.Add(empty);
            await _context.SaveChangesAsync();
            var res = new ProblemProvider(_workspace, _context, empty);
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
            int _id = int.Parse(id);
            var item = await _context.Problems.FindAsync(_id);
            if (item != null)
            {
                var prov = new ProblemProvider(_workspace, _context, item);

                {
                    var submis = (from x in _context.Submissions where x.ProblemId == item.Id select x).ToArray();
                    _context.Submissions.RemoveRange(submis);
                }

                _context.Problems.Remove(item);

                await _context.SaveChangesAsync();
            }
        }

        public async Task<IProblemProvider> Get(string id)
        {
            int _id = int.Parse(id);
            var item = await _context.Problems.FindAsync(_id);
            if (item == null)
            {
                return null;
            }
            else
            {
                return new ProblemProvider(_workspace, _context, item);
            }
        }

        public Task<IEnumerable<IProblemProvider>> GetAll()
        {
            List<IProblemProvider> res = new List<IProblemProvider>();
            foreach (var v in _context.Problems)
            {
                res.Add(new ProblemProvider(_workspace, _context, v));
            }
            return Task.FromResult((IEnumerable<IProblemProvider>)res);
        }

        public async Task<bool> Has(string id)
        {
            int _id = int.Parse(id);
            return await _context.Problems.FindAsync(_id) != null;
        }
    }
    
}
