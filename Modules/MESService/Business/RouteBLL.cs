using AuthService;
using Common.IdGenerator;
using Common.Share;
using MESService.DAL;
using MESService.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MESService.Business
{
    public class RouteBLL
    {
        private ITAServiceProvider _provider;
        private RouteDAL _routeDAL;
        public RouteBLL(ITAServiceProvider provider, RouteDAL routeDAL)
        {
            _provider = provider;
            _routeDAL = routeDAL;
        }

        public virtual async Task<PageObject<MZ_ProductRoute>> SelectList(In_RouteList query, IUserInfo user)
        {
            var pagelist = await _routeDAL.SelectByPage(query, user.OrgId);
            var ids = pagelist.List.Select(x => x.Id).ToList();
            if (ids.Count > 0)
            {
                var routeOpers = await _provider.GetService<RouteOperDAL>().SelectList(x => ids.Contains(x.RouteId));
                foreach (var routeOper in routeOpers)
                {
                    var routeItem = pagelist.List.Where(x => x.Id == routeOper.RouteId).FirstOrDefault();
                    if (routeItem != null)
                    {
                        if (routeItem.Items == null)
                        {
                            routeItem.Items = new List<MZ_ProductRouteOper>();
                        }
                        routeItem.Items.Add(routeOper);
                    }
                }
            }

            return pagelist;
        }
        public virtual async Task<BusResponse<MZ_ProductRoute>> Info(string id)
        {
            var info = await _routeDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_ProductRoute>.Error(111, "工艺路线不存在");
            }
            var routeOpers = await _provider.GetService<RouteOperDAL>().SelectList(x => x.RouteId == id);
            info.Items = new List<MZ_ProductRouteOper>();
            info.Items.AddRange(routeOpers);
            return BusResponse<MZ_ProductRoute>.Success(info);
        }


        public virtual async Task<BusResponse<string>> Add(MZ_ProductRoute data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(133, "请切换到企业账号");
            }
            var snowflake = _provider.GetService<SnowflakeHelper>();
            data.Id = snowflake.NextId().ToString();
            data.OrgId = user.OrgId;

            if (string.IsNullOrEmpty(data.RouteName))
            {
                return BusResponse<string>.Error(111, "名称不能为空");
            }
            data.SetCreateBy(user);
            await _routeDAL.Insert(data);
            if (data.Items != null && data.Items.Count > 0)
            {
                var routeOperDAL = _provider.GetService<RouteOperDAL>();
                foreach (var item in data.Items)
                {
                    item.Id = snowflake.NextId().ToString();
                    item.OrgId = user.OrgId;
                    item.RouteId = data.Id;
                }
                await routeOperDAL.Insert(data.Items);
            }
            return BusResponse<string>.Success(data.Id);
        }

        public virtual async Task<BusResponse<int>> Edit(MZ_ProductRoute data, IUserInfo user)
        {
            var old = await _routeDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(111, "工艺路线不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(112, "所属组织错误");
            }

            data.OrgId = null;
            data.SetUpdateBy(user);
            var rs = await _routeDAL.Update(data);
            var routeOperDAL = _provider.GetService<RouteOperDAL>();
            await routeOperDAL.Delete(x => x.RouteId == data.Id);
            var snowflake = _provider.GetService<SnowflakeHelper>();
            if (data.Items != null && data.Items.Count > 0)
            {
                foreach (var item in data.Items)
                {
                    item.Id = snowflake.NextId().ToString();
                    item.OrgId = user.OrgId;
                    item.RouteId = data.Id;
                }
                await routeOperDAL.Insert(data.Items);
            }

            return BusResponse<int>.Success(rs);
        }

        public virtual async Task<BusResponse<int>> Delete(string id, IUserInfo user)
        {
            MZ_ProductRoute old = await _routeDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(111, "工艺路线不存在");
            }
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<int>.Error(112, "所属组织错误");
            }
            var rs = await _routeDAL.Delete(id);
            var routeOperDAL = _provider.GetService<RouteOperDAL>();
            await routeOperDAL.Delete(x => x.RouteId == id);
            return BusResponse<int>.Success(rs);
        }

        /// <summary>
        /// 预计工艺路线所需工时（分钟）
        /// </summary>
        /// <param name="routeId">工艺路线</param>
        /// <param name="num">数量</param>
        /// <returns></returns>
        public virtual async Task<decimal> PredictedWorkTime(string routeId, int num)
        {
            decimal needWorkTime = 0;
            var routeOperDAL = _provider.GetService<RouteOperDAL>();
            var operList = await routeOperDAL.SelectList(x => x.RouteId == routeId);
            foreach (var oper in operList)
            {
                needWorkTime += oper.WorkTime.Value * oper.PropOf.Value * num;
            }
            return needWorkTime;
        }
    }
}
