using StarOJ.Core.Helpers;
using StarOJ.Core.Problems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StarOJ.Core.Identity
{
    public interface IRoleListProvider : IItemListProvider<IRoleProvider,RoleMetadata>
    {
        IRoleProvider GetByName(string name);
    }
}
