using AuthService;
using Common.Share;
using FlowService.DAL;
using MESService.DAL;
using MESService.Model;
using ProducerService.DAL;
using ProducerService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MESService.Business
{
    public class ConfigBLL
    {
        private ITAServiceProvider _provider;
        public ConfigBLL(ITAServiceProvider provider)
        {
            _provider = provider;
        }

        public virtual async Task<BusResponse<string>> SetFactoryMes(MZ_FactoryMes data)
        {
            try
            {
                var mesDAL = _provider.GetService<FactoryMesDAL>();
                if (data.PlanTemplateId == null)
                {
                    data.PlanTemplateId = 0;
                }
                if (data.ReportTemplateId == null)
                {
                    data.ReportTemplateId = 0;
                }
                data.PlanFlowInitJson ??= string.Empty;
                data.ReportFlowInitJson ??= string.Empty;
                await mesDAL.CreateOrUpdate(data);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }
        public virtual async Task<BusResponse<MZ_FactoryMes>> Info(long orgId)
        {
            var mesDAL = _provider.GetService<FactoryMesDAL>();
            var info = await mesDAL.Select(orgId);
            if (info == null)
            {
                info = new MZ_FactoryMes();
                info.Id = orgId;
                info.PlanTemplateId = 0;
                info.PlanTemplateName = string.Empty;
                info.PlanFlowInitJson = string.Empty;
                info.ReportTemplateId = 0;
                info.ReportTemplateName = string.Empty;
                info.ReportFlowInitJson = string.Empty;
            }
            else
            {
                var templateDAL = _provider.GetService<FlowTemplateDAL>();
                if (info.PlanTemplateId > 0)
                {
                    info.PlanTemplateName = (await templateDAL.SelecFlowTemplateById(info.PlanTemplateId.Value))?.Name;
                }
                else
                {
                    info.PlanTemplateName = string.Empty;
                }

                if (info.ReportTemplateId > 0)
                {
                    info.ReportTemplateName = (await templateDAL.SelecFlowTemplateById(info.ReportTemplateId.Value))?.Name;
                }
                else
                {
                    info.ReportTemplateName = string.Empty;
                }
            }
            return BusResponse<MZ_FactoryMes>.Success(info);
        }
    }
}
