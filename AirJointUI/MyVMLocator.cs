using AirJointUI.ViewModels;
using Avalonia.Controls.Templates;
using Avalonia.Controls;
using AirJointUI.Views;
using AirJointUI.Utils;

namespace AirJointUI
{
    public class MyVMLocator : IDataTemplate
    {
        private static MyVMLocator _instance;
        public static MyVMLocator Instance
        {
            get
            {
                return _instance;
            }
        }
        public bool SupportsRecycling => false;
        private Control _preSetView;
        private Control _preSetNetView;
        private Control _preSetProtoView;
        private Control _preSetRuleView;
        private Control _preSetSerialView;
        private Control _preAlarmListView;

        private Control _preIndexView;


        public void ResetSetView()
        {
            if (_isPortraitDisplay)
            {
                _preSetView = new VPortSetView();
            }
            else
            {
                _preSetView = new SetView();
            }
        }
        public Control Build(object data)
        {
            var name = data.GetType().FullName.Replace("ViewModel", "View");
            switch (name)
            {
                case "AirJointUI.Views.LoadView":
                    {
                        if (_isPortraitDisplay)
                        {
                            return new VPortLoadView();
                        }
                        else
                        {
                            return new LoadView();
                        }
                    }
                case "AirJointUI.Views.SetNetView":
                    {
                        if (_preSetNetView != null)
                        {
                            return _preSetNetView;
                        }
                        if (_isPortraitDisplay)
                        {
                            _preSetNetView = new VPortSetNetView();
                        }
                        else
                        {
                            _preSetNetView = new SetNetView();
                        }
                        return _preSetNetView;
                    }
                case "AirJointUI.Views.SetProtoView":
                    {
                        if (_preSetProtoView != null)
                        {
                            return _preSetProtoView;
                        }
                        if (_isPortraitDisplay)
                        {
                            _preSetProtoView = new VPortSetProtoView();
                        }
                        else
                        {
                            _preSetProtoView = new SetProtoView();
                        }
                        return _preSetProtoView;
                    }
                case "AirJointUI.Views.SetRuleView":
                    {
                        if (_preSetRuleView != null)
                        {
                            return _preSetRuleView;
                        }
                        if (_isPortraitDisplay)
                        {
                            _preSetRuleView = new VPortSetRuleView();
                        }
                        else
                        {
                            _preSetRuleView = new SetRuleView();
                        }
                        return _preSetRuleView;
                    }
                case "AirJointUI.Views.SetSerialView":
                    {
                        if (_preSetSerialView != null)
                        {
                            return _preSetSerialView;
                        }
                        if (_isPortraitDisplay)
                        {
                            _preSetSerialView = new VPortSetSerialView();
                        }
                        else
                        {
                            _preSetSerialView = new SetSerialView();
                        }
                        return _preSetSerialView;
                    }
                case "AirJointUI.Views.SetView":
                    {
                        if (_preSetView != null)
                        {
                            return _preSetView;
                        }
                        if (_isPortraitDisplay)
                        {
                            _preSetView = new VPortSetView();
                        }
                        else
                        {
                            _preSetView = new SetView();
                        }
                        return _preSetView;
                    }
                case "AirJointUI.Views.SetRuleParamView":
                    {
                        if (_isPortraitDisplay)
                        {
                            return new VPortSetRuleParamView();
                        }
                        else
                        {
                            return new SetRuleParamView();
                        }
                    }
                case "AirJointUI.Views.IndexView":
                    {
                        if (_preIndexView != null)
                        {
                            return _preIndexView;
                        }
                        if (_isPortraitDisplay)
                        {
                            _preIndexView = new VPortIndexView();
                        }
                        else
                        {
                            _preIndexView = new IndexView();
                        }
                        return _preIndexView;
                    }
                case "AirJointUI.Views.AlarmListView":
                    {
                        if (_preAlarmListView != null)
                        {
                            return _preAlarmListView;
                        }
                        if (_isPortraitDisplay)
                        {
                            _preAlarmListView = new VPortAlarmListView();
                        }
                        else
                        {
                            _preAlarmListView = new AlarmListView();
                        }
                        return _preAlarmListView;
                    }
                default:
                    return new TextBlock { Text = "Not Found: " + name };
            }
            //var type = Type.GetType(name);

            //if (type != null)
            //{
            //    return (Control)Activator.CreateInstance(type);
            //}
            //else
            //{
            //    return new TextBlock { Text = "Not Found: " + name };
            //}
        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
        private MainWindowViewModel _window;
        public MainWindowViewModel Window
        {
            get => _window;
        }
        private LoadViewModel _loadViewModel;
        /// <summary>
        /// 启动页模型
        /// </summary>
        public LoadViewModel LoadViewModel
        {
            get => _loadViewModel;
        }

        private SetViewModel _setViewModel;
        /// <summary>
        /// 设置页模型
        /// </summary>
        public SetViewModel SetViewModel
        {
            get => _setViewModel;
        }
        private IndexViewModel _indexViewModel;
        public IndexViewModel IndexViewModel
        {
            get => _indexViewModel;
        }
        private AlarmListViewModel _alarmListViewModel;
        public AlarmListViewModel AlarmListViewModel
        {
            get => _alarmListViewModel;
        }

        private bool _isPortraitDisplay;
        public bool IsPortraitDisplay
        {
            get => _isPortraitDisplay;
            set => _isPortraitDisplay = value;
        }
        public string WifiName { get; set; } = "wlan0";

        public MyVMLocator()
        {
            _instance = this;
            _setViewModel = new SetViewModel();
            _indexViewModel = new IndexViewModel();
            _alarmListViewModel = new AlarmListViewModel();
            _window = new MainWindowViewModel();
            _loadViewModel = new LoadViewModel();
        }

    }
}
