using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;

namespace AirJointUI.Component
{
    public partial class MessageBox : Window
    {
        public Bitmap ImagePath { get; }
        public MessageBox(string icon, string msg)
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
