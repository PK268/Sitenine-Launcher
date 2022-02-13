using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;
using Microsoft.Maui.Handlers;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Reflection;

using ZstdNet;

namespace Sitenine_Launcher
{
    public partial class MainPage : ContentPage
    {
        string version;
        string onlineVersion;
        string originURL;
        int count = 0;
        string[] parentDirectories = { @"C:\ProgramData\SiteNine", $"/Users/{Environment.UserName}/SiteNine" };
        string root;

        public MainPage()
        {
            InitializeComponent();

            /*
            System.Drawing.Icon icon = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("icon256.ico")!);

            WindowHandler.WindowMapper.Add(nameof(IWindow), (handler, view) => {
                IntPtr windowHandle = ((Microsoft.Maui.MauiWinUIWindow)handler.NativeView).WindowHandle;
                PInvoke.User32.SendMessage(windowHandle, PInvoke.User32.WindowMessage.WM_SETICON, (IntPtr)0, icon.Handle);
                PInvoke.User32.SendMessage(windowHandle, PInvoke.User32.WindowMessage.WM_SETICON, (IntPtr)1, icon.Handle);
            });
            */

            if (Environment.OSVersion.Platform.ToString().Contains("Win"))
            {
                root = parentDirectories[0];
            }
            else
            {
                root = parentDirectories[1];
            }

            string versionNumber = new WebClient().DownloadString("https://matgames.net/sitenine/version.txt");
            VersionLabel.Text = $"V: {versionNumber}";

            MainButton.Text = "Checking For Updates";
            if (!Directory.Exists(@$"{root}\SiteNine"))
            {
                Directory.CreateDirectory(@$"{root}\SiteNine");
            }
            WebClient wb = new WebClient();
            originURL = wb.DownloadString("https://drive.google.com/uc?export=download&id=1XVMbtyWLCLVqX9ans-YuEviS8SNtYIFM");
            wb.DownloadFile(new Uri($"{originURL}/sitenine/version.txt"), @$"{root}\SiteNine\onlineVersion.txt");
            onlineVersion = File.ReadAllText(@$"{root}\SiteNine\onlineVersion.txt");
            File.Delete(@$"{root}\SiteNine\onlineVersion.txt");

            if (File.Exists(@$"{root}\SiteNine\version.txt"))
            {
                version = File.ReadAllText(@$"{root}\SiteNine\version.txt");
                if (onlineVersion == version)
                {
                    MainButton.Text = "Play";
                }
                else
                {
                    MainButton.Text = "Update";
                }
            }
            else
            {
                var file = File.Create(@$"{root}\SiteNine\version.txt");
                file.Close();
                File.WriteAllText(@$"{root}\SiteNine\version.txt", onlineVersion);
                MainButton.Text = "Install";
            }
            VersionLabel.Text = version;
        }

        private void MainButton_Click(object sender, EventArgs e)
        {
            switch (MainButton.Text)
            {
                case "Play":
                    {
                        if (root == parentDirectories[0])
                        {
                            Process.Start(@$"{root}\SiteNine\Build\Site 9.exe");
                        }
                        else
                        {
                            Process.Start(@$"SiteNine/Site 9.app/Contents/MacOS/Site 9");
                        }
                        Environment.Exit(0);
                    }
                    break;
                case "Install":
                    {
                        InstallFiles();
                        File.WriteAllText(@$"{root}\SiteNine\version.txt", onlineVersion);
                    }
                    break;
                case "Update":
                    {
                        InstallFiles();
                        File.WriteAllText(@$"{root}\SiteNine\version.txt", onlineVersion);
                    }
                    break;
                default:
                    break;
            }
        }

        void InstallFiles()
        {
            MainButton.Text = "Downloading Files...";
            //MainButton.Font = new Font("Microsoft HeiYe", 18, FontStyle.Bold);
            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(finalizeInstall);
            webClient.DownloadFileAsync(new Uri($"{originURL}/sitenine/win/Build.7z"), @$"{root}\SiteNine\build.7z");
        }
        void finalizeInstall(object sender, AsyncCompletedEventArgs e)
        {
            //MainButton.Font = new Font("Microsoft HeiYe", 22, FontStyle.Bold);
            if (root == parentDirectories[0] && Directory.Exists(@$"{root}\SiteNine\build"))
            {
                Directory.Delete(@$"{root}\SiteNine\build", true);
            }
            else if (root == parentDirectories[1] && Directory.Exists(@$"{root}\SiteNine\Build\Site 9.app"))
            {
                try
                {
                    Directory.Delete(@$"SiteNine/Site 9.app", true);
                    Directory.Delete(@"SiteNine/__MACOSX", true);
                }
                catch (Exception)
                {
                }
            }
            var src = File.ReadAllBytes(@$"{root}\SiteNine\build.7z");
            using var decompressor = new Decompressor();
            var decompressed = decompressor.Unwrap(src);
            ZipFile.ExtractToDirectory(@$"{root}\SiteNine\build.7z", @$"{root}\SiteNine\");
            File.Delete(@$"{root}\SiteNine\build.7z");
            File.WriteAllText(@$"{root}\SiteNine\version.txt", onlineVersion);
            MainButton.Text = "Play";
            VersionLabel.Text = onlineVersion;
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            //SemanticScreenReader.Announce(CounterLabel.Text);
        }
    }
}
