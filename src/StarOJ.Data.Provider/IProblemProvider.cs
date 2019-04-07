using System.Threading.Tasks;

namespace StarOJ.Core.Problems
{
    public interface IProblemProvider : IHasId<string>, IHasMetadata<ProblemMetadata>
    {
        Task<ProblemDescription> GetDescription();

        Task SetDescription(ProblemDescription value);

        ITestCaseListProvider Samples { get; }

        ITestCaseListProvider Tests { get; }
    }
}
