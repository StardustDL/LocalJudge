using Newtonsoft.Json;
using StarOJ.Core;
using StarOJ.Core.Judgers;
using StarOJ.Core.Submissions;
using StarOJ.Data.Provider.SqlServer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.SqlServer
{
    public class SubmissionProvider : ISubmissionProvider
    {
        private readonly Workspace _workspace;
        private readonly OJContext _context;
        private readonly Submission _submission;

        public SubmissionProvider(Workspace workspace, OJContext context, Submission submission)
        {
            _workspace = workspace;
            _context = context;
            _submission = submission;
        }

        public string Id => _submission.Id.ToString();

        public Task<SubmissionMetadata> GetMetadata()
        {
            return Task.FromResult(new SubmissionMetadata
            {
                Id = Id,
                Code = _submission.Code,
                CodeLength = _submission.CodeLength,
                Language = _submission.Language,
                ProblemId = _submission.ProblemId.ToString(),
                Time = _submission.Time,
                UserId = _submission.UserId.ToString(),
            });
        }

        public Task<SubmissionResult> GetResult()
        {
            TimeSpan? totalTime = null;
            if (_submission.TotalTime.HasValue)
                totalTime = TimeSpan.FromTicks(_submission.TotalTime.Value);
            return Task.FromResult(new SubmissionResult
            {
                AcceptedCase = _submission.AcceptedCase,
                HasIssue = _submission.HasIssue,
                Issues = JsonConvert.DeserializeObject<List<Issue>>(_submission.Issues),
                MaximumMemory = _submission.MaximumMemory,
                TotalTime = totalTime,
                TotalCase = _submission.TotalCase,
                Samples = JsonConvert.DeserializeObject<List<JudgeResult>>(_submission.SampleResults),
                Tests = JsonConvert.DeserializeObject<List<JudgeResult>>(_submission.TestResults),
                State = _submission.State,
            });
        }

        public async Task SetMetadata(SubmissionMetadata value)
        {
            _submission.Code = value.Code;
            _submission.Language = value.Language;
            _submission.ProblemId = int.Parse(value.ProblemId);
            _submission.Time = value.Time;
            _submission.UserId = int.Parse(value.UserId);
            await _context.SaveChangesAsync();
        }

        public async Task SetResult(SubmissionResult value)
        {
            if (value == null)
            {
                _submission.AcceptedCase = null;
                _submission.HasIssue = false;
                _submission.Issues = "";
                _submission.TotalCase = null;
                _submission.TotalTime = null;
                _submission.SampleResults = "";
                _submission.TestResults = "";
                _submission.State = JudgeState.Pending;
                _submission.MaximumMemory = null;
            }
            else
            {
                _submission.AcceptedCase = value.AcceptedCase;
                _submission.HasIssue = value.HasIssue;
                _submission.Issues = JsonConvert.SerializeObject(value.Issues);
                _submission.TotalCase = value.TotalCase;
                _submission.TotalTime = value.TotalTime?.Ticks;
                _submission.SampleResults = JsonConvert.SerializeObject(value.Samples);
                _submission.TestResults = JsonConvert.SerializeObject(value.Tests);
                _submission.State = value.State;
                _submission.MaximumMemory = value.MaximumMemory;
            }
            await _context.SaveChangesAsync();
        }
    }
}
