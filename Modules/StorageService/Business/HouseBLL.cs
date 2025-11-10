using AuthService;
using Common.IdGenerator;
using Common.Share;
using StorageService.DAL;
using StorageService.Model;
using FlowService.DAL;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using TemplateAction.Core;
using ProducerService.DAL;
using ProducerService.Model;

namespace StorageService.Business
{
    public class HouseBLL
    {
        private IOptions<GeneralOption> _conf;
        private ITAServiceProvider _provider;
        private StoreHouseDAL _storeHouseDAL;
        private SnowflakeHelper _snowflake;
        public HouseBLL(ITAServiceProvider provider, IOptions<GeneralOption> conf, StoreHouseDAL storeHouseDAL, SnowflakeHelper snowflake)
        {
            _provider = provider;
            _conf = conf;
            _storeHouseDAL = storeHouseDAL;
            _snowflake = snowflake;
        }
        public virtual async Task<PageObject<MZ_StoreHouse>> SelectList(In_HouseList query)
        {
            var user = _provider.GetUser();
            var scope = await user.GetScope(_provider);
            return await _storeHouseDAL.SelectByPage(query, scope, user);
        }
        public virtual async Task<BusResponse<MZ_StoreHouse>> Info(string id, bool showTemplateName)
        {
            var info = await _storeHouseDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_StoreHouse>.Error(111, "仓库不存在");
            }
            if (showTemplateName)
            {
                var userDAL = _provider.GetService<UserDAL>();
                info.LeaderName = string.Empty;
                if (info.LeaderId > 0)
                {
                    var leader = await userDAL.GetAdminById(info.LeaderId.Value);
                    if (leader != null)
                    {
                        info.LeaderName = leader.RealName;
                    }
                }



                var templateDAL = _provider.GetService<FlowTemplateDAL>();
                if (info.LeaveTemplateId > 0)
                {
                    info.LeaveTemplateName = (await templateDAL.SelecFlowTemplateById(info.LeaveTemplateId.Value))?.Name;
                }
                if (info.EnterTemplateId > 0)
                {
                    info.EnterTemplateName = (await templateDAL.SelecFlowTemplateById(info.EnterTemplateId.Value))?.Name;
                }
                if (info.LeaveApplyTemplateId > 0)
                {
                    info.LeaveApplyTemplateName = (await templateDAL.SelecFlowTemplateById(info.LeaveApplyTemplateId.Value))?.Name;
                }
            }


