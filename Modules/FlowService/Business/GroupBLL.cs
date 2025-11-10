using AuthService;
using Common.Share;
using FlowService.DAL;
using FlowService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace FlowService.Business
{
    public class GroupBLL
    {
        private GroupDAL _group;
        private FlowTemplateDAL _template;
        private ITAContext _context;
        public GroupBLL(GroupDAL group, FlowTemplateDAL template, ITAContext context)
        {
            _group = group;
            _template = template;
            _context = context;
        }
        public async Task<BusResponse<int>> UpdateGroupSort(List<long> list)
        {
            return BusResponse<int>.Success(await _group.UpdateGroupSort(list));
        }

        public async Task<List<MZ_FlowGroup>> SelectGroupList(In_GroupList query)
        {
            query.orgId = Data_ServerTokenInfo.From(_context).OrgId;
            //获取分组
            var grouplist = await _group.SelectGroupList(query);
            if (query.withItems == true)
            {
                //其它分组
                MZ_FlowGroup other = new MZ_FlowGroup();
                other.Name = "其它";
                other.Id = 0;
                other.Items = new List<MZ_FlowTemplate>();
                other.Sort = 98;
                //停用分组
                MZ_FlowGroup stoped = new MZ_FlowGroup();
                stoped.Name = "停用";
                stoped.Id = 0;
                stoped.Items = new List<MZ_FlowTemplate>();
                stoped.Sort = 99;
                //获取所有模板
                var templist = await _template.SelectAll(query.orgId.Value);
                //组合数据
                Dictionary<long, MZ_FlowGroup> gphs = new Dictionary<long, MZ_FlowGroup>();
                foreach (var gitem in grouplist)
                {
                    gitem.Items = new List<MZ_FlowTemplate>();
                    gphs.Add(gitem.Id.Value, gitem);
                }
                grouplist.Add(other);
                grouplist.Add(stoped);
                foreach (var temp in templist)
                {
                    if (temp.Status == "1")
                    {
                        stoped.Items.Add(temp);
                    }
                    else
                    {
                        MZ_FlowGroup g;
                        if (gphs.TryGetValue(temp.GroupId.Value, out g))
                        {
                            g.Items.Add(temp);
                        }
                        else
                        {
                            other.Items.Add(temp);
                        }
                    }
                }
            }


            return grouplist;
        }
        public async Task<BusResponse<long>> InsertGroup(MZ_FlowGroup g)
        {
            if (g.Sort == null)
            {
                g.Sort = 0;
            }
            g.OrgId = Data_ServerTokenInfo.From(_context).OrgId;
            await _group.SortIncrease(g.OrgId.Value, (int)g.Sort);
            return BusResponse<long>.Success(await _group.InsertGroup(g));
        }



        public async Task<BusResponse<int>> UpdateGroup(MZ_FlowGroup g)
        {
            MZ_FlowGroup old = await _group.SelecById(g.Id.Value);
            if (old == null)
            {
                return BusResponse<int>.Error(23, "指定模板组不存在");
            }
            var curOrgId = Data_ServerTokenInfo.From(_context).OrgId;
            if (old.OrgId != curOrgId)
            {
                return BusResponse<int>.Error(24, "无权操作当前模板组");
            }
            if (g.Sort != null)
            {
                await _group.SortIncrease(old.OrgId.Value, (int)g.Sort);
            }
            return BusResponse<int>.Success(await _group.UpdateGroup(g));
        }

        public async Task<BusResponse<int>> DeleteGroup(long id)
        {
            MZ_FlowGroup old = await _group.SelecById(id);
            if (old == null)
            {
                return BusResponse<int>.Error(23, "指定模板组不存在");
            }
            var curOrgId = Data_ServerTokenInfo.From(_context).OrgId;
            if (old.OrgId != curOrgId)
            {
                return BusResponse<int>.Error(24, "无权操作当前模板组");
            }
            return BusResponse<int>.Success(await _group.DeleteGroupById(id));
        }
    }
}
