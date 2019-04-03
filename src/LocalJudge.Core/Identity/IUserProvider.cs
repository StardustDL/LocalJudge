using LocalJudge.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LocalJudge.Core.Identity
{
    public interface IUserProvider : IHasId<string>, IHasMetadata<UserMetadata>
    {
    }
}
