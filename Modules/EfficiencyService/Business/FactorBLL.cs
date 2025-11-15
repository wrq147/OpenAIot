using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthService;
using Common.IdGenerator;
using Common.Share;
using EfficiencyService.Controller;
using EfficiencyService.DAL;
using EfficiencyService.Model;
using Google.Protobuf.Collections;
using InfluxDB.Client.Api.Domain;
using Minio.DataModel;
using MySqlX.XDevAPI.Common;
using NPOI.POIFS.Properties;
using StackExchange.Redis;
using TemplateAction.Core;

namespace EfficiencyService.Business
{
    public class FactorBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private FactorDAL _factorDAL;

        public FactorBLL(ITAServiceProvider provider, FactorDAL factorDAL, SnowflakeHelper snowflake)
        {
            _provider = provider;
            _snowflake = snowflake;
            _factorDAL = factorDAL;
        }

        /// <summary>
        /// 新增排放因子
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddFactor(In_Factor2 in_Factor)
        {
            var user = _provider.GetUser();

            T_ENG_Factor factor = new T_ENG_Factor();
            factor.Id = _snowflake.NextId().ToString();

            factor.Year = in_Factor.Year;
            factor.Version = in_Factor.Version;
            factor.TypeId = in_Factor.TypeId;
            factor.FactorName = in_Factor.FactorName;
            factor.EmissionFactor = in_Factor.EmissionFactor;
            factor.Memo = in_Factor.Memo;
            factor.AvgCalorific = in_Factor.AvgCalorific;
            factor.EqCoal = in_Factor.EqCoal;

            factor.del_flag = "0";
            factor.createId = user.UserId;
            factor.create_time = DateTime.Now;
            factor.updateId = user.UserId;
            factor.update_time = DateTime.Now;
            await _factorDAL.AddFactor(factor);
            return BusResponse<string>.Success("新增排放因子成功");
        }

        /// <summary>
        /// 新增排放因子类型
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddFactorType(In_FactorType in_FactorType)
        {
            var user = _provider.GetUser();

            T_ENG_FactorType factor = new T_ENG_FactorType();
            factor.Id = _snowflake.NextId().ToString();

            factor.TypeName = in_FactorType.TypeName;
            factor.FactorType = in_FactorType.FactorType;
            factor.FactorDigits = in_FactorType.FactorDigits;
            factor.NameTitle = in_FactorType.NameTitle;
            factor.FactorTitle = in_FactorType.FactorTitle;
            factor.FactorUnit = in_FactorType.FactorUnit;
            factor.ActivityUnit = in_FactorType.ActivityUnit;
            factor.ActivityConversion = in_FactorType.ActivityConversion;
            if (string.IsNullOrEmpty(in_FactorType.ParentId))
            {
                factor.ParentId = "-";
            }
            else
            {
                factor.ParentId = in_FactorType.ParentId;
            }
            factor.del_flag = "0";
            factor.createId = user.UserId;
            factor.create_time = DateTime.Now;
            factor.updateId = user.UserId;
            factor.update_time = DateTime.Now;
            await _factorDAL.AddFactorType(factor);
            return BusResponse<string>.Success("新增排放因子类型成功");
        }

        /// <summary>
        /// 新增排放年份
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddFactorYear(In_FactorYear in_FactorYear)
        {
            var user = _provider.GetUser();

            T_ENG_FactorYear factor = new T_ENG_FactorYear();
            factor.Id = _snowflake.NextId().ToString();

            factor.Year = in_FactorYear.Year;

            factor.del_flag = "0";
            factor.createId = user.UserId;
            factor.create_time = DateTime.Now;
            factor.updateId = user.UserId;
            factor.update_time = DateTime.Now;
            await _factorDAL.AddFactorYear(factor);
            return BusResponse<string>.Success("新增排放年份成功");
        }

        /// <summary>
        /// 新增排放年份版本
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddFactorYearVersion(In_FactorYearVersion in_FactorYearVersion)
        {
            var user = _provider.GetUser();

            T_ENG_FactorYearVersion factor = new T_ENG_FactorYearVersion();
            factor.Id = _snowflake.NextId().ToString();

            factor.Version = in_FactorYearVersion.Version;
            factor.YearId = in_FactorYearVersion.YearId;

            factor.del_flag = "0";
            factor.createId = user.UserId;
            factor.create_time = DateTime.Now;
            factor.updateId = user.UserId;
            factor.update_time = DateTime.Now;
            await _factorDAL.AddFactorYearVersion(factor);
            return BusResponse<string>.Success("新增排放年份版本成功");
        }

