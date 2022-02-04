using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpartanShield
{
    public static class SessionInfo
    {
        public static bool IsLoggedIn { get; set; } = false;
        public static bool IsEncrypted { get; set; } = false;
        public static string? CurrentUsername { get; set; } = string.Empty;
    }
}
