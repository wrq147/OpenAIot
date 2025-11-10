using AuthService.Business;
using AuthService.Model;
using ChannelUtility.Tsl;
using IoTService.DAL;
using IoTService.Models;
using MyAccess.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.Business
{
    public class IotTagBLL
    {
        private IotDeviceTagDAL _tagDAL;
        private IotDeviceDAL _deviceDAL;
        private ITAServiceProvider _provider;
        public IotTagBLL(ITAServiceProvider provider, IotDeviceDAL deviceDAL, IotDeviceTagDAL tagDAL)
        {
            _provider = provider;
            _deviceDAL = deviceDAL;
            _tagDAL = tagDAL;
        }
        public virtual async Task Remove(string id)
        {
            await _tagDAL.Delete(x => x.Id == id);
        }
        /// <summary>
        /// 存储的标签转显示用数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tag"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public Out_DeviceTagItem ToTagItem(string id, BaseTagInfo tag, object val)
        {
            if (tag.code == "position" || tag.code == "state")
            {
                return null;
            }
            else if (tag.option.type == "geo")
            {
                Out_DeviceTagItem newout = new Out_DeviceTagItem();
                newout.Id = id;
                newout.Name = tag.name;
                newout.Option = tag.option;
                newout.Code = tag.code;
                newout.MapCode = tag.mapcode;
                newout.Value = val;
                newout.DisplayValue = string.Empty;
                var tmpdict = val as Dictionary<string, object>;
                if (tmpdict != null)
                {
                    newout.DisplayValue = $"经度:{tmpdict["lng"].ToString()},纬度:{tmpdict["lat"]}";
                }
                return newout;
            }
            else
            {
                Out_DeviceTagItem newout = new Out_DeviceTagItem();
                newout.Id = id;
                newout.Name = tag.name;
                newout.MapCode = tag.mapcode;
                newout.Code = tag.code;
                newout.Option = tag.option;
                if (tag.option.type == "int")
                {
                    newout.Value = val;
                    newout.DisplayValue = val.ToString();
                }
                else if (tag.option.type == "float")
                {
                    newout.Value = val;
                    newout.DisplayValue = val.ToString();
                }
                else if (tag.option.type == "boolean")
                {
                    bool tmpb = Convert.ToBoolean(val);
                    newout.Value = tmpb;
                    if (tmpb)
                    {
                        newout.DisplayValue = ((BooleanOption)tag.option).trueText;
                    }
                    else
                    {
                        newout.DisplayValue = ((BooleanOption)tag.option).falseText;
                    }
                }
                else if (tag.option.type == "enum")
                {
                    string tval = val.ToString();
                    newout.Value = tval;
                    newout.DisplayValue = tval;
                }
                else if (tag.option.type == "date")
                {
                    long llvl = Convert.ToInt64(val);
                    newout.Value = llvl;
                    if (llvl > 0)
                    {
                        newout.DisplayValue = MyAccess.Core.TypeConvert.Unix2Time(llvl).ToString(((DateOption)tag.option).format);
                    }
                    else
                    {
                        newout.DisplayValue = string.Empty;
                    }
                }
                else if (tag.option.type == "string")
                {
                    newout.Value = val;
                    newout.DisplayValue = (val ?? "").ToString();
                }
                else
                {
                    newout.Value = val;
                    newout.DisplayValue = "未知";
                }
                return newout;
            }
        }
        /// <summary>
        /// 更新设备标签
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="val"></param>
        /// <param name="id">设备Id</param>
        /// <returns></returns>
        public async Task TagUpdateValue(BaseTagInfo tag, object val, string id)
        {
            if (tag.code == "position")
            {
                if (val is JObject jobj)
                {
                    MZ_IotDevice newdev = new MZ_IotDevice();
                    newdev.Id = id;
                    newdev.Lng = jobj.Value<double>("lng");
                    newdev.Lat = jobj.Value<double>("lat");
                    newdev.GeoHash = MyAccess.Core.GeoHash.Encode(newdev.Lat.Value, newdev.Lng.Value);
                    var areaInfo = await _provider.GetService<CodeBLL>().SelectAreaByLatLng(newdev.Lng.Value, newdev.Lat.Value);
                    if (areaInfo != null)
                    {
                        newdev.AreaCode = areaInfo.Id;
                    }
                    else
                    {
                        newdev.AreaCode = string.Empty;
                    }
                    await _deviceDAL.Update(newdev);
                }
                else if (val is IDictionary<string, object> dict)
                {

                    MZ_IotDevice newdev = new MZ_IotDevice();
                    newdev.Id = id;
                    newdev.Lng = Convert.ToDouble(dict["lng"]);
                    newdev.Lat = Convert.ToDouble(dict["lat"]);
                    newdev.GeoHash = MyAccess.Core.GeoHash.Encode(newdev.Lat.Value, newdev.Lng.Value);
                    var areaInfo = await _provider.GetService<CodeBLL>().SelectAreaByLatLng(newdev.Lng.Value, newdev.Lat.Value);
                    if (areaInfo != null)
                    {
                        newdev.AreaCode = areaInfo.Id;
                    }
                    else
                    {
                        newdev.AreaCode = string.Empty;
                    }
                    await _deviceDAL.Update(newdev);
                }
                else if (val is MZ_Area area)
                {
                    MZ_IotDevice newdev = new MZ_IotDevice();
                    newdev.Id = id;
                    newdev.Lng = Convert.ToDouble(area.Lng);
                    newdev.Lat = Convert.ToDouble(area.Lat);
                    newdev.GeoHash = MyAccess.Core.GeoHash.Encode(newdev.Lat.Value, newdev.Lng.Value);
                    newdev.AreaCode = area.Id;
                    await _deviceDAL.Update(newdev);
                }
                else
                {
                    return;
                }
            }
            else if (tag.code == "state")
            {
                var enumOpt = tag.option as EnumOption;
                if (enumOpt == null)
                {
                    return;
                }
                List<string> stateFilter = null;
                if (enumOpt.elements != null)
                {
                    stateFilter = enumOpt.elements.Keys.ToList();
                }

                MZ_IotDevice newdev = new MZ_IotDevice();
                newdev.DState = val as string;
                if (stateFilter != null)
                {
                    await _deviceDAL.Update(newdev, x => x.Id == id && stateFilter.Contains(x.DState));
                }
                else
                {
                    newdev.Id = id;
                    await _deviceDAL.Update(newdev);
                }
            }
            else if (tag.code == "signal")
            {
                MZ_IotDevice newdev = new MZ_IotDevice();
                newdev.Id = id;
                newdev.dBm = Convert.ToSingle(val);
                await _deviceDAL.Update(newdev);
            }
            else
            {
                if (tag.option.type == "geo")
                {
                    IDictionary<string, object> geodef;
                    if (val is JObject jobj)
                    {
                        geodef = new Dictionary<string, object>();
                        geodef.Add("lng", jobj.Value<double>("lng"));
                        geodef.Add("lat", jobj.Value<double>("lat"));
                    }
                    else if (val is IDictionary<string, object> dobj)
                    {
                        geodef = dobj;
                    }
                    else
                    {
                        return;
                    }
                    object deflng;
                    object deflat;
                    if (geodef.TryGetValue("lng", out deflng) && geodef.TryGetValue("lat", out deflat))
                    {
                        var llhs = MyAccess.Core.GeoHash.Encode((double)deflat, (double)deflng);
                        MZ_IotDeviceTag updateItemlng = new MZ_IotDeviceTag();
                        updateItemlng.Id = id;
                        updateItemlng.Name = tag.name + "@经度";
                        updateItemlng.Code = tag.code + "@lng";
                        updateItemlng.NumValue = Convert.ToDouble(deflng);
                        await _tagDAL.CreateOrUpdate(updateItemlng);

                        MZ_IotDeviceTag updateItemlat = new MZ_IotDeviceTag();
                        updateItemlat.Id = id;
                        updateItemlat.Name = tag.name + "@纬度";
                        updateItemlat.Code = tag.code + "@lat";
                        updateItemlat.NumValue = Convert.ToDouble(deflat);
                        await _tagDAL.CreateOrUpdate(updateItemlat);

                        MZ_IotDeviceTag updateItem = new MZ_IotDeviceTag();
                        updateItem.Id = id;
                        updateItem.Name = tag.name + "@geo";
                        updateItem.Code = tag.code + "@hs";
                        updateItem.Value = llhs;
                        await _tagDAL.CreateOrUpdate(updateItem);

                        MZ_IotDeviceTag updateValue = new MZ_IotDeviceTag();
                        updateItemlng.Id = id;
                        updateItemlng.Name = tag.name;
                        updateItemlng.Code = tag.code;
                        updateItemlng.Value = JsonConvert.SerializeObject(new
                        {
                            lng = deflng,
                            lat = deflat
                        });
                        await _tagDAL.CreateOrUpdate(updateValue);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    string tmpval = null;
                    double? tmpnumval = null;
                    if (tag.option.type == "int")
                    {
                        decimal tmpdec = ((IntOption)tag.option).Limit(val);
                        tmpnumval = (double)tmpdec;
                    }
                    else if (tag.option.type == "float")
                    {
                        tmpnumval = ((FloatOption)tag.option).Limit(val);
                    }
                    else if (tag.option.type == "enum")
                    {
                        tmpval = val.ToString();
                    }
                    else if (tag.option.type == "date")
                    {
                        DateTime? valDate = null;
                        if (val is string valstr)
                        {
                            DateTime tmpdate;
                            if (DateTime.TryParse(valstr, out tmpdate))
                            {
                                valDate = tmpdate;
                            }
                        }
                        else if (val is long valll)
                        {
                            valDate = MyAccess.Core.TypeConvert.Unix2Time(valll);
                        }
                        if (valDate == null)
                        {
                            tmpnumval = 0;
                        }
                        else
                        {
                            string clientTZ = TAAction.Current.Context.Request.Header["TZ"];
                            if (!string.IsNullOrEmpty(clientTZ))
                            {
                                int tz;
                                if (int.TryParse(clientTZ, out tz))
                                {
                                    valDate = TimeZoneInfo.ConvertTimeFromUtc(valDate.Value.AddMinutes(tz), TimeZoneInfo.Local);
                                }
                            }
                            tmpnumval = MyAccess.Core.TypeConvert.Time2Unix(valDate.Value);
                        }

                    }
                    else if (tag.option.type == "boolean")
                    {
                        tmpval = Convert.ToBoolean(val) ? "true" : "false";
                    }
                    else if (tag.option.type == "string")
                    {
                        tmpval = (val ?? "").ToString();
                    }
                    else
                    {
                        tmpval = JsonConvert.SerializeObject(val);
                    }
                    MZ_IotDeviceTag updateItem = new MZ_IotDeviceTag();
                    updateItem.Id = id;
                    updateItem.Name = tag.name;
                    updateItem.Code = tag.code;
                    updateItem.Value = tmpval;
                    updateItem.NumValue = tmpnumval;
                    await _tagDAL.CreateOrUpdate(updateItem);
                }
            }
        }

    }
}
