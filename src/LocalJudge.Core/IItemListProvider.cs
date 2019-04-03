using System.Collections.Generic;

namespace LocalJudge.Core
{
    public interface IItemListProvider<TItem, TMetadata> where TItem : IHasId<string>, IHasMetadata<TMetadata>
    {
        TItem Create(TMetadata metadata);

        TItem Create(string id);

        void Delete(string id);

        TItem Get(string id);

        IEnumerable<TItem> GetAll();

        bool Has(string id);
    }
}
