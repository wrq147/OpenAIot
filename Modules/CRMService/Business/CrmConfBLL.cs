using AuthService;
using AuthService.DAL;
using AuthService.Model;
using Common.Share;
using CRMService.Model;
using Microsoft.Extensions.Options;
using MyAccess.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace CRMService.Business
{
    public class CrmConfBLL
    {
        private IOptions<GeneralOption> _conf;
        private ITAServiceProvider _provider;
        private OperatorHelper _operator;
        private OrgExtDAL _orgExtDAL;
        public CrmConfBLL(ITAServiceProvider provider, IOptions<GeneralOption> conf, OperatorHelper operatorHelper, OrgExtDAL orgExtDAL)
        {
            _provider = provider;
            _conf = conf;
            _operator = operatorHelper;
            _orgExtDAL = orgExtDAL;
        }
   
        [Trans]
        public virtual async Task<BusResponse<int>> SetCrm(MZ_CrmConf conf)
        {
            var user = _provider.GetUser();

            //跨阶段推进
            MZ_OrgExt EnableKPer = new MZ_OrgExt()
            {
                OrgId = user.OrgId,
                ExtField = "EnableKPer",
                ExtValue = conf.EnableKPer.ToString()
            };
            await _orgExtDAL.InsertOrUpdate(EnableKPer);

            //进行中阶段回退
            MZ_OrgExt EnableIngBack = new MZ_OrgExt()
            {
                OrgId = user.OrgId,
                ExtField = "EnableIngBack",
                ExtValue = conf.EnableIngBack.ToString()
            };
            await _orgExtDAL.InsertOrUpdate(EnableIngBack);

            //终点阶段回退
            MZ_OrgExt EnableWinBack = new MZ_OrgExt()
            {
                OrgId = user.OrgId,
                ExtField = "EnableWinBack",
                ExtValue = conf.EnableWinBack.ToString()
            };
            await _orgExtDAL.InsertOrUpdate(EnableWinBack);


            //设置几天未跟进将被回收，为0不回收
            MZ_OrgExt FollowReturnDay = new MZ_OrgExt()
            {
                OrgId = user.OrgId,
                ExtField = "FollowReturnDay",
                ExtValue = conf.FollowReturnDay.ToString()
            };
            await _orgExtDAL.InsertOrUpdate(FollowReturnDay);


            return BusResponse<int>.Success();
        }

        public virtual async Task<MZ_CrmConf> CrmInfo(long id)
        {
            var tlist = await _orgExtDAL.SelectList(x => x.OrgId == id);
            Dictionary<string, MZ_OrgExt> dict = new Dictionary<string, MZ_OrgExt>();
            foreach (var t in tlist)
            {
                dict.Add(t.ExtField, t);
            }

            MZ_CrmConf conf = new MZ_CrmConf();
            MZ_OrgExt ext;
            if (dict.TryGetValue("EnableKPer", out ext))
            {
                conf.EnableKPer = Convert.ToBoolean(ext.ExtValue);
            }
            else
            {
                conf.EnableKPer = true;
            }
            if (dict.TryGetValue("EnableIngBack", out ext))
            {
                conf.EnableIngBack = Convert.ToBoolean(ext.ExtValue);
            }
            else
            {
                conf.EnableIngBack = true;
            }
            if (dict.TryGetValue("EnableWinBack", out ext))
            {
                conf.EnableWinBack = Convert.ToBoolean(ext.ExtValue);
            }
            else
            {
                conf.EnableWinBack = true;
            }
            if (dict.TryGetValue("FollowReturnDay", out ext))
            {
                conf.FollowReturnDay = Convert.ToInt32(ext.ExtValue);
            }


            return conf;
        }
    }
}
