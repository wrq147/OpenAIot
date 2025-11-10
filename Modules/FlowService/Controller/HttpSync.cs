using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Share;
using Common;
using DeveloperService;
using FlowService.FlowNode.FormFields;
using FlowService.Model;
using Newtonsoft.Json;
using TemplateAction.Route;
using FlowService.Business;
using TemplateAction.Core;
using TemplateAction.Label;
using DeveloperService.Model;
using AuthService;
namespace FlowService.Controller
{
    public class HttpSync : AbstractDeveloperController
    {
        private MZ_Developer _develper;
        private FlowBLL _flowBLL;
        public HttpSync(FlowBLL flowBLL)
        {
            _flowBLL = flowBLL;
        }
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
        /// 流程记录接口（开发者用）
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [ShareCheck]
        public async Task<DefaultAjaxResult<DataTable>> FlowRecords(In_FlowRecord query)
        {
            try
            {
                var template = await _flowBLL.GetFlowTemplate(query.TemplateId);
                FormField[] fields = JsonConvert.DeserializeObject<FormField[]>(template.Data.Form.FormFields, new JsonFieldConvert());
                List<FormField> fieldlist = new List<FormField>();
                foreach (var ff in fields)
                {
                    ff.FindTo(x => x.props.enablePrint, fieldlist);
                }

                PageObject<Out_FlowRecord> page = await _flowBLL.GetFlowRecord(query, _develper.ToUserInfo());
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

                return this.Success(dt);
            }
            catch (Exception ex)
            {
                return this.Error<DataTable>(12, ex.Message);
            }
        }
    }
}