        /// <summary>
        /// 企业排放因子
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddFactorOrg(In_FactorOrg in_FactorOrg)
        {
            //先删除
            await _factorDAL.DeleteFactorOrg(in_FactorOrg.OrgId);

            string[] factorIds = in_FactorOrg.FactorId.Split(',');
            foreach (string f in factorIds)
            {
                var user = _provider.GetUser();

                T_ENG_FactorOrg factorOrg = new T_ENG_FactorOrg();
                factorOrg.Id = _snowflake.NextId().ToString();

                factorOrg.OrgId = in_FactorOrg.OrgId;
                factorOrg.FactorId = f;

                factorOrg.createId = user.UserId;
                factorOrg.create_time = DateTime.Now;
                factorOrg.updateId = user.UserId;
                factorOrg.update_time = DateTime.Now;
                await _factorDAL.AddFactorOrg(factorOrg);
            }
            return BusResponse<string>.Success("新增企业排放因子");
        }


        /// <summary>
        /// 删除企业排放因子
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> DeleteFactorById(string Id)
        {
            try
            {
                await _factorDAL.DeleteFactor(Id);
                return BusResponse<int>.Success(0, "删除排放因子成功");
            }
            catch
            {
                return BusResponse<int>.ErrorBusy();
            }
        }


        /// <summary>
        /// 删除排放因子类型
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> DeleteFactorTypeById(string Id)
        {
            try
            {
                await _factorDAL.DeleteFactorType(Id);
                return BusResponse<int>.Success(0, "删除排放因子类型成功");
            }
            catch
            {
                return BusResponse<int>.ErrorBusy();
            }
        }

        /// <summary>
        /// 删除排放年份
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> DeleteFactorYearById(string Id)
        {
            try
            {
                await _factorDAL.DeleteFactorYear(Id);
                return BusResponse<int>.Success(0, "删除排放年份成功");
            }
            catch
            {
                return BusResponse<int>.ErrorBusy();
            }
        }

        /// <summary>
        /// 删除排放年份版本
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> DeleteFactorYearVersionById(string Id)
        {
            try
            {
                await _factorDAL.DeleteFactorYearVersion(Id);
                return BusResponse<int>.Success(0, "删除排放年份成功");
            }
            catch
            {
                return BusResponse<int>.ErrorBusy();
            }
        }

        /// <summary>
        /// 修改排放因子
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateFactor(In_Factor2 in_Factor)
        {
            var user = _provider.GetUser();

            T_ENG_Factor data = new T_ENG_Factor();
            data.Id = in_Factor.Id;
            data.Year = in_Factor.Year;
            data.Version = in_Factor.Version;
            data.TypeId = in_Factor.TypeId;
            data.FactorName = in_Factor.FactorName;
            data.EmissionFactor = in_Factor.EmissionFactor;
            data.Memo = in_Factor.Memo;
            data.AvgCalorific = in_Factor.AvgCalorific;
            data.EqCoal = in_Factor.EqCoal;
            data.updateId = user.UserId;
            data.update_time = DateTime.Now;

            int result = await _factorDAL.UpdateFactor(data);
            return BusResponse<int>.Success(result, "修改成功");
        }

        /// <summary>
        /// 修改排放因子类型
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateFactorType(In_FactorType in_FactorType)
        {
            var user = _provider.GetUser();

            T_ENG_FactorType data = new T_ENG_FactorType();
            data.Id = in_FactorType.Id;
            data.TypeName = in_FactorType.TypeName;
            data.FactorType = in_FactorType.FactorType;
            data.FactorDigits = in_FactorType.FactorDigits;
            data.NameTitle = in_FactorType.NameTitle;
            data.FactorTitle = in_FactorType.FactorTitle;
            data.FactorUnit = in_FactorType.FactorUnit;
            data.ActivityUnit = in_FactorType.ActivityUnit;
            data.ActivityConversion = in_FactorType.ActivityConversion;
            data.ParentId = in_FactorType.ParentId;
            data.updateId = user.UserId;
            data.update_time = DateTime.Now;

            int result = await _factorDAL.UpdateFactorType(data);
            return BusResponse<int>.Success(result, "修改成功");
        }

        /// <summary>
        /// 修改排放年份
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateFactorYear(In_FactorYear in_FactorYear)
        {
            var user = _provider.GetUser();

            T_ENG_FactorYear data = new T_ENG_FactorYear();
            data.Id = in_FactorYear.Id;
            data.Year = in_FactorYear.Year;
            data.updateId = user.UserId;
            data.update_time = DateTime.Now;

            int result = await _factorDAL.UpdateFactorYear(data);
            return BusResponse<int>.Success(result, "修改成功");
        }

