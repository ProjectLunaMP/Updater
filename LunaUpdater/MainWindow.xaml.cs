using LunaUpdater.Services;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace LunaUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow
    {
        public MainWindow(/*IContentDialogService contentDialogService*/)
        {
            InitializeComponent();
           // contentDialogService.SetContentPresenter(RootContentDialog);

          //  testc(contentDialogService);
        }

        public async void testc(/*IContentDialogService contentDialogService*/)
        {
           
            //    ContentDialogResult result = await contentDialogService.ShowSimpleDialogAsync(
            //    new SimpleContentDialogCreateOptions()
            //    {
            //        Title = "Text",
            //        Content = "e",

            //        CloseButtonText = "I Agree",
            //    }

            //);

            //    await contentDialogService.ShowAlertAsync("tes", "test", "fs");
            //CancellationToken cancellationToken = CancellationToken.None;
            //await contentDialogService.ShowAsync(new ContentDialog
            //{
            //    Title = "TET",
            //    CloseButtonText = "ew"
            //}, cancellationToken); ; ;
        }

        private async void FluentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var windowsIdentity = WindowsIdentity.GetCurrent();
            var windowsPrincipal = new WindowsPrincipal(windowsIdentity);
            if (windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                MainTextBlock.Text = "Closing Luna";

                Process[] processs = Process.GetProcessesByName("Luna"); // :skull:

                if (processs.Length > 0)
                {
                    foreach (Process process1 in processs)
                    {
                        if (!process1.CloseMainWindow())
                        {
                            process1.Kill();
                        }
                    }
                }

                ProcessBarFr.IsIndeterminate = true;
                MainTextBlock.Text = "Checking Files";

                string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string DataFolder = System.IO.Path.Combine(BaseFolder, "Luna");
                Directory.CreateDirectory(DataFolder);

                await Task.Delay(500);

                // Old Fortnite CPU Fix
                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OPENSSL_ia32cap")))
                {
                    Environment.SetEnvironmentVariable("OPENSSL_ia32cap", "~0x20000000", EnvironmentVariableTarget.Machine);
                }


                HttpClient httpClient = new HttpClient();
                
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync($"{Globals.LauncherApi}{Globals.LauncherInfoEndpoint}");
                    if (response.IsSuccessStatusCode)
                    {
                        string Info = await response.Content.ReadAsStringAsync();

                        if(!string.IsNullOrEmpty(Info))
                        {
                            LauncherJson launcherJson = JsonConvert.DeserializeObject<LauncherJson>(Info)!;

                            if (launcherJson.updaterversion == Globals.UpdaterVersion)
                            {
                                ProcessBarFr.IsIndeterminate = false;
                                string[] yaNO = new string[]
                                {
                                    "/Q"
                                };
                           
                                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DevDiv\VC\Servicing\14.0\RuntimeMinimum", false);
                                if (key != null)
                                {
                                    var GrabVersion = key.GetValue("Version");

                                    if (!((string)GrabVersion).StartsWith("14"))
                                    {
                                        await Task.Run(async () => await DownloadAndRun("https://aka.ms/vs/17/release/vc_redist.x64.exe", Path.Combine(DataFolder, "vc_redist.x64.exe"), "VC Redist", yaNO));
                                        await Task.Run(async () => File.Delete(Path.Combine(DataFolder, "vc_redist.x64.exe")));

                                    }
                                }
                                else
                                {
                                    await Task.Run(async () => await DownloadAndRun("https://aka.ms/vs/17/release/vc_redist.x64.exe", Path.Combine(DataFolder, "vc_redist.x64.exe"), "VC Redist", yaNO));
                                }


                                using (RegistryKey directxkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DirectX", false))
                                {
                                    if (directxkey == null)
                                    {
                                        await Task.Run(async () => await DownloadAndRun($"https://download.microsoft.com/download/1/7/1/1718CCC4-6315-4D8E-9543-8E28A4E18C4C/dxwebsetup.exe", Path.Combine(DataFolder, "https://download.microsoft.com/download/1/7/1/1718CCC4-6315-4D8E-9543-8E28A4E18C4C/dxwebsetup.exe"), "DirectX", yaNO));
                                    }
                                }

                                MainTextBlock.Text = "Downloading Launcher";
                                var LauncherExe = "Luna.exe";
                                await Task.Run(async () => await DownloadAndRun($"{launcherJson.launcher}", Path.Combine(DataFolder, "LunaLauncher.zip"), "Luna Launcher", yaNO, false));
                                
                                ProcessBarFr.IsIndeterminate = true;

                                string BaseFolder1 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                                string DataFolder1 = System.IO.Path.Combine(BaseFolder1, "Luna");
                                Directory.CreateDirectory(DataFolder1);

                                await Task.Run(async () =>
                                {
                                  
                                    using (ZipArchive archive = ZipFile.OpenRead(Path.Combine(DataFolder, "LunaLauncher.zip")))
                                    {
                                        foreach (ZipArchiveEntry entry in archive.Entries)
                                        {
                                            string filePath = Path.Combine(DataFolder1, entry.FullName);
                                            filePath = Path.GetFullPath(filePath);

                                            string directoryPath = Path.GetDirectoryName(filePath);
                                            Directory.CreateDirectory(directoryPath);

                                            if (!entry.FullName.EndsWith("/"))
                                            {

                                                if (File.Exists(filePath))
                                                {
                                                    File.Delete(filePath);
                                                }

                                                entry.ExtractToFile(filePath);
                                            }
                                            else
                                            {
                                                Directory.CreateDirectory(filePath);
                                            }
                                        }
                                    }

                                });

                                MainTextBlock.Text = "Cleaning Up...";

                               

                                if (File.Exists(Path.Combine(DataFolder, "vc_redist.x64.exe")))
                                {
                                    await Task.Run(async () => File.Delete(Path.Combine(DataFolder, "vc_redist.x64.exe")));
                                }
                                if (File.Exists(Path.Combine(DataFolder, "dxwebsetup.exe")))
                                {
                                    await Task.Run(async () => File.Delete(Path.Combine(DataFolder, "dxwebsetup.exe")));
                                }

                                await Task.Run(async () => File.Delete(Path.Combine(DataFolder, "LunaLauncher.zip")));

                                MainTextBlock.Text = "Setting up a few things";

                                await Task.Run(() =>
                                {
                                    string DesktopFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                                    string userProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                                    string StatupProgams = Path.Combine(userProfileFolder, "AppData", "Roaming", "Microsoft", "Windows", "Start Menu", "Programs");

                                    try
                                    {
                                        string StatupProgams1 = Path.Combine(StatupProgams, "Luna", "Luna" + ".lnk");
                                        Directory.CreateDirectory(Path.Combine(StatupProgams, "Luna"));
                                        string DesktopFolderPath1 = Path.Combine(DesktopFolderPath, "Luna" + ".lnk");

                                        if (!File.Exists(StatupProgams1))
                                        {

                                            var shellType = Type.GetTypeFromProgID("WScript.Shell");
                                            dynamic shell = Activator.CreateInstance(shellType);
                                            //string escapedFolderPath = Uri.EscapeDataString(DataFolder);

                                            var shortcut = shell.CreateShortcut(StatupProgams1);
                                            shortcut.TargetPath = Path.Combine(DataFolder1, LauncherExe);
                                            shortcut.WorkingDirectory = DataFolder1;
                                            shortcut.Save();
                                        }

                                        if (!File.Exists(DesktopFolderPath1))
                                        {
                                            var shellType = Type.GetTypeFromProgID("WScript.Shell");
                                            dynamic shell = Activator.CreateInstance(shellType);
                      
                                            var shortcut = shell.CreateShortcut(DesktopFolderPath1);
                                            shortcut.TargetPath = Path.Combine(DataFolder1, LauncherExe);
                                            shortcut.WorkingDirectory = DataFolder1;
                                            shortcut.Save();
                                        }


                                        try
                                        {
                                            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall", true))
                                            {
                                                if (key != null)
                                                {
                                                    string PATHGOGMMGO = Path.Combine(DataFolder1, LauncherExe);
                                                    string Uninstall = "";
                                                    //System.Windows.MessageBox.Show(Process.GetCurrentProcess().MainModule.FileName);
                                                    Application.Current.Dispatcher.Invoke(() =>
                                                    {
                                                        Uninstall = Process.GetCurrentProcess().MainModule.FileName + " -uninstall";
                                                    });

                                                    using (RegistryKey appKey = key.CreateSubKey("LunaLauncher"))
                                                    {
                                                        appKey.SetValue("DisplayName", "Luna");
                                                        appKey.SetValue("DisplayIcon", PATHGOGMMGO);
                                                        appKey.SetValue("DisplayVersion", launcherJson.launcherversion);
                                                        appKey.SetValue("Publisher", "Luna");
                                                        appKey.SetValue("InstallLocation", PATHGOGMMGO);
                                                        appKey.SetValue("UninstallString", Uninstall);
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        System.Windows.MessageBox.Show("ERROR SAVING SHORTCUTS! " + ex.Message);
                                    }
                                });


                                Process process = new Process();
                                process.StartInfo.FileName = Path.Combine(DataFolder1, LauncherExe);
                                process.Start();
                                Application.Current.Shutdown();
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("Out of date updater.. please download the latest version from our discord!");
                                Application.Current.Shutdown();
                            }
                        }
                       
                    }
                    else { }
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                System.Windows.MessageBox.Show("PLEASE RUN THE APP AS ADMIN");
                Application.Current.Shutdown();
            }
                //ProcessBarFr
                //  CustomContetnDialog.Visibility = Visibility.Visible;
        }

        public async Task DownloadAndRun(string FileDownload, string WhereToDownload, string FileName1, string[] args, bool RunFile = true)//string[] args
        {
            try
            {
              
                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(5);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ProcessBarFr.Value = 0;
                    MainTextBlock.Text = $"Downloading... {FileName1}";
                });

                using (client)
                {
                    //string FileName = System.IO.Path.GetFileName(new Uri(FileDownload).LocalPath);
                    using (FileStream outputStream = new FileStream(WhereToDownload, FileMode.Create, FileAccess.Write))
                    {
                        using (HttpResponseMessage response = await client.GetAsync(FileDownload, HttpCompletionOption.ResponseHeadersRead))
                        {
                            response.EnsureSuccessStatusCode();

                            using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                            {
                                long? totalBytes = response.Content.Headers.ContentLength;
                                byte[] buffer = new byte[8192];
                                long bytesRead = 0;
                                int bytesReadThisIteration;

                                while ((bytesReadThisIteration = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    outputStream.Write(buffer, 0, bytesReadThisIteration);
                                    bytesRead += bytesReadThisIteration;
                                    double percentComplete = totalBytes.HasValue ? (double)bytesRead / totalBytes.Value * 100 : 0;
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        ProcessBarFr.Value = percentComplete;
                                        MainTextBlockPer.Text = $"{percentComplete:F2}%";
                                    });
                                }
                               
                            }
                        }
                    }

                    if (RunFile)
                    {

                        System.Windows.MessageBox.Show(WhereToDownload);
                        Process Yafds = new Process();
                        Yafds.StartInfo.Arguments = string.Join(" ", args);
                        Yafds.StartInfo.FileName = WhereToDownload;
                        Yafds.StartInfo.UseShellExecute = true;
                        Yafds.Start();
                        Yafds.WaitForExit();
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MainTextBlockPer.Text = "";
                            MainTextBlock.Text = $"{FileName1} Installed";
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                System.Windows.MessageBox.Show("Your Antiviruses might effect the launcher\nThats why we and other server recommend you to use Windows Defender");
            }
        }
    }
}