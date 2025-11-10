using ReactiveUI;
using System.Collections.ObjectModel;


namespace AirJointUI.Models
{
    public class InputItem_IdxV : ParamObject
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        private bool _isOnline;
        public bool IsOnline
        {
            get => _isOnline;
            set => this.RaiseAndSetIfChanged(ref _isOnline, value);
        }


        private string _remark;

        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark
        {
            get => _remark;
            set => this.RaiseAndSetIfChanged(ref _remark, value);
        }
        private bool _enableShow;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool EnableShow
        {
            get => _enableShow;
            set => this.RaiseAndSetIfChanged(ref _enableShow, value);
        }
    }
}
