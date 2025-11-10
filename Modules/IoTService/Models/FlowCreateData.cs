using AuthService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.Models
{
    public class FlowCreateData
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
        /// 发起人
        /// </summary>
        public long UserId { get; set; }
    }
    public class FlowItem
    {
        public string id { get; set; }
        public string title { get; set; }
        /// <summary>
        /// 表单项类型
        /// </summary>
        public string eltype { get; set; }
        public int way { get; set; }
        public string val { get; set; }
        public async Task<string> GetRealValue(ITAServiceProvider provider, MZ_IotWarning warn, MZ_IotDevice device, IDictionary<string, MZ_Org> tmporg)
        {
            if (way == 1)
            {
                switch (val)
                {
                    case "告警名称":
                        return warn.Name;
                    case "告警描述":
                        return warn.Description;
                    case "告警级别":
                        if (warn.Level == 0)
                        {
                            return "普通";
                        }
                        else if (warn.Level == 1)
                        {
                            return "告警";
                        }
                        else if (warn.Level == 2)
                        {
                            return "紧急";
                        }
                        return string.Empty;
                    case "设备来源":
                        {
                            MZ_Org sourceOrg;
                            if (!tmporg.TryGetValue("source", out sourceOrg))
                            {
                                sourceOrg = await provider.GetService<OrgDAL>().SelectById(device.OrgId.Value);
                                tmporg.Add("source", sourceOrg);
                            }
                            if (sourceOrg != null)
                            {
                                return sourceOrg.OrgName;
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }
                    case "设备拥有者":
                        {
                            MZ_Org ownerOrg;
                            if (!tmporg.TryGetValue("owner", out ownerOrg))
                            {
                                ownerOrg = await provider.GetService<OrgDAL>().SelectById(device.OwnerOrgId.Value);
                                tmporg.Add("owner", ownerOrg);
                            }
                            if (ownerOrg != null)
                            {
                                return ownerOrg.OrgName;
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }
                  
                    case "拥有者地址":
                        {
                            MZ_Org ownerOrg;
                            if (!tmporg.TryGetValue("owner", out ownerOrg))
                            {
                                ownerOrg = await provider.GetService<OrgDAL>().SelectById(device.OwnerOrgId.Value);
                                tmporg.Add("owner", ownerOrg);
                            }
                            if (ownerOrg != null)
                            {
                                return ownerOrg.AddressDetail;
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }
                  
                    case "使用者地址":
                        {
                            MZ_Org useOrg;
                            if (!tmporg.TryGetValue("use", out useOrg))
                            {
                                useOrg = await provider.GetService<OrgDAL>().SelectById(device.UseOrgId.Value);
                                tmporg.Add("use", useOrg);
                            }
                            if (useOrg != null)
                            {
                                return useOrg.AddressDetail;
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }
                    

                }
                return string.Empty;
            }
            return val;
        }
    }
}
