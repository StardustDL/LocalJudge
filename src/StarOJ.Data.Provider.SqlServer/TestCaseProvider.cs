using StarOJ.Core;
using StarOJ.Core.Problems;
using StarOJ.Data.Provider.SqlServer.Models;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StarOJ.Data.Provider.SqlServer
{
    public class TestCaseProvider : ITestCaseProvider
    {
        public const string PF_Input = "input.data", PF_Output = "output.data";

        private readonly Workspace _workspace;
        private readonly OJContext _context;
        private readonly TestCase _testcase;

        public TestCaseProvider(Workspace workspace, OJContext context, TestCase testcase)
        {
            _workspace = workspace;
            _context = context;
            _testcase = testcase;
        }

        internal string GetRoot()
        {
            return Path.Join(_workspace.TestCaseStoreRoot, Id);
        }

        private string GetInputPath()
        {
            return Path.Join(GetRoot(), PF_Input);
        }

        private string GetOutputPath()
        {
            return Path.Join(GetRoot(), PF_Output);
        }

        public string Id => _testcase.Id.ToString();

        public Task<Stream> GetInput()
        {
            return Task.FromResult((Stream)File.OpenRead(GetInputPath()));
        }

        public Task<DataPreview> GetInputPreview(int maxbytes)
        {
            using (FileStream fs = File.OpenRead(GetInputPath()))
            {
                int len = (int)Math.Min(fs.Length, maxbytes);
                using (BinaryReader br = new BinaryReader(fs))
                    return Task.FromResult(new DataPreview
                    {
                        Content = Encoding.UTF8.GetString(br.ReadBytes(len), 0, len),
                        RemainBytes = fs.Length - len,
                    });
            }
        }

        public Task<TestCaseMetadata> GetMetadata()
        {
            return Task.FromResult(new TestCaseMetadata
            {
                Id = Id,
                MemoryLimit = _testcase.MemoryLimit,
                TimeLimit = _testcase.TimeLimit,
            });
        }

        public Task<Stream> GetOutput()
        {
            return Task.FromResult((Stream)File.OpenRead(GetOutputPath()));
        }

        public Task<DataPreview> GetOutputPreview(int maxbytes)
        {
            using (FileStream fs = File.OpenRead(GetOutputPath()))
            {
                int len = (int)Math.Min(fs.Length, maxbytes);
                using (BinaryReader br = new BinaryReader(fs))
                    return Task.FromResult(new DataPreview
                    {
                        Content = Encoding.UTF8.GetString(br.ReadBytes(len), 0, len),
                        RemainBytes = fs.Length - len,
                    });
            }
        }

        public async Task SetInput(Stream value)
        {
            using (FileStream fs = File.Open(GetInputPath(), FileMode.Create))
                await value.CopyToAsync(fs);
        }

        public async Task SetMetadata(TestCaseMetadata value)
        {
            _testcase.MemoryLimit = value.MemoryLimit;
            _testcase.TimeLimit = value.TimeLimit;
            await _context.SaveChangesAsync();
        }

        public async Task SetOutput(Stream value)
        {
            using (FileStream fs = File.Open(GetOutputPath(), FileMode.Create))
                await value.CopyToAsync(fs);
        }
    }
}
