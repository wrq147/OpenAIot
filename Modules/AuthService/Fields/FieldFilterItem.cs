using Minio.DataModel;
using MyAccess.DB.Builder;
using System;

namespace AuthService.Fields
{
    public class FieldFilterItem
    {
        /// <summary>
        /// 过滤字段
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// 比较符号：大于、小于、大于等于、小于等于、不等于、等于、包含、不包含、关联
        /// </summary>
        public string compare { get; set; }
        public void AppendFilter<A>(QueryOneBuilder<A> sql, string prefix = "")
        {
            if (val_num != null)
            {
                sql.Append($" and {prefix}{this.field}");
                //判断数字
                switch (this.compare)
                {
                    case "大于":
                        sql.Append(">");
                        break;
                    case "小于":
                        sql.Append("<");
                        break;
                    case "大于等于":
                        sql.Append(">=");
                        break;
                    case "小于等于":
                        sql.Append("<=");
                        break;
                    case "不等于":
                        sql.Append("<>");
                        break;
                    case "等于":
                        sql.Append("=");
                        break;
                }
                sql.AppendParam(this.val_num);
            }
            else if (this.val_arr != null && this.val_arr.Length > 0)
            {
                sql.Append($" and {prefix}{this.field}");
                //判断数组
                switch (this.compare)
                {
                    case "不等于":
                        sql.Append("<>");
                        break;
                    case "等于":
                        sql.Append("=");
                        break;
                    case "包含":
                        sql.Append(" in (");
                        break;
                    case "不包含":
                        sql.Append(" not in (");
                        break;
                }
                int i = 0;
                foreach (var itemval in this.val_arr)
                {
                    if (i == 0)
                    {
                        sql.AppendParam(itemval);
                    }
                    else
                    {
                        sql.Append(",").AppendParam(itemval);
                    }
                }
                if (this.compare == "包含" || this.compare == "不包含")
                {
                    sql.Append(")");
                }
            }
            else if (this.val != null)
            {
                sql.Append($" and {prefix}{this.field}");
                //判断字符串
                switch (this.compare)
                {
                    case "不等于":
                        sql.Append("<>").AppendParam(this.val);
                        break;
                    case "等于":
                        sql.Append("=").AppendParam(this.val);
                        break;
                    case "包含":
                        {
                            string tmpval = MyAccess.Core.StringTool.SqlLikeFilter(this.val);
                            sql.Append(" like ").AppendParam("%" + tmpval + "%");
                        }
                        break;
                    case "不包含":
                        {
                            string tmpval = MyAccess.Core.StringTool.SqlLikeFilter(this.val);
                            sql.Append(" not like ").AppendParam("%" + tmpval + "%");
                        }
                        break;
                    case "关联":
                        {
                            if (this.field.StartsWith("StrExt"))
                            {
                                int didx = this.val.IndexOf(',');
                                if (didx != -1)
                                {
                                    sql.Append(" like ").AppendParam(this.val.Substring(0, didx + 1) + "%");
                                }
                            }
                            else
                            {
                                int didx = this.val.IndexOf(',');
                                if (didx != -1)
                                {
                                    sql.Append("=").AppendParam(this.val.Substring(0, didx + 1));
                                }
                                else if (this.val != "")
                                {
                                    sql.Append("=").AppendParam(this.val);
                                }
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 比较的值（字符串）
        /// </summary>
        public string val { get; set; }
        /// <summary>
        /// 比较的值（数字）
        /// </summary>
        public double? val_num { get; set; }
        /// <summary>
        /// 比较的值（字符串数组）
        /// </summary>
        public string[] val_arr { get; set; }
    }
}
