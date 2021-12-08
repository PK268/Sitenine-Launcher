using Microsoft.Maui.Controls;

using System;

namespace Sitenine_Launcher
{
    public partial class MainPage : ContentPage
    {
        //string version;
        //string onlineVersion;
        //string originURL;
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            //SemanticScreenReader.Announce(CounterLabel.Text);
        }
    }
}
