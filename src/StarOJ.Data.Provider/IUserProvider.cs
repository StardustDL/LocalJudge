namespace StarOJ.Core.Identity
{
    public interface IUserProvider : IHasId<string>, IHasMetadata<UserMetadata>
    {
    }
}
