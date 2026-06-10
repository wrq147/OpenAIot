using AuthService.Controller;
using Common;
using Common.Share;
using LLMService.Business;
using LLMService.Model;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
namespace LLMService.Controller
{
    /// <summary>
    /// AI会话接口
    /// </summary>
    public class Chat : AbstractLoginedController
    {
        /// <summary>
        /// 用户输入提问
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Message(string userInput)
        {
            await this.ServiceProvider.GetService<ChatBLL>().ChatAsync(GetUser(), userInput);
            return this.Success(string.Empty);
        }

        /// <summary>
        /// 最近会话记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<Out_ChatMessage>>> RecentHistory()
        {
            List<Out_ChatMessage> tlist = new List<Out_ChatMessage>();
            var shortMemorys = await this.ServiceProvider.GetService<ShortMemoryBLL>().GetShortMemoryList(GetUser().UserId.ToString());
            if (shortMemorys.Count > 0)
            {
                foreach (var chatItem in shortMemorys)
                {
                    long atTime = MyAccess.Core.TypeConvert.Time2Unix(DateTime.Parse(chatItem.Time));
                    tlist.Add(new Out_ChatMessage()
                    {
                        role = "user",
                        status = 0,
                        data = chatItem.User,
                        time = atTime,
                    });
                    tlist.Add(new Out_ChatMessage()
                    {
                        role = "assistant",
                        status = 0,
                        data = chatItem.Assistant,
                        time = atTime
                    });
                }
            }
            return this.Success(tlist);
        }
    }
}
