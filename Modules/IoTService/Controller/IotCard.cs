using AuthService.Controller;
using Common.Share;
using IoTService.Business;
using IoTService.Models;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using Common;
using System.Collections.Generic;
using System.IO;
using TemplateAction.Label;

namespace IoTService.Controller
{
    public class IotCard : AbstractLoginedController
    {
        private IotCardBLL _cardBLL;
        public IotCard(IotCardBLL cardBLL)
        {
            _cardBLL = cardBLL;
        }

        /// <summary>
        /// 物联卡列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [About]
        public async Task<DefaultAjaxResult<PageObject<MZ_IotCard>>> ListPage(In_IotCardListPage query)
        {
            return this.Success(await _cardBLL.ListPage(query, GetUser()));
        }
        /// <summary>
        /// 获取物联卡详情数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTService/IotDevice/ListPage")]
        public async Task<DefaultAjaxResult<MZ_IotCard>> Info(string id)
        {
            return this.Success(await _cardBLL.Info(id));
        }
        /// <summary>
        /// 同步并获取最新物联卡数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTService/IotDevice/ListPage")]
        public async Task<AjaxResult> SyncCard(string id)
        {
            return (await _cardBLL.QueryNewestInfo(id)).ToAjaxResult();
        }
        /// <summary>
        /// 手动停卡
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTService/IotDevice/ListPage")]
        public async Task<AjaxResult> StopCard(string id)
        {
            return (await _cardBLL.StopCard(id)).ToAjaxResult();
        }

        /// <summary>
        /// 物联卡解绑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotDevice/ListPage")]
        public async Task<AjaxResult> UnUsing(string id)
        {
            return (await _cardBLL.UnUsing(id)).ToAjaxResult();
        }

        /// <summary>
        /// 物联卡绑定设备
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotDevice/ListPage")]
        public async Task<AjaxResult> UsingDevice(In_UsingDevice data)
        {
            return (await _cardBLL.UsingDevice(data.Id, data.devId)).ToAjaxResult();
        }
        /// <summary>
        /// 物联卡续费
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTService/IotDevice/ListPage")]
        public async Task<AjaxResult> Recharge(In_RechargeData data)
        {
            return (await _cardBLL.Recharge(data.Ids, data.Month)).ToAjaxResult();
        }
        /// <summary>
        /// 删除物联卡
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTService/IotDevice/ListPage")]
        public async Task<AjaxResult> Remove(string[] ids)
        {
            return (await _cardBLL.Remove(ids, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 导入物联卡
        /// </summary>
        /// <param name="cardFrom"></param>
        /// <returns></returns>
        [About("/IoTService/IotDevice/ListPage")]
        [HttpPost]
        public async Task<AjaxResult> Import(string cardFrom)
        {
            var user = GetUser();
            var form = await this.Context.Request.ReadFormAsync();
            if (form.Files.Length == 0)
            {
                return this.Error<string>(12, "请选择文件");
            }
            var file = form.Files[0];
            return (await _cardBLL.Import(cardFrom, file, user)).ToAjaxResult();
        }

        /// <summary>
        /// 导出模板
        /// </summary>
        /// <returns></returns>
        [About("/IoTService/IotDevice/ListPage")]
        [HttpGet]
        public IResult ExportTemplate()
        {
            try
            {
                Dictionary<string, ParamRenderToExcel<MZ_IotCard>> FiedNames = new Dictionary<string, ParamRenderToExcel<MZ_IotCard>>();
                FiedNames.Add("ICCID", new ParamRenderToExcel<MZ_IotCard>("ICCID号"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<MZ_IotCard>("物联网卡导入模板", new List<MZ_IotCard>(), FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }
    }
}
