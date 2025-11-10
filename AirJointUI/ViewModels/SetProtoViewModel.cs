using AirJointUI.Api;
using AirJointUI.Models;
using AirJointUI.Utils;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AirJointUI.ViewModels
{
    /// <summary>
    /// 协议设置模型
    /// </summary>
    public class SetProtoViewModel : ViewModelBase
    {
        private ObservableCollection<DeviceItem_Proto> _deviceList;
        public ObservableCollection<DeviceItem_Proto> DeviceList
        {
            get => _deviceList;
            set => this.RaiseAndSetIfChanged(ref _deviceList, value);
        }
        private List<ComboItem> _tmpcomboList;
        private int _lockInit = 0;

        private string _pageInfo;
        public string PageInfo
        {
            get => _pageInfo;
            set => this.RaiseAndSetIfChanged(ref _pageInfo, value);
        }
        private List<DeviceItem_Proto> _list;
        private int _curPage;
        public SetProtoViewModel()
        {
            this.DeviceList = new ObservableCollection<DeviceItem_Proto>();
        }
        public async Task Init()
        {
            if (Interlocked.Exchange(ref _lockInit, 1) == 0)
            {
                _list = new List<DeviceItem_Proto>();
                var tdevlist = await DeviceApi.GetDeviceList();

                //初始化协议列表
                if (_tmpcomboList == null)
                {
                    _tmpcomboList = new List<ComboItem>();
                    var tnames = await DeviceApi.GetProductNames();
                    for (int i = 0; i < tnames.Count; i++)
                    {
                        var proname = tnames[i];
                        _tmpcomboList.Add(new ComboItem()
                        {
                            Name = I18NExt.Translate(proname.Name, proname.Name),
                            Val = proname.Id
                        });
                    }
                }


                for (int idx = 0; idx < tdevlist.Count; idx++)
                {
                    DeviceItem dev = tdevlist[idx];
                    DeviceItem_Proto dvpp = new DeviceItem_Proto();
                    dvpp.ProdList = new ObservableCollection<ComboItem>();
                    dvpp.ProdList.Add(_tmpcomboList);
                    dvpp.Id = dev.Id;
                    dvpp.ProductId = dev.ProductId;
                    dvpp.SelectIndex = _tmpcomboList.FindIndex((x) => x.Val == dvpp.ProductId);
                    dvpp.DeviceName = I18NExt.Translate(dev.Name, dev.Name);
                    dvpp.DeviceNumber = dev.DeviceNumber;

                    var tagrs = await DeviceApi.GetTagList(dev.DeviceNumber);
                    if (tagrs.code == 0 && tagrs.data != null)
                    {
                        foreach (var tagitem in tagrs.data)
                        {
                            try
                            {
                                var je = (JsonElement)tagitem.Value;
                                switch (tagitem.Code)
                                {
                                    case "EnableShow":
                                        dvpp.HasEnableShow = true;
                                        dvpp.EnableShow = je.GetBoolean();
                                        break;
                                }
                            }
                            catch { }
                        }
                    }

                    _list.Add(dvpp);
                }

                this.ChangePage(1);
                Interlocked.Exchange(ref _lockInit, 0);
            }
        }
        public void OnPrePage()
        {
            if (_curPage > 1)
            {
                ChangePage(_curPage - 1);
            }
        }
        public void OnNextPage()
        {
            int pageSize = 4;
            if (MyVMLocator.Instance.IsPortraitDisplay)
            {
                pageSize = 7;
            }
            int totalPage = (int)Math.Ceiling((double)_list.Count / pageSize);
            if (_curPage < totalPage)
            {
                ChangePage(_curPage + 1);
            }
        }
        private void ChangePage(int page)
        {
            if (_list == null)
            {
                return;
            }
            _curPage = page;

            int pageSize = 4;
            if (MyVMLocator.Instance.IsPortraitDisplay)
            {
                pageSize = 7;
            }
            // 计算起始索引
            int startIndex = (page - 1) * pageSize;
            // 计算结束索引
            int endIndex = Math.Min(startIndex + pageSize - 1, _list.Count - 1);
            int totalPage = (int)Math.Ceiling((double)_list.Count / pageSize);
            List<DeviceItem_Proto> tmplst = new List<DeviceItem_Proto>();
            for (int i = startIndex; i <= endIndex; i++)
            {
                tmplst.Add(_list[i]);
            }
            this.DeviceList = new ObservableCollection<DeviceItem_Proto>(tmplst);
            this.PageInfo = string.Format(I18NExt.Translate("PageTJ"), _curPage, totalPage);
        }
        public async Task<bool> Save()
        {
            foreach (var item in this._list)
            {
                DeviceProto_In data = new DeviceProto_In();
                data.DeviceNumber = item.DeviceNumber;
                var cb = item.ProdList[item.SelectIndex];
                data.ProductId = cb.Val;
                var rs = await DeviceApi.SaveDeviceProto(data);
                if (rs.code != 0)
                {
                    return false;
                }

                if (item.HasEnableShow == true)
                {
                    TagSave_In ndata = new TagSave_In();
                    ndata.number = item.DeviceNumber;
                    ndata.list = new List<TagSaveItem>();
                    ndata.list.Add(new TagSaveItem()
                    {
                        Code = "EnableShow",
                        Value = item.EnableShow
                    });
                    var nrs = await DeviceApi.SaveTags(ndata);
                    if (nrs.code != 0)
                    {
                        return false;
                    }
                }

            }
            return true;
        }
    }
}
