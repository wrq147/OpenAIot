using AirJointUI.Models;
using AirJointUI.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using Avalonia.Threading;
using System.Threading.Tasks;
using AirJointUI.Component;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using AirJointUI.Utils;


namespace AirJointUI.Views;

public partial class IndexView : UserControl
{
    public IndexView()
    {
        InitializeComponent();
    }
    private bool _isFirst = true;
    private bool _isInit = false;
    private int previousSelectedIndex = -1;
    protected override void OnInitialized()
    {
        _isFirst = false;
        _isInit = true;
    }
    public override void BeginInit()
    {
        base.BeginInit();
        var svm = MyVMLocator.Instance.IndexViewModel;
        for (int i = 0; i < svm.LangList.Count; i++)
        {
            if (svm.LangList[i].Val == I18NExt.Culture.Name)
            {
                svm.LanIdx = i;
                previousSelectedIndex = i;
                break;
            }
        }
    }
    // 上一页按钮点击处理
    public void HandlePreviousPage(object sender, RoutedEventArgs e)
    {
        var svm = this.DataContext as IndexViewModel;
        svm.OnPrePage();

    }

    // 下一页按钮点击处理
    public void HandleNextPage(object sender, RoutedEventArgs e)
    {
        var svm = this.DataContext as IndexViewModel;
        svm.OnNextPage();
    }

