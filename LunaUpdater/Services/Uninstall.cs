using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaUpdater.Services
{
    public class Uninstall
    {
        public static async Task UninstallStuff()
        {
            string DesktopFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string userProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string StatupProgams = Path.Combine(userProfileFolder, "AppData", "Roaming", "Microsoft", "Windows", "Start Menu", "Programs");
            string StatupProgams1 = Path.Combine(StatupProgams, "Luna", "Luna" + ".lnk");
            string DesktopFolderPath1 = Path.Combine(DesktopFolderPath, "Luna" + ".lnk");


            if (File.Exists(StatupProgams1))
            {
                File.Delete(StatupProgams1);
            }

            if (File.Exists(DesktopFolderPath1))
            {
                File.Delete(DesktopFolderPath1);
            }

            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall", true))
                {
                    if (key != null)
                    {
                        foreach (string subkeyName in key.GetSubKeyNames())
                        {
                            using (RegistryKey subkey = key.OpenSubKey(subkeyName, true))
                            {
                                if (subkey != null)
                                {
                                    object displayName = subkey.GetValue("DisplayName");
                                    if (displayName != null && displayName.ToString() == "LunaLauncher")
                                    {
                                        key.DeleteSubKeyTree(subkeyName);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                Registry.LocalMachine.DeleteSubKeyTree(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\LunaLauncher");
            }
            catch (Exception ex)
            {

            }

        }
          
    }
}
