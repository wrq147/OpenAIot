using LLMService.Model;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAccess.DB;
using MySql.Data.MySqlClient;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Cache;
using TemplateAction.Core;

namespace LLMService.Business
{
    public class DbSearchBLL
    {
        private ILogger<DbSearchBLL> _log;
        private AiClientRegistry _registry;
        private ITAServiceProvider _provider;
        private Dictionary<string, List<T_TableField>> _tbDict;
        private Dictionary<string, string> _tableCommentDict;
        private IOptions<LLMOption> _option;
        public DbSearchBLL(IOptions<LLMOption> option, AiClientRegistry registry, ITAServiceProvider provider, ILoggerFactory logFactory)
        {
            _option = option;
            _registry = registry;
            _provider = provider;
            _log = logFactory.CreateLogger<DbSearchBLL>();
        }

        public string TableInfo()
        {
            StringBuilder promptSb = new();
            promptSb.AppendLine("可查询表：");
            foreach (var table in _tableCommentDict)
            {
                promptSb.AppendLine($"【表】{table.Key} 说明：{table.Value}");
            }
            return promptSb.ToString();
        }
        public async Task InitDbStruct()
        {
            if (_tableCommentDict != null && _tbDict != null)
            {
                return;
            }
            _tbDict = new Dictionary<string, List<T_TableField>>();
            _tableCommentDict = new Dictionary<string, string>();

            MySqlConnection con = new MySqlConnection(_option.Value.DBConnectionString);
            await con.OpenAsync();
            string exesql = $"SELECT COLUMN_NAME,DATA_TYPE,TABLE_NAME,COLUMN_COMMENT FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '{con.Database}'";
            DataSet ds = new DataSet();
            MySqlCommand cmd = new MySqlCommand(exesql, con);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(ds);
            await con.CloseAsync();


            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string tbName = dr["TABLE_NAME"].ToString();
                    string colName = dr["COLUMN_NAME"].ToString();
                    string dataType = dr["DATA_TYPE"].ToString();
                    string colComment = dr["COLUMN_COMMENT"].ToString();
                    colComment = colComment.Replace("Description:", string.Empty);
                    T_TableField tmpfield = new T_TableField();
                    tmpfield.DataType = dataType;
                    tmpfield.TableName = tbName;
                    tmpfield.ColumnName = colName;
                    tmpfield.ColumnComment = colComment;

                    if (_tbDict.ContainsKey(tbName))
                    {
                        _tbDict[tbName].Add(tmpfield);
                    }
                    else
                    {
                        List<T_TableField> tmplist = new List<T_TableField>();
                        tmplist.Add(tmpfield);
                        _tbDict.Add(tbName, tmplist);
                    }
                }
            }

            DataSet tableDs = new DataSet();
            await con.OpenAsync();
            string tableSql = @"
        SELECT TABLE_NAME, TABLE_COMMENT 
        FROM INFORMATION_SCHEMA.TABLES 
        WHERE TABLE_SCHEMA = @dbName AND TABLE_TYPE = 'BASE TABLE'";
            using MySqlCommand tableCmd = new MySqlCommand(tableSql, con);
            tableCmd.Parameters.AddWithValue("@dbName", con.Database);
            MySqlDataAdapter tableAdapter = new MySqlDataAdapter(tableCmd);
            await tableAdapter.FillAsync(tableDs);
            await con.CloseAsync();

