using AirJointUI.Models;
using ReactiveUI;
using System;

namespace AirJointUI.ViewModels
{
    /// <summary>
    /// 设置的框架模型
    /// </summary>
    public class SetViewModel : ViewModelBase
    {
        private ViewModelBase _tabModel;
        public ViewModelBase TabViewModel
        {
            get => _tabModel;
            set => this.RaiseAndSetIfChanged(ref _tabModel, value);
        }
        private SetNetViewModel _setNetViewModel;
        public SetNetViewModel SetNet
        {
            get { return _setNetViewModel; }
        }
        private SetProtoViewModel _setProtoViewModel;
        public SetProtoViewModel SetProto
        {
            get { return _setProtoViewModel; }
        }
        private SetRuleViewModel _setRuleViewModel;
        public SetRuleViewModel SetRule
        {
            get { return _setRuleViewModel; }
        }
        private SetSerialViewModel _setSerialViewModel;
        public SetSerialViewModel SetSerial
        {
            get { return _setSerialViewModel; }
        }
        private int _initTabIndex;
        public int InitTabIndex
        {
            get => _initTabIndex;
            set => this.RaiseAndSetIfChanged(ref _initTabIndex, value);
        }
        private bool _isShowContent;
        public bool IsShowConent
        {
            get => _isShowContent;
            set => this.RaiseAndSetIfChanged(ref _isShowContent, value);
        }
        public SetViewModel()
        {
            _setNetViewModel = new SetNetViewModel();
            _setProtoViewModel = new SetProtoViewModel();
            _setRuleViewModel = new SetRuleViewModel();
            _setSerialViewModel = new SetSerialViewModel();
            _tabModel = _setNetViewModel;
            _initTabIndex = 0;
            _isShowContent = false;
        }
        /// <summary>
        /// 顶部信息
        /// </summary>
        public TopInfo Top
        {
            get { return TopInfo.Instance; }
        }

        public void ChangeTab(string name)
        {
            switch (name)
            {
                case "tab1":
                    TabViewModel = _setNetViewModel;
                    this.InitTabIndex = 0;
                    break;
                case "tab2":
                    TabViewModel = _setProtoViewModel;
                    this.InitTabIndex = 1;
                    break;
                case "tab3":
                    TabViewModel = _setRuleViewModel;
                    this.InitTabIndex = 2;
                    break;
                case "tab4":
                    TabViewModel = _setSerialViewModel;
                    this.InitTabIndex = 3;
                    break;
            }
        }
    }
}
