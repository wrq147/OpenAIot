using AuthService.DAL;
using AuthService.Model;
using Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace AuthService.Business
{
    public class CodeBLL
    {
        private ITAServiceProvider _provider;
        private CodeDAL _codeDAL;
        public CodeBLL(ITAServiceProvider provider, CodeDAL codeDAL)
        {
            _provider = provider;
            _codeDAL = codeDAL;
        }
        public async Task<List<MZ_Area>> SelectCodeList()
        {
            var redis = _provider.GetService<GeneralRedisHelper>();
            var cacheHelper = _provider.GetService<CacheHelper>();
            var tmplocallist = cacheHelper.GetCache<List<MZ_Area>>(AuthConstant.CONFIG_AREA_KEY);
            if (tmplocallist == null)
            {
                var list = await redis.StringGetAsync<List<MZ_Area>>(AuthConstant.CONFIG_AREA_KEY);
                if (list == null)
                {
                    list = await _codeDAL.SelectCodeList();
                    await redis.StringSetAsync<List<MZ_Area>>(AuthConstant.CONFIG_AREA_KEY, list);
                }
                cacheHelper.SetCache(AuthConstant.CONFIG_AREA_KEY, list, DateTime.Now.AddHours(6));
                tmplocallist = list;
            }
            return tmplocallist;
        }
        public async Task<MZ_Area> SelectAreaByLatLng(double lng, double lat)
        {
            return await _codeDAL.SelectAreaByLatLng(lng, lat, 3);
        }
        public async Task<MZ_Area> IpToArea(string ip)
        {
            try
            {
                string address = await IpHelper.Instance.GetRealAddressByIP(ip);
                string[] tarr = address.Split(",");
                if (tarr.Length == 3)
                {
                    var area = await _codeDAL.SelectAreaByCity(tarr[1], tarr[2]);
                    return area;
                }
                else if (tarr.Length == 4)
                {
                    var area = await _codeDAL.SelectAreaByDistrict(tarr[1], tarr[2], tarr[3]);
                    return area;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取地址字典
        /// </summary>
        /// <returns></returns>
        public async Task<AreaDict> SelectAreaDict()
        {
            var tlist = await SelectCodeList();
            return new AreaDict(tlist);
        }
        public async Task<List<MZ_Industry>> SelectIndustryList()
        {
            var redis = _provider.GetService<GeneralRedisHelper>();
            var list = await redis.StringGetAsync<List<MZ_Industry>>(AuthConstant.CONFIG_INDUSTRY_KEY);
            if (list == null)
            {
                list = await _codeDAL.SelectIndustryList();
                await redis.StringSetAsync<List<MZ_Industry>>(AuthConstant.CONFIG_INDUSTRY_KEY, list);
            }
            return list;
        }
        /// <summary>
        /// 获取行业字典
        /// </summary>
        /// <returns></returns>
        public async Task<IndustryDict> SelectIndustryDict()
        {
            var tlist = await SelectIndustryList();
            return new IndustryDict(tlist);
        }
    }
}
