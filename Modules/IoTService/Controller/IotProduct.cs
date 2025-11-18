using AuthService.Controller;
using ChannelUtility.Config;
using Common;
using Common.Share;
using IoTService.Business;
using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace IoTService.Controller
{
    /// <summary>
    /// 物联协议接口
    /// </summary>
    public class IotProduct : AbstractLoginedController
    {
        private IotDeviceBLL _device;
        private IotProductBLL _product;
        private IotCodeBLL _code;
        private IotWinRuleBLL _winRule;

        public IotProduct(IotProductBLL product, IotDeviceBLL device, IotCodeBLL code, IotWinRuleBLL winRule)
        {
            _device = device;
            _product = product;
            _code = code;
            _winRule = winRule;
        }
        /// <summary>
        /// 获取协议名称列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<Out_ProductName>>> ProductNames(In_ProductNamePage query)
        {
            IotProductBLL productBLL = this.ServiceProvider.GetService<IotProductBLL>();
            return this.Success(await productBLL.ProductNamePage(query, GetUser()));
        }
        /// <summary>
        /// 册除指定设备的历史信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotProduct/ListPage")]
        public async Task<AjaxResult> DebugDelHistory(In_HistoryDelete data)
        {
            return (await this.ServiceProvider.GetService<IotInfluxBLL>().DeleteHistory(data)).ToAjaxResult();
        }
        /// <summary>
        /// 发送modbus调试
        /// </summary>
        /// <param name="data">参数</param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotProduct/ListPage")]
        public async Task<AjaxResult> DebugModbus(In_DebugModbus data)
        {
            var device = await _device.Info(data.DeviceId);
            if (device == null)
            {
                return this.Error<string>(12, "设备不存在");
            }
            if (string.IsNullOrEmpty(device.ProductId))
            {
                return this.Error<string>(13, "设备未绑定协议");
            }

            var sbp = this.ServiceProvider.GetService<ServerBusProxy>();
            var product = await _product.Info(device.ProductId);
            await sbp.DownUpdateProductSys(product);
            await sbp.DownModbusMessage(device.ProductId, device.DeviceId, product.NetworkWay, data.MatchName);
            return this.Success<string>();
        }
        /// <summary>
        /// 发送读属性调试
        /// </summary>
        /// <param name="data">参数</param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotProduct/ListPage")]
        public async Task<AjaxResult> DebugProperty(In_DebugProperty data)
        {
            var device = await _device.Info(data.DeviceId);
            if (device == null)
            {
                return this.Error<string>(12, "设备不存在");
            }
            if (string.IsNullOrEmpty(device.ProductId))
            {
                return this.Error<string>(13, "设备未绑定协议");
            }
            var sbp = this.ServiceProvider.GetService<ServerBusProxy>();
            var product = await _product.Info(device.ProductId);
            await sbp.DownUpdateProductSys(product);
            await sbp.WaitDownReadProperty(device.ProductId, device.DeviceId, product.NetworkWay, data.Properties);
            return this.Success<string>();
        }
        /// <summary>
        /// 发送执行功能调试
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotProduct/ListPage")]
        public async Task<AjaxResult> DebugFunc(In_DebugFunction data)
        {
            var device = await _device.Info(data.DeviceId);
            if (device == null)
            {
                return this.Error<string>(12, "设备不存在");
            }
            if (string.IsNullOrEmpty(device.ProductId))
            {
                return this.Error<string>(13, "设备未绑定协议");
            }
            var sbp = this.ServiceProvider.GetService<ServerBusProxy>();
            var product = await _product.Info(device.ProductId);
            await sbp.DownUpdateProductSys(product);
            await sbp.DownFunction(device.ProductId, device.DeviceId, product.NetworkWay, data.FunctionId, data.Inputs);
            return this.Success<string>();
        }
        /// <summary>
        /// 发送执行事件调试
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotProduct/ListPage")]
        public async Task<AjaxResult> DebugEvent(In_DebugEvent data)
        {
            var device = await _device.Info(data.DeviceId);
            if (device == null)
            {
                return this.Error<string>(12, "设备不存在");
            }
            if (string.IsNullOrEmpty(device.ProductId))
            {
                return this.Error<string>(13, "设备未绑定协议");
            }
            var sbp = this.ServiceProvider.GetService<ServerBusProxy>();
            var product = await _product.Info(device.ProductId);
            await sbp.DownUpdateProductSys(product);
            await sbp.SendEvent(device.ProductId, device.DeviceId, data.EventId, data.Inputs ?? new Dictionary<string, object>());
            return this.Success<string>();
        }
        /// <summary>
        /// 发送文本到设备
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotProduct/ListPage")]
        public async Task<AjaxResult> DebugText(In_DebugText data)
        {
            var device = await _device.Info(data.DeviceId);
            if (device == null)
            {
                return this.Error<string>(12, "设备不存在");
            }
            if (string.IsNullOrEmpty(device.ProductId))
            {
                return this.Error<string>(13, "设备未绑定协议");
            }

            if (data.IsHex)
            {
                byte[] bytes;
                try
                {
                    bytes = MyAccess.Core.Utility.strToToHexByte(data.Text);
                }
                catch
                {
                    return this.Error<string>(14, "HEX格式错误");
                }
                var tsl = await TslCache.GetTslModel(device.ProductId, this.ServiceProvider);
                await this.ServiceProvider.GetService<ServerBusProxy>().DownRawData(device.ProductId, device.DeviceId, tsl.NetworkWay, bytes);
            }
            else
            {
                var tsl = await TslCache.GetTslModel(device.ProductId, this.ServiceProvider);
                await this.ServiceProvider.GetService<ServerBusProxy>().DownRawData(device.ProductId, device.DeviceId, tsl.NetworkWay, Encoding.UTF8.GetBytes(data.Text.Replace("\n", "\r\n")));
            }
            return this.Success<string>();
        }
        /// <summary>
        /// 发送特殊消息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotProduct/ListPage")]
        public async Task<AjaxResult> DebugMsg(In_DebugMsg data)
        {
            var device = await _device.Info(data.DeviceId);
            if (device == null)
            {
                return this.Error<string>(12, "设备不存在");
            }
            if (string.IsNullOrEmpty(device.ProductId))
            {
                return this.Error<string>(13, "设备未绑定协议");
            }
            var tsl = await TslCache.GetTslModel(device.ProductId, this.ServiceProvider);
            switch (data.MsgType)
            {
                case "Bind":
                    {
                        await this.ServiceProvider.GetService<ServerBusProxy>().DownBind(device.ProductId, device.DeviceId, tsl.NetworkWay);
                    }
                    break;
                case "QueryICCID":
                    {
                        await this.ServiceProvider.GetService<ServerBusProxy>().DownICCID(device.ProductId, device.DeviceId, tsl.NetworkWay);
                    }
                    break;
                case "Connect":
                    {
                        await this.ServiceProvider.GetService<ServerBusProxy>().SendConnect(device.ProductId, device.DeviceId);
                    }
                    break;
                case "Disconnect":
                    {
                        await this.ServiceProvider.GetService<ServerBusProxy>().SendDisconnect(device.ProductId, device.DeviceId);
                    }
                    break;
            }

            return this.Success<string>();
        }
        /// <summary>
        /// 获取接入通道配置列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTService/IotProduct/ListPage")]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<List<Out_ChannelInfo>>))]
        public async Task<AjaxResult> ChannelList()
        {
            return this.Success(await _product.GetChannelList());
        }
        /// <summary>
        /// 通过接入方式获取通道配置信息
        /// </summary>
        /// <param name="code">接入方式的代码</param>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTService/IotProduct/ListPage")]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<ChannelConfig>))]
        public async Task<AjaxResult> Channel(string code = "")
        {
            return this.Success(await _product.GetChannel(code));
        }
        /// <summary>
        /// 协议列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [About]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<PageObject<MZ_IotProduct>>))]
        public async Task<AjaxResult> ListPage(In_ProductListPage query)
        {
            return this.Success(await _product.ListPage(query));
        }
        /// <summary>
        /// 批量获取物模型
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTService/IotProduct/ListPage")]
        public async Task<DefaultAjaxResult<List<MZ_IotProduct>>> TSLList(string[] ids)
        {
            return this.Success(await _product.TSLList(ids));
        }
        /// <summary>
        /// 获取指定协议信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="notsl">是否不返回物模型信息（加速用）</param>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<MZ_IotProduct>))]
        public async Task<AjaxResult> Info(string id, bool notsl = false)
        {
            return this.Success(await _product.Info(id, notsl));
        }
        /// <summary>
        /// 添加协议
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotProduct/ListPage")]
        [Des("新增一条协议记录")]
        public async Task<AjaxResult> Add(MZ_IotProduct data)
        {
            return (await _product.Insert(data)).ToAjaxResult();
        }
        /// <summary>
        /// 编辑协议
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotProduct/ListPage")]
        public async Task<AjaxResult> Edit(MZ_IotProduct data)
        {
            return (await _product.Update(data)).ToAjaxResult();
        }
        /// <summary>
        /// 删除协议
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTService/IotProduct/ListPage")]
        [Des("删除一条协议记录")]
        public async Task<AjaxResult> Remove(string id)
        {
            return (await _product.Remove(id)).ToAjaxResult();
        }
        /// <summary>
        /// 拷贝协议
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTService/IotProduct/ListPage")]
        public async Task<AjaxResult> Copy(string id)
        {
            return (await _product.Copy(id)).ToAjaxResult();
        }


        /// <summary>
        /// 标识符模板树
        /// </summary>
        /// <param name="t">0为属性、1为功能</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> CodeListTree(int t)
        {
            return (await _code.ListTree(t)).ToAjaxResult();
        }

        /// <summary>
        /// 属性规则列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_IotWinRule>>> PropRuleList(string id)
        {
            return this.Success(await _winRule.SelectList(id, GetUser()));
        }

        /// <summary>
        /// 获取属性规则信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_IotWinRule>> PropRuleInfo(string id)
        {
            return (await _winRule.Info(id)).ToAjaxResult();
        }

        /// <summary>
        /// 添加属性规则
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> AddPropRule(MZ_IotWinRule data)
        {
            return (await _winRule.Insert(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 修改属性规则
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> EditPropRule(MZ_IotWinRule data)
        {
            return (await _winRule.Update(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 删除属性规则
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> RemovePropRule(string[] ids)
        {
            return (await _winRule.Delete(ids, GetUser())).ToAjaxResult();
        }
    }
}
