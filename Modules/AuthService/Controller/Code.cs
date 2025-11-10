using AuthService.Business;
using AuthService.Model;
using Common;
using Common.KuaiDi;
using Common.Share;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;
using static Common.KuaiDi.KuaiDiPreRead;

namespace AuthService.Controller
{
    public class Code : TANetController
    {
        private CodeBLL _code;
        private ConfigBLL _config;
        public Code(CodeBLL code, ConfigBLL config)
        {
            _code = code;
            _config = config;
        }
        /// <summary>
        /// 获取省市区列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> AreaList()
        {
            return this.Success(await _code.SelectCodeList());
        }
        /// <summary>
        /// 根据经纬度获取区域代码等信息
        /// </summary>
        /// <param name="location">lat纬度,lng经度</param>
        /// <param name="maptype">地图类型：默认腾讯地图，百度地图bd、高德地图gd,本地算法lc</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Geocoder(string location, string maptype = "")
        {
            string mapKey = await _config.SelectConfigByKey($"map.{maptype}key");
            Out_GeoInfo newinfo = new Out_GeoInfo();
            string[] tmpll = location.Split(',');
            newinfo.Lng = Convert.ToDouble(tmpll[1]);
            newinfo.Lat = Convert.ToDouble(tmpll[0]);
            if (string.IsNullOrEmpty(mapKey))
            {
                var areaInfo = await _code.SelectAreaByLatLng(newinfo.Lng, newinfo.Lat);
                if (areaInfo == null)
                {
                    return this.Error<string>(41, "无法识别位置");
                }
                newinfo.AddressCode = areaInfo.Id;
                if (!string.IsNullOrEmpty(areaInfo.Province))
                {
                    newinfo.AddressName = areaInfo.Province;
                }
                if (!string.IsNullOrEmpty(areaInfo.City))
                {
                    newinfo.AddressName = newinfo.AddressName + " " + areaInfo.City;
                }
                if (!string.IsNullOrEmpty(areaInfo.District))
                {
                    newinfo.AddressName = newinfo.AddressName + " " + areaInfo.District;
                }
                newinfo.AddressDetail = string.Empty;
            }
            else
            {
                if (string.IsNullOrEmpty(maptype))
                {
                    var rss = await HttpHelper.Instance.GetAsync($"https://apis.map.qq.com/ws/geocoder/v1/?location={location}&key={mapKey}");
                    var lors = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(rss);
                    if (lors.status == 0)
                    {
                        string adcode = lors.result.ad_info.adcode;
                        if (!string.IsNullOrEmpty(adcode))
                        {
                            newinfo.AddressCode = adcode;
                        }
                        string province = lors.result.address_component.province;
                        string city = lors.result.address_component.city;
                        string district = lors.result.address_component.district;
                        if (!string.IsNullOrEmpty(province))
                        {
                            if (province == city)
                            {
                                newinfo.AddressName = province + " " + district;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(district))
                                {
                                    newinfo.AddressName = province + " " + city;
                                }
                                else
                                {
                                    newinfo.AddressName = province + " " + city + " " + district;
                                }
                            }
                        }
                        else
                        {
                            newinfo.AddressName = string.Empty;
                        }


                        newinfo.AddressDetail = lors.result.address;
                        if (lors.result.formatted_addresses != null)
                        {
                            string standard_address = lors.result.formatted_addresses.standard_address;
                            if (!string.IsNullOrEmpty(standard_address))
                            {
                                newinfo.AddressDetail = standard_address;
                            }
                            string recommend = lors.result.formatted_addresses.recommend;
                            if (!string.IsNullOrEmpty(recommend))
                            {
                                newinfo.AddressDetail = recommend;
                            }
                        }
                    }
                    else
                    {
                        return this.Error<string>(22, (string)lors.message);
                    }
                }
                else if (maptype == "gd")
                {
                    var rss = await HttpHelper.Instance.GetAsync($"https://restapi.amap.com/v3/geocode/regeo?location={newinfo.Lng},{newinfo.Lat}&key={mapKey}");
                    var lors = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(rss);
                    if (lors.status == "1")
                    {
                        newinfo.AddressCode = string.Empty;
                        if (lors.regeocode.addressComponent.adcode is JValue)
                        {
                            newinfo.AddressCode = lors.regeocode.addressComponent.adcode;
                        }
                        string province = string.Empty;
                        if (lors.regeocode.addressComponent.province is JValue)
                        {
                            province = lors.regeocode.addressComponent.province;
                        }
                        string city = string.Empty;
                        if (lors.regeocode.addressComponent.city is JValue)
                        {
                            city = lors.regeocode.addressComponent.city;
                        }
                        string district = string.Empty;
                        if (lors.regeocode.addressComponent.district is JValue)
                        {
                            district = lors.regeocode.addressComponent.district;
                        }

                        if (!string.IsNullOrEmpty(province))
                        {
                            if (province == city)
                            {
                                newinfo.AddressName = province + " " + district;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(district))
                                {
                                    newinfo.AddressName = province + " " + city;
                                }
                                else if (string.IsNullOrEmpty(city))
                                {
                                    newinfo.AddressName = province + " " + district;
                                }
                                else
                                {
                                    newinfo.AddressName = province + " " + city + " " + district;
                                }
                            }
                        }
                        else
                        {
                            newinfo.AddressName = string.Empty;
                        }

                        newinfo.AddressDetail = lors.regeocode.formatted_address;
                    }
                    else
                    {
                        return this.Error<string>(22, (string)lors.info);
                    }
                }
                else if (maptype == "lc")
                {
                    var areaInfo = await _code.SelectAreaByLatLng(newinfo.Lng, newinfo.Lat);
                    if (areaInfo == null)
                    {
                        return this.Error<string>(41, "无法识别位置");
                    }
                    newinfo.AddressCode = areaInfo.Id;
                    if (!string.IsNullOrEmpty(areaInfo.Province))
                    {
                        newinfo.AddressName = areaInfo.Province;
                    }
                    if (!string.IsNullOrEmpty(areaInfo.City))
                    {
                        newinfo.AddressName = newinfo.AddressName + " " + areaInfo.City;
                    }
                    if (!string.IsNullOrEmpty(areaInfo.District))
                    {
                        newinfo.AddressName = newinfo.AddressName + " " + areaInfo.District;
                    }
                    newinfo.AddressDetail = string.Empty;
                }
                else
                {
                    return this.Error<string>(32, "不支持该类型地图");
                }
            }

            string[] replaceArr = newinfo.AddressName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in replaceArr)
            {
                if (string.IsNullOrEmpty(s)) continue;
                newinfo.AddressDetail = newinfo.AddressDetail.Replace(s, string.Empty);
            }
            return this.Success(newinfo);
        }
        /// <summary>
        /// 获取行业列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> IndustryList()
        {
            return this.Success(await _code.SelectIndustryList());
        }

        /// <summary>
        /// 根据参数键名查询参数值
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> GetByKey(string id)
        {
            return this.Success(await _config.SelectConfigByKey(id));
        }
        /// <summary>
        /// 获取所有配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<ObjectItem>>> GetConfigList(string[] data)
        {
            return this.Success(await _config.SelectConfigList(data));
        }
        /// <summary>
        /// 自动识别快递单号的对应快递企业
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet]
        public DefaultAjaxResult<string> AutoCompany(string number)
        {
            return this.Success(KuaiDiPreRead.Simi(number));
        }
        /// <summary>
        /// 获取所有快递企业
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public DefaultAjaxResult<List<KuaiDiCompany>> KuaiDiCompanys()
        {
            return this.Success(KuaiDiPreRead.GetCompanyList());
        }
    }
}
