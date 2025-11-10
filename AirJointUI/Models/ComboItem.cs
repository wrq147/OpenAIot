using ReactiveUI;
using System;

namespace AirJointUI.Models
{
    public class ComboItem : ReactiveObject
    {
        private string _val;
        /// <summary>
        /// 值
        /// </summary>
        public string Val
        {
            get
            {
                return _val;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _val, value);
            }
        }
        private string _name;
        /// <summary>
        /// 名
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _name, value);
            }
        }
    }
}