            return BusResponse<MZ_StoreHouse>.Success(info);
        }
        public virtual async Task<BusResponse<string>> Add(MZ_StoreHouse data)
        {
            var user = _provider.GetUser();
            data.Id = _snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            data.IsSystem = 0;
            data.Status = "1";
            data.del_flag = "0";

            data.LeaveFlowInitJson = data.LeaveFlowInitJson == null ? string.Empty : data.LeaveApplyFlowInitJson;
            data.EnterFlowInitJson = data.EnterFlowInitJson == null ? string.Empty : data.EnterFlowInitJson;
            data.LeaveApplyFlowInitJson = data.LeaveApplyFlowInitJson == null ? string.Empty : data.LeaveApplyFlowInitJson;


            if (data.LeaderId == null)
            {
                data.LeaderId = 0;
                data.DeptId = 0;
            }
            else
            {
                if (data.LeaderId <= 0)
                {
                    return BusResponse<string>.Error(121, "请选择正确的负责人");
                }
                var userOrg = await _provider.GetService<AuthService.OrgDAL>().SelectUserOrg(data.LeaderId.Value, user.OrgId);
                if (userOrg == null)
                {
                    data.DeptId = 0;
                }
                else
                {
                    data.DeptId = userOrg.dept_id;
                }

            }

            await _storeHouseDAL.Insert(data);
            return BusResponse<string>.Success(data.Id);
        }
        public virtual async Task<BusResponse<string>> Edit(MZ_StoreHouse data)
        {
            MZ_StoreHouse old = await _storeHouseDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "仓库不存在");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }
            if (data.LeaderId != null)
            {
                if (data.LeaderId > 0)
                {
                    var userOrg = await _provider.GetService<AuthService.OrgDAL>().SelectUserOrg(data.LeaderId.Value, user.OrgId);
                    data.DeptId = userOrg.dept_id;
                }
                else
                {
                    data.LeaderId = 0;
                    data.DeptId = 0;
                }
            }


            data.OrgId = null;
            data.IsSystem = null;
            data.Status = null;
            data.del_flag = null;
            await _storeHouseDAL.Update(data);
            return BusResponse<string>.Success();
        }

        public virtual async Task<BusResponse<string>> Delete(string id)
        {
            var user = _provider.GetUser();
            MZ_StoreHouse old = await _storeHouseDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "仓库不存在");
            }

            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }

            if (old.IsSystem == 1)
            {
                return BusResponse<string>.Error(125, "系统仓库不可删除");
            }

            MZ_StoreHouse data = new MZ_StoreHouse();
            data.Id = id;
            data.del_flag = "2";
            await _storeHouseDAL.Update(data);
            return BusResponse<string>.Success();
        }

        public virtual async Task<BusResponse<string>> ChangeStaus(string id, string status)
        {
            MZ_StoreHouse old = await _storeHouseDAL.Select(id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "仓库不存在");
            }
            if (old.del_flag != "0")
            {
                return BusResponse<string>.Error(125, "仓库状态错误");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }

            if (old.IsSystem == 1)
            {
                return BusResponse<string>.Error(126, "系统仓库无法更改状态");
            }

            MZ_StoreHouse data = new MZ_StoreHouse();
            data.Id = id;
            data.Status = status == "0" ? "0" : "1";
            await _storeHouseDAL.Update(data);
            return BusResponse<string>.Success();
        }

        public virtual async Task AddDefHouse(long factoryId)
        {

            if (!await _storeHouseDAL.Some(x => x.OrgId == factoryId && x.IsSystem == 1))
            {
                //新增仓库
                MZ_StoreHouse house = new MZ_StoreHouse();
                house.Id = _snowflake.NextId().ToString();
                house.OrgId = factoryId;
                house.IsSystem = 1;
                house.StoreName = "默认仓库";
                house.Remark = string.Empty;
                house.Status = "1";
                house.del_flag = "0";
                house.LeaderId = 0;
                house.DeptId = 0;
                house.LeaveTemplateId = 0;
                house.EnterTemplateId = 0;
                house.LeaveApplyTemplateId = 0;
                house.EnterFlowInitJson = string.Empty;
                house.LeaveFlowInitJson = string.Empty;
                house.LeaveApplyFlowInitJson = string.Empty;
                await _storeHouseDAL.Insert(house);
            }


        }


        public virtual async Task JoinByOtherMod(Tmp_JoinEventData data)
        {

            if (!await _storeHouseDAL.Some(x => x.OrgId == data.OrgId && x.IsSystem == 1))
            {
                //新增仓库
                MZ_StoreHouse house = new MZ_StoreHouse();
                house.Id = _snowflake.NextId().ToString();
                house.OrgId = data.OrgId;
                house.IsSystem = 1;
                house.StoreName = "默认仓库";
                house.Remark = string.Empty;
                house.Status = "1";
                house.del_flag = "0";
                house.LeaderId = 0;
                house.DeptId = 0;
                house.LeaveTemplateId = 0;
                house.EnterTemplateId = 0;
                house.LeaveApplyTemplateId = 0;
                house.LeaveFlowInitJson = string.Empty;
                house.EnterFlowInitJson = string.Empty;
                house.LeaveApplyFlowInitJson = string.Empty;
                await _storeHouseDAL.Insert(house);
            }

        }
    }
}
