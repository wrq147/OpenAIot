using AuthService.Controller;
using AuthService.Model;
using Common.IdGenerator;
using Common.Share;
using MyAccess.Aop;
using MyAccess.DB.Builder.WhereToSql;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TemplateAction.Core;

namespace AuthService
{
    public class StyleBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private StyleDAL _styleDAL;
        public StyleBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, StyleDAL styleDAL)
        {
            _provider = provider;
            _snowflake = snowflake;
            _styleDAL = styleDAL;
        }

        public virtual async Task<PageObject<MZ_AppStyle>> SelectList(In_StyleList query)
        {
            var user = _provider.GetUser();
            Expression<Func<MZ_AppStyle, bool>> expression = x => true;
            if (!string.IsNullOrEmpty(query.Name))
            {
                expression = x => x.Name.Contains(query.Name);
            }
            if (query.OrgId != null)
            {
                string isorgStyle = "Id in (select StyleId from mz_org_style where OrgId=" + query.OrgId + ")";
                expression = expression.And(x => SonSqlFun.SqlCondition(isorgStyle));
            }
            return await _styleDAL.SelectPage(expression, query, "create_time desc");
        }
        public virtual async Task<PageObject<MZ_Org>> StyleOrgList(In_StyleOrgList query)
        {
            var orgStyleDAL = _provider.GetService<OrgStyleDAL>();
            return await orgStyleDAL.StyleOrgList(query);
        }
        public virtual async Task<BusResponse<string>> Add(MZ_AppStyle data)
        {
            var user = _provider.GetUser();
            data.Id = _snowflake.NextId().ToString();
            data.SetCreateBy(user);
            await _styleDAL.Insert(data);
            return BusResponse<string>.Success(data.Id);
        }


        public virtual async Task<BusResponse<string>> Edit(MZ_AppStyle data)
        {
            var user = _provider.GetUser();
            data.SetUpdateBy(user);
            await _styleDAL.Update(data);
            return BusResponse<string>.Success();
        }

        public virtual async Task<BusResponse<string>> Delete(string id)
        {
            var orgStyleDAL = _provider.GetService<OrgStyleDAL>();
            if (await orgStyleDAL.Some(x => x.StyleId == id))
            {
                return BusResponse<string>.Error(111, "无法删除,主题已被使用");
            }
            await _styleDAL.Delete(id);
            return BusResponse<string>.Success();
        }
        public virtual async Task<BusResponse<MZ_AppStyle>> Info(string id)
        {
            var info = await _styleDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_AppStyle>.Error(111, "主题不存在");
            }

            return BusResponse<MZ_AppStyle>.Success(info);
        }
        public virtual async Task<BusResponse<MZ_AppStyle>> OrgStyle(long orgId)
        {
            var orgStyleDAL = _provider.GetService<OrgStyleDAL>();
            var info = await orgStyleDAL.OrgStyleById(orgId);
            if (info == null)
            {
                return BusResponse<MZ_AppStyle>.Success(null);
            }
            return BusResponse<MZ_AppStyle>.Success(info);
        }
        public virtual async Task<BusResponse<string>> SetOrgStyle(MZ_OrgStyle orgStyle)
        {
            orgStyle.FrowWay = 2;
            orgStyle.CreatedOn = DateTime.Now;
            orgStyle.IsUsing = true;

            var orgStyleDAL = _provider.GetService<OrgStyleDAL>();
            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    await orgStyleDAL.ClearUsing(orgStyle.OrgId.Value);
                    await orgStyleDAL.CreateOrUpdate(orgStyle);

                    // 完成
                    await scope.CompleteAsync();
                }

                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }

        public virtual async Task<BusResponse<string>> DelOrgStyle(long orgId, string styleId)
        {
            var orgStyleDAL = _provider.GetService<OrgStyleDAL>();
            await orgStyleDAL.Delete(x => x.OrgId == orgId && x.StyleId == styleId);
            return BusResponse<string>.Success();
        }
    }
}
