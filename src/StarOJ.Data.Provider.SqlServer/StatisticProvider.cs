using Microsoft.EntityFrameworkCore;
using StarOJ.Core.Identity;
using StarOJ.Core.Judgers;
using StarOJ.Core.Problems;
using StarOJ.Data.Provider.SqlServer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.SqlServer
{
    public class StatisticProvider : IStatisticProvider
    {
        private readonly Workspace _workspace;
        private readonly OJContext _context;

        public StatisticProvider(Workspace workspace, OJContext context)
        {
            _workspace = workspace;
            _context = context;
        }

        public async Task<ProblemStatistics> GetProblem(string id)
        {
            int _id = int.Parse(id);
            ProblemStatistics res = new ProblemStatistics();
            Submission[] subs = await (from item in _context.Submissions where item.ProblemId == _id select item).ToArrayAsync();
            res.SubmissionCount = subs.Length;
            res.SubmissionAcceptedCount = subs.Count(x => x.State == Core.Judgers.JudgeState.Accepted);
            return res;
        }

        public async Task<UserStatistics> GetUser(string id)
        {
            int _id = int.Parse(id);
            UserStatistics res = new UserStatistics();
            Submission[] subs = await (from item in _context.Submissions where item.UserId == _id select item).ToArrayAsync();
            res.SubmissionCount = subs.Length;
            res.SubmissionLanguageCount = new Dictionary<ProgrammingLanguage, int>();
            res.SubmissionStateCount = new Dictionary<JudgeState, int>();
            HashSet<int> solvedProblems = new HashSet<int>();
            foreach (Submission v in subs)
            {
                if (res.SubmissionLanguageCount.ContainsKey(v.Language))
                    res.SubmissionLanguageCount[v.Language]++;
                else
                    res.SubmissionLanguageCount.Add(v.Language, 1);

                if (res.SubmissionStateCount.ContainsKey(v.State))
                    res.SubmissionStateCount[v.State]++;
                else
                    res.SubmissionStateCount.Add(v.State, 1);
                if (v.State == JudgeState.Accepted)
                    solvedProblems.Add(v.ProblemId);
            }
            res.ProblemAcceptedId = solvedProblems.Select(x => x.ToString()).ToHashSet();
            return res;
        }
    }
}
