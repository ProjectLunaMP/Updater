using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaUpdater.Services
{
    public class Globals
    {
#if RELEASE
        public static string LauncherApi { get; set; } = "https://launcher.lunafn.org";
#else
        public static string LauncherApi { get; set; } = "http://127.0.0.1:1111";
#endif
        public static string LauncherInfoEndpoint { get; set; } = "/launcher/api/v1/version";

        public static string LauncherVersion { get; set; } = "0.0.1";
        public static string UpdaterVersion { get; set; } = "0.0.3"; // doesnt change much
    }
}
