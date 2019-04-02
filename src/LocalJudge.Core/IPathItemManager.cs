using System.Collections.Generic;

namespace LocalJudge.Core
{
    public interface IPathItemManager<TPathItem> : IHasRoot where TPathItem : IHasId<string>
    {
        TPathItem Create(string id);

        void Delete(string id);

        TPathItem Get(string id);

        IEnumerable<TPathItem> GetAll();

        bool Has(string id);
    }
}
