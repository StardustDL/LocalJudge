using System;
using System.Collections.Generic;
using System.Text;

namespace StarOJ.Core.Problems
{
    public interface ITestCaseListProvider : IItemListProvider<ITestCaseProvider, TestCaseMetadata>
    {
    }
}
