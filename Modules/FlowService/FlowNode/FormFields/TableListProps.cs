using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FlowService.FlowNode.FormFields
{
    public class TableListProps : BaseProps
    {
        public string placeholder { get; set; }
        public FormField[] columns { get; set; }
        /// <summary>
        /// 展示边框
        /// </summary>
        public bool showBorder { get; set; }
        /// <summary>
        /// 最大行数
        /// </summary>
        public int maxSize { get; set; }
        /// <summary>
        /// 布局方式:true按表格,false按表单
        /// </summary>
        public bool rowLayout { get; set; }
        /// <summary>
        /// 索引字段
        /// </summary>
        public string[] IdxColName { get; set; }
        /// <summary>
        /// 是否展示合计
        /// </summary>
        public bool showSummary { get; set; }
        /// <summary>
        /// 关联合计字段
        /// </summary>
        public string deductid { get; set; }
        /// <summary>
        /// 合计字段
        /// </summary>
        public string[] summaryColumns { get; set; }
        /// <summary>
        /// 合计单位
        /// </summary>
        public string summaryUnit { get; set; }

        public override bool Check(Dictionary<string, string> commitOperates, FormField field, Dictionary<string, object> model, Dictionary<string, string> inputParams, out string msg)
        {
            if (!base.Check(commitOperates, field, model, inputParams, out msg))
            {
                return false;
            }
            foreach (var item in columns)
            {
                if (!item.Check(commitOperates, model, inputParams, out msg))
                {
                    return false;
                }
            }


            msg = string.Empty;
            return true;
        }
        public override void FieldRelated(FormField field, Dictionary<string, object> model)
        {
            //填充关联合计字段
            if (showSummary && !string.IsNullOrEmpty(deductid))
            {
                if (!model.ContainsKey(deductid) && model.ContainsKey(field.id))
                {
                    double totalnum = 0;
                    var tlista = model[field.id] as IEnumerable;
                    foreach (var a in tlista)
                    {
                        var aobj = a as IDictionary<string, object>;
                        double rowval = 1;
                        foreach (string b in this.summaryColumns)
                        {
                            rowval = rowval * Convert.ToDouble(aobj[b]);
                        }
                        totalnum = totalnum + rowval;
                    }
                    model[deductid] = totalnum;
                }
            }

        }
    }
}
