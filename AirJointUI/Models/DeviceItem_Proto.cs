using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace AirJointUI.Models
{
    public class DeviceItem_Proto : ReactiveObject
    {
        public string Id { get; set; }
        private string _deviceName;
        public string DeviceName
        {
            get => _deviceName;
            set => this.RaiseAndSetIfChanged(ref _deviceName, value);
        }
        public string DeviceNumber { get; set; }
        private string _productId;
        public string ProductId
        {
            get => _productId;
            set => this.RaiseAndSetIfChanged(ref _productId, value);
        }
        private int _selectIndex;
        public int SelectIndex
        {
            get => _selectIndex;
            set => this.RaiseAndSetIfChanged(ref _selectIndex, value);
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
        private bool _hasEnableShow = false;
        public bool HasEnableShow
        {
            get => _hasEnableShow;
            set => this.RaiseAndSetIfChanged(ref _hasEnableShow, value);
        }
        public ObservableCollection<ComboItem> ProdList { get; set; }
    }
}
