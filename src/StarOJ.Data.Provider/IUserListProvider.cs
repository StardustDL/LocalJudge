using System.Threading.Tasks;

namespace StarOJ.Core.Identity
{
    public interface IUserListProvider : IItemListProvider<IUserProvider, UserMetadata>
    {
        Task<IUserProvider> GetByName(string name);
    }
}
