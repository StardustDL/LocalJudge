namespace StarOJ.Core
{
    public interface IHasId<TId>
    {
        TId Id { get; }
    }

    public interface IHasMetadata<TMetadata>{
        TMetadata GetMetadata();

        void SetMetadata(TMetadata value);
    }
}
