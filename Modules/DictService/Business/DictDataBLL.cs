using DictService.DAL;
using System;
using System.Collections.Generic;
using TemplateAction.Core;
using DictService.Model;
using Common.Share;
using Common;
using AuthService;
using System.Threading.Tasks;

namespace DictService.Business
{
    public class DictDataBLL
    {
        private DictTypeDAL _dictTypeDAL;
        private DictDataDAL _dictDataDAL;
        private ITAServiceProvider _provider;
        private ITAContext _context;
        public DictDataBLL(ITAServiceProvider provider, DictTypeDAL dictTypeDAL, DictDataDAL dictDataDAL, ITAContext context)
        {
            _provider = provider;
            _dictTypeDAL = dictTypeDAL;
            _dictDataDAL = dictDataDAL;
            _context = context;
        }


        /// <summary>
        /// 根据条件查询字典数据
        /// </summary>
        /// <param name="dictData"></param>
        /// <returns></returns>
        public virtual async Task<List<MZ_DictData>> SelectDictDataList(In_DictDataList dictData)
        {
            return await _dictDataDAL.SelectDictDataList(dictData);
        }


        /// <summary>
        /// 根据字典类型和字典键值查询字典数据信息
        /// </summary>
        /// <param name="dictType"></param>
        /// <param name="dictValue"></param>
        /// <returns></returns>
        public virtual async Task<string> SelectDictLabel(string dictType, string dictValue)
        {
            return await _dictDataDAL.SelectDictLabel(dictType, dictValue);
        }


        /// <summary>
        /// 根据字典数据ID查询信息
        /// </summary>
        /// <param name="dictCode"></param>
        /// <returns></returns>
        public virtual async Task<MZ_DictData> SelectDictDataById(long dictCode)
        {
            return await _dictDataDAL.SelectDictDataById(dictCode);
        }


        /// <summary>
        /// 批量删除字典数据信息
        /// </summary>
        /// <param name="dictCodes"></param>
        public virtual async Task<BusResponse<string>> DeleteDictDataByIds(long[] dictCodes)
        {
            foreach (long dictCode in dictCodes)
            {
                MZ_DictData data = await SelectDictDataById(dictCode);
                await _dictDataDAL.DeleteDictDataById(dictCode);
                List<MZ_DictData> dictDatas = await _dictDataDAL.SelectDictDataByType(data.dict_type);

                await _provider.GetService<GeneralRedisHelper>().StringSetAsync(DictConstant.CACHE_PRE + data.dict_type, dictDatas);
            }
            return BusResponse<string>.Success();
        }


        /// <summary>
        /// 新增保存字典数据信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> InsertDictData(MZ_DictData data)
        {
            int cc = await _dictDataDAL.CountDictDataByType(data.dict_type);
            data.is_default = cc > 0 ? "N" : "Y";
            data.css_class ??= string.Empty;
            data.remark ??= string.Empty;
            data.SetCreateBy(Data_ServerTokenInfo.From(_context));
            int row = await _dictDataDAL.InsertDictData(data);
            if (row > 0)
            {
                List<MZ_DictData> dictDatas = await _dictDataDAL.SelectDictDataByType(data.dict_type);
                await _provider.GetService<GeneralRedisHelper>().StringSetAsync(DictConstant.CACHE_PRE + data.dict_type, dictDatas);
                return BusResponse<int>.Success(row);
            }
            else
            {
                return BusResponse<int>.Error(13, "新增失败");
            }
        }


        /// <summary>
        /// 修改保存字典数据信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateDictData(MZ_DictData data)
        {
            data.SetUpdateBy(Data_ServerTokenInfo.From(_context));
            int row = await _dictDataDAL.UpdateDictData(data);
            if (row > 0)
            {
                List<MZ_DictData> dictDatas = await _dictDataDAL.SelectDictDataByType(data.dict_type);
                await _provider.GetService<GeneralRedisHelper>().StringSetAsync(DictConstant.CACHE_PRE + data.dict_type, dictDatas);
                return BusResponse<int>.Success(row);
            }
            else
            {
                return BusResponse<int>.Error(row == -1 ? 13 : 14, "更新失败");
            }
        }
        public virtual async Task<BusResponse<int>> SetDefault(long id)
        {
            try
            {
                var dic_data = await _dictDataDAL.SelectDictDataById(id);
                await _dictDataDAL.ClearDefault(dic_data.dict_type);
                MZ_DictData up = new MZ_DictData();
                up.dict_code = id;
                up.is_default = "Y";
                await _dictDataDAL.UpdateDictData(up);
                List<MZ_DictData> dictDatas = await _dictDataDAL.SelectDictDataByType(dic_data.dict_type);
                await _provider.GetService<GeneralRedisHelper>().StringSetAsync(DictConstant.CACHE_PRE + dic_data.dict_type, dictDatas);
                return BusResponse<int>.Success();
            }
            catch
            {
                return BusResponse<int>.Error(112, "更新失败");
            }

        }
    }
}
