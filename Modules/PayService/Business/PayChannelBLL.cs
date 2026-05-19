using Common.IdGenerator;
using Common.Share;
using PayService.DAL;
using PayService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayService.Business
{
    public class PayChannelBLL
    {
        private readonly PayChannelDAL _payChannelDAL;
        private readonly SnowflakeHelper _snowflake;

        public PayChannelBLL(PayChannelDAL payChannelDAL, SnowflakeHelper snowflake)
        {
            _payChannelDAL = payChannelDAL;
            _snowflake = snowflake;
        }

        /// <summary>
        /// 分页查询支付渠道
        /// </summary>
        /// <param name="query"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<PageObject<MZ_PayChannel>> ListAsync(In_PayChannelList query, IUserInfo user)
        {
            return await _payChannelDAL.ListAsync(query);
        }

        /// <summary>
        /// 获取单条支付渠道
        /// </summary>
        /// <param name="id">渠道ID</param>
        /// <returns>支付渠道</returns>
        public virtual async Task<MZ_PayChannel> InfoAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id), "支付渠道ID不能为空");
            }
            return await _payChannelDAL.Select(id);
        }

        /// <summary>
        /// 新增支付渠道
        /// </summary>
        /// <param name="data">渠道信息</param>
        /// <returns>业务响应</returns>
        public virtual async Task<BusResponse<string>> AddAsync(MZ_PayChannel data)
        {
            // 参数校验
            if (data == null)
            {
                return BusResponse<string>.Error(211, "支付渠道信息不能为空");
            }
            if (string.IsNullOrEmpty(data.ChannelName))
            {
                return BusResponse<string>.Error(212, "渠道名称不能为空");
            }
            if (string.IsNullOrEmpty(data.ChannelLabel))
            {
                return BusResponse<string>.Error(213, "渠道标识符不能为空");
            }
            if (string.IsNullOrEmpty(data.AppId))
            {
                return BusResponse<string>.Error(214, "商户AppId不能为空");
            }

            // 唯一性校验：同一企业下渠道标识不能重复
            var existChannel = await _payChannelDAL.GetByLabelAsync(data.OrgId, data.ChannelLabel);
            if (existChannel != null)
            {
                return BusResponse<string>.Error(215, $"当前企业已存在{data.ChannelLabel}渠道");
            }

            // 通用字段赋值
            data.Id = _snowflake.NextId().ToString();

            int rows = await _payChannelDAL.Insert(data);
            return rows > 0 ? BusResponse<string>.Success(data.Id) : BusResponse<string>.Error(216, "新增支付渠道失败");
        }

        /// <summary>
        /// 编辑支付渠道
        /// </summary>
        /// <param name="data">渠道信息</param>
        /// <returns>业务响应</returns>
        public virtual async Task<BusResponse<bool>> EditAsync(MZ_PayChannel data)
        {
            if (data == null || string.IsNullOrEmpty(data.Id))
            {
                return BusResponse<bool>.Error(217, "渠道ID不能为空");
            }

            int rows = await _payChannelDAL.Update(data);
            return rows > 0 ? BusResponse<bool>.Success(true) : BusResponse<bool>.Error(218, "编辑支付渠道失败");
        }
    }
}
