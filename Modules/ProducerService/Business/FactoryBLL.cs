using ProducerService.DAL;
using ProducerService.Model;
using AuthService;
using AuthService.Business;
using Common.IdGenerator;
using Common.Share;
using DictService.Business;
using JiebaNet.Segmenter;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using TemplateAction.Core;
using AuthService.Model;
using MyAccess.Aop;
using Common.EventBus;

namespace ProducerService.Business
{
    public class FactoryBLL
    {
        private IOptions<GeneralOption> _conf;
        private ITAServiceProvider _provider;
        private FactoryDAL _factoryDAL;
        private UserDAL _userDAL;
        private SnowflakeHelper _snowflake;
        private GradeDAL _gradeDAL;
        public FactoryBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, GradeDAL gradeDAL, IOptions<GeneralOption> conf,
            FactoryDAL factory, UserDAL userDAL)
        {
            _provider = provider;
            _conf = conf;
            _factoryDAL = factory;
            _userDAL = userDAL;
            _snowflake = snowflake;
            _gradeDAL = gradeDAL;
        }


        /// <summary>
        /// 查询工厂列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual async Task<PageObject<Out_FactoryItem>> SelectList(In_FactoryList query)
        {
            var tlist = await _factoryDAL.SelectByPage(query);
            var codeBLL = _provider.GetService<CodeBLL>();
            var industryDict = await codeBLL.SelectIndustryDict();
            var dictTypeBLL = _provider.GetService<DictTypeBLL>();
            var dictDict = await dictTypeBLL.SelectDictConverter("org_size");
            foreach (var item in tlist.List)
            {
                item.IndustryName = industryDict.ToName(item.Industry);
                item.SizeName = dictDict.ToName(item.Size.ToString());
            }
            return tlist;
        }
        public virtual async Task<BusResponse<MZ_Factory>> Info(long id)
        {
            var factory = await _factoryDAL.Select(id);
            if (factory == null)
            {
                return BusResponse<MZ_Factory>.Error(6, "您还没有成为生产商");
            }

            return BusResponse<MZ_Factory>.Success(factory);
        }
        public virtual async Task<BusResponse<string>> Delete(long id)
        {
            try
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    await _factoryDAL.Delete(x => x.Id == id);
                    await _userDAL.DeleteUserRoleByOrg(3, id);
                    await _gradeDAL.Delete(x => x.FactoryId == id);
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
        public virtual async Task<BusResponse<string>> Edit(MZ_Factory data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data.PHNumPrefix))
                {
                    if (await _factoryDAL.Some(x => x.PHNumPrefix == data.PHNumPrefix && x.Id != data.Id) || data.PHNumPrefix == "PH")
                    {
                        return BusResponse<string>.Error(112, "自定义批次前缀已被使用");
                    }
                }
                //修改生产商
                var user = _provider.GetUser();
                data.SetUpdateBy(user);
                await _factoryDAL.Update(data);

                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }

        public virtual async Task<BusResponse<string>> Add(MZ_Factory data)
        {
            if (await _factoryDAL.Some(x => x.Id == data.Id))
            {
                return BusResponse<string>.Error(111, "无法重复添加工厂");
            }
            try
            {
                if (!string.IsNullOrEmpty(data.PHNumPrefix))
                {
                    if (await _factoryDAL.Some(x => x.PHNumPrefix == data.PHNumPrefix) || data.PHNumPrefix == "PH")
                    {
                        return BusResponse<string>.Error(112, "自定义批次前缀已被使用");
                    }
                }
                //新增生产商
                var user = _provider.GetUser();
                data.CertTemplateId = string.Empty;
                data.PHNumPrefix ??= string.Empty;
                data.GradeWay ??= "auto";
                data.SetCreateBy(user);

                //获取当前生产商的管理员列表
                var userIdList = await _userDAL.SelectManUserIds(data.Id.Value);

                //添加默认代理级别
                MZ_Grade grade = new MZ_Grade();
                grade.Id = _snowflake.NextId().ToString();
                grade.GradeName = "一级代理";
                grade.Sort = 0;
                grade.IsSystem = 1;
                grade.FactoryId = data.Id;

                //分配生产商角色权限
                List<MZ_UserRole> urlist = new List<MZ_UserRole>();
                foreach (var tid in userIdList)
                {
                    MZ_UserRole ur = new MZ_UserRole();
                    ur.UserId = tid;
                    ur.RoleID = 3;
                    ur.OrgId = data.Id;
                    urlist.Add(ur);
                }
                if (urlist.Count == 0)
                {
                    return BusResponse<string>.Error(112, "生产商没有管理员");
                }

                await BusUtility.Dispatch("AddFactory", new
                {
                    FactoryId = data.Id
                });

                using (BLLTranScope scope = new BLLTranScope())
                {

                    await _factoryDAL.Insert(data);
                    await _userDAL.AddUserRole(urlist);
                    await _gradeDAL.Insert(grade);
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
    }
}