        /// <summary>
        /// 修改排放年份版本
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateFactorYearVersion(In_FactorYearVersion in_FactorYearVersion)
        {
            var user = _provider.GetUser();

            T_ENG_FactorYearVersion data = new T_ENG_FactorYearVersion();
            data.Id = in_FactorYearVersion.Id;
            data.YearId = in_FactorYearVersion.YearId;
            data.Version = in_FactorYearVersion.Version;
            data.updateId = user.UserId;
            data.update_time = DateTime.Now;

            int result = await _factorDAL.UpdateFactorYearVersion(data);
            return BusResponse<int>.Success(result, "修改成功");
        }

        /// <summary>
        /// 查询企业排放因子
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_Factor>> SelectFactorOrg(string OrgId)
        {
            List<Out_Factor> data = await _factorDAL.SelectFactororg(OrgId);

            return data;
        }

        /// <summary>
        /// 查询排放因子
        /// </summary>
        /// <returns></returns>
        public async Task<T_ENG_Factor> SelectFactor(string FactorId)
        {
            //排放因子
            return await _factorDAL.SelectFactor(FactorId);
        }

       

        /// <summary>
        /// 查询企业排放因子
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_Factor>> SelectFactorOrgByTypeId(string OrgId, string TypeId)
        {
            List<Out_Factor> data = await _factorDAL.SelectFactorOrgByTypeId(OrgId, TypeId);

            return data;
        }

        /// <summary>
        /// 查询企业能源类型
        /// </summary>
        /// <returns></returns>
        public async Task<List<T_ENG_FactorType>> SelectFactorTypeOrg(string OrgId)
        {
            return await _factorDAL.SelectOrgFactorType(OrgId);
        }

        /// <summary>
        /// 查询排放因子列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_FactorTypeInfo>> SelectFactorList(In_Factor query)
        {
            List<T_ENG_FactorType> data = await _factorDAL.SelectFactorType();

            List<Out_FactorTypeInfo> list = new List<Out_FactorTypeInfo>();

            if (string.IsNullOrEmpty(query.TypeId))
            {
                query.TypeId = "-";
            }
            
            foreach (var item in data)
            {
                if (item.Id == query.TypeId)
                {
                    Out_FactorTypeInfo ft = new Out_FactorTypeInfo();
                    ft.Id = item.Id;
                    ft.TypeName = item.TypeName;
                    ft.FactorType = item.FactorType;
                    ft.FactorDigits = item.FactorDigits;
                    ft.NameTitle = item.NameTitle;
                    ft.FactorTitle = item.FactorTitle;
                    ft.FactorUnit = item.FactorUnit;
                    ft.ActivityUnit = item.ActivityUnit;
                    ft.ActivityConversion = item.ActivityConversion;
                    ft.ParentId = item.ParentId;
                    if (ft.FactorType)
                    {
                        list.Add(ft);
                    }
                    DeppTypeTree(item.Id, data, ref list);

                }
            }
            
            In_Factor itemQuery = new In_Factor();
            itemQuery.Year = query.Year;
            itemQuery.Version = query.Version;
            foreach (var item in list)
            {
                itemQuery.TypeId = item.Id;
                item.Factors = await _factorDAL.SelectFactorList(itemQuery);
            }
            return list;
        }

        /// <summary>
        /// 查询排放因子列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_FactorTypeNew>> SelectOrgFactorList()
        {
            List<T_ENG_FactorType> data = await _factorDAL.SelectFactorType();
            List<T_ENG_Factor> factors = await _factorDAL.SelectFactorNewList();
            List<Out_FactorTypeNew> list = DeppTreeNew("-", data, factors);
            return list;
        }

        /// <summary>
        /// 查询排放因子分类树
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_FactorType>> SelectFactorTypeTree()
        {
            List<T_ENG_FactorType> data = await _factorDAL.SelectFactorType();
            List<Out_FactorType> list = DeppTree("-", data);
            return list;
        }


        /// <summary>
        /// 查询排放因子分类
        /// </summary>
        /// <returns></returns>
        public async Task<Out_FactorType> SelectFactorType(string Id)
        {
            Out_FactorType out_FactorType = new Out_FactorType();
            T_ENG_FactorType data = await _factorDAL.SelectFactorTypeInfo(Id);
            if (data != null)
            {
                out_FactorType.Id = data.Id;
                out_FactorType.TypeName = data.TypeName;
                out_FactorType.FactorType = data.FactorType;
                out_FactorType.FactorDigits = data.FactorDigits;
                out_FactorType.NameTitle = data.NameTitle;
                out_FactorType.FactorTitle = data.FactorTitle;
                out_FactorType.FactorUnit = data.FactorUnit;
                out_FactorType.ActivityUnit = data.ActivityUnit;
                out_FactorType.ActivityConversion = data.ActivityConversion;
                out_FactorType.ParentId = data.ParentId;
                T_ENG_FactorType parent = await _factorDAL.SelectFactorTypeInfo(data.ParentId);
                if (parent == null)
                {
                    out_FactorType.ParentName = "所有";
                }
                else
                {
                    out_FactorType.ParentName = parent.TypeName;
                }
                out_FactorType.Children = DeppTree(out_FactorType.Id, await _factorDAL.SelectFactorType());
            }
            return out_FactorType;
        }


