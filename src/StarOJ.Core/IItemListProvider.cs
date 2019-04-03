using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarOJ.Core
{
    public interface IItemListProvider<TItem, TMetadata> where TItem : IHasId<string>, IHasMetadata<TMetadata>
    {
        Task<TItem> Create(TMetadata metadata);

        Task<TItem> Create();

        Task Delete(string id);

        Task<TItem> Get(string id);

        Task<IEnumerable<TItem>> GetAll();

        Task<bool> Has(string id);
    }
}
