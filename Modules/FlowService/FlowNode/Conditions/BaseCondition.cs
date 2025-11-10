using AuthService;
using FlowService.FlowNode.Builder;
using FlowService.Model;
using System;
using System.Collections.Generic;

namespace FlowService.FlowNode.Conditions
{
    public abstract class BaseCondition
    {
        /// <summary>
        /// 表单字段Id或RootNode的Id
        /// </summary>
        public string id { get; set; }
        public string title { get; set; }
        /// <summary>
        ///  User 人员选择、 Dept 部门选择、 Number 数字、Date 日期、 String 字符串
        /// </summary>
        public string valueType { get; set; }
        public abstract string ToExpressionString(BuilderContext context);
    }
}
