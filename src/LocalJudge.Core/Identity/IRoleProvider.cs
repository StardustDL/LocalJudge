using LocalJudge.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LocalJudge.Core.Identity
{
    public interface IRoleProvider : IHasId<string>,IHasMetadata<RoleMetadata>
    {
    }
}
