using AuthService;
using FlowService.FlowNode;
using FlowService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Model
{
    public class EnterFlowCreate
    {
        /// <summary>
        /// 流程模板ID
        /// </summary>
        public long templateId { get; set; }
        /// <summary>
        /// 提交的表单数据
        /// </summary>
        public Dictionary<string, object> model { get; set; }
        /// <summary>
        /// 自选人
        /// </summary>
        public Dictionary<string, List<Out_UserItem>> assign { get; set; }
        /// <summary>
        /// 发起人
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 要编辑的工作流
        /// </summary>
        public long flowId { get; set; }
    }

    public class EnterFlowItem
    {
        public string id { get; set; }
        public string title { get; set; }
        /// <summary>
        /// 表单项类型
        /// </summary>
        public string eltype { get; set; }
        /// <summary>
        /// 0为自定义、1为系统值
        /// </summary>
        public int way { get; set; }
        public object val { get; set; }
        public object GetRealValue(MZ_EnterStock stock)
        {
            if (way == 1)
            {
                string sysval = (string)val;
                switch (sysval)
                {
                    case "入库方式":
                        switch (stock.EnterMethod)
                        {
                            case 0:
                                return "出库";
                            case 1:
                                return "退货";
                            case 2:
                                return "调拨";
                            case 3:
                                return "手动";
                        }
                        return "";
                }
                return string.Empty;
            }
            else
            {
                return val;
            }
        }
    }
}
