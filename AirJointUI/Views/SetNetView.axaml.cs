using AirJointUI.Component;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using AirJointUI.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using AirJointUI.Utils;
using System.Threading;

namespace AirJointUI.Views
{
    public partial class SetNetView : UserControl
    {
        public SetNetView()
        {
            InitializeComponent();
            netPassword.ContextRequested += (sender, e) =>
            {
                e.Handled = true;
            };
        }
        private byte _isTab = 0;
        private TextBox hostControl;
        private async void HandleGotFocus(object sender, GotFocusEventArgs e)
        {
            if (Interlocked.CompareExchange(ref _isTab, 1, 0) == 0)
            {
                hostControl = (TextBox)sender;
                hostControl.Focusable = false;
                var topLevel = (Window)TopLevel.GetTopLevel(this);
                var result = await new CharacterKeyboard(hostControl.Text ?? string.Empty).ShowDialog<string>(topLevel);
                if (result != null)
                {
                    hostControl.Text = result;
                }

                hostControl.Focusable = true;
                FocusHolder.Focus();
                _isTab = 0;
            }

        }

        private void Wifi_Checked(object sender, RoutedEventArgs e)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    var svm = this.DataContext as SetNetViewModel;
                    Task.Run(() =>
                    {
                        svm.OpenNet();
                        if (svm.SSIDList.Count == 0)
                        {
                            svm.GetWifiList();
                        }
                    });
                }
                catch { }
            }

        }
        private void Wifi_Uncheck(object sender, RoutedEventArgs e)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    var svm = this.DataContext as SetNetViewModel;
                    Task.Run(() =>
                    {
                        svm.CloseNet();
                    });
                }
                catch { }
            }

        }
        private void HandleSave(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            btn.IsEnabled = false;
            var svm = this.DataContext as SetNetViewModel;
            Task.Run(async () =>
            {
                var rs = svm.Save();
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    if (rs)
                    {
                        var topLevel = (Window)TopLevel.GetTopLevel(this);
                        await new MessageBox("success", Properties.Resources.ResourceManager.GetString("SavedSuccessfully", I18NExt.Culture)).ShowDialog(topLevel);
                    }
                    else
                    {
                        var topLevel = (Window)TopLevel.GetTopLevel(this);
                        await new MessageBox("warning", Properties.Resources.ResourceManager.GetString("WifiPasswordError", I18NExt.Culture)).ShowDialog(topLevel);
                    }
                    btn.IsEnabled = true;
                });
            });
        }
    }
}
