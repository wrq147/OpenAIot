using AirJointUI.Component;
using AirJointUI.Utils;
using AirJointUI.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System.Threading.Tasks;
namespace AirJointUI.Views
{
    public partial class VPortSetProtoView : UserControl
    {
        public VPortSetProtoView()
        {
            InitializeComponent();
        }
        // 上一页按钮点击处理
        public void HandlePreviousPage(object sender, RoutedEventArgs e)
        {
            var svm = this.DataContext as SetProtoViewModel;
            svm.OnPrePage();

        }

        // 下一页按钮点击处理
        public void HandleNextPage(object sender, RoutedEventArgs e)
        {
            var svm = this.DataContext as SetProtoViewModel;
            svm.OnNextPage();
        }

        private void HandleSave(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            btn.IsEnabled = false;
            var svm = this.DataContext as SetProtoViewModel;
            Task.Run(async () =>
            {
                var rs = await svm.Save();
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
                        await new MessageBox("warning", Properties.Resources.ResourceManager.GetString("SaveFailed", I18NExt.Culture)).ShowDialog(topLevel);
                    }
                    btn.IsEnabled = true;
                });
            });
        }
    }
}
