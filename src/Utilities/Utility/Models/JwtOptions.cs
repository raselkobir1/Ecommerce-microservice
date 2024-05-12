using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Models
{
    public class JwtOptions
    {
        public string Secret { get; set; }
        public int ExpiryMinutes { get; set; }
    }
}
