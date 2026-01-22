using AuthService;
using Common.IdGenerator;
using Common.Share;
using FlowService.DAL;
using FlowService.FlowNode;
using FlowService.FlowNode.FormFields;
using FlowService.Model;
using Microsoft.AspNetCore.Mvc.Formatters;
using MyAccess.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace FlowService.Business
{
    public class FlowBLL
    {
        private FlowTemplateDAL _template;
        private SnowflakeHelper _snowflake;
        private FormDAL _form;
        private FormDataDAL _formData;
        private FlowTemplateLinkDAL _templateLinkDAL;
        private FlowService.DAL.OrgDAL _org;
        private ITAContext _context;
        public FlowBLL(FlowTemplateDAL template, FormDAL form, FlowTemplateLinkDAL templateLinkDAL, PermissionDAL permDAL,
            SnowflakeHelper snowflake, FlowService.DAL.OrgDAL org, FormDataDAL formData, ITAContext context)
        {
            _template = template;
            _form = form;
            _templateLinkDAL = templateLinkDAL;
            _snowflake = snowflake;
            _org = org;
            _context = context;
            _formData = formData;
        }

        public virtual BusResponse<long> GenerateId()
        {
            return BusResponse<long>.Success(_snowflake.NextId());
        }
        [Trans]
        public virtual async Task<BusResponse<string>> DeleteFlowTemplate(long id)
        {

            try
            {
                MZ_FlowTemplate template = await _template.SelecFlowTemplateById(id);
                if (template == null)
                {
                    return BusResponse<string>.Error(23, "指定模板不存在");
                }
                var curOrgId = Data_ServerTokenInfo.From(_context).OrgId;
                if (template.OrgId != curOrgId)
                {
                    return BusResponse<string>.Error(24, "无权删除当前模板");
                }
                var rt = await _template.DeleteFlowTemplateAsync(id);
                if (template.FormId != null && template.FormId > 0)
                {
                    await _form.DeleteFormAsync(template.FormId.Value);
                }
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(22, ex.Message);
            }

        }

        public virtual async Task<BusResponse<MZ_FlowTemplate>> GetFlowTemplate(long id)
        {
            MZ_FlowTemplate template = await _template.SelecFlowTemplateById(id);
            if (template == null)
            {
                return BusResponse<MZ_FlowTemplate>.Error(21, "流程模板不存在");
            }
            if (template.FormId != null)
            {
                template.Form = await _form.SelecFormById(template.FormId.Value);
            }
            return BusResponse<MZ_FlowTemplate>.Success(template);
        }

        [Trans]
        public virtual async Task<BusResponse<long>> AddFlowDetail(MZ_FlowTemplate template)
        {
            try
            {
                template.Id = _snowflake.NextId();
                template.OrgId = Data_ServerTokenInfo.From(_context).OrgId;
                if (template.Form == null)
                {
                    return BusResponse<long>.Error(21, "流程的表单参数不能为null");
                }
                template.Sort = template.Sort ?? 0;
                template.Form.Id = _snowflake.NextId();
                template.FormId = template.Form.Id;
                template.Status = "0";
                template.del_flag = "0";
                //设置允许提交的人员
                RootNode rootNode = System.Text.Json.JsonSerializer.Deserialize<RootNode>(template.FlowJson, FlowJsonSerializerConfig.NodeOptions);
                ObjData[] commitObjs = rootNode.props.assignedUser;
                if (commitObjs.Length == 0)
                {
                    MZ_Dept dept = await _org.SelectTopDeptById(template.OrgId.Value);
                    ObjData tmpdata = new ObjData();
                    tmpdata.id = dept.dept_id.Value;
                    tmpdata.type = "dept";
                    tmpdata.name = string.Empty;
                    commitObjs = new ObjData[1] { tmpdata };
                }
                await _templateLinkDAL.SetFlowTemplateLinks(template.Id.Value, commitObjs);

                await _form.InsertFormAsync(template.Form);
                await _template.InsertFlowTemplateAsync(template, Data_ServerTokenInfo.From(_context));

                return BusResponse<long>.Success(template.Id.Value);
            }
            catch (Exception ex)
            {
                return BusResponse<long>.Error(22, ex.Message);
            }
        }

        [Trans]
        public virtual async Task<BusResponse<int>> UpdateFlowDetail(MZ_FlowTemplate template)
        {
            try
            {
                template.FormId = null;
                MZ_FlowTemplate oldTemp = await _template.SelecFlowTemplateById(template.Id.Value);
                if (oldTemp == null)
                {
                    return BusResponse<int>.Error(23, "指定模板不存在");
                }
                var curOrgId = Data_ServerTokenInfo.From(_context).OrgId;
                if (oldTemp.OrgId != curOrgId)
                {
                    return BusResponse<int>.Error(24, "无权修改当前模板");
                }

                if (template.Form != null)
                {
                    template.Form.Id = oldTemp.FormId;
                    await _form.UpdateFormAsync(template.Form);
                }
                if (!string.IsNullOrEmpty(template.FlowJson))
                {
                    //设置允许提交的人员
                    RootNode rootNode = System.Text.Json.JsonSerializer.Deserialize<RootNode>(template.FlowJson, FlowJsonSerializerConfig.NodeOptions);
                    ObjData[] commitObjs = rootNode.props.assignedUser;
                    if (commitObjs.Length == 0)
                    {
                        MZ_Dept dept = await _org.SelectTopDeptById(oldTemp.OrgId.Value);
                        ObjData tmpdata = new ObjData();
                        tmpdata.id = dept.dept_id.Value;
                        tmpdata.type = "dept";
                        tmpdata.name = string.Empty;
                        commitObjs = new ObjData[1] { tmpdata };
                    }
                    await _templateLinkDAL.SetFlowTemplateLinks(template.Id.Value, commitObjs);
                }

                template.OrgId = null;

                return BusResponse<int>.Success(await _template.UpdateFlowTemplateAsync(template, Data_ServerTokenInfo.From(_context)));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(22, ex.Message);
            }
        }

        public virtual async Task<PageObject<Out_FlowRecord>> GetFlowRecord(In_FlowRecord query, IUserInfo user)
        {
            if (query.Depts != null && query.Depts.Length > 0)
            {
                var deptlist = await _org.SelectAllDeptIn(query.Depts, user.OrgId);
                query.Depts = deptlist.ToArray();

            }
            var tform = await _form.SelectFormByTemplateId(query.TemplateId);
            FormField[] fields = JsonConvert.DeserializeObject<FormField[]>(tform.FormFields, new JsonFieldConvert());
            var fieldDict = FormField.ToFieldDict(fields);
            var tlist = await _template.GetFlowRecord(query, fieldDict);
            List<long> flowIds = tlist.List.Select(x => x.Id.Value).ToList();
            if (flowIds.Count <= 0)
            {
                return PageObject<Out_FlowRecord>.Empty();
            }
            var dataItems = await _formData.SelecFormDataByIds(flowIds);
            if (query.showAll)
            {
                Dictionary<long, Dictionary<string, string>> tmpdict = new Dictionary<long, Dictionary<string, string>>();
                foreach (var flowId in flowIds)
                {
                    var tmpdatas = dataItems.Where(x => x.FlowId == flowId);
                    foreach (var fkvp in fieldDict)
                    {
                        var vals = tmpdatas.Where(x => x.FieldId == fkvp.Value.id).FirstOrDefault();
                        if (vals != null)
                        {
                            Dictionary<string, string> dictrs;
                            if (!tmpdict.TryGetValue(flowId, out dictrs))
                            {
                                dictrs = new Dictionary<string, string>();
                                tmpdict.Add(flowId, dictrs);
                            }


                            object tmpval = fkvp.Value.FromSave(vals);
                            if (tmpval == null)
                            {
                                continue;
                            }
                            if (tmpval is Array arr)
                            {
                                dictrs.Add(fkvp.Value.id, string.Join(',', arr));
                            }
                            else
                            {
                                dictrs.Add(fkvp.Value.id, tmpval.ToString());
                            }
                        }
                    }
                }
                foreach (var titem in tlist.List)
                {
                    Dictionary<string, string> dictrs;
                    if (tmpdict.TryGetValue(titem.Id.Value, out dictrs))
                    {
                        titem.DataItems = dictrs;
                    }
                }
            }
            else
            {

                foreach (var titem in tlist.List)
                {
                    titem.DataItems = new Dictionary<string, string>();
                    var tmpdatas = dataItems.Where(x => x.FlowId == titem.Id);

                    foreach (var fkvp in fieldDict)
                    {
                        var vals = tmpdatas.Where(x => x.FieldId == fkvp.Value.id).FirstOrDefault();
                        if (vals != null)
                        {
                            object tmpval = fkvp.Value.FromSave(vals);
                            if (tmpval == null)
                            {
                                continue;
                            }
                            if (tmpval is Array arr)
                            {
                                titem.DataItems.Add(fkvp.Value.id, string.Join(',', arr));
                            }
                            else
                            {
                                titem.DataItems.Add(fkvp.Value.id, tmpval.ToString());
                            }
                        }
                    }
                }
            }

            return tlist;
        }
    }
}
