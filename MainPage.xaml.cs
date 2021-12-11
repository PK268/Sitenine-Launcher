using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;

using ZstdNet;

namespace Sitenine_Launcher
{
    public partial class MainPage : ContentPage
    {
        string version;
        string onlineVersion;
        string originURL;
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            string versionNumber = new WebClient().DownloadString("https://matgames.net/sitenine/version.txt");
            VersionLabel.Text = $"V: {versionNumber}";

            MainButton.Text = "Checking For Updates";
            if (!Directory.Exists(@"C:\ProgramData\SiteNine"))
            {
                Directory.CreateDirectory(@"C:\ProgramData\SiteNine");
            }
            WebClient wb = new WebClient();
            originURL = wb.DownloadString("https://drive.google.com/uc?export=download&id=1XVMbtyWLCLVqX9ans-YuEviS8SNtYIFM");
            wb.DownloadFile(new Uri($"{originURL}/sitenine/version.txt"), @"C:\ProgramData\SiteNine\onlineVersion.txt");
            onlineVersion = File.ReadAllText(@"C:\ProgramData\SiteNine\onlineVersion.txt");
            File.Delete(@"C:\ProgramData\SiteNine\onlineVersion.txt");

            if (File.Exists(@"C:\ProgramData\SiteNine\version.txt"))
            {
                version = File.ReadAllText(@"C:\ProgramData\SiteNine\version.txt");
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
                var file = File.Create(@"C:\ProgramData\SiteNine\version.txt");
                file.Close();
                File.WriteAllText(@"C:\ProgramData\SiteNine\version.txt", onlineVersion);
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
                        Process.Start(@"C:\ProgramData\SiteNine\Build\Site 9.exe");
                        Environment.Exit(0);
                    }
                    break;
                case "Install":
                    {
                        InstallFiles();
                        File.WriteAllText(@"C:\ProgramData\SiteNine\version.txt", onlineVersion);
                    }
                    break;
                case "Update":
                    {
                        InstallFiles();
                        File.WriteAllText(@"C:\ProgramData\SiteNine\version.txt", onlineVersion);
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
            webClient.DownloadFileAsync(new Uri($"{originURL}/sitenine/win/Build.zip"), @"C:\ProgramData\SiteNine\build.zip");
        }
        void finalizeInstall(object sender, AsyncCompletedEventArgs e)
        {
            //MainButton.Font = new Font("Microsoft HeiYe", 22, FontStyle.Bold);
            if (Directory.Exists(@"C:\ProgramData\SiteNine\build"))
            {
                Directory.Delete(@"C:\ProgramData\SiteNine\build", true);
            }

            var src = File.ReadAllBytes(@"C:\ProgramData\SiteNine\build.zip");
            using var decompressor = new Decompressor();
            var decompressed = decompressor.Unwrap(src);
            ZipFile.ExtractToDirectory(@"C:\ProgramData\SiteNine\build.zip", @"C:\ProgramData\SiteNine\");
            File.Delete(@"C:\ProgramData\SiteNine\build.zip");
            File.WriteAllText(@"C:\ProgramData\SiteNine\version.txt", onlineVersion);
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
