using MyAccess.DB;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DataAc
{
    public class ActionChangeData
    {
        /// <summary>
        /// 执行的表单名
        /// </summary>
        public string TargetName { get; set; }
        /// <summary>
        /// 执行的表单
        /// </summary>
        public string TargetForm { get; set; }
        /// <summary>
        /// Update、Delete
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 当前流程执行者
        /// </summary>
        public long UpdateId { get; set; }
        /// <summary>
        /// 源组织Id
        /// </summary>
        public long OrgId { get; set; }
        /// <summary>
        /// 流程Id
        /// </summary>
        public long FlowId { get; set; }
        /// <summary>
        /// 过滤条件
        /// </summary>
        public List<ActionCondition> conditions { get; set; }
        /// <summary>
        /// 执行插入或更新的字段
        /// </summary>
        public List<ActionInfo> actions { get; set; }

        /// <summary>
        /// 转换成Sql并执行
        /// </summary>
        /// <param name="db"></param>
        /// <param name="newCondition"></param>
        /// <param name="newAction"></param>
        /// <returns></returns>
        public async Task<int> DoAsync(DbHelp db, List<ActionCondition> newCondition, List<ActionInfo> newAction)
        {



            var sqlBuilder = new SqlBuilder(db);
            switch (this.Action)
            {
                case "Insert":
                    {

                    }
                    break;
                case "Update":
                    {
                        if (newAction.Count == 0)
                        {
                            throw new Exception("流程的数据执行节点配置错误");
                        }
                        if (conditions.Count > 0)
                        {
                            sqlBuilder.Append("UPDATE ").Append(TargetForm).Append(" SET ");
                            for (int i = 0; i < newAction.Count; i++)
                            {
                                var act = newAction[i];
                                if (i > 0)
                                {
                                    if (string.IsNullOrEmpty(act.FieldValue))
                                    {
                                        sqlBuilder.Append(",").Append(act.TargetField).Append("=").AppendParam(act.FinalValue);
                                    }
                                    else
                                    {
                                        sqlBuilder.Append(",").Append(act.TargetField).Append("=").Append(act.FieldValue);
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(act.FieldValue))
                                    {
                                        sqlBuilder.Append(act.TargetField).Append("=").AppendParam(act.FinalValue);
                                    }
                                    else
                                    {
                                        sqlBuilder.Append(act.TargetField).Append("=").Append(act.FieldValue);
                                    }
                                }
                            }
                            sqlBuilder.Append(" where ");
                            for (int j = 0; j < newCondition.Count; j++)
                            {
                                var condition = newCondition[j];
                                if (j > 0)
                                {
                                    if (string.IsNullOrEmpty(condition.FieldValue))
                                    {
                                        sqlBuilder.Append(" and ").Append(condition.TargetField).Append(condition.Compare).AppendParam(condition.FinalValue);
                                    }
                                    else
                                    {
                                        sqlBuilder.Append(" and ").Append(condition.TargetField).Append(condition.Compare).Append(condition.FieldValue);
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(condition.FieldValue))
                                    {
                                        sqlBuilder.Append(" ").Append(condition.TargetField).Append(condition.Compare).AppendParam(condition.FinalValue);
                                    }
                                    else
                                    {
                                        sqlBuilder.Append(" ").Append(condition.TargetField).Append(condition.Compare).Append(condition.FieldValue);
                                    }
                                }
                            }

                            return (await sqlBuilder.DoAsync<DoExecSql>()).RowCount;
                        }

                    }
                    break;
                case "Delete":
                    break;
            }
            return 0;
        }
    }
    public class ActionCondition
    {
        /// <summary>
        /// 过滤目标字段
        /// </summary>
        public string TargetField { get; set; }
        /// <summary>
        /// 比较值（常量）
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 最终比较
        /// </summary>
        public string Compare { get; set; }
        /// <summary>
        /// 最终值
        /// </summary>
        public object FinalValue { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public string FieldValue { get; set; }
    }
    public class ActionInfo
    {
        /// <summary>
        /// 目标字段
        /// </summary>
        public string TargetField { get; set; }
        /// <summary>
        /// 对应值（常量）
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 最终值
        /// </summary>
        public object FinalValue { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public string FieldValue { get; set; }
    }
}
