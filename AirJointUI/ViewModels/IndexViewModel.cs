using AirJointUI.Api;
using AirJointUI.Models;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using DynamicData;
using AirJointUI.Utils;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Collections;
using System.Reactive.Linq;

namespace AirJointUI.ViewModels
{
    public class IndexViewModel : ViewModelBase
    {
        /// <summary>
        /// 顶部信息
        /// </summary>
        public TopInfo Top
        {
            get { return TopInfo.Instance; }
        }
        private string _deviceCC = "";
        public string DeviceCount
        {
            get => _deviceCC;
            set => this.RaiseAndSetIfChanged(ref _deviceCC, value);
        }
        private string _inputCC = "";
        public string InputCount
        {
            get => _inputCC;
            set => this.RaiseAndSetIfChanged(ref _inputCC, value);
        }

        private int _lanIdx;
        public int LanIdx
        {
            get => _lanIdx;
            set => this.RaiseAndSetIfChanged(ref _lanIdx, value);
        }
        public ObservableCollection<ComboItem> LangList { get; set; }
        private ObservableCollection<DeviceItem_IdxV> _devitems;
        public ObservableCollection<DeviceItem_IdxV> DeviceItems
        {
            get => _devitems;
            set => this.RaiseAndSetIfChanged(ref _devitems, value);
        }
        public ObservableCollection<InputItem_IdxV> InputItems { get; set; }

        private System.Threading.Timer _timer;
        private int inTimer = 0;

