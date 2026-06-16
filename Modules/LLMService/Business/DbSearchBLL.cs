using LLMService.Model;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private IAiClientRegistry _registry;
        private ITAServiceProvider _provider;
        private Dictionary<string, List<T_TableField>> _tbDict;
        private Dictionary<string, string> _tableCommentDict;
        private IOptions<LLMOption> _option;
        public DbSearchBLL(IOptions<LLMOption> option, IAiClientRegistry registry, ITAServiceProvider provider, ILoggerFactory logFactory)
        {
            _option = option;
            _registry = registry;
            _provider = provider;
            _log = logFactory.CreateLogger<DbSearchBLL>();
        }
        private async Task GetDbStruct(string conn)
        {
            _tbDict = new Dictionary<string, List<T_TableField>>();
            _tableCommentDict = new Dictionary<string, string>();

            MySqlConnection con = new MySqlConnection(conn);
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


        /// <summary>
        /// 根据表结构+用户问题，让LLM生成可执行SELECT SQL
        /// </summary>
        /// <param name="question"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<string> GenerateSqlBySchemaAsync(string question, int page, int pageSize)
        {
            if (_tableCommentDict == null || _tbDict == null)
            {
                await GetDbStruct(_option.Value.DBConnectionString);
            }

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
            sysStrBuilder.Append("你是SQL生成器，仅输出纯净SQL，无多余文字、markdown、解释，严格遵守以下规则：");
            sysStrBuilder.AppendLine("1. 仅输出完整SELECT语句，禁止任何解释、注释、额外文字；");
            sysStrBuilder.AppendLine("2. 所有表名、字段名严格按照下面给出的库结构使用；");
            sysStrBuilder.AppendLine($"3. 查询末尾强制添加分页 LIMIT " + offset + "," + pageSize + "；");
            sysStrBuilder.AppendLine("4. 按需使用WHERE、GROUP BY、SUM、COUNT、JOIN等聚合关联语法；");
            sysStrBuilder.AppendLine("5. 禁止DELETE/UPDATE/INSERT/ALTER/DROP等修改语句；");

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
    }

}
