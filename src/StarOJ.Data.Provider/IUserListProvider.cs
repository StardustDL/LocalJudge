using StarOJ.Core.Helpers;
using StarOJ.Core.Problems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarOJ.Core.Identity
{
    public interface IUserListProvider : IItemListProvider<IUserProvider,UserMetadata>
    {
        Task<IUserProvider> GetByName(string name);
    }
}
