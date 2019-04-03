using System.Collections.Generic;
using System.Threading.Tasks;
using StarOJ.Core.Submissions;
using StarOJ.Data.Provider.SqlServer.Models;

namespace StarOJ.Data.Provider.SqlServer
{
    public class SubmissionListProvider : ISubmissionListProvider
    {
        private readonly OJContext _context;

        public SubmissionListProvider(OJContext context)
        {
            _context = context;
        }

        public async Task<ISubmissionProvider> Create(SubmissionMetadata metadata)
        {
            Submission empty = new Submission();
            _context.Submissions.Add(empty);
            await _context.SaveChangesAsync();
            var res = new SubmissionProvider(_context, empty);
            await res.SetMetadata(metadata);
            return res;
        }

        public Task<ISubmissionProvider> Create()
        {
            return Create(new SubmissionMetadata());
        }

        public async Task Delete(string id)
        {
            var item = await _context.Submissions.FindAsync(id);
            if (item != null)
            {
                _context.Submissions.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ISubmissionProvider> Get(string id)
        {
            var item = await _context.Submissions.FindAsync(id);
            if (item == null)
            {
                return null;
            }
            else
            {
                return new SubmissionProvider(_context, item);
            }
        }

        public Task<IEnumerable<ISubmissionProvider>> GetAll()
        {
            List<ISubmissionProvider> res = new List<ISubmissionProvider>();
            foreach (var v in _context.Submissions)
            {
                res.Add(new SubmissionProvider(_context, v));
            }
            return Task.FromResult((IEnumerable<ISubmissionProvider>)res);
        }

        public async Task<bool> Has(string id)
        {
            return await _context.Submissions.FindAsync(id) != null;
        }
    }
}
