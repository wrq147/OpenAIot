using DictService.Business;
using DictService.Model;
using System;
using System.Collections.Generic;
using TemplateAction.Core;
using Common;
using Common.Share;
using TemplateAction.Label;
using AuthService.Controller;
using System.Threading.Tasks;
using TemplateAction.Route;

namespace DictService.Controller
{
    public class DictData : AbstractLoginedController
    {
        private DictDataBLL _data;
        private DictTypeBLL _type;
        public DictData(DictTypeBLL type, DictDataBLL data)
        {
            _type = type;
            _data = data;
        }
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="dictData"></param>
        /// <returns></returns>
        [About("/DictService/DictType/List")]
        [HttpGet]
        public async Task<AjaxResult> List(In_DictDataList dictData = null)
        {
            if (dictData == null) dictData = new In_DictDataList();
            List<MZ_DictData> list = await _data.SelectDictDataList(dictData);
            return this.Success(new PageObject<MZ_DictData>()
            {
                List = list,
                Total = list.Count
            });
        }
        [About("/DictService/DictType/Export")]
        [HttpGet]
        public async Task<IResult> Export(In_DictDataList dictData)
        {
            try
            {
                List<MZ_DictData> list = await _data.SelectDictDataList(dictData);

                Dictionary<string, ParamRenderToExcel<MZ_DictData>> FiedNames = new Dictionary<string, ParamRenderToExcel<MZ_DictData>>();
                FiedNames.Add("dict_code", new ParamRenderToExcel<MZ_DictData>("字典编码"));
                FiedNames.Add("dict_label", new ParamRenderToExcel<MZ_DictData>("字典标签"));
                FiedNames.Add("dict_value", new ParamRenderToExcel<MZ_DictData>("字典键值"));
                FiedNames.Add("dict_type", new ParamRenderToExcel<MZ_DictData>("字典类型"));
                FiedNames.Add("status", new ParamRenderToExcel<MZ_DictData>("状态（0正常 1停用）"));
                FiedNames.Add("remark", new ParamRenderToExcel<MZ_DictData>("备注"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<MZ_DictData>("字典数据表", list, FiedNames);

                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }


        /// <summary>
        /// 查询字典数据详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info(long id)
        {
            return this.Success(await _data.SelectDictDataById(id));
        }




        /// <summary>
        /// 根据字典类型查询字典数据信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> DictType(string id)
        {
            List<MZ_DictData> data = await _type.SelectDictDataByType(id);
            return this.Success(data);
        }


        /// <summary>
        /// 新增字典类型
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        [About("/DictService/DictType/Add")]
        [HttpPost]
        public async Task<AjaxResult> Add(MZ_DictData dict)
        {
            return (await _data.InsertDictData(dict)).ToAjaxResult();
        }



        /// <summary>
        /// 修改保存字典类型
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        [About("/DictService/DictType/Edit")]
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_DictData dict)
        {
            return (await _data.UpdateDictData(dict)).ToAjaxResult();
        }
        /// <summary>
        /// 设置为默认
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/DictService/DictType/Edit")]
        [HttpGet]
        public async Task<AjaxResult> Default(long id)
        {
            return (await _data.SetDefault(id)).ToAjaxResult();
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
            return (await _data.DeleteDictDataByIds(id)).ToAjaxResult();
        }
    }
}
