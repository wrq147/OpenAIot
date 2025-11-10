using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;

namespace AirJointUI.Component
{
    public partial class ConfirmBox : Window
    {
        public Bitmap ImagePath { get; }
        private Action _callback;
        private Action _no;
        public ConfirmBox(string msg, Action callback, Action no = null)
        {
            InitializeComponent();
            _callback = callback;
            _no = no;
            this.MsgIcon.Source = new Bitmap(AssetLoader
                .Open(new Uri(
                    $"avares://AirJointUI/Assets/Images/question.png")));
            this.MsgTxt.Text = msg;
        }
        private void HandleOkClick(object sender, RoutedEventArgs e)
        {
            _callback.Invoke();
            Close(null);
        }
        private void HandleCancelClick(object sender, RoutedEventArgs e)
        {
            if (_no != null)
            {
                _no.Invoke();
            }
            Close(null);
        }
    }
}
