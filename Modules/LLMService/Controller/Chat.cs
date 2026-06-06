using AuthService.Controller;
using Common.Share;
using LLMService.Business;
using System;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Route;
using TemplateAction.Core;
using Common;
namespace LLMService.Controller
{
    public class Chat : AbstractLoginedController
    {
        /// <summary>
        /// 用户输入提问
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> Message(string userInput)
        {
            await this.ServiceProvider.GetService<ChatBLL>().ChatAsync(GetUser(), userInput);
            return this.Success(string.Empty);
        }
    }
}
