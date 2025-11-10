using AirJointUI.Utils;
using Avalonia.Markup.Xaml.MarkupExtensions;
using ReactiveUI;
using System;

namespace AirJointUI.Models
{
    public class AlarmItem_V : ReactiveObject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DeviceName { get; set; }
        public string Description { get; set; }
        private int _status;
        /// <summary>
        /// 读状态
        /// </summary>
        public int Status
        {
            get => _status;
            set
            {
                if (value == 0)
                {
                    this.StatusStr = I18NExt.Translate("待处理", "待处理");
                }
                else
                {
                    this.StatusStr = I18NExt.Translate("已处理", "已处理") + $"【{this.ClearOn}】";
                }
                this.RaiseAndSetIfChanged(ref _status, value);
            }
        }
        private string _statusStr;
        public string StatusStr
        {
            get => _statusStr;
            set => this.RaiseAndSetIfChanged(ref _statusStr, value);
        }
        public string ClearOn { get; set; }
        public string CreateOn { get; set; }
        private string _alForeground;
        public string AlForeground
        {
            get => _alForeground;
            set => this.RaiseAndSetIfChanged(ref _alForeground, value);
        }
    }
}
