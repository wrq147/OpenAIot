using Common.Share;
using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.Third
{
    /// <summary>
    /// 第三方接口
    /// </summary>
    public interface IThirdApi
    {
        /// <summary>
        /// 查询单卡信息
        /// </summary>
        /// <param name="iccid"></param>
        /// <returns></returns>
        Task<MZ_IotCard> QueryCardInfo(string iccid);
        /// <summary>
        /// 批量查询卡信息
        /// </summary>
        /// <param name="iccids"></param>
        /// <returns></returns>
        Task<List<MZ_IotCard>> QueryCardInfoList(List<string> iccids);
        /// <summary>
        /// 单卡套餐续费
        /// </summary>
        /// <param name="card"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        Task<BusResponse<string>> Recharge(MZ_IotCard card, int month);


        /// <summary>
        /// 批量办理物联卡停机
        /// </summary>
        /// <param name="msisdns"></param>
        /// <param name="iccids"></param>
        /// <returns></returns>
        Task<BusResponse<string>> StopSimStatusBatch(List<string> msisdns, List<string> iccids);
    }
}
