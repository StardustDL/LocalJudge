using System.Threading.Tasks;

namespace StarOJ.Core.Identity
{
    public interface IRoleListProvider : IItemListProvider<IRoleProvider, RoleMetadata>
    {
        Task<IRoleProvider> GetByName(string name);
    }
}
