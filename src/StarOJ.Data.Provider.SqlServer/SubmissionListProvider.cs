using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StarOJ.Core.Submissions;
using StarOJ.Data.Provider.SqlServer.Models;

namespace StarOJ.Data.Provider.SqlServer
{
    public class SubmissionListProvider : ISubmissionListProvider
    {
        private readonly Workspace _workspace;
        private readonly OJContext _context;

        public SubmissionListProvider(Workspace workspace, OJContext context)
        {
            _workspace = workspace;
            _context = context;
        }

        public async Task<ISubmissionProvider> Create(SubmissionMetadata metadata)
        {
            Submission empty = new Submission();
            _context.Submissions.Add(empty);
            await _context.SaveChangesAsync();
            var res = new SubmissionProvider(_workspace, _context, empty);
            await res.SetMetadata(metadata);
            return res;
        }

        public Task<ISubmissionProvider> Create()
        {
            return Create(new SubmissionMetadata());
        }

        public async Task Delete(string id)
        {
            int _id = int.Parse(id);
            var item = await _context.Submissions.FindAsync(_id);
            if (item != null)
            {
                _context.Submissions.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ISubmissionProvider> Get(string id)
        {
            int _id = int.Parse(id);
            var item = await _context.Submissions.FindAsync(_id);
            if (item == null)
            {
                return null;
            }
            else
            {
                return new SubmissionProvider(_workspace, _context, item);
            }
        }

        public Task<IEnumerable<ISubmissionProvider>> GetAll()
        {
            List<ISubmissionProvider> res = new List<ISubmissionProvider>();
            foreach (var v in _context.Submissions)
            {
                res.Add(new SubmissionProvider(_workspace, _context, v));
            }
            return Task.FromResult((IEnumerable<ISubmissionProvider>)res);
        }

        public async Task<bool> Has(string id)
        {
            int _id = int.Parse(id);
            return await _context.Submissions.FindAsync(_id) != null;
        }

        public async Task Clear()
        {
            _context.Submissions.RemoveRange(_context.Submissions.ToArray());
            await _context.SaveChangesAsync();
        }
    }
}
