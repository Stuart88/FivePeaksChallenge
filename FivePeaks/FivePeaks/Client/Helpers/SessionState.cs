using FivePeaks.Shared.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FivePeaks.Client.Helpers
{
    public static class SessionState
    {
        public static bool AdminLoggedIn = false;
        public static AdminUser Admin = null;
        public static bool UserLoggedIn = false;
        public static UserAccount User = null;
    }
}
