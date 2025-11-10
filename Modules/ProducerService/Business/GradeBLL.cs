using ProducerService.DAL;
using ProducerService.Model;
using AuthService;
using AuthService.Business;
using Common.Share;
using DictService.Business;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using Common.IdGenerator;
using System.Transactions;

namespace ProducerService.Business
{
    public class GradeBLL
    {
        private IOptions<GeneralOption> _conf;
        private ITAServiceProvider _provider;
        private OperatorHelper _operator;
        private GradeDAL _gradeDAL;
        private FactoryDAL _factoryDAL;
        private SnowflakeHelper _snowflake;
        private AgentDAL _agentDAL;
        public GradeBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, IOptions<GeneralOption> conf,
            OperatorHelper operatorHelper, GradeDAL gradeDAL, FactoryDAL factoryDAL, AgentDAL agentDAL)
        {
            _provider = provider;
            _snowflake = snowflake;
            _conf = conf;
            _operator = operatorHelper;
            _gradeDAL = gradeDAL;
            _factoryDAL = factoryDAL;
            _agentDAL = agentDAL;
        }


        /// <summary>
        /// 查询等级列表
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<MZ_Grade>> SelectList()
        {
            var user = _provider.GetUser();
            return await _gradeDAL.SelectList(x => x.FactoryId == user.OrgId, "Sort asc");
        }

        public virtual async Task<BusResponse<int>> UpdateSort(List<string> idList)
        {
            try
            {
                return BusResponse<int>.Success(await _gradeDAL.UpdateSort(idList));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(112, ex.Message);
            }
        }

        public virtual async Task<BusResponse<int>> Update(MZ_Grade data)
        {
            data.FactoryId = null;
            data.IsSystem = null;
            return BusResponse<int>.Success(await _gradeDAL.Update(data));
        }
        public virtual async Task<BusResponse<int>> Add(MZ_Grade data)
        {
            var user = _provider.GetUser();
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(133, "请切换到企业账号");
            }
            if (!await _factoryDAL.Some(x => x.Id == user.OrgId))
            {
                return BusResponse<int>.Error(134, "当前企业不是生产商");
            }
            data.Id = _snowflake.NextId().ToString();
            data.IsSystem = 0;
            data.FactoryId = user.OrgId;
            data.Sort = 0;
            return BusResponse<int>.Success(await _gradeDAL.Insert(data));
        }
        public virtual async Task<BusResponse<string>> Delete(string id)
        {
            try
            {
                var user = _provider.GetUser();
                if (user.OrgId <= 0)
                {
                    return BusResponse<string>.Error(131, "请切换到企业账号");
                }
                if (!await _factoryDAL.Some(x => x.Id == user.OrgId))
                {
                    return BusResponse<string>.Error(132, "当前企业不是生产商");
                }

                var old = await _gradeDAL.Select(id);
                if (old.IsSystem == 1)
                {
                    return BusResponse<string>.Error(133, "系统级别无法删除");
                }

                if (await _agentDAL.Some(x => x.GradeId == id))
                {
                    return BusResponse<string>.Error(134, "无法删除，代理级别已被使用");
                }
                await _gradeDAL.Delete(id);

                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }
    }
}
