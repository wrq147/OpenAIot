using AuthService;
using AuthService.DAL;
using AuthService.Model;
using Common.IdGenerator;
using Common.Share;
using IoTService.DAL;
using IoTService.Models;
using MyAccess.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.Business
{
    public class IotWarnConfigBLL
    {
        private ITAServiceProvider _provider;
        private IotWarnConfigDAL _warnConfigDAL;
        public IotWarnConfigBLL(ITAServiceProvider provider, IotWarnConfigDAL warnConfigDAL)
        {
            _provider = provider;
            _warnConfigDAL = warnConfigDAL;
        }
        public virtual async Task<BusResponse<int>> SetWarnConfig(MZ_IotWarnConfig conf, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法配置告警");
            }
            if (await _warnConfigDAL.Some(x => x.ProductId == conf.ProductId))
            {
                MZ_IotWarnConfig newconfig = new MZ_IotWarnConfig();
                newconfig.WarnFlowId = conf.WarnFlowId;
                newconfig.WarnFlowName = conf.WarnFlowName;
                newconfig.WarnFlowInitJson = conf.WarnFlowInitJson;
                await _warnConfigDAL.Update(newconfig, x => x.ProductId == conf.ProductId);
            }
            else
            {
                MZ_IotWarnConfig newconfig = new MZ_IotWarnConfig();
                var snowflake = _provider.GetService<SnowflakeHelper>();
                newconfig.Id = snowflake.NextId().ToString();
                newconfig.OrgId = user.OrgId;
                newconfig.ProductId = conf.ProductId;
                newconfig.WarnFlowId = conf.WarnFlowId;
                newconfig.WarnFlowName = conf.WarnFlowName;
                newconfig.WarnFlowInitJson = conf.WarnFlowInitJson;
                await _warnConfigDAL.Insert(newconfig);
            }

            return BusResponse<int>.Success();
        }
        public virtual async Task<MZ_IotWarnConfig> WarnConfigInfoByPro(string pid)
        {
            var tmplist = await _warnConfigDAL.SelectList(x => x.ProductId == pid);
            if (tmplist.Count > 0)
            {
                return tmplist[0];
            }
            else
            {
                return null;
            }
        }

    }
}
