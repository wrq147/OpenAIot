using AirJointUI.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System.Threading.Tasks;

namespace AirJointUI.Views
{
    public partial class SetView : UserControl
    {
        public SetView()
        {
            InitializeComponent();
        }

        private void HandleBackClick(object sender, RoutedEventArgs e)
        {
            //跳转回主窗口
            MyVMLocator.Instance.Window.ContentViewModel = MyVMLocator.Instance.IndexViewModel;
        }
        private void HandleSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tabBtn = (TabControl)sender;
            tabBtn.IsEnabled = false;
            TabItem selTab = (TabItem)e.AddedItems[0];
            MyVMLocator.Instance.SetViewModel.IsShowConent = false;
            MyVMLocator.Instance.SetViewModel.ChangeTab(selTab.Name);
            if (selTab.Name == "tab1")
            {
                Task.Run(async () =>
                {
                    await MyVMLocator.Instance.SetViewModel.SetNet.Init();
                    await Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        MyVMLocator.Instance.SetViewModel.IsShowConent = true;
                    });
                });
            }
            if (selTab.Name == "tab2")
            {
                Task.Run(async () =>
                {
                    await MyVMLocator.Instance.SetViewModel.SetProto.Init();
                    await Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        MyVMLocator.Instance.SetViewModel.IsShowConent = true;
                    });
                });
            }
            if (selTab.Name == "tab3")
            {
                Task.Run(async () =>
                {
                    await MyVMLocator.Instance.SetViewModel.SetRule.Init();
                    await Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        MyVMLocator.Instance.SetViewModel.IsShowConent = true;
                    });
                });
            }
            if (selTab.Name == "tab4")
            {
                Task.Run(async () =>
                {
                    await MyVMLocator.Instance.SetViewModel.SetSerial.Init();
                    await Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        MyVMLocator.Instance.SetViewModel.IsShowConent = true;
                    });
                });
            }
            tabBtn.IsEnabled = true;
        }
    }
}
