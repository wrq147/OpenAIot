using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;

namespace AirJointUI.Component
{
    public partial class InfoBox : Window
    {
        public Bitmap ImagePath { get; }
        public InfoBox(string icon, string msg)
        {
            InitializeComponent();
            this.MsgIcon.Source = new Bitmap(AssetLoader
                .Open(new Uri(
                    $"avares://AirJointUI/Assets/Images/{icon.ToString().ToLowerInvariant()}.png")));
            this.MsgTxt.Text = msg;
        }
        private void HandleOkClick(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}