        private bool _isShowDevice;
        public bool IsShowDevice
        {
            get => _isShowDevice;
            set
            {
                if (isNeedPage)
                {
                    this.ShowPage = value;
                }

                this.RaiseAndSetIfChanged(ref _isShowDevice, value);

            }
        }
        private bool _isRestore;
        public bool ExistRestore
        {
            get
            {
                return _isRestore;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _isRestore, value);
            }
        }
        private bool _showPage = false;
        public bool ShowPage
        {
            get => _showPage;
            set => this.RaiseAndSetIfChanged(ref _showPage, value);
        }
        private string _pageInfo;
        public string PageInfo
        {
            get => _pageInfo;
            set => this.RaiseAndSetIfChanged(ref _pageInfo, value);
        }
        private int _curPage = 1;
        public void OnPrePage()
        {
            if (_curPage > 1)
            {
                ChangePage(_curPage - 1);
            }
        }
        private bool _enableGoWeb;
        public bool EnableGoWeb
        {
            get
            {
                return _enableGoWeb;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _enableGoWeb, value);
            }
        }

        public void OnNextPage()
        {
            int pageSize = 4;
            var tmpalllist = this._allDev.Where(x => x.EnableShow != false).ToList();
            int totalPage = (int)Math.Ceiling((double)tmpalllist.Count / pageSize);
            if (_curPage < totalPage)
            {
                ChangePage(_curPage + 1);
            }
        }
        private void ChangePage(int page)
        {
            if (_allDev == null)
            {
                return;
            }
            _curPage = page;
            var tmpalllist = this._allDev.Where(x => x.EnableShow != false).ToList();
            int pageSize = 4;
            // 计算起始索引
            int startIndex = (page - 1) * pageSize;
            // 计算结束索引
            int endIndex = Math.Min(startIndex + pageSize - 1, tmpalllist.Count - 1);
            int totalPage = (int)Math.Ceiling((double)tmpalllist.Count / pageSize);
            List<DeviceItem_IdxV> tmplst = new List<DeviceItem_IdxV>();
            for (int i = startIndex; i <= endIndex; i++)
            {
                tmplst.Add(tmpalllist[i]);
            }

            this.DeviceItems = new ObservableCollection<DeviceItem_IdxV>(tmplst);
            this.PageInfo = string.Format(I18NExt.Translate("PageTJ"), _curPage, totalPage);
        }
        private List<DeviceItem_IdxV> _allDev;
        public IndexViewModel()
        {
            _allDev = new List<DeviceItem_IdxV>();
            DeviceItems = new ObservableCollection<DeviceItem_IdxV>();
            InputItems = new ObservableCollection<InputItem_IdxV>();
            LangList = new ObservableCollection<ComboItem>();
            LangList.Add(new ComboItem() { Name = "简体中文", Val = "zh" });
            LangList.Add(new ComboItem() { Name = "English", Val = "en" });
            this.IsShowDevice = true;
            this.ExistRestore = File.Exists("savedev");
        }
        public void StartInit()
        {
            _timer = new System.Threading.Timer(new TimerCallback(Init), null, 1000, 2000);
        }

        private int _alarmCount;
        public int AlarmCount
        {
            get => _alarmCount;
            set
            {
                this.RaiseAndSetIfChanged(ref _alarmCount, value);
                if (_alarmCount > 0)
                {
                    EnableAlarm = true;
                }
                else
                {
                    EnableAlarm = false;
                }
            }
        }
        public bool _enableAlarm;
        public bool EnableAlarm
        {
            get => _enableAlarm;
            set => this.RaiseAndSetIfChanged(ref _enableAlarm, value);
        }
        private async void Init(object state)
        {
            try
            {
                if (Interlocked.Exchange(ref inTimer, 1) == 0)
                {
                    var allList = await DeviceApi.GetDeviceList();
                    await InitDevice(allList);
                    await InitInput(allList);
                    this.AlarmCount = await AlarmApi.GetAlarmCount();

                    Interlocked.Exchange(ref inTimer, 0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private List<DeviceItem> _tmpdevlist;
        private async Task InitDevice(List<DeviceItem> alllist)
        {
            if (_tmpdevlist == null)
            {
                _tmpdevlist = alllist.Where(x => x.Name.Contains("设备")).ToList();
                _tmpdevlist.Sort((a, b) => string.Compare(a.Name, b.Name));
            }

            bool hasChange = false;
            for (int i = 0; i < _tmpdevlist.Count; i++)
            {
                DeviceItem dev = _tmpdevlist[i];
                DeviceItem_IdxV viewdev = _allDev.Where(x => x.Id == dev.Id).FirstOrDefault();
                if (viewdev != null)
                {
                    string newtransname = I18NExt.Translate(dev.Name, dev.Name);
                    if (viewdev.Name != newtransname)
                    {
                        viewdev.Name = newtransname;
                    }
                }
                else
                {
                    hasChange = true;
                    viewdev = new DeviceItem_IdxV();
                    viewdev.Id = dev.Id;
                    viewdev.Name = I18NExt.Translate(dev.Name, dev.Name);
                    viewdev.Number = dev.DeviceNumber;
                    viewdev.EnableShow = true;
                    _allDev.Add(viewdev);
                }


                var tagrs = await DeviceApi.GetTagList(dev.DeviceNumber);
                if (tagrs.code == 0 && tagrs.data != null)
                {
                    foreach (var tagitem in tagrs.data)
                    {
                        try
                        {
                            if (tagitem.Value == null)
                            {
                                continue;
                            }
                            var je = (JsonElement)tagitem.Value;
                            switch (tagitem.Code)
                            {
                                case "RunTime":
                                    viewdev.RunTimes = (je.GetInt32() / 3600000).ToString();
                                    break;
                                case "StopTime":
                                    viewdev.StopTimes = (je.GetInt32() / 3600000).ToString();
                                    break;
                                case "EnableNet":
                                    viewdev.JointControlState = je.GetBoolean();
                                    break;
                                case "EnableShow":
                                    {
                                        var tmpbb = je.GetBoolean();
                                        if (viewdev.EnableShow != tmpbb)
                                        {
                                            hasChange = true;
                                        }
                                        viewdev.EnableShow = tmpbb;
                                    }
                                    break;
                                case "WaitStart":
                                    {
                                        viewdev.EnableStart = je.GetBoolean();
                                    }
                                    break;
                                case "WaitStop":
                                    {
                                        viewdev.EnableStop = je.GetBoolean();
                                    }
                                    break;
                            }
                        }
                        catch { }
                    }
                }

                if (!viewdev.EnableShow)
                {
                    continue;
                }

                if (viewdev.ParamItems == null)
                {
                    viewdev.ParamItems = new ObservableCollection<ParamItem>();
                }
                var proprs = await DeviceApi.GetPropList(dev.DeviceId);
                if (proprs.code == 0 && proprs.data == null)
                {
                    viewdev.IsOnline = false;
                }
                else
                {
                    viewdev.IsOnline = true;
                }

                if (proprs.code == 0 && proprs.data != null)
                {
                    if (proprs.data.Count > 0)
                    {
                        int realisx = 0;
                        for (int x = 0; x < proprs.data.Count; x++)
                        {
                            var propitem = proprs.data[x];
                            try
                            {
                                if (propitem.Value == null)
                                {
                                    continue;
                                }
                                string tmpval = string.Empty;
                                if (propitem.Value is JsonElement tje)
                                {
                                    tmpval = (tje.ValueKind == JsonValueKind.String ? tje.GetString() : tje.GetRawText()) ?? string.Empty;
                                }
                                else if (propitem.Value is string tjss)
                                {
                                    tmpval = tjss;
                                }

                                if (propitem.Code == "RunState")
                                {
                                    viewdev.RunText = I18NExt.Translate(tmpval, tmpval);
                                    viewdev.RunState = tmpval == "运行" ? 1 : 0;
                                    if (viewdev.RunState == 1)
                                    {
                                        viewdev.IsRuning = true;

                                    }
                                    else
                                    {
                                        viewdev.IsRuning = false;
                                    }
                                }
                                else
                                {
                                    var curitemparam = viewdev.ParamItems.Where(x => x.ParamCode == propitem.Code).FirstOrDefault();
                                    if (curitemparam == null)
                                    {
                                        curitemparam = new ParamItem();
                                        viewdev.ParamItems.Add(curitemparam);
                                    }
                                    curitemparam.ItemIndex = realisx;
                                    curitemparam.ShowInIdx = realisx < 8;
                                    curitemparam.ParamName = I18NExt.Translate(propitem.Name, propitem.Name);
                                    curitemparam.Unit = "(" + propitem.Unit + ")";
                                    curitemparam.Val = tmpval;
                                    curitemparam.ParamCode = propitem.Code;
                                    ++realisx;

                                }

                            }
                            catch { }
                        }
                        viewdev.ParamItems = new ObservableCollection<ParamItem>(
                              viewdev.ParamItems.OrderBy(p => p.ItemIndex)
                          );

                    }


                }

            }
            var tmpcc = this._allDev.Where(x => x.EnableShow != false).Count();
            if (hasChange)
            {
                if (tmpcc > 4)
                {
                    isNeedPage = true;
                    this.ShowPage = true;
                }
                else
                {
                    isNeedPage = false;
                    this.ShowPage = false;
                }
                ChangePage(_curPage);
            }

            DeviceCount = string.Format(I18NExt.Translate("DeviceTag"), tmpcc.ToString());
        }
        private bool isNeedPage = false;
        private bool isMulti = false;
        private List<DeviceItem> _tmpiptlist;
        private async Task InitInput(List<DeviceItem> alllist)
        {
            if (_tmpiptlist == null)
            {
                _tmpiptlist = alllist.Where(x => x.Name.Contains("传感器")).ToList();
            }

            for (int i = 0; i < _tmpiptlist.Count; i++)
            {
                DeviceItem iptdv = _tmpiptlist[i];
                InputItem_IdxV viewIpt = InputItems.Where(x => x.Id == iptdv.Id).FirstOrDefault();
                if (viewIpt != null)
                {
                    string newtransname = I18NExt.Translate(iptdv.Name, iptdv.Name);
                    if (viewIpt.Name != newtransname)
                    {
                        viewIpt.Name = newtransname;
                    }
                }
                else
                {
                    viewIpt = new InputItem_IdxV();
                    viewIpt.EnableShow = true;
                    viewIpt.Id = iptdv.Id;
                    viewIpt.Name = I18NExt.Translate(iptdv.Name, iptdv.Name);
                    InputItems.Add(viewIpt);
                }

                viewIpt.Remark = iptdv.Remark;


                var tagrs = await DeviceApi.GetTagList(iptdv.DeviceNumber);
                if (tagrs.code == 0 && tagrs.data != null)
                {
                    foreach (var tagitem in tagrs.data)
                    {
                        try
                        {
                            switch (tagitem.Code)
                            {
                                case "EnableShow":
                                    {
                                        var t = (JsonElement)tagitem.Value;
                                        viewIpt.EnableShow = t.GetBoolean();
                                        break;
                                    }

                            }
                        }
                        catch { }

                    }
                }

                if (!viewIpt.EnableShow)
                {
                    continue;
                }

                if (viewIpt.ParamItems == null)
                {
                    viewIpt.ParamItems = new ObservableCollection<ParamItem>();
                }
                var proprs = await DeviceApi.GetPropList(iptdv.DeviceId);
                if (proprs.code == 0 && proprs.data == null)
                {
                    viewIpt.IsOnline = false;
                }
                else
                {
                    viewIpt.IsOnline = true;
                }
                if (proprs.code == 0 && proprs.data != null)
                {
                    int realisx = 0;
                    isMulti = proprs.data.Count > 8;
                    for (int x = 0; x < proprs.data.Count; x++)
                    {
                        var propitem = proprs.data[x];
                        try
                        {
                            if (propitem.Value == null)
                            {
                                continue;
                            }
                            string tmpval = string.Empty;
                            if (propitem.Value is JsonElement tje)
                            {
                                tmpval = (tje.ValueKind == JsonValueKind.String ? tje.GetString() : tje.GetRawText()) ?? string.Empty;
                            }
                            else if (propitem.Value is string tjss)
                            {
                                tmpval = tjss;
                            }

                            var curitemparam = viewIpt.ParamItems.Where(x => x.ParamCode == propitem.Code).FirstOrDefault();
                            if (curitemparam == null)
                            {
                                curitemparam = new ParamItem();
                                viewIpt.ParamItems.Add(curitemparam);
                            }
                            curitemparam.ItemIndex = realisx;
                            curitemparam.ShowInIdx = realisx < 8;
                            curitemparam.ParamName = I18NExt.Translate(propitem.Name, propitem.Name);
                            curitemparam.Unit = "(" + propitem.Unit + ")";
                            curitemparam.Val = tmpval;
                            curitemparam.ParamCode = propitem.Code;
                            ++realisx;

                        }
                        catch { }

                    }
                    viewIpt.ParamItems = new ObservableCollection<ParamItem>(
                            viewIpt.ParamItems.OrderBy(p => p.ItemIndex)
                        );
                }

            }
            InputCount = string.Format(I18NExt.Translate("SensorTag"), this.InputItems.Where(x => x.EnableShow != false).Count().ToString());
        }

        public async Task<bool> EnableAutoControl(string number, bool enabled)
        {
            TagSave_In data = new TagSave_In();
            data.number = number;
            data.list = new List<TagSaveItem>();
            data.list.Add(new TagSaveItem()
            {
                Code = "EnableNet",
                Value = enabled
            });
            var rs = await DeviceApi.SaveTags(data);
            return rs.code == 0;
        }
        public async Task<bool> StartDev(string number)
        {
            FunExe_In exei = new FunExe_In();
            exei.Number = number;
            exei.FunctionId = isMulti ? "startMulti" : "starMachine";
            exei.Inputs = new Dictionary<string, object>();
            var rs = await DeviceApi.ExeFunc(exei);
            return rs.code == 0;
        }
        public async Task<bool> StopDev(string number)
        {
            FunExe_In exei = new FunExe_In();
            exei.Number = number;
            exei.FunctionId = isMulti ? "stopMulti" : "stopmachine";
            exei.Inputs = new Dictionary<string, object>();
            var rs = await DeviceApi.ExeFunc(exei);
            return rs.code == 0;
        }
    }
}
