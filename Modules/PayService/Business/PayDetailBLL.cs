using Common.IdGenerator;
using Common.Share;
using PayService.DAL;
using PayService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace PayService.Business
{
    public class PayDetailBLL
    {
        private readonly ITAServiceProvider _provider;
        private readonly PayDetailDAL _payDetailDAL;
        private readonly SnowflakeHelper _snowflake;

        public PayDetailBLL(ITAServiceProvider provider, PayDetailDAL payDetailDAL, SnowflakeHelper snowflake)
        {
            _provider = provider;
            _payDetailDAL = payDetailDAL;
            _snowflake = snowflake;
        }

        /// <summary>
        /// 分页查询支付记录
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>分页结果</returns>
        public virtual async Task<PageObject<MZ_PayDetail>> ListAsync(In_PayDetailList query, IUserInfo user)
        {
            return await _payDetailDAL.ListAsync(query);
        }

        /// <summary>
        /// 获取单条支付记录
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <returns>支付记录</returns>
        public virtual async Task<MZ_PayDetail> InfoAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id), "支付记录ID不能为空");
            }
            return await _payDetailDAL.Select(id);
        }

        /// <summary>
        /// 新增支付记录
        /// </summary>
        /// <param name="data">支付记录信息</param>
        /// <param name="user">当前用户</param>
        /// <returns>业务响应</returns>
        public virtual async Task<BusResponse<string>> AddAsync(MZ_PayDetail data, IUserInfo user)
        {
            // 参数校验
            if (data == null)
            {
                return BusResponse<string>.Error(201, "支付记录信息不能为空");
            }
            if (data.OrgId <= 0)
            {
                return BusResponse<string>.Error(202, "所属企业ID不能为空");
            }
            if (data.PayAmount <= 0)
            {
                return BusResponse<string>.Error(203, "支付金额必须大于0");
            }

            // 通用字段赋值
            data.Id = _snowflake.NextId().ToString();
            data.PayUser = user.UserId;

            // 数据库插入
            int rows = await _payDetailDAL.Insert(data);
            return rows > 0 ? BusResponse<string>.Success(data.Id) : BusResponse<string>.Error(204, "新增支付记录失败");
        }

        /// <summary>
        /// 更新支付记录状态
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <param name="status">状态</param>
        /// <param name="errorMsg">错误信息</param>
        /// <returns>业务响应</returns>
        public virtual async Task<BusResponse<bool>> UpdateStatusAsync(string id, int status, string errorMsg = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                return BusResponse<bool>.Error(205, "支付记录ID不能为空");
            }
            if (!new[] { 0, 1, 2, 3, 4, 5 }.Contains(status))
            {
                return BusResponse<bool>.Error(206, "支付状态不合法");
            }

            var data = new MZ_PayDetail
            {
                Id = id,
                Status = status,
                ErrorMsg = errorMsg
            };

            int rows = await _payDetailDAL.Update(data);
            return rows > 0 ? BusResponse<bool>.Success(true) : BusResponse<bool>.Error(207, "更新支付状态失败");
        }
    }
}