using System;
using System.Collections.Generic;
using System.Text;

namespace LocalJudge.Core.Problems
{

    public class ProblemPath
    {
        public string Root { get; private set; }

        public string Profile { get; private set; }

        public ProblemMetadata GetMetadata()
        {
            throw new NotImplementedException();
        }

        public string Description { get; private set; }

        public ProblemDescription GetDescription()
        {
            throw new NotImplementedException();
        }

        public string Sample { get; private set; }

        public IEnumerable<TestCasePath> GetSamples()
        {
            throw new NotImplementedException();
        }

        public string Test { get; private set; }

        public IEnumerable<TestCasePath> GetTests()
        {
            throw new NotImplementedException();
        }

        public string Code { get; private set; }

        public string Extra { get; private set; }

        public ProblemPath(string root)
        {
            throw new NotImplementedException();
        }
    }
}
