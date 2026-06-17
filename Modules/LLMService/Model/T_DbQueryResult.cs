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
        /// <summary>
        /// 是否还有下一页
        /// </summary>
        public bool HasNext => Page * PageSize < TotalCount;
        /// <summary>
        /// 是否还有上一页
        /// </summary>
        public bool HasPrev => Page > 1;
        public string ToReadableText()
        {
            if (!this.Success)
            {
                return $"【数据库查询失败】{this.ErrorMsg}\n执行SQL：{this.Sql}";
            }

            StringBuilder sb = new StringBuilder();
            if (this.DataRows.Count == 0)
            {
                sb.AppendLine("当前分页无匹配数据");
                return sb.ToString();
            }

            // 提取表头
            var columns = this.DataRows[0].Keys.ToList();

            // Markdown 表头行
            sb.AppendLine("|" + string.Join("|", columns) + "|");
            // Markdown 分隔行
            sb.AppendLine("|" + string.Join("|", Enumerable.Repeat("---", columns.Count)) + "|");

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
                sb.AppendLine("|" + string.Join("|", cells) + "|");
            }

            sb.AppendLine("===== 数据库查询结果分页数据 =====");
            sb.AppendLine($"分页信息：第{this.Page}页 / 共{this.TotalPage}页，每页{this.PageSize}条，总数据{this.TotalCount}条");
            sb.AppendLine($"翻页提示：{(this.HasNext ? "存在下一页，你可回复「下一页」继续查看" : "无下一页")} {(this.HasPrev ? "存在上一页，可回复「上一页」" : "")}");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
