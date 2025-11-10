using AirJointUI.Component;
using AirJointUI.Models;
using AirJointUI.Utils;
using AirJointUI.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using System;

namespace AirJointUI.Views
{
    public partial class SetRuleView : UserControl
    {
        public SetRuleView()
        {
            InitializeComponent();
        }
        // 上一页按钮点击处理
        public void HandlePreviousPage(object sender, RoutedEventArgs e)
        {
            var svm = this.DataContext as SetRuleViewModel;
            svm.OnPrePage();

        }

        // 下一页按钮点击处理
        public void HandleNextPage(object sender, RoutedEventArgs e)
        {
            var svm = this.DataContext as SetRuleViewModel;
            svm.OnNextPage();
        }
        private async void HandleClickRule(object sender, RoutedEventArgs e)
        {
            var clkbt = (Button)sender;
            var ruleItem = clkbt.DataContext as RuleItem_V;
            var ruleParamModel = new SetRuleParamViewModel(ruleItem);
            await ruleParamModel.Init();
            MyVMLocator.Instance.Window.ContentViewModel = ruleParamModel;
        }
        private async void HandleChangeStatus(object sender, RoutedEventArgs e)
        {
            var clkbt = (ToggleButton)sender;
            var ruleItem = clkbt.DataContext as RuleItem_V;
            var svm = this.DataContext as SetRuleViewModel;
            var rs = await svm.EnableRule(ruleItem.Id.Value, ruleItem.IsUsing);
            if (!rs)
            {
                var topLevel = (Window)TopLevel.GetTopLevel(this);
                await new MessageBox("warning", Properties.Resources.ResourceManager.GetString("SaveFailed", I18NExt.Culture)).ShowDialog(topLevel);
                await svm.Init();
            }
        }
    }
}
