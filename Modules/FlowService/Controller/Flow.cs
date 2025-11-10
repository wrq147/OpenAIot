using AuthService.Controller;
using Common;
using Common.DataAc;
using Common.EventBus;
using Common.Share;
using FlowService.Business;
using FlowService.FlowNode.FormFields;
using FlowService.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace FlowService.Controller
{
    /// <summary>
    /// 流程模板接口
    /// </summary>
    [About("/FlowService/Flow")]
    public class Flow : AbstractLoginedController
    {
        private FlowBLL _flowBLL;
        public Flow(FlowBLL flowBLL)
        {
            _flowBLL = flowBLL;
        }

        /// <summary>
        ///生成唯一Id
        /// </summary>
        /// <returns></returns>
        [About("List")]
        [HttpGet]
        public AjaxResult GenerateId()
        {
            return _flowBLL.GenerateId().ToAjaxResult();
        }
        /// <summary>
        /// 获取指定流程模板
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info(long id)
        {
            return (await _flowBLL.GetFlowTemplate(id)).ToAjaxResult();
        }
        /// <summary>
        /// 发布流程模板
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [About("List")]
        [HttpPost]
        public async Task<AjaxResult> SaveFlowDetail(MZ_FlowTemplate template)
        {
            if (template.Id == null)
            {
                return (await _flowBLL.AddFlowDetail(template)).ToAjaxResult();
            }
            else
            {
                return (await _flowBLL.UpdateFlowDetail(template)).ToAjaxResult();
            }

        }
        /// <summary>
        /// 删除流程模板
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("List")]
        [HttpGet]
        public async Task<AjaxResult> Remove(long id)
        {
            return (await _flowBLL.DeleteFlowTemplate(id)).ToAjaxResult();
        }
        /// <summary>
        /// 获取流程记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About("List")]
        [HttpGet]
        [HttpPost]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<PageObject<Out_FlowRecord>>))]
        public async Task<AjaxResult> GetFlowRecord(In_FlowRecord query)
        {
            return this.Success(await _flowBLL.GetFlowRecord(query, GetUser()));
        }
        /// <summary>
        /// 获取可变动数据列表
        /// </summary>
        /// <returns></returns>
        [About("List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<List<DA_Table>>> DataList()
        {
            var redis = this.ServiceProvider.GetService<GeneralRedisHelper>();
            var alldict = await redis.HashGetAllAsync<List<DA_Table>>("BusChange-Event");
            List<DA_Table> list = new List<DA_Table>();
            foreach (var item in alldict)
            {
                list.AddRange(item.Value);
            }
            return this.Success(list);
        }


        /// <summary>
        /// 导出指定模板的流程记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About("List")]
        [HttpGet]
        public async Task<IResult> ExportRecord(In_FlowRecord query)
        {
            try
            {
                if (query.TemplateId <= 0)
                {
                    return this.Error<string>(16, "需要参数TemplateId");
                }
                var template = await _flowBLL.GetFlowTemplate(query.TemplateId);
                FormField[] fields = JsonConvert.DeserializeObject<FormField[]>(template.Data.Form.FormFields, new JsonFieldConvert());
                List<FormField> fieldlist = new List<FormField>();
                foreach (var ff in fields)
                {
                    ff.FindTo(x => x.props.enablePrint, fieldlist);
                }
                query.pageSize = 0;
                query.showAll = true;
                PageObject<Out_FlowRecord> page = await _flowBLL.GetFlowRecord(query, GetUser());
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("流程编号"));
                dt.Columns.Add(new DataColumn("提交时间"));
                dt.Columns.Add(new DataColumn("完成时间"));
                dt.Columns.Add(new DataColumn("申请人"));
                dt.Columns.Add(new DataColumn("部门"));
                foreach (var ff in fieldlist)
                {
                    dt.Columns.Add(new DataColumn(ff.title + "(" + ff.id + ")"));
                }
                dt.Columns.Add(new DataColumn("流程状态"));
                foreach (var item in page.List)
                {
                    var tmpdr = dt.NewRow();
                    tmpdr["流程编号"] = item.Id;
                    tmpdr["提交时间"] = item.create_time?.ToString("yyyy-MM-dd HH:mm:ss");
                    tmpdr["完成时间"] = item.FinishTime?.ToString("yyyy-MM-dd HH:mm:ss");
                    tmpdr["申请人"] = item.RealName;
                    tmpdr["部门"] = item.dept_name;

                    if (item.DataItems != null)
                    {
                        foreach (var ff in fieldlist)
                        {
                            string tmprs;
                            if (item.DataItems.TryGetValue(ff.id, out tmprs))
                            {
                                tmpdr[ff.title + "(" + ff.id + ")"] = tmprs;
                            }
                        }
                    }


                    switch (item.Status)
                    {
                        case FlowStatus.Complete:
                            tmpdr["流程状态"] = "已完成";
                            break;
                        case FlowStatus.Runnable:
                            tmpdr["流程状态"] = "运行中";
                            break;
                        case FlowStatus.Suspended:
                            tmpdr["流程状态"] = "保存中";
                            break;
                        case FlowStatus.Terminated:
                            tmpdr["流程状态"] = "已取消";
                            break;
                    }
                    dt.Rows.Add(tmpdr);
                }


                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer(template.Data.Name, dt);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }
    }
}
