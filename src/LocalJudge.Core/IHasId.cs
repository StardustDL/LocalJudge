namespace LocalJudge.Core
{
    public interface IHasId<TId>
    {
        TId Id { get; }
    }
}