    private void Lang_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isInit)
        {
            ComboBox comboBox = sender as ComboBox;
            var item = comboBox.SelectedItem as ComboItem;
            if (item != null)
            {
                var topLevel = (Window)TopLevel.GetTopLevel(this);
                string saveTip = Properties.Resources.ResourceManager.GetString("SaveTip", I18NExt.Culture);
                if (topLevel != null)
                {
                    new ConfirmBox(saveTip, () =>
                    {
                        try
                        {
                            I18NExt.Culture = new CultureInfo(item.Val);
                            File.WriteAllText("lang", item.Val);

                            Process.Start("bash", "-c \"sudo shutdown -r now\"");
                        }
                        catch (Exception ex)
                        {
                            Dispatcher.UIThread.Invoke(() =>
                            {
                                new MessageBox("warning", ex.Message).ShowDialog(topLevel);
                            });
                        }

                    }, () =>
                    {
                        if (!_isFirst)
                        {
                            var svm = this.DataContext as IndexViewModel;
                            if (svm != null)
                            {
                                _isInit = false;
                                svm.LanIdx = previousSelectedIndex;
                            }

                        }

                    }).ShowDialog(topLevel);
                }
            }
        }
        else
        {
            _isInit = true;
        }
    }
    private void btn_alarmPressed(object sender, PointerPressedEventArgs e)
    {
        //跳转告警提示页面
        MyVMLocator.Instance.Window.ContentViewModel = MyVMLocator.Instance.AlarmListViewModel;
        Task t = MyVMLocator.Instance.AlarmListViewModel.Init();
    }
    private void btn_setPressed(object sender, PointerPressedEventArgs e)
    {
        SetBTT.IsEnabled = false;
        sysSetTxt.Text = Properties.Resources.ResourceManager.GetString("Doing", I18NExt.Culture);

        Task.Run(async () =>
        {
            //跳转到设置页面
            MyVMLocator.Instance.Window.ContentViewModel = MyVMLocator.Instance.SetViewModel;
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                sysSetTxt.Text = Properties.Resources.ResourceManager.GetString("SetButton", I18NExt.Culture);
                SetBTT.IsEnabled = true;
            });
        });
    }
    private void btn_GoView(object sender, PointerPressedEventArgs e)
    {
        GoViewBTT.IsEnabled = false;
        goViewTxt.Text = Properties.Resources.ResourceManager.GetString("Doing", I18NExt.Culture);

        Task.Run(async () =>
        {
            //跳转到设置页面
            MyVMLocator.Instance.Window.ContentViewModel = MyVMLocator.Instance.GoWebViewModel;
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                goViewTxt.Text = Properties.Resources.ResourceManager.GetString("WebReport", I18NExt.Culture);
                GoViewBTT.IsEnabled = true;
            });
        });
    }

    private async void btn_restoreAll(object sender, PointerPressedEventArgs e)
    {
        var topLevel = (Window)TopLevel.GetTopLevel(this);
        var svm = this.DataContext as IndexViewModel;
        string allRestoreTip = Properties.Resources.ResourceManager.GetString("AllRestoreTip", I18NExt.Culture);
        await new ConfirmBox(allRestoreTip, async () =>
        {
            if (!File.Exists("savedev"))
            {
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await new MessageBox("error", "file not found").ShowDialog(topLevel);
                });
                return;
            }
            var savestr = File.ReadAllText("savedev");
            var saveList = JsonSerializer.Deserialize(savestr, GenericJsonContext.Default.ListDeviceSave);
            foreach (var tmpdev in svm.DeviceItems)
            {
                if (tmpdev.EnableShow == true)
                {
                    var saveitem = saveList.Where(x => x.Id == tmpdev.Id).FirstOrDefault();
                    if (saveitem != null)
                    {
                        await svm.EnableAutoControl(tmpdev.Number, saveitem.IsControl);
                    }
                }
            }

            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await new MessageBox("success", Properties.Resources.ResourceManager.GetString("OpSuccess", I18NExt.Culture)).ShowDialog(topLevel);
            });

        }).ShowDialog(topLevel);
    }
    private async void btn_shutdownAll(object sender, PointerPressedEventArgs e)
    {

        var topLevel = (Window)TopLevel.GetTopLevel(this);
        var svm = this.DataContext as IndexViewModel;
        string allStopTip = Properties.Resources.ResourceManager.GetString("AllStopTip", I18NExt.Culture);
        await new ConfirmBox(allStopTip, async () =>
        {
            try
            {
                List<DeviceSave> saveList = new List<DeviceSave>();
                bool isjoint = false;
                foreach (var tmpdev in svm.DeviceItems)
                {
                    if (tmpdev.EnableShow == true)
                    {
                        saveList.Add(new DeviceSave()
                        {
                            Id = tmpdev.Id,
                            IsControl = tmpdev.JointControlState,
                            IsRunning = tmpdev.IsRuning
                        });
                    }
                    if (tmpdev.JointControlState)
                    {
                        isjoint = true;
                    }
                }
                if (!isjoint)
                {
                    return;
                }
                var tdatastr = JsonSerializer.Serialize(saveList, GenericJsonContext.Default.ListDeviceSave);
                File.WriteAllText("savedev", tdatastr);
                svm.ExistRestore = true;
                foreach (var tmpdev in svm.DeviceItems)
                {
                    if (tmpdev.EnableShow == true)
                    {
                        if (tmpdev.JointControlState)
                        {
                            await svm.EnableAutoControl(tmpdev.Number, false);
                        }
                    }
                }

                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await new MessageBox("success", Properties.Resources.ResourceManager.GetString("OpSuccess", I18NExt.Culture)).ShowDialog(topLevel);
                });
            }
            catch (Exception ex)
            {
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await new MessageBox("error", ex.Message).ShowDialog(topLevel);
                });
            }


        }).ShowDialog(topLevel);
    }
    private async void btn_resetSystem(object sender, PointerPressedEventArgs e)
    {
        var topLevel = (Window)TopLevel.GetTopLevel(this);
        string rebootTip = Properties.Resources.ResourceManager.GetString("RebootTip", I18NExt.Culture);
        await new ConfirmBox(rebootTip, () =>
        {
            try
            {
                Process.Start("bash", "-c \"sudo shutdown -r now\"");
            }
            catch { }
        }).ShowDialog(topLevel);
    }
    private async void ButtonMore_Click(object sender, RoutedEventArgs e)
    {
        var clkbt = (Button)sender;
        var devItem = clkbt.DataContext as DeviceItem_IdxV;
        var topLevel = (Window)TopLevel.GetTopLevel(this);
        await new ParamDetailBoard(devItem).ShowDialog(topLevel);
    }
    private async void ButtonIptMore_Click(object sender, RoutedEventArgs e)
    {
        var clkbt = (Button)sender;
        var iptItem = clkbt.DataContext as InputItem_IdxV;
        var topLevel = (Window)TopLevel.GetTopLevel(this);
        await new ParamDetailBoard(iptItem).ShowDialog(topLevel);
    }
    
    private async void ButtonStop_Click(object? sender, RoutedEventArgs e)
    {
        // 设备停止
        var clkbt = (Button)sender;
        var devItem = clkbt.DataContext as DeviceItem_IdxV;
        var svm = this.DataContext as IndexViewModel;
        clkbt.IsEnabled = false;
        try
        {
            var rs = await svm.StopDev(devItem.Number);
            clkbt.IsEnabled = true;
            if (!rs)
            {
                var topLevel = (Window)TopLevel.GetTopLevel(this);
                await new MessageBox("warning", Properties.Resources.ResourceManager.GetString("ShutdownFailed", I18NExt.Culture)).ShowDialog(topLevel);
            }
        }
        catch (Exception ex)
        {
            clkbt.IsEnabled = true;
            var topLevel = (Window)TopLevel.GetTopLevel(this);
            await new MessageBox("error", Properties.Resources.ResourceManager.GetString("ShutdownException", I18NExt.Culture) + ex.Message).ShowDialog(topLevel);
        }

    }
    private async void ButtonStar_Click(object? sender, RoutedEventArgs e)
    {
        // 设备启动
        var clkbt = (Button)sender;
        var devItem = clkbt.DataContext as DeviceItem_IdxV;
        var svm = this.DataContext as IndexViewModel;
        clkbt.IsEnabled = false;
        try
        {
            var rs = await svm.StartDev(devItem.Number);
            clkbt.IsEnabled = true;
            if (!rs)
            {
                var topLevel = (Window)TopLevel.GetTopLevel(this);
                await new MessageBox("warning", Properties.Resources.ResourceManager.GetString("StartFailed", I18NExt.Culture)).ShowDialog(topLevel);

            }
        }
        catch (Exception ex)
        {
            clkbt.IsEnabled = true;
            var topLevel = (Window)TopLevel.GetTopLevel(this);
            await new MessageBox("error", Properties.Resources.ResourceManager.GetString("StartException", I18NExt.Culture) + ex.Message).ShowDialog(topLevel);

        }

    }
    private async void HandleChangeStatus(object sender, RoutedEventArgs e)
    {
        var clkbt = (ToggleButton)sender;
        var devItem = clkbt.DataContext as DeviceItem_IdxV;
        var svm = this.DataContext as IndexViewModel;
        clkbt.IsEnabled = false;
        try
        {
            var rs = await svm.EnableAutoControl(devItem.Number, devItem.JointControlState);
            clkbt.IsEnabled = true;
            if (!rs)
            {
                var topLevel = (Window)TopLevel.GetTopLevel(this);
                await new MessageBox("warning", Properties.Resources.ResourceManager.GetString("ConfigurationFailed", I18NExt.Culture)).ShowDialog(topLevel);

            }
        }
        catch (Exception ex)
        {
            clkbt.IsEnabled = true;
            var topLevel = (Window)TopLevel.GetTopLevel(this);
            await new MessageBox("error", Properties.Resources.ResourceManager.GetString("ConfigurationException", I18NExt.Culture) + ex.Message).ShowDialog(topLevel);

        }
    }
}