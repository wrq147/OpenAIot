
using DictService.Business;
using DictService.Model;
using System;
using System.Collections.Generic;
using TemplateAction.Core;
using Common;
using Common.Share;
using TemplateAction.Label;
using AuthService;
using AuthService.Controller;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DictService.Controller
{
    public class DictType : AbstractLoginedController
    {
        private DictTypeBLL _type;
        public DictType(DictTypeBLL type)
        {
            _type = type;
        }
        [About("/DictService/DictType/List")]
        [HttpGet]
        public async Task<AjaxResult> List(In_DictTypeList query = null)
        {
            if (query == null)
            {
                query = new In_DictTypeList();
            }
            return this.Success(await _type.SelectDictTypeList(query));
        }
        [About("/DictService/DictType/Export")]
        [HttpGet]
        public async Task<IResult> Export(In_DictTypeList query)
        {
            try
            {
                query.showAll = true;
                PageObject<MZ_DictType> page = await _type.SelectDictTypeList(query);
                List<MZ_DictType> list = page.List;

                Dictionary<string, ParamRenderToExcel<MZ_DictType>> FiedNames = new Dictionary<string, ParamRenderToExcel<MZ_DictType>>();
                FiedNames.Add("dict_id", new ParamRenderToExcel<MZ_DictType>("字典主键"));
                FiedNames.Add("dict_name", new ParamRenderToExcel<MZ_DictType>("字典名称"));
                FiedNames.Add("dict_type", new ParamRenderToExcel<MZ_DictType>("字典类型"));
                FiedNames.Add("status", new ParamRenderToExcel<MZ_DictType>("状态（0正常 1停用）"));
                FiedNames.Add("remark", new ParamRenderToExcel<MZ_DictType>("备注"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<MZ_DictType>("字典类别表", list, FiedNames);

                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }



        /// <summary>
        /// 查询字典类型详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info(long id)
        {
            return this.Success(await _type.SelectDictTypeById(id));
        }

        /// <summary>
        /// 新增字典类型
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        [About("/DictService/DictType/Add")]
        [HttpPost]
        public async Task<AjaxResult> Add(MZ_DictType dict)
        {
            dict.SetCreateBy(GetUser());
            return (await _type.InsertDictType(dict)).ToAjaxResult();
        }


        /// <summary>
        /// 修改字典类型
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        [About("/DictService/DictType/Edit")]
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_DictType dict)
        {
            dict.SetUpdateBy(GetUser());
            return (await _type.UpdateDictType(dict)).ToAjaxResult();
        }



        /// <summary>
        /// 删除字典类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/DictService/DictType/Remove")]
        [HttpGet]
        public async Task<AjaxResult> Remove(long[] id)
        {
            return (await _type.DeleteDictTypeByIds(id)).ToAjaxResult();
        }


        /// <summary>
        /// 刷新字典缓存
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> RefreshCache()
        {
            await _type.ResetDictCache();
            return this.Success<string>();
        }


        /// <summary>
        /// 获取字典选择框列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> OptionSelect()
        {
            List<MZ_DictType> dictTypes = await _type.SelectDictTypeAll();
            return this.Success(dictTypes);
        }
    }
}