            if (tableDs.Tables.Count > 0)
            {
                foreach (DataRow dr in tableDs.Tables[0].Rows)
                {
                    string tbName = dr["TABLE_NAME"].ToString();
                    string tbComment = dr["TABLE_COMMENT"].ToString();
                    _tableCommentDict[tbName] = tbComment;
                }
            }
        }

        public string GetTableDes(List<string> tb)
        {
            StringBuilder promptSb = new();
            promptSb.AppendLine("查询的相关表结构信息：");
            foreach (var table in _tableCommentDict)
            {
                promptSb.AppendLine($"【表】{table.Key} 说明：{table.Value}");
            }
            return promptSb.ToString();
        }

        /// <summary>
        /// 根据表结构+用户问题，让LLM生成可执行SELECT SQL
        /// </summary>
        /// <param name="question"></param>
        /// <param name="userinfo"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<string> GenerateSqlBySchemaAsync(string question, string userinfo, int page, int pageSize)
        {
            StringBuilder promptSb = new();
            promptSb.AppendLine("表结构信息：");
            // 序列化表结构给LLM阅读
            foreach (var table in _tbDict)
            {
                if (_tableCommentDict.TryGetValue(table.Key, out string tbcomment))
                {
                    promptSb.AppendLine($"【表】{table.Key} 说明：{tbcomment}");
                    foreach (var col in table.Value)
                    {
                        promptSb.AppendLine($"  {col.ColumnName}({col.DataType})：{col.ColumnComment}");
                    }
                    promptSb.AppendLine();
                }
            }
            long offset = (page - 1) * pageSize;
            promptSb.AppendLine("用户查询需求：" + question);
            promptSb.AppendLine("直接输出唯一一条可执行SELECT SQL：");
            var chatClient = _registry.GetDefaultChat();
            var sysStrBuilder = new StringBuilder();
            sysStrBuilder.AppendLine("你是SQL生成器，仅输出纯净SQL，无多余文字、markdown、解释，严格遵守以下规则：");
            sysStrBuilder.AppendLine(userinfo);
            sysStrBuilder.AppendLine("1. 仅输出完整SELECT语句，禁止任何解释、注释、额外文字；");
            sysStrBuilder.AppendLine("2. 需严格按照用户的身份信息过滤用户的查询数据，不能超过用户所属企业的查看范围；");
            sysStrBuilder.AppendLine($"3. 查询末尾强制添加分页 LIMIT " + offset + "," + pageSize + "；");
            sysStrBuilder.AppendLine("4. 禁止DELETE/UPDATE/INSERT/ALTER/DROP等修改语句；");

            var response = await chatClient.GetResponseAsync(new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System,sysStrBuilder.ToString() ),
                new ChatMessage(ChatRole.User, promptSb.ToString())
            }, new ChatOptions
            {
                ToolMode = ChatToolMode.None // 关闭工具调用，纯文本输出SQL
            });
            var sql = response.Text.Replace("```sql", "").Replace("```", "").Trim();
            return sql;
        }

        public async Task<T_DbQueryResult> ExecutePageSelectSqlAsync(string pageSql, int page, int pageSize)
        {
            T_DbQueryResult res = new()
            {
                Sql = pageSql,
                Page = page,
                PageSize = pageSize
            };
            string tmpSql = pageSql.Trim();
            if (!AiSqlSecurityValidator.Validate(tmpSql, _tableCommentDict, out List<string> tbs, out string erromsg))
            {
                res.Success = false;
                res.ErrorMsg = erromsg;
                return res;
            }


            try
            {
                using MySqlConnection con = new MySqlConnection(_option.Value.DBConnectionString);
                await con.OpenAsync();
                // 1. 截取不带LIMIT的SQL，查询总条数
                string tmpLitSql = tmpSql.Substring(0, tmpSql.ToLower().LastIndexOf("limit"));
                string countSql = $"SELECT COUNT(1) FROM ({tmpLitSql}) t";
                long totalCount;
                using (MySqlCommand countCmd = new MySqlCommand(countSql, con))
                {
                    object countObj = await countCmd.ExecuteScalarAsync();
                    totalCount = Convert.ToInt64(countObj);
                }
                res.TotalCount = totalCount;
                res.TotalPage = totalCount == 0 ? 0 : (totalCount + pageSize - 1) / pageSize;
                await con.CloseAsync();

                // 2. 查询当前页数据
                await con.OpenAsync();
                MySqlDataAdapter adapter = new MySqlDataAdapter(pageSql, con);
                DataTable dt = new DataTable();
                await adapter.FillAsync(dt);
                await con.CloseAsync();

                foreach (DataRow row in dt.Rows)
                {
                    Dictionary<string, object> rowDict = new();
                    foreach (DataColumn col in dt.Columns)
                    {
                        rowDict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                    }
                    res.DataRows.Add(rowDict);
                }
                res.Success = true;
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.ErrorMsg = $"分页查询异常：{ex.Message}";
            }
            return res;
        }


    }

}
