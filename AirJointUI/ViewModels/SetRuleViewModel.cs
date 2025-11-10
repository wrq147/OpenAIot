using AirJointUI.Api;
using AirJointUI.Models;
using AirJointUI.Utils;
using Avalonia.Animation;
using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace AirJointUI.ViewModels
{
    /// <summary>
    /// 规则设置模型
    /// </summary>
    public class SetRuleViewModel : ViewModelBase
    {
        private ObservableCollection<RuleItem_V> _ruleList;
        public ObservableCollection<RuleItem_V> RuleList
        {
            get => _ruleList;
            set => this.RaiseAndSetIfChanged(ref _ruleList, value);
        }
        private string _pageInfo;
        public string PageInfo
        {
            get => _pageInfo;
            set => this.RaiseAndSetIfChanged(ref _pageInfo, value);
        }
        private List<RuleItem> _list;
        private int _curPage;
        public SetRuleViewModel()
        {
            RuleList = new ObservableCollection<RuleItem_V>();
        }
        private int _lockInit = 0;
        public async Task Init()
        {
            if (Interlocked.Exchange(ref _lockInit, 1) == 0)
            {
                _list = await RuleApi.GetRuleList();
                ChangePage(1);
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
            int pageSize = 6;
            if (MyVMLocator.Instance.IsPortraitDisplay)
            {
                pageSize = 8;
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

            int pageSize = 6;
            if (MyVMLocator.Instance.IsPortraitDisplay)
            {
                pageSize = 8;
            }
            // 计算起始索引
            int startIndex = (page - 1) * pageSize;
            // 计算结束索引
            int endIndex = Math.Min(startIndex + pageSize - 1, _list.Count - 1);
            int totalPage = (int)Math.Ceiling((double)_list.Count / pageSize);
            List<RuleItem_V> tmplst = new List<RuleItem_V>();
            for (int i = startIndex; i <= endIndex; i++)
            {
                var rule = _list[i];
                var item = new RuleItem_V();
                item.Id = rule.Id;
                item.IsUsing = rule.Status == "0";
                item.Name = I18NExt.Translate(rule.Name, rule.Name);
                item.Remark = I18NExt.Translate(rule.Remark, rule.Remark);
                tmplst.Add(item);
            }
            this.RuleList = new ObservableCollection<RuleItem_V>(tmplst);
            this.PageInfo = string.Format(I18NExt.Translate("PageTJ"), _curPage, totalPage);
        }
        public async Task<bool> EnableRule(long id, bool isEnable)
        {
            var rs = await RuleApi.EnableRule(id, isEnable);
            return rs.code == 0;
        }
    }
}
