using AirJointUI.Component;
using AirJointUI.Utils;
using AirJointUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Diagnostics;
using System.Threading;


namespace AirJointUI.Views;

public partial class SetSerialView : UserControl
{
    public SetSerialView()
    {
        InitializeComponent();
        ipTxt.ContextRequested += (sender, e) =>
        {
            e.Handled = true;
        };
        downDetaTxt.ContextRequested += (sender, e) =>
        {
            e.Handled = true;
        };
        xxDetaTxt.ContextRequested += (sender, e) =>
        {
            e.Handled = true;
        };
        turnOff.ContextRequested += (sender, e) =>
        {
            e.Handled = true;
        };
    }
    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = sender as ComboBox;
        if (comboBox != null)
        {
            if (comboBox.SelectedIndex == 2)
            {
                var svm = this.DataContext as SetSerialViewModel;
                if (svm != null)
                {
                    svm.ShowIp = true;
                }
            }
            else
            {
                var svm = this.DataContext as SetSerialViewModel;
                if (svm != null)
                {
                    svm.ShowIp = false;
                }
            }
        }
    }
    private byte _isTab = 0;
    private async void HandleIpPortFocus(object sender, GotFocusEventArgs e)
    {
        if (Interlocked.CompareExchange(ref _isTab, 1, 0) == 0)
        {
            TextBox hostControl = (TextBox)sender;
            hostControl.Focusable = false;
            var topLevel = (Window)TopLevel.GetTopLevel(this);
            var result = await new CharacterKeyboard(hostControl.Text).ShowDialog<string>(topLevel);
            if (result != null)
            {
                hostControl.Text = result;
            }
            hostControl.Focusable = true;
            FocusHolder.Focus();
            _isTab = 0;
        }

    }
    private async void HandleGotFocus(object sender, GotFocusEventArgs e)
    {
        if (Interlocked.CompareExchange(ref _isTab, 1, 0) == 0)
        {
            TextBox hostControl = (TextBox)sender;
            hostControl.Focusable = false;
            var topLevel = (Window)TopLevel.GetTopLevel(this);
            var result = await new NumberKeyboard(hostControl.Text).ShowDialog<string>(topLevel);
            if (result != null)
            {
                int tmprs;
                if (!int.TryParse(result, out tmprs))
                {
                    tmprs = 300;
                }
                hostControl.Text = tmprs.ToString();
            }

            hostControl.Focusable = true;
            FocusHolder.Focus();
            _isTab = 0;
        }

    }
    private async void HandleTurnOffGotFocus(object sender, GotFocusEventArgs e)
    {
        if (Interlocked.CompareExchange(ref _isTab, 1, 0) == 0)
        {
            TextBox hostControl = (TextBox)sender;
            hostControl.Focusable = false;
            var topLevel = (Window)TopLevel.GetTopLevel(this);
            var result = await new NumberKeyboard(hostControl.Text).ShowDialog<string>(topLevel);
            if (result != null)
            {
                int tmprs;
                if (!int.TryParse(result, out tmprs))
                {
                    tmprs = 300;
                }
                hostControl.Text = tmprs.ToString();
            }

            hostControl.Focusable = true;
            FocusHolder.Focus();
            _isTab = 0;
        }

    }
    private async void HandleSave(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;
        var svm = this.DataContext as SetSerialViewModel;
        var topLevel = (Window)TopLevel.GetTopLevel(this);
        string saveTip = Properties.Resources.ResourceManager.GetString("SaveTip", I18NExt.Culture);
        await new ConfirmBox(saveTip, () =>
        {
            try
            {
                var rs = svm.Save();

                if (rs)
                {
                    Process.Start("bash", "-c \"sudo shutdown -r now\"");
                }
                else
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        new MessageBox("warning", Properties.Resources.ResourceManager.GetString("SaveFailed", I18NExt.Culture)).ShowDialog(topLevel);
                    });
                }
            }
            catch (Exception ex)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    new MessageBox("warning", ex.Message).ShowDialog(topLevel);
                });
            }

        }).ShowDialog(topLevel);
    }

    private void RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        RadioButton rb = (RadioButton)sender;
        var svm = this.DataContext as SetSerialViewModel;
        svm.EnablePort = true;
        svm.EnableHoriz = false;
    }

    private void RadioButton2_Checked(object sender, RoutedEventArgs e)
    {
        RadioButton rb = (RadioButton)sender;
        var svm = this.DataContext as SetSerialViewModel;
        svm.EnablePort = false;
        svm.EnableHoriz = true;
    }

}