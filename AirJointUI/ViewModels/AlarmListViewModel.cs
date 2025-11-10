using AirJointUI.Api;
using AirJointUI.Models;
using AirJointUI.Utils;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AirJointUI.ViewModels
{
    public class AlarmListViewModel : ViewModelBase
    {
        /// <summary>
        /// 顶部信息
        /// </summary>
        public TopInfo Top
        {
            get { return TopInfo.Instance; }
        }
        public ObservableCollection<AlarmItem_V> AlarmItems { get; set; }


        private AlarmItem_V _activeItem;
        public AlarmItem_V ActiveItem
        {
            get => _activeItem;
            set => this.RaiseAndSetIfChanged(ref _activeItem, value);
        }
        private bool _showAlarmItem = false;
        public bool ShowAlarmItem
        {
            get => _showAlarmItem;
            set => this.RaiseAndSetIfChanged(ref _showAlarmItem, value);
        }

        public AlarmListViewModel()
        {
            AlarmItems = new ObservableCollection<AlarmItem_V>();
        }
        public async Task<string> Init()
        {
            try
            {
                var items = await AlarmApi.GetAlarmList(1);
                for (int i = 0; i < items.Count; i++)
                {
                    AlarmItem item = items[i];
                    AlarmItem_V itemv;
                    if (AlarmItems.Count > i)
                    {
                        itemv = AlarmItems[i];
                    }
                    else
                    {
                        itemv = new AlarmItem_V();
                        AlarmItems.Add(itemv);
                    }

                    itemv.Id = item.Id.ToString();
                    itemv.DeviceName = I18NExt.Translate(item.DeviceName, item.DeviceName);
                    itemv.Description = I18NExt.Translate(item.Description, item.Description);
                    if (item.Status == 1)
                    {
                        itemv.ClearOn = DateTime.Parse(item.ClearOn).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    itemv.Status = item.Status.Value;
                    itemv.CreateOn = DateTime.Parse(item.CreateOn).ToString("yyyy-MM-dd HH:mm:ss");
                    itemv.Name = I18NExt.Translate(item.Name, item.Name);
                }

                if (AlarmItems.Count > 0)
                {
                    this.ChangeActive(0);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void ChangeActive(int idx)
        {
            this.ShowAlarmItem = true;
            this.ActiveItem = this.AlarmItems[idx];
            foreach (var item in AlarmItems)
            {
                item.AlForeground = "#333";
            }
            this.ActiveItem.AlForeground = "#2371FF";
        }
        public async Task<bool> ReadAlarm()
        {
            if (this.ActiveItem == null)
            {
                return false;
            }
            var rs = await AlarmApi.ReadAlarm(this.ActiveItem.Id);
            return rs.code == 0;
        }
        public async Task<bool> ReadAllAlarm()
        {
            await AlarmApi.ReadAllAlarm();
            return true;
        }
    }
}
