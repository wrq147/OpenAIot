using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJointUI.Models
{
    public class ParamObject : ReactiveObject
    {

        private ObservableCollection<ParamItem> _paramitems;
        public ObservableCollection<ParamItem> ParamItems
        {
            get => _paramitems;
            set => this.RaiseAndSetIfChanged(ref _paramitems, value);
        }
    }


    public class ParamItem : ReactiveObject
    {
        public int ItemIndex { get; set; }
        private bool _showInIdx;
        public bool ShowInIdx
        {
            get => _showInIdx;
            set => this.RaiseAndSetIfChanged(ref _showInIdx, value);
        }

        private string _paramName;
        public string ParamName
        {
            get => _paramName;
            set => this.RaiseAndSetIfChanged(ref _paramName, value);
        }
        private string _paramCode;
        public string ParamCode
        {
            get => _paramCode;
            set => this.RaiseAndSetIfChanged(ref _paramCode, value);
        }

        private string _unit;
        public string Unit
        {
            get => _unit;
            set => this.RaiseAndSetIfChanged(ref _unit, value);
        }

        private string _val;
        public string Val
        {
            get => _val;
            set => this.RaiseAndSetIfChanged(ref _val, value);
        }
    }
}
