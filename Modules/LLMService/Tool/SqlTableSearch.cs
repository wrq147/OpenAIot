using AuthService;
using LLMService.Business;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace LLMService.Tool
{
    public class SqlTableSearch
    {
        public static async Task<AITool> CreateSqlTableSearchTool(ITAServiceProvider provider)
        {
            var dbBLL = provider.GetService<DbSearchBLL>();
            await dbBLL.InitDbStruct();

            return AIFunctionFactory.Create(
               async ([Description("用户需要统计/查询的业务需求，自然语言描述，如：查询本月订单明细")] string userQuestion,
        [Description("分页页码，默认第1页")] int page = 1,
        [Description("每页展示条数，默认20，最大不允许超过100")] int pageSize = 20) =>
                {
                    try
                    {
                        var context = FunctionInvokingChatClient.CurrentContext;
                        var user = context.Options.AdditionalProperties["UserInfo"] as Data_ServerTokenInfo;
                        if (user == null || user.OrgId <= 0)
                        {
                            return "只有企业级用户才能执行数据库查询工具";
                        }
                        var userstr = context.Options.AdditionalProperties["UserStr"] as string;
                        var tmpbll = provider.GetService<DbSearchBLL>();
                        string gensql = await tmpbll.GenerateSqlBySchemaAsync(userQuestion, userstr, page, pageSize);
                        var res = await tmpbll.ExecutePageSelectSqlAsync(gensql, page, pageSize);
                        return res.ToReadableText();
                    }
                    catch (Exception ex)
                    {
                        return "工具调用异常：" + ex.Message;
                    }

                },
                name: "自然语言查询",
                description: "数据库自然语言查询工具，根据用户业务描述自动查表、生成SQL并返回统计数据。\r\n " + dbBLL.TableInfo()
            );
        }
    }
}
