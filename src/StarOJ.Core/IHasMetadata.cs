using System.Threading.Tasks;

namespace StarOJ.Core
{
    public interface IHasMetadata<TMetadata>
    {
        Task<TMetadata> GetMetadata();

        Task SetMetadata(TMetadata value);
    }
}
