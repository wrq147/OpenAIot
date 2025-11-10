using Common.Share;
using System;
using System.Collections.Generic;

namespace FlowService.Model
{
    public class In_FlowRecord : BaseQueryParam
    {
        /// <summary>
        /// 完成时间段开始
        /// </summary>
        public DateTime? StartFinish { get; set; }
        /// <summary>
        /// 完成时间段结束
        /// </summary>
        public DateTime? EndFinish { get; set; }
        /// <summary>
        /// 过滤状态
        /// </summary>
        public FlowStatus? Status { get; set; }
        /// <summary>
        /// 过滤部门
        /// </summary>
        public long[] Depts { get; set; }
        /// <summary>
        /// 过滤创建人
        /// </summary>
        public long[] Users { get; set; }
        /// <summary>
        /// 过滤审批编号
        /// </summary>
        public long? Id { get; set; } 
        /// <summary>
        /// 模板Id必填项
        /// </summary>
        public long TemplateId { get; set; }
        /// <summary>
        /// 过滤字段
        /// </summary>
        public LimitFieldItem[] Items { get; set; }
    }

    public class LimitFieldItem
    {
        /// <summary>
        /// 要过滤的字段Id
        /// </summary>
        public string FieldId { get; set; }
        /// <summary>
        /// 比较字符串
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 设备Id数组，人员Id数组
        /// </summary>
        public string[] ValueArray { get; set; }
        /// <summary>
        /// 大于等于最小值
        /// </summary>
        public double? Min { get; set; }
        /// <summary>
        /// 小于等于最大值
        /// </summary>
        public double? Max { get; set; }
    }
}
