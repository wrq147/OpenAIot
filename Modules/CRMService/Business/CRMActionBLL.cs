using AuthService;
using Common.IdGenerator;
using Common.Share;
using CRMService.DAL;
using MyAccess.Aop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using AuthService.DAL;
using Common.DataAc;
using Microsoft.Extensions.Logging;

namespace CRMService.Business
{
    public class CRMActionBLL
    {
        private ITAServiceProvider _provider;
        private CRMActionDAL _crmActionDAL;
        private SnowflakeHelper _snowflake;
        private ILogger<CRMActionBLL> _log;
        public CRMActionBLL(ITAServiceProvider provider, CRMActionDAL crmActionDAL, SnowflakeHelper snowflake,ILoggerFactory factory)
        {
            _provider = provider;
            _crmActionDAL = crmActionDAL;
            _snowflake = snowflake;
            _log = factory.CreateLogger<CRMActionBLL>();
        }
        /// <summary>
        /// 未跟进定时转到公海
        /// </summary>
        /// <returns></returns>
        public virtual async Task NoFollowToPublic()
        {
            var extList = await _provider.GetService<OrgExtDAL>().SelectList(x => x.ExtField == "FollowReturnDay" && x.ExtValue != "0");
            var customerDAL = _provider.GetService<CustomerDAL>();
            var clueDAL = _provider.GetService<ClueDAL>();
            foreach (var ext in extList)
            {
                int day = Convert.ToInt32(ext.ExtValue);
                if (day < 0)
                {
                    continue;
                }
                //未跟进客户转到公海
                await customerDAL.AutoToPublic(ext.OrgId.Value, day);
                //未跟进线索转到公海
                await clueDAL.AutoToPublic(ext.OrgId.Value, day);
            }
            //清除联系人的负责人
            await _provider.GetService<ContactDAL>().ClearLeader();
        }
   
    }
}
