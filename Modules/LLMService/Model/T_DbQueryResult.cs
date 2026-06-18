using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMService.Model
{
    public class T_DbQueryResult
    {
        public T_DbQueryResult()
        {
            DataRows = new List<Dictionary<string, object>>();
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 失败错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 数据行列表（当前页）
        /// </summary>
        public List<Dictionary<string, object>> DataRows { get; set; }


        /// <summary>
        /// 当前页码，从1开始
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 符合条件总记录数
        /// </summary>
        public long TotalCount { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public long TotalPage { get; set; }
        public T_ToolResult ToReadableText()
        {
            if (!this.Success)
            {
                return new T_ToolResult()
                {
                    IsSuccess = false,
                    Output = "数据库查询失败，请重新尝试",
                    LogInfo = $"失败原因：查询语句 {this.Sql}，{this.ErrorMsg}"
                };
            }

            StringBuilder sb = new StringBuilder();
            if (this.DataRows.Count == 0)
            {
                return new T_ToolResult()
                {
                    IsSuccess = false,
                    Output = "当前无匹配数据",
                    LogInfo = $"查询语句 {this.Sql}"
                };
            }

            // 提取表头
            var columns = this.DataRows[0].Keys.ToList();

            // Markdown 表头行
            sb.AppendLine("| " + string.Join(" | ", columns) + " |");
            // Markdown 分隔行
            sb.AppendLine("| " + string.Join(" | ", Enumerable.Repeat(":----:", columns.Count)) + " |");

            // 逐行拼接 Markdown 数据行
            foreach (var row in this.DataRows)
            {
                List<string> cells = new List<string>();
                foreach (var col in columns)
                {
                    var val = row[col];
                    string cellText = val == null ? "" : val.ToString();
                    // 处理内容包含 | 竖线，避免表格错位
                    cellText = cellText.Replace("|", "\\|");
                    cells.Add(cellText);
                }
                sb.AppendLine("| " + string.Join(" | ", cells) + " |");
            }
            sb.AppendLine();
            sb.AppendLine("#### 查询结果的分页数据");
            sb.AppendLine($"分页信息：第{this.Page}页 / 共{this.TotalPage}页，每页{this.PageSize}条，总数据{this.TotalCount}条");
            sb.AppendLine();
            string tmpstr = sb.ToString();
            return new T_ToolResult()
            {
                IsSuccess = true,
                Output = tmpstr,
                LogInfo = $"查询语句 {this.Sql}"
            };
        }
    }
}
