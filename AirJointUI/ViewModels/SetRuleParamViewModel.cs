using AirJointUI.Api;
using AirJointUI.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections;
using AirJointUI.Utils;
using System.Threading;
using AirJointUI.Component;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia;
using Avalonia.Threading;
using System.Linq;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Splat;
namespace AirJointUI.ViewModels
{
    /// <summary>
    /// 规则参数模型
    /// </summary>
    public class SetRuleParamViewModel : ViewModelBase
    {
        private long _id;
        public long Id { get { return _id; } }
        private string _name;
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        /// <summary>
        /// 顶部信息
        /// </summary>
        public TopInfo Top
        {
            get { return TopInfo.Instance; }
        }
        private List<RuleParamItem> _paramItems;
        public List<RuleParamItem> ParamItems
        {
            get
            {
                return _paramItems;
            }
        }
        private List<RuleParamItem> _allItems;
        public SetRuleParamViewModel(RuleItem_V item)
        {
            _paramItems = new List<RuleParamItem>();
            _allItems = new List<RuleParamItem>();
            _id = item.Id.Value;
            _name = item.Name;

        }
        private RuleItem _item;
        private int _lockInit = 0;
        public async Task Init()
        {
            if (Interlocked.Exchange(ref _lockInit, 1) == 0)
            {
                _item = await RuleApi.RuleInfo(_id);
                if (!string.IsNullOrEmpty(_item.HttpParams))
                {
                    _allItems = JsonSerializer.Deserialize(_item.HttpParams, GenericJsonContext.Default.ListRuleParamItem);
                    ChangePage(1);
                }
                Interlocked.Exchange(ref _lockInit, 0);
            }
        }
        public async Task<ApiResult<int>> SaveRuleParams()
        {
            var rs = await RuleApi.SaveRuleParams(_id, _allItems);
            if (_item.Status == "0")
            {
                await RuleApi.EnableRule(_id, false);
                await RuleApi.ResetRule(_id);
                await RuleApi.EnableRule(_id, true);
            }
            return rs;
        }



        private string _pageInfo;
        public string PageInfo
        {
            get => _pageInfo;
            set => this.RaiseAndSetIfChanged(ref _pageInfo, value);
        }
        private int _curPage;

        public void OnPrePage(UserControl control)
        {
            if (_curPage > 1)
            {
                ChangePage(_curPage - 1);
                this.RefreshWrap(control);
            }
        }
        public void OnNextPage(UserControl control)
        {
            int pageSize = 14;
            if (MyVMLocator.Instance.IsPortraitDisplay)
            {
                pageSize = 18;
            }
            var tmplist = _allItems.Where(x => !x.readOnly).ToList();
            int totalPage = (int)Math.Ceiling((double)tmplist.Count / pageSize);
            if (_curPage < totalPage)
            {
                ChangePage(_curPage + 1);
                this.RefreshWrap(control);
            }
        }

