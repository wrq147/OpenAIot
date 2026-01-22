using AuthService;
using ChannelUtility.Config;
using ChannelUtility.Tsl;
using Common.IdGenerator;
using Common.Json;
using Common.Share;
using IoTService.DAL;
using IoTService.Models;
using Microsoft.Extensions.Options;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.Business
{
    public class IotProductBLL
    {
        private ITAServiceProvider _provider;
        private IotProductDAL _productDAL;
        private IotClassDAL _classDAL;
        private SnowflakeHelper _snowflake;
        private IotDeviceDAL _deviceDAL;
        public IotProductBLL(ITAServiceProvider provider, IotProductDAL productDAL, IotClassDAL classDAL, IotDeviceDAL deviceDAL, SnowflakeHelper snowflake)
        {
            _provider = provider;
            _productDAL = productDAL;
            _classDAL = classDAL;
            _deviceDAL = deviceDAL;
            _snowflake = snowflake;
        }
        public virtual async Task<List<Out_ChannelInfo>> GetChannelList()
        {
            var redis = _provider.GetService<IotRedisHelper>();
            var channels = await redis.HashGetAllAsync<string>("IotChannels");
            List<Out_ChannelInfo> outlist = new List<Out_ChannelInfo>();
            string defaultImgUrl = _provider.GetService<IOptions<GeneralOption>>().Value.default_imgurl;
            outlist.Add(new Out_ChannelInfo("无", string.Empty, defaultImgUrl, "非物联协议选择这个"));
            foreach (var kvp in channels)
            {
                var config = System.Text.Json.JsonSerializer.Deserialize<ChannelConfig>(kvp.Value, MyDefaultTextJsonConfig.DefaultOptions);
                outlist.Add(new Out_ChannelInfo(config.Name, config.Code, config.ImageUrl, config.Remark));
            }
            return outlist;
        }
        public virtual async Task<ChannelConfig> GetChannel(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return new ChannelConfig();
            }
            var redis = _provider.GetService<IotRedisHelper>();
            var channelstr = await redis.HashGetAsync<string>("IotChannels", code);
            if (channelstr == null)
            {
                return new ChannelConfig();
            }
            var config = System.Text.Json.JsonSerializer.Deserialize<ChannelConfig>(channelstr, MyDefaultTextJsonConfig.DefaultOptions);
            return config;
        }
        public virtual async Task<PageObject<Out_ProductName>> ProductNamePage(In_ProductNamePage query, IUserInfo user)
        {
            return await _productDAL.SelectNames(query, user);
        }
        public virtual async Task<List<MZ_IotProduct>> TSLList(string[] ids)
        {
            return await _productDAL.SelectProductTSL(ids);
        }
        public virtual async Task<PageObject<MZ_IotProduct>> ListPage(In_ProductListPage query)
        {
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);
            query.OrgId = user.OrgId;
            if (query.ClassId != null)
            {
                MZ_IotClass cls = await _classDAL.Select(query.ClassId);
                if (cls != null)
                {
                    query.ClassPath = cls.Path;
                }
            }
            var rs = await _productDAL.SelectWithClassPage(query);
            UserDAL userDAL = _provider.GetService<UserDAL>();
            var userDict1 = await userDAL.NavigateDict(rs.List, x => true, x => x.createId.Value);
            var userDict2 = await userDAL.NavigateDict(rs.List, x => true, x => x.updateId.Value);
            foreach (var iotPro in rs.List)
            {
                MZ_AdminInfo user1;
                if (userDict1.TryGetValue(iotPro.createId.Value, out user1))
                {
                    iotPro.createName = user1.RealName;
                }
                MZ_AdminInfo user2;
                if (userDict2.TryGetValue(iotPro.updateId.Value, out user2))
                {
                    iotPro.updateName = user2.RealName;
                }

            }
            return rs;
        }
        public virtual async Task<MZ_IotProduct> Info(string id, bool notsl = false)
        {
            MZ_IotProduct product = null;
            if (notsl)
            {
                product = await _productDAL.SelectWithNoTsl(id);
            }
            else
            {
                product = await _productDAL.Select(id);
            }
            if (product == null)
            {
                return product;
            }
            var proclass = await _classDAL.Select(id);
            if (proclass != null)
            {
                product.ClassName = proclass.Name;
            }
            return product;
        }

        public virtual async Task<BusResponse<int>> Update(MZ_IotProduct data)
        {
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);
            data.SetUpdateBy(user);
            data.OrgId = null;
            var old = await _productDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(114, "协议不存在");
            }

            if (data.ModelTSL != null)
            {
                HashSet<string> codehs = new HashSet<string>();
                var model = TslModel.CreateFrom(data.ModelTSL);
                if (model != null)
                {
                    foreach (var prop in model.properties)
                    {
                        if (!codehs.Contains(prop.code))
                        {
                            codehs.Add(prop.code);
                        }
                        else
                        {
                            return BusResponse<int>.Error(121, $"属性标识符{prop.code}重复声明");
                        }
                        if (prop.code.StartsWith("$"))
                        {
                            return BusResponse<int>.Error(129, "属性标识符格式错误");
                        }
                    }

                    foreach (var func in model.functions)
                    {
                        if (!codehs.Contains(func.code))
                        {
                            codehs.Add(func.code);
                        }
                        else
                        {
                            return BusResponse<int>.Error(121, $"功能标识符{func.code}重复声明");
                        }
                    }
                    foreach (var evt in model.events)
                    {
                        if (!codehs.Contains(evt.code))
                        {
                            codehs.Add(evt.code);
                        }
                        else
                        {
                            return BusResponse<int>.Error(121, $"事件标识符{evt.code}重复声明");
                        }
                    }
                    foreach (var tag in model.tags)
                    {
                        if (!codehs.Contains(tag.code))
                        {
                            codehs.Add(tag.code);
                        }
                        else
                        {
                            return BusResponse<int>.Error(121, $"标签标识符{tag.code}重复声明");
                        }
                    }
                }
            }
            bool bpublic = data.Status == "1" && old.Status == "0";
            if (bpublic)
            {
                data.PublicTime = DateTime.Now;
                data.Version = old.Version + 1;
            }
            data.TSLUpdated = null;
            if (!string.IsNullOrEmpty(data.ModelTSL))
            {
                if (data.ModelTSL != old.ModelTSL)
                {
                    data.TSLUpdated = DateTime.Now;
                }
            }
            if (old.TSLUpdated == null)
            {
                data.TSLUpdated = DateTime.Now;
            }
            int rs = await _productDAL.Update(data);

            //更新协议物模型缓存
            var product = new MZ_IotProduct();
            product.Version = data.Version != null ? data.Version : old.Version;
            product.ModelTSL = data.ModelTSL != null ? data.ModelTSL : old.ModelTSL;
            product.StorageConfig = data.StorageConfig != null ? data.StorageConfig : old.StorageConfig;
            product.InterScripts = data.InterScripts != null ? data.InterScripts : old.InterScripts;
            product.Status = data.Status != null ? data.Status : old.Status;
            product.NetworkWay = data.NetworkWay != null ? data.NetworkWay : old.NetworkWay;
            product.Id = data.Id;
            product.OrgId = old.OrgId;
            await _provider.GetService<ServerBusProxy>().DownUpdateProductSys(product);

            if (bpublic)
            {
                if (!string.IsNullOrEmpty(product.NetworkWay))
                {
                    var channelConfig = await GetChannel(product.NetworkWay);
                    if (channelConfig.CanBind)
                    {
                        if (old.PublicTime == null || old.TSLUpdated == null || old.PublicTime < old.TSLUpdated)
                        {
                            //在线设备添加到待同步更新表
                            var updateDAL = _provider.GetService<IotUpdateDAL>();
                            await updateDAL.InsertProductUpdate(data.Id, data.Version.Value, 10, product.OrgId.Value);
                        }

                    }

                }

            }

            return BusResponse<int>.Success(rs);
        }

        public virtual async Task RefreshAllProductCache(IUserInfo user)
        {
            var prolist = await _productDAL.SelectList(x => x.OrgId == user.OrgId);
            foreach (var pro in prolist)
            {
                await _provider.GetService<ServerBusProxy>().DownUpdateProductSys(pro);
            }
        }
        public virtual async Task<BusResponse<string>> Insert(MZ_IotProduct data)
        {
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(111, "非企业用户无法添加协议");
            }
            if (string.IsNullOrEmpty(data.ModelTSL))
            {
                var tmpmodel = new TslModel();
                tmpmodel.properties = new List<BaseProperty>();
                tmpmodel.functions = new List<BaseFunc>();
                tmpmodel.events = new List<BaseEvent>();
                tmpmodel.tags = new List<BaseTagInfo>();
                data.ModelTSL = System.Text.Json.JsonSerializer.Serialize(tmpmodel, TslModel.TSLOptions);
            }
            data.NoticeWay ??= string.Empty;
            data.PhotoUrl ??= string.Empty;
            data.Remark ??= string.Empty;
            data.Id = _snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            data.SetCreateBy(user);
            data.Status = "0";
            data.Version = 0;
            data.MonitorReportToken ??= string.Empty;
            data.TSLUpdated = DateTime.Now;

            if (string.IsNullOrEmpty(data.NetworkWay))
            {
                data.InterScripts = string.Empty;
            }
            else
            {
                var channelConfig = await GetChannel(data.NetworkWay);
                if (channelConfig != null && channelConfig.CanModbus)
                {
                    var model = TslModel.CreateFrom(data.ModelTSL);
                    if (model != null && model.modbus == null)
                    {
                        model.modbus = new ModbusInfo();
                        model.modbus.BaudRate = 9600;
                        model.modbus.DataBits = 8;
                        model.modbus.Parity = "0";
                        model.modbus.StopBits = "1";
                        model.modbus.Mode = "RTU";
                        model.modbus.PollTime = 5000;
                        model.modbus.Matches = new List<ModbusMatch>();
                        data.ModelTSL = System.Text.Json.JsonSerializer.Serialize(model, TslModel.TSLOptions);
                    }
                }
            }
            await _productDAL.Insert(data);

            //更新协议物模型缓存
            await _provider.GetService<ServerBusProxy>().DownUpdateProductSys(data);

            return BusResponse<string>.Success(data.Id);
        }

        public virtual async Task<BusResponse<int>> Remove(string id)
        {
            var old = await _productDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(114, "协议不存在");
            }
            if (await _deviceDAL.Some(x => x.ProductId == id))
            {
                return BusResponse<int>.Error(115, "无法删除存在设备的协议");
            }
            IotRedisHelper redis = _provider.GetService<IotRedisHelper>();
            int rs = await _productDAL.Delete(id);
            await redis.KeyDeleteAsync("ProductSys:" + id);
            await _provider.GetService<ServerBusProxy>().PublishKeyDel("ProductSys:" + id);
            return BusResponse<int>.Success(rs);
        }

        public virtual async Task<BusResponse<string>> Copy(string id)
        {
            var old = await _productDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(114, "协议不存在");
            }
            var newProduct = old.Copy();
            if (newProduct == null)
            {
                return BusResponse<string>.Error(115, "拷贝失败");
            }
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);
            newProduct.Id = _snowflake.NextId().ToString();
            newProduct.Name = $"{old.Name} - 副本";
            newProduct.SetCreateBy(user);
            newProduct.Status = "0";
            newProduct.Version = 0;
            newProduct.PublicTime = null;
            await _productDAL.Insert(newProduct);
            //更新协议物模型缓存
            await _provider.GetService<ServerBusProxy>().DownUpdateProductSys(newProduct);

            //拷贝属性规则
            var iotWinRuleDAL = _provider.GetService<IotWinRuleDAL>();
            var twrlist = await iotWinRuleDAL.SelectList(x => x.ProductId == old.Id);
            foreach (var twitem in twrlist)
            {
                twitem.Id = _snowflake.NextId().ToString();
                twitem.ProductId = newProduct.Id;
            }
            if (twrlist.Count > 0)
            {
                await iotWinRuleDAL.Insert(twrlist);
            }

            return BusResponse<string>.Success(newProduct.Id);
        }
    }
}
