using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.JWT
{
    public interface IJwtBuilder
    {
        string GetToken(string userId);
        string ValidateToken(string token);
    }
}