        private void ChangePage(int page)
        {
            if (_allItems == null)
            {
                return;
            }
            var tmplist = _allItems.Where(x => !x.readOnly).ToList();
            _curPage = page;

            int pageSize = 14;
            if (MyVMLocator.Instance.IsPortraitDisplay)
            {
                pageSize = 18;
            }
            // 计算起始索引
            int startIndex = (page - 1) * pageSize;
            // 计算结束索引
            int endIndex = Math.Min(startIndex + pageSize - 1, tmplist.Count - 1);
            int totalPage = (int)Math.Ceiling((double)tmplist.Count / pageSize);
            _paramItems.Clear();
            List<RuleItem_V> tmplst = new List<RuleItem_V>();
            for (int i = startIndex; i <= endIndex; i++)
            {
                _paramItems.Add(tmplist[i]);
            }
            this.PageInfo = string.Format(I18NExt.Translate("PageTJ"), _curPage, totalPage);
        }
        private byte _isTab = 0;
        public void RefreshWrap(UserControl control)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                //初始化动态控件
                var panel = control.FindControl<Panel>("ParamPanel");
                panel.Children.Clear();
                foreach (var item in _paramItems)
                {
                    StackPanel sp = new StackPanel();
                    sp.Orientation = Avalonia.Layout.Orientation.Horizontal;
                    TextBlock tb = new TextBlock();
                    tb.Width = 290;
                    tb.Text = I18NExt.Translate(item.name, item.name);
                    sp.Children.Add(tb);

                    Image img = new Image();
                    img.Width = 25;
                    img.Margin = new Thickness(5, 0, 0, 0);
                    img.Source = new Bitmap(AssetLoader.Open(new Uri($"avares://AirJointUI/Assets/Images/ts.png")));

                    img.PointerPressed += async (object sender, PointerPressedEventArgs e) =>
                    {
                        var topLevel = (Window)TopLevel.GetTopLevel(control);
                        await new InfoBox("ts", I18NExt.Translate(item.remark ?? string.Empty, item.remark ?? string.Empty)).ShowDialog(topLevel);
                    };
                    sp.Children.Add(img);

                    if (item.type == "int" || item.type == "float")
                    {
                        TextBox box = new TextBox();
                        if (item.defval != null)
                        {
                            if (item.defval is JsonElement ele)
                            {
                                box.Text = (ele.ValueKind == JsonValueKind.String ? ele.GetString() : ele.GetRawText());
                            }
                            else
                            {
                                box.Text = item.defval.ToString();
                            }
                        }
                        box.ContextRequested += (sender, e) =>
                        {
                            e.Handled = true;
                        };
                        box.GotFocus += async (object sender, GotFocusEventArgs e) =>
                        {
                            if (Interlocked.CompareExchange(ref _isTab, 1, 0) == 0)
                            {
                                box.Focusable = false;
                                var topLevel = (Window)TopLevel.GetTopLevel(control);
                                var result = await new NumberKeyboard(box.Text).ShowDialog<string>(topLevel);
                                if (result != null)
                                {
                                    if (item.type == "int")
                                    {
                                        int dotidx = result.IndexOf(".");
                                        if (dotidx != -1)
                                        {
                                            result = result.Substring(0, dotidx);
                                        }
                                        box.Text = result;
                                        item.defval = Convert.ToInt32(result);
                                    }
                                    else
                                    {
                                        box.Text = result;
                                        item.defval = Convert.ToDouble(result);
                                    }
                                }

                                box.Focusable = true;
                                control.FindControl<TextBox>("FocusHolder").Focus();
                                _isTab = 0;
                            }

                        };
                        sp.Children.Add(box);
                    }
                    else if (item.type == "boolean")
                    {
                        SwitchButton swbt = new SwitchButton();
                        swbt.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
                        swbt.Margin = Thickness.Parse("20 0 25 0");
                        swbt.ContentTrue = Properties.Resources.ResourceManager.GetString("On", I18NExt.Culture);
                        swbt.ContentFalse = Properties.Resources.ResourceManager.GetString("Off", I18NExt.Culture);
                        if (item.defval is JsonElement ele)
                        {
                            swbt.IsChecked = ((JsonElement)item.defval).GetBoolean();
                        }
                        else
                        {
                            swbt.IsChecked = Convert.ToBoolean(item.defval);
                        }
                        swbt.IsCheckedChanged += (object sender, RoutedEventArgs e) =>
                        {
                            item.defval = swbt.IsChecked;
                        };
                        sp.Children.Add(swbt);
                    }
                    else if (item.type == "enum")
                    {
                        if (item.multi)
                        {
                            var items = new List<string>();
                            foreach (var kvp in item.elements)
                            {
                                items.Add(kvp.Key);
                            }
                            ListBox listBox = new ListBox
                            {
                                SelectionMode = SelectionMode.Multiple,
                            };
                            foreach (var it in items)
                            {
                                listBox.Items.Add(I18NExt.Translate(it, it));
                            }
                            if (item.defval != null)
                            {
                                string defstr = string.Empty;
                                if (item.defval is JsonElement ele)
                                {
                                    defstr = (ele.ValueKind == JsonValueKind.String ? ele.GetString() : ele.GetRawText());
                                }
                                else
                                {
                                    defstr = item.defval.ToString();
                                }
                                string[] defarr = defstr.Split(',', StringSplitOptions.RemoveEmptyEntries);

                                if (defarr.Length > 0)
                                {
                                    foreach(var tmpitem in defarr)
                                    {
                                        listBox.SelectedItems.Add(tmpitem);
                                    }
                                }
                            }
                            listBox.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
                            {
                                if (listBox.SelectedItems.Count == 0)
                                {
                                    item.defval = string.Empty;
                                }
                                else
                                {
                                    var selectedItems = listBox.SelectedItems;
                                    var selectedText = string.Join(",", selectedItems);
                                    item.defval = selectedText;
                                }

                            };
                        }
                        else
                        {
                            ComboBox listBox = new ComboBox();
                            var items = new List<string>();
                            foreach (var kvp in item.elements)
                            {
                                items.Add(kvp.Key);
                            }
                            foreach (var it in items)
                            {
                                listBox.Items.Add(I18NExt.Translate(it, it));
                            }
                            if (item.defval != null)
                            {
                                string defstr = string.Empty;
                                if (item.defval is JsonElement ele)
                                {
                                    defstr = (ele.ValueKind == JsonValueKind.String ? ele.GetString() : ele.GetRawText());
                                }
                                else
                                {
                                    defstr = item.defval.ToString();
                                }
                                int tmpidx = items.IndexOf(defstr);
                                if (tmpidx > -1)
                                {
                                    listBox.SelectedIndex = tmpidx;
                                }
                            }
                            listBox.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
                            {
                                if (e.AddedItems.Count > 0)
                                {
                                    int selidx = listBox.Items.IndexOf(e.AddedItems[0]);
                                    item.defval = items[selidx];
                                }
                            };
                            sp.Children.Add(listBox);
                        }
                    }
                    else
                    {
                        continue;
                    }
                    panel.Children.Add(sp);
                }
            });

        }

        private void Img_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
