using AirJointUI.Models;
using AirJointUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Linq;
using AirJointUI.Component;
using Splat;
using AirJointUI.Utils;

namespace AirJointUI.Views;

public partial class AlarmListView : UserControl
{
    public AlarmListView()
    {
        InitializeComponent();
    }
    public override void BeginInit()
    {
        base.BeginInit();
        InitModel();
    }
    private async void InitModel()
    {
        await MyVMLocator.Instance.AlarmListViewModel.Init();
    }
    private void HandleBackClick(object sender, RoutedEventArgs e)
    {
        //跳转回主窗口
        MyVMLocator.Instance.Window.ContentViewModel = MyVMLocator.Instance.IndexViewModel;
    }

    private void HandleItemPressed(object sender, PointerPressedEventArgs e)
    {
        Border stPanel = (Border)sender;
        var alarmItem = stPanel.DataContext as AlarmItem_V;
        var svm = this.DataContext as AlarmListViewModel;

        int idx = 0;
        for (int i = 0; i < svm.AlarmItems.Count; i++)
        {
            if (svm.AlarmItems[i].Id == alarmItem.Id)
            {
                idx = i;
                break;
            }
        }
        svm.ChangeActive(idx);
    }
    private async void HandleReadClick(object sender, RoutedEventArgs e)
    {
        var svm = this.DataContext as AlarmListViewModel;
        var rs = await svm.ReadAlarm();
        if (!rs)
        {
            var topLevel = (Window)TopLevel.GetTopLevel(this);
            await new MessageBox("warning", Properties.Resources.ResourceManager.GetString("ExecutionFailed", I18NExt.Culture)).ShowDialog(topLevel);
        }
        else
        {
            await svm.Init();
            var topLevel = (Window)TopLevel.GetTopLevel(this);
            await new MessageBox("success", Properties.Resources.ResourceManager.GetString("ExecutionSuccessful", I18NExt.Culture)).ShowDialog(topLevel);
        }
    }

    private async void HandleClearAll(object sender, RoutedEventArgs e)
    {
        var svm = this.DataContext as AlarmListViewModel;
        var rs = await svm.ReadAllAlarm();
        if (!rs)
        {
            var topLevel = (Window)TopLevel.GetTopLevel(this);
            await new MessageBox("warning", Properties.Resources.ResourceManager.GetString("ExecutionFailed", I18NExt.Culture)).ShowDialog(topLevel);
        }
        else
        {
            await svm.Init();
            var topLevel = (Window)TopLevel.GetTopLevel(this);
            await new MessageBox("success", Properties.Resources.ResourceManager.GetString("ExecutionSuccessful", I18NExt.Culture)).ShowDialog(topLevel);

        }

    }
}