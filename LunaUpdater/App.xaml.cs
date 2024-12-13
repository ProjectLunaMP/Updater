
using LunaUpdater.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace LunaUpdater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
          //  base.OnStartup(e);
            
            if (Array.Exists(e.Args, arg => arg == "-uninstall"))
            {

                System.Windows.MessageBox.Show("Uninstalling...");

                await Uninstall.UninstallStuff();

                string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string DataFolder = System.IO.Path.Combine(BaseFolder, "Luna");
                if (Directory.Exists(DataFolder))
                {
                    Directory.Delete(DataFolder, true);
                }

                Shutdown();

                // We dont want the base addr so we will do local app data!
                // we dont want to delete the users download folder

                string LunaUpdater = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Luna");
                if (Directory.Exists(LunaUpdater))
                {
                    Directory.Delete(DataFolder, true); // we cant always open stuff so this prob won't work
                }

                  
            }
            else
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
              //  _host.Start();
            }
           
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {

        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {

        }
    }

}
