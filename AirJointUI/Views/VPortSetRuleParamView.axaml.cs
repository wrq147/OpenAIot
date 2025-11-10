using AirJointUI.Component;
using AirJointUI.Utils;
using AirJointUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace AirJointUI.Views
{
    public partial class VPortSetRuleParamView : UserControl
    {
        public VPortSetRuleParamView()
        {
          
            InitializeComponent();
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            ((SetRuleParamViewModel)this.DataContext).RefreshWrap(this);
        }
        // 上一页按钮点击处理
        public void HandlePreviousPage(object sender, RoutedEventArgs e)
        {
            var svm = this.DataContext as SetRuleParamViewModel;
            svm.OnPrePage(this);

        }

        // 下一页按钮点击处理
        public void HandleNextPage(object sender, RoutedEventArgs e)
        {
            var svm = this.DataContext as SetRuleParamViewModel;
            svm.OnNextPage(this);
        }
        private void HandleBackClick(object sender, RoutedEventArgs e)
        {
            //跳转回规则配置窗口
            MyVMLocator.Instance.Window.ContentViewModel = MyVMLocator.Instance.SetViewModel;
            MyVMLocator.Instance.SetViewModel.InitTabIndex = 2;
        }
        private void HandleSave(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            btn.IsEnabled = false;
            var svm = this.DataContext as SetRuleParamViewModel;
            Task.Run(async () =>
            {
                var rs = await svm.SaveRuleParams();
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    if (rs.code == 0)
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
