namespace StarOJ.Core.Identity
{
    public interface IRoleProvider : IHasId<string>, IHasMetadata<RoleMetadata>
    {
    }
}
