using FlowService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AfterService.Model
{
    public class In_AddPlaneTask
    {
        /// <summary>
        /// 任务数据
        /// </summary>
        public MZ_PlaneTask task { get; set; }
        /// <summary>
        /// 初始化表单
        /// </summary>
        public Dictionary<string,object> model { get; set; }
        /// <summary>
        /// 自选人
        /// </summary>
        public Dictionary<string, List<Out_UserItem>> assign { get; set; }
    }
}
