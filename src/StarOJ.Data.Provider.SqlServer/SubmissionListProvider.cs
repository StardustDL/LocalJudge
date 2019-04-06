using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StarOJ.Core.Judgers;
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
            Directory.CreateDirectory(res.GetRoot());
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
                var res = new SubmissionProvider(_workspace, _context, item);
                Directory.Delete(res.GetRoot(), true);
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

        public async Task<IEnumerable<ISubmissionProvider>> GetAll()
        {
            List<ISubmissionProvider> res = new List<ISubmissionProvider>();
            var all = await (from item in _context.Submissions orderby item.Time descending select item).ToArrayAsync();
            foreach (var v in all)
            {
                res.Add(new SubmissionProvider(_workspace, _context, v));
            }
            return res;
        }

        public async Task<bool> Has(string id)
        {
            int _id = int.Parse(id);
            return await _context.Submissions.FindAsync(_id) != null;
        }

        public async Task Clear()
        {
            _context.Submissions.RemoveRange(await _context.Submissions.ToArrayAsync());
            foreach (var s in Directory.GetDirectories(_workspace.SubmissionStoreRoot))
                Directory.Delete(s, true);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ISubmissionProvider>> Query(string id,string problemId, string userId, ProgrammingLanguage? language,JudgeState? state)
        {
            var query = from item in _context.Submissions select item;
            if (!string.IsNullOrEmpty(id) && int.TryParse(id, out var _id))
            {
                query = query.Where(item => item.Id == _id);
            }
            if (!string.IsNullOrEmpty(problemId) && int.TryParse(problemId, out var _problemId))
            {
                query = query.Where(item => item.ProblemId == _problemId);
            }
            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out var _userId))
            {
                query = query.Where(item => item.UserId == _userId);
            }
            if (language.HasValue)
            {
                query = query.Where(item => item.Language == language.Value);
            }
            if (state.HasValue)
            {
                query = query.Where(item => item.State == state.Value);
            }
            query = query.OrderByDescending(item => item.Time);
            return (await query.ToListAsync()).Select(x => new SubmissionProvider(_workspace, _context, x));
        }
    }
}
