using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpartanShield
{
    public class SessionInfo
    {
        public bool IsLoggedIn { get; set; }
        public bool IsEncrypted { get; set; }
        public string? CurrentUsername { get; set; }
    }
}
