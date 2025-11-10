using StorageService.Business;
using StorageService.Model;
using DeveloperService;
using TemplateAction.Core;
using TemplateAction.Route;
using Common;
using Common.Share;
using TemplateAction.Label;
using DeveloperService.Model;
using ProducerService.Business;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace StorageService.Controller
{
    /// <summary>
    /// 同步设备入库
    /// </summary>
    public class HttpSync : AbstractDeveloperController
    {
        private MZ_Developer _develper;
        /// <summary>
        /// 校验开发者权限
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override async Task<IResult> CallAction(TAAction ac, object[] parameters)
        {
            _develper = GetDeveloper();
            if (_develper.UserType != 1)
            {
                return this.Error<string>(11, "必需为企业开发者");
            }
            return await base.CallAction(ac, parameters);
        }

        /// <summary>
        /// 物品入库
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddPileIn(In_Pile data)
        {
            if (string.IsNullOrEmpty(data.StockNumber))
            {
                return this.Error<string>(12, "请传入第三方编码");
            }
            if (data.StockNumber.StartsWith("RK"))
            {
                return this.Error<string>(13, "第三方编码不可使用RK开头");
            }
            var user = _develper.ToUserInfo();
            StockBLL stockBLL = this.ServiceProvider.GetService<StockBLL>();
            ProductBatchBLL proBatchBLL = this.ServiceProvider.GetService<ProductBatchBLL>();
            In_ManualStock ipt = new In_ManualStock();
            ipt.StockNumber = data.StockNumber;
            ipt.ExpressNumber = string.Empty;
            ipt.ExpressCompany = string.Empty;
            ipt.ExpressPhone = string.Empty;
            ipt.HouseId = data.ToHouseId;
            ipt.InDate = data.InDate;
            ipt.Remark = data.Remark ?? string.Empty;
            ipt.List = new List<MZ_EnterDetail>();
            foreach (var item in data.Items)
            {
                MZ_EnterDetail detail = new MZ_EnterDetail();
                detail.Quantity = item.Quantity;
                detail.Price = item.Price;
                var batchInfo = await proBatchBLL.SelectVByNumber(user.OrgId, item.TargetNumber);
                if (batchInfo == null)
                {
                    return this.Error<string>(32, "第三方编码不存在");
                }
                detail.TargetType = batchInfo.ProductLabel == "F" ? 1 : 0;
                detail.TargetId = batchInfo.Id;
                ipt.List.Add(detail);
            }

            var brs = await stockBLL.ManualPile(ipt, user);
            if (brs.IsSuccess())
            {
                var submitRs = await stockBLL.SubmitManualPile(user, brs.Data, 0);
                if (submitRs.IsSuccess())
                {
                    return this.Success(brs.Data);
                }
                else
                {
                    return this.Error<string>(11, submitRs.Message);
                }
            }
            else
            {
                return brs.ToAjaxResult();
            }
        }

        /// <summary>
        /// 物品出库
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddPileOut(In_OutPile data)
        {
            if (string.IsNullOrEmpty(data.StockNumber))
            {
                return this.Error<string>(12, "请传入第三方编码");
            }
            if (data.StockNumber.StartsWith("CK"))
            {
                return this.Error<string>(13, "第三方编码不可使用CK开头");
            }
            var user = _develper.ToUserInfo();
            StockBLL stockBLL = this.ServiceProvider.GetService<StockBLL>();
            ProductBatchBLL proBatchBLL = this.ServiceProvider.GetService<ProductBatchBLL>();
            MZ_LeaveStock leave = new MZ_LeaveStock();
            leave.StockNumber = data.StockNumber;
            leave.ExpressNumber = data.ExpressNumber;
            leave.ExpressPhone = data.ExpressPhone;
            leave.ExpressCompany = data.ExpressCompany;

            var agent = await this.ServiceProvider.GetService<AgentBLL>().Info(data.ToAgentId);
            if (agent == null)
            {
                return this.Error<string>(31, "代理商不存在");
            }
            leave.ToOrgId = agent.OrgId;
            leave.FromHouseId = data.FromHouseId;
            leave.LeaveMethod = 0;
            leave.OutDate = data.OutDate;
            leave.Remark = data.Remark ?? string.Empty;
            leave.List = new List<MZ_LeaveDetail>();
            foreach (var item in data.Items)
            {
                MZ_LeaveDetail detail = new MZ_LeaveDetail();
                detail.Quantity = item.Quantity;
                detail.Price = item.Price;
                var batchInfo = await proBatchBLL.SelectVByNumber(user.OrgId, item.TargetNumber);
                if (batchInfo == null)
                {
                    return this.Error<string>(32, "第三方编码不存在");
                }
                detail.TargetType = batchInfo.ProductLabel == "F" ? 1 : 0;
                detail.TargetId = batchInfo.Id;
                leave.List.Add(detail);
            }

            var brs = await stockBLL.AddLeave(leave, user);
            if (brs.IsSuccess())
            {
                var submitRs = await stockBLL.Submit(user, brs.Data, 0);
                if (submitRs.IsSuccess())
                {
                    return this.Success(brs.Data);
                }
                else
                {
                    return this.Error<string>(11, submitRs.Message);
                }
            }
            else
            {
                return brs.ToAjaxResult();
            }
        }

        /// <summary>
        /// 清除指定入库单号（注：将清除入库单关联物品的所有库存信息、记录信息和关联的出入库单据）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> ClearPile(string number)
        {
            var user = _develper.ToUserInfo();
            return (await this.ServiceProvider.GetService<StockBLL>().ClearPile(number, user)).ToAjaxResult();
        }


    }
}
