using Common.IdGenerator;
using Common.Share;
using EfficiencyService.DAL;
using EfficiencyService.Model.Org;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace EfficiencyService.Business
{
    public class OrgConfBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private OrgConfDAL _orgConfDAL;

        public OrgConfBLL(ITAServiceProvider provider, OrgConfDAL orgConfDAL, SnowflakeHelper snowflake)
        {
            _provider = provider;
            _snowflake = snowflake;
            _orgConfDAL = orgConfDAL;
        }


        public virtual async Task<BusResponse<int>> SaveOrgConf(T_OrgConf data, IUserInfo user)
        {
            data.Id = user.OrgId;
            var rs = await _orgConfDAL.CreateOrUpdate(data);
            return BusResponse<int>.Success(rs);
        }
        public virtual async Task<BusResponse<T_OrgConf>> GetOrgConf(IUserInfo user)
        {
            var tmpOrgConf = await _orgConfDAL.Select(user.OrgId);
            if (tmpOrgConf == null)
            {
                tmpOrgConf = new T_OrgConf();
                tmpOrgConf.Id = user.OrgId;
                tmpOrgConf.EmissionSourceJson = string.Empty;
                tmpOrgConf.PricePolicyId = string.Empty;
                tmpOrgConf.ContectName = string.Empty;
                tmpOrgConf.ContectTel = string.Empty;
                tmpOrgConf.ElecCode = string.Empty;
                tmpOrgConf.ElecPowerCode = string.Empty;
            }
            return BusResponse<T_OrgConf>.Success(tmpOrgConf);
        }
    }
}
