using AuthService.Controller;
using Common;
using Common.Share;
using LLMService.Business;
using LLMService.DAL;
using LLMService.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Route;
using TemplateAction.Core;
namespace LLMService.Controller
{
    public class KbColumn : AbstractLoginedController
    {
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_KbColumn>>> ListByKbId(string kbId)
        {
            var result = await ServiceProvider.GetService<KbColumnBLL>().GetByKbId(kbId);
            return this.Success(result);
        }

        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_KbColumn>> Info(string id)
        {
            var result = await ServiceProvider.GetService<KbColumnBLL>().GetById(id, GetUser());
            return this.Success(result);
        }


        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_KbColumn>>> GetByParentId(string parentId)
        {
            var result = await ServiceProvider.GetService<KbColumnBLL>().GetByParentId(parentId, GetUser());
            return this.Success(result);
        }

        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_KbColumn entity)
        {
            var id = await ServiceProvider.GetService<KbColumnBLL>().Add(entity, GetUser());
            return this.Success(id);
        }

        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Update(MZ_KbColumn entity)
        {
            int rs = await ServiceProvider.GetService<KbColumnBLL>().Update(entity, GetUser());
            return this.Success(rs);
        }

        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Delete(string id)
        {
            int rs = await ServiceProvider.GetService<KbColumnBLL>().Delete(id, GetUser());
            return this.Success(rs);
        }

        [HttpPost]
        public async Task<DefaultAjaxResult<int>> ChangeStatus(string id, int status)
        {
            int rs = await ServiceProvider.GetService<KbColumnBLL>().ChangeStatus(id, status, GetUser());
            return this.Success(rs);
        }
    }
}