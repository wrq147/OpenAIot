using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DataAc
{
    /// <summary>
    /// 可数据变动表信息
    /// </summary>
    public class DA_Table
    {
        /// <summary>
        /// 表中文名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 表代码名称
        /// </summary>
        public string code { get; set; }

        public List<DA_Field> fields { get; set; }
        public bool IsThisTable(ActionChangeData data)
        {
            if (data.TargetForm == code || data.TargetName == name || data.TargetForm == name )
            {
                data.TargetForm = code;
                data.TargetName = name;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
