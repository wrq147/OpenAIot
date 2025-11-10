using Common;
using DictService.Business;
using DictService.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace DictService.Controller
{
    public class DictInfo : TANetController
    {
        private DictTypeBLL _type;
        public DictInfo(DictTypeBLL type)
        {
            _type = type;
        }
        /// <summary>
        /// 根据字典类型查询字典数据信息（无需登录）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> List(string id)
        {
            List<MZ_DictData> data = await _type.SelectDictDataByType(id);
            return this.Success(data);
        }
    }
}
