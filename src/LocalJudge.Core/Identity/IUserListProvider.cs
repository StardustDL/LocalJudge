using LocalJudge.Core.Helpers;
using LocalJudge.Core.Problems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LocalJudge.Core.Identity
{
    public interface IUserListProvider : IItemListProvider<IUserProvider,UserMetadata>
    {
        IUserProvider GetByName(string name);
    }
}
