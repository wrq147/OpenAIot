using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJointUI.Models
{
    public class RuleItem_V: ReactiveObject
    {
        private bool _isUsing;
        /// <summary>
        /// 是否使用
        /// </summary>
        public bool IsUsing
        {
            get
            {
                return _isUsing;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _isUsing, value);
            }
        }
        public long? Id { get; set; }
        /// <summary>
        /// 规则名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public RuleItem_V()
        {
        }
    }
}
