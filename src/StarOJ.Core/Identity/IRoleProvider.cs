using StarOJ.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StarOJ.Core.Identity
{
    public interface IRoleProvider : IHasId<string>,IHasMetadata<RoleMetadata>
    {
    }
}
