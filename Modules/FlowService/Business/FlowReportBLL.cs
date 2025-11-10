using AuthService;
using FlowService.DAL;
using FlowService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;

namespace FlowService.Business
{
    public class FlowReportBLL
    {
        private ITAServiceProvider _provider;
        private FlowDAL _flowDAL;
        public FlowReportBLL(ITAServiceProvider provider, FlowDAL flowDAL)
        {
            _provider = provider;
            _flowDAL = flowDAL;
        }

        public async Task<Out_FlowStatistics> StatisticsInfo()
        {
            var user = _provider.GetUser();
            Out_FlowStatistics res = await _flowDAL.GetMyStatistics(user);
            if (res == null)
            {
                res = new Out_FlowStatistics();
            }
            res.MyFlowCount = res.MyWaitCount + res.MyDoingCount + res.MyFinishCount + res.MyCancelCount;
            res.PendingCount = await _flowDAL.GetPendingCount(user);
            res.CopyCount = await _flowDAL.GetCopyCount(user);
            res.FinishCount = await _flowDAL.GetFinishCount(user);
            return res;
        }
    }
}
