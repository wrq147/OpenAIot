using AuthService;
using Common.Share;
using FlowService.FlowNode.Builder;
using FlowService.FlowNode.FormFields;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace FlowService.Model
{
    [TableName("mz_flow_template")]
    public class MZ_FlowTemplate : BaseEntity
    {
        public MZ_FlowTemplate()
        {
        }
        /// <summary>
        /// 主键
        /// </summary>
        [ID(false)]
        public long? Id { get; set; }
        /// <summary>
        /// 关联组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 所属分组
        /// </summary>
        public long? GroupId { get; set; }
        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 分组名
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [DataIgnore]
        public string GroupName { get; set; }
        /// <summary>
        /// 排序用，值越小越前面
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 状态（0正常 1暂停）
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        public string Background { get; set; }
        /// <summary>
        /// 对应表单类型
        /// </summary>
        public long? FormId { get; set; }
        /// <summary>
        /// 流程内容
        /// </summary>
        public string FlowJson { get; set; }
        /// <summary>
        /// 消息通知方式(json内容)
        /// </summary>
        public string notify { get; set; }
        /// <summary>
        /// 审批同意时是否需要签字
        /// </summary>
        public bool? sign { get; set; }
        /// <summary>
        /// 审批可提交次数，为0不限制
        /// </summary>
        public int? sublimit { get; set; }
        /// <summary>
        /// 是否禁止直接发起
        /// </summary>
        public bool? startlimit { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
        /// <summary>
        /// 对应的表单
        /// </summary>
        [DataIgnore]
        public MZ_Form Form { get; set; }
    }
    /// <summary>
    /// 消息通知实体
    /// </summary>
    public class FlowTemplateNotice
    {
        public string[] types { get; set; }
        public string title { get; set; }
        private string _parsedTitle = null;
        public string GetTitleOfParsed(BuilderContext context)
        {
            if (_parsedTitle != null)
            {
                return _parsedTitle;
            }
            string pattern = @"\$\{(\d+)\}";
            var matchs = Regex.Matches(this.title, pattern);
            _parsedTitle = this.title;
            foreach (Match match in matchs)
            {
                string fieldId = match.Groups[1].Value;
                List<FormField> targets = new List<FormField>();
                context.FindTo(x => x.id == fieldId, targets);
                if (targets.Count > 0)
                {
                    if (targets[0].name == "DevicPicker")
                    {
                        if (context.FormItems.TryGetValue(fieldId, out object outval))
                        {
                            List<string> tnames = new List<string>();
                            var selectedlist = outval as IEnumerable<object>;
                            if (selectedlist != null)
                            {
                                foreach (var selectItem in selectedlist)
                                {
                                    var seldict = selectItem as IDictionary<string, object>;
                                    var jk = seldict["name"];
                                    if (jk == null)
                                    {
                                        continue;
                                    }
                                    string devName = jk.ToString();
                                    tnames.Add(devName);
                                }
                            }
                            _parsedTitle = _parsedTitle.Replace("${" + fieldId + "}", string.Join(',', tnames));
                        }
                    }
                    else
                    {
                        if (context.FormItems.TryGetValue(fieldId, out object outval))
                        {
                            string tvalstr = outval as string;
                            _parsedTitle = _parsedTitle.Replace("${" + fieldId + "}", tvalstr);
                        }
                    }
                }
            }
            return _parsedTitle;
        }
    }
}
