using System;

namespace LocalJudge.Core.Problems
{
    public class TestCasePath
    {
        public string Name { get; private set; }

        public string Profile { get; private set; }

        public TestCaseMetadata GetMetadata()
        {
            throw new NotImplementedException();
        }

        public string Input { get; private set; }

        public string GetInput()
        {
            throw new NotImplementedException();
        }

        public string Output { get; set; }

        public string GetOutput()
        {
            throw new NotImplementedException();
        }
    }
}