        /// <summary>
        /// 分页查询排放年份
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_FactorYear>> SelectFactorYear()
        {
            List<Out_FactorYear> data = await _factorDAL.SelectFactorYear();

            foreach (var item in data)
            {
                item.Versions = await _factorDAL.SelectFactorYearVersion(item.Id);
            }
            return data;
        }

        private static List<Out_FactorTypeNew> DeppTreeNew(string ParentId, List<T_ENG_FactorType> data, List<T_ENG_Factor> factors)
        {
            List<Out_FactorTypeNew> list = new List<Out_FactorTypeNew>();

            foreach (var item in data)
            {
                if (item.ParentId == ParentId)
                {
                    Out_FactorTypeNew ft = new Out_FactorTypeNew();
                    ft.Id = item.Id;
                    ft.TypeName = item.TypeName;
                    ft.FactorType = item.FactorType;
                    ft.FactorDigits = item.FactorDigits;
                    ft.NameTitle = item.NameTitle;
                    ft.FactorTitle = item.FactorTitle;
                    ft.FactorUnit = item.FactorUnit;
                    ft.ActivityUnit = item.ActivityUnit;
                    ft.ActivityConversion = item.ActivityConversion;
                    ft.ParentId = item.ParentId;
                    List<string> pnames = data.Where(a => a.Id == item.ParentId).Select(a => a.TypeName).ToList();
                    if (pnames.Count > 0)
                    {
                        ft.ParentName = pnames[0];
                    }
                    else
                    {
                        ft.ParentName = "所有";
                    }
                    ft.Factors = new List<T_ENG_Factor>();
                    foreach (var factor in factors)
                    {
                        if (factor.TypeId == ft.Id)
                        {
                            ft.Factors.Add(factor);
                        }
                    }
                    ft.Children = DeppTreeNew(item.Id, data, factors);
                    list.Add(ft);
                }
            }
            return list;
        }

        private static List<Out_FactorType> DeppTree(string ParentId, List<T_ENG_FactorType> data)
        {
            List<Out_FactorType> list = new List<Out_FactorType>();

            foreach (var item in data)
            {
                if (item.ParentId == ParentId)
                {
                    Out_FactorType ft = new Out_FactorType();
                    ft.Id = item.Id;
                    ft.TypeName = item.TypeName;
                    ft.FactorType = item.FactorType;
                    ft.FactorDigits = item.FactorDigits;
                    ft.NameTitle = item.NameTitle;
                    ft.FactorTitle = item.FactorTitle;
                    ft.FactorUnit = item.FactorUnit;
                    ft.ActivityUnit = item.ActivityUnit;
                    ft.ActivityConversion = item.ActivityConversion;
                    ft.ParentId = item.ParentId;
                    List<string> pnames = data.Where(a => a.Id == item.ParentId).Select(a => a.TypeName).ToList();
                    if (pnames.Count > 0)
                    {
                        ft.ParentName = pnames[0];
                    }
                    else
                    {
                        ft.ParentName = "所有";
                    }
                    ft.Children = DeppTree(item.Id, data);

                    list.Add(ft);
                }
            }
            return list;
        }

        private static void DeppTypeTree(string ParentId, List<T_ENG_FactorType> data, ref List<Out_FactorTypeInfo> typeList)
        {
            foreach (var item in data)
            {
                if (item.ParentId == ParentId)
                {
                    Out_FactorTypeInfo ft = new Out_FactorTypeInfo();
                    ft.Id = item.Id;
                    ft.TypeName = item.TypeName;
                    ft.FactorType = item.FactorType;
                    ft.FactorDigits = item.FactorDigits;
                    ft.NameTitle = item.NameTitle;
                    ft.FactorTitle = item.FactorTitle;
                    ft.FactorUnit = item.FactorUnit;
                    ft.ActivityUnit = item.ActivityUnit;
                    ft.ActivityConversion = item.ActivityConversion;
                    ft.ParentId = item.ParentId;
                    if (ft.FactorType)
                    {
                        typeList.Add(ft);
                    }
                    DeppTypeTree(item.Id, data, ref typeList);

                }
            }
        }
    }
}
