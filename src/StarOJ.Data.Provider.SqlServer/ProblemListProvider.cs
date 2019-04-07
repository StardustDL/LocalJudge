using Microsoft.EntityFrameworkCore;
using StarOJ.Core.Problems;
using StarOJ.Data.Provider.SqlServer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            Problem[] ls = _context.Problems.ToArray();
            foreach (Problem v in ls)
                await Delete(v.Id.ToString());
        }

        public async Task<IProblemProvider> Create(ProblemMetadata metadata)
        {
            Problem empty = new Problem();
            _context.Add(empty);
            await _context.SaveChangesAsync();
            ProblemProvider res = new ProblemProvider(_workspace, _context, empty);
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
            Problem item = await _context.Problems.FindAsync(_id);
            if (item != null)
            {
                ProblemProvider prov = new ProblemProvider(_workspace, _context, item);

                await prov.Samples.Clear();
                await prov.Tests.Clear();

                {
                    int[] submis = (from x in _context.Submissions where x.ProblemId == item.Id select x.Id).ToArray();

                    foreach (int s in submis)
                        await _workspace.Submissions.Delete(s.ToString());
                }

                _context.Problems.Remove(item);

                await _context.SaveChangesAsync();
            }
        }

        public async Task<IProblemProvider> Get(string id)
        {
            int _id = int.Parse(id);
            Problem item = await _context.Problems.FindAsync(_id);
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
            foreach (Problem v in _context.Problems)
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

        public async Task<IEnumerable<IProblemProvider>> Query(string id, string userId, string name, string source)
        {
            IQueryable<Problem> query = from item in _context.Problems select item;
            if (!string.IsNullOrEmpty(id) && int.TryParse(id, out int _id))
            {
                query = query.Where(item => item.Id == _id);
            }
            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int _userId))
            {
                query = query.Where(item => item.UserId == _userId);
            }
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(item => item.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(source))
            {
                query = query.Where(item => item.Source.Contains(source));
            }
            return (await query.ToListAsync()).Select(x => new ProblemProvider(_workspace, _context, x));
        }
    }

}
