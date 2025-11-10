using DictService.DAL;
using System;
using System.Collections.Generic;
using TemplateAction.Core;
using DictService.Model;
using Common;
using Common.Share;
using AuthService;
using System.Threading.Tasks;

namespace DictService.Business
{
    public class DictTypeBLL
    {
        private DictTypeDAL _dictTypeDAL;
        private DictDataDAL _dictDataDAL;
        private ITAServiceProvider _provider;

        public DictTypeBLL(ITAServiceProvider provider, DictTypeDAL dictTypeDAL, DictDataDAL dictDataDAL)
        {
            _provider = provider;
            _dictTypeDAL = dictTypeDAL;
            _dictDataDAL = dictDataDAL;
        }

        /// <summary>
        /// 缓存字典数据
        /// </summary>
        private async Task _LoadingDictCache()
        {
            List<MZ_DictType> dictTypeList = await _dictTypeDAL.SelectDictTypeAll();
            foreach (MZ_DictType dictType in dictTypeList)
            {
                List<MZ_DictData> dictDatas = await _dictDataDAL.SelectDictDataByType(dictType.dict_type);
                await _provider.GetService<GeneralRedisHelper>().StringSetAsync(DictConstant.CACHE_PRE + dictType.dict_type, dictDatas);
            }
        }



        /// <summary>
        /// 根据条件查询字典类型
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual async Task<PageObject<MZ_DictType>> SelectDictTypeList(In_DictTypeList query)
        {
            return await _dictTypeDAL.SelectDictTypeList(query);
        }


        /// <summary>
        /// 根据所有字典类型
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<MZ_DictType>> SelectDictTypeAll()
        {
            return await _dictTypeDAL.SelectDictTypeAll();
        }


        /// <summary>
        /// 根据字典类型查询字典数据
        /// </summary>
        /// <param name="dictType"></param>
        /// <returns></returns>
        public virtual async Task<List<MZ_DictData>> SelectDictDataByType(string dictType)
        {
            List<MZ_DictData> dictDatas = await _provider.GetService<GeneralRedisHelper>().StringGetAsync<List<MZ_DictData>>(DictConstant.CACHE_PRE + dictType);
            if (dictDatas != null)
            {
                return dictDatas;
            }
            dictDatas = await _dictDataDAL.SelectDictDataByType(dictType);
            if (dictDatas != null)
            {
                await _provider.GetService<GeneralRedisHelper>().StringSetAsync(DictConstant.CACHE_PRE + dictType, dictDatas);
                return dictDatas;
            }
            return null;
        }
        /// <summary>
        /// 根据字典类型获取字典转换
        /// </summary>
        /// <param name="dictType"></param>
        /// <returns></returns>
        public virtual async Task<DictConverter> SelectDictConverter(string dictType)
        {
            var tlist = await SelectDictDataByType(dictType);
            return new DictConverter(tlist);
        }


        /// <summary>
        /// 根据字典类型ID查询信息
        /// </summary>
        /// <param name="dictId"></param>
        /// <returns></returns>
        public virtual async Task<MZ_DictType> SelectDictTypeById(long dictId)
        {
            return await _dictTypeDAL.SelectDictTypeById(dictId);
        }


        /// <summary>
        /// 根据字典类型查询信息
        /// </summary>
        /// <param name="dictType"></param>
        /// <returns></returns>
        public virtual async Task<MZ_DictType> SelectDictTypeByType(string dictType)
        {
            return await _dictTypeDAL.SelectDictTypeByType(dictType);
        }


        /// <summary>
        /// 批量删除字典类型信息
        /// </summary>
        /// <param name="dictIds"></param>
        public virtual async Task<BusResponse<string>> DeleteDictTypeByIds(long[] dictIds)
        {
            foreach (long dictId in dictIds)
            {
                MZ_DictType dictType = await SelectDictTypeById(dictId);
                if (await _dictDataDAL.CountDictDataByType(dictType.dict_type) > 0)
                {
                    return BusResponse<string>.Error(121, String.Format("{0}已分配,不能删除", dictType.dict_name));
                }
                await _dictTypeDAL.DeleteDictTypeById(dictId);
                await _provider.GetService<GeneralRedisHelper>().KeyDeleteAsync(DictConstant.CACHE_PRE + dictType);
            }
            return BusResponse<string>.Success();
        }


        /// <summary>
        /// 清空字典缓存数据
        /// </summary>
        private async Task _ClearDictCache()
        {
            GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
            List<string> keys = await redis.KeysAsync(DictConstant.CACHE_PRE + "*");
            foreach (string s in keys)
            {
                await redis.KeyDeleteAsync(s);
            }
        }


        /// <summary>
        /// 重置字典缓存数据
        /// </summary>
        public virtual async Task ResetDictCache()
        {
            await _ClearDictCache();
            await _LoadingDictCache();
        }

        /// <summary>
        /// 新增保存字典类型信息
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> InsertDictType(MZ_DictType dict)
        {
            if (!await CheckDictTypeUnique(dict))
            {
                return BusResponse<int>.Error(11, "新增字典'" + dict.dict_name + "'失败，字典类型已存在");
            }
            int row = await _dictTypeDAL.InsertDictType(dict);
            if (row > 0)
            {
                await _provider.GetService<GeneralRedisHelper>().KeyDeleteAsync(DictConstant.CACHE_PRE + dict.dict_type);
                return BusResponse<int>.Success(row);
            }
            else
            {
                return BusResponse<int>.Error(13, "新增失败");
            }
        }


        /// <summary>
        /// 修改保存字典类型信息
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateDictType(MZ_DictType dict)
        {
            if (!await CheckDictTypeUnique(dict))
            {
                return BusResponse<int>.Error(11, "修改字典'" + dict.dict_name + "'失败，字典类型已存在");
            }
            MZ_DictType oldDict = await _dictTypeDAL.SelectDictTypeById(dict.dict_id.Value);
            await _dictDataDAL.UpdateDictDataType(oldDict.dict_type, dict.dict_type);
            int row = await _dictTypeDAL.UpdateDictType(dict);
            if (row > 0)
            {
                List<MZ_DictData> dictDatas = await _dictDataDAL.SelectDictDataByType(dict.dict_type);
                await _provider.GetService<GeneralRedisHelper>().StringSetAsync(DictConstant.CACHE_PRE + dict.dict_type, dictDatas);
                return BusResponse<int>.Success(row);
            }
            else
            {
                return BusResponse<int>.Error(row == -1 ? 13 : 14, "更新失败");
            }
        }


        /// <summary>
        /// 校验字典类型称是否唯一
        /// </summary>
        /// <param name="dict"></param>
        /// <returns>true表示唯一</returns>
        public virtual async Task<bool> CheckDictTypeUnique(MZ_DictType dict)
        {
            long dictId = dict.dict_id == null ? -1L : dict.dict_id.Value;
            return await _dictTypeDAL.CheckDictTypeUnique(dict.dict_type, dictId);
        }
    }
}
