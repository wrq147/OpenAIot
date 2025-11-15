using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthService;
using Common.IdGenerator;
using Common.Share;
using EfficiencyService.DAL;
using EfficiencyService.Model;
using TemplateAction.Core;
using TemplateAction.Label.Element;

namespace EfficiencyService.Business
{
    public class PolicyBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private PolicyDAL _policyDAL;
        private FactorDAL _factorDAL;

        public PolicyBLL(ITAServiceProvider provider, PolicyDAL policyDAL, FactorDAL factorDAL, SnowflakeHelper snowflake)
        {
            _provider = provider;
            _snowflake = snowflake;
            _policyDAL = policyDAL;
            _factorDAL = factorDAL;
        }

        /// <summary>
        /// 检测是否已经配置满12个月
        /// </summary>
        /// <param name="input"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool Validate1To12Unique(string input, out string msg)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                msg = "生效月份格式错误";
                return false;
            }

            // 1. 按逗号拆分并清理空格
            var parts = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim())
                            .ToArray();

            // 2. 快速失败：检查数量是否为12
            if (parts.Length != 12)
            {
                msg = "生效月份，配置数为【" + parts.Length + "】不是12个月";
                return false;
            }

            // 3. 解析数字并验证 
            int[] numbers;
            try
            {
                numbers = parts.Select(int.Parse).ToArray();
            }
            catch (FormatException) // 非数字内容 
            {
                msg = "生效月份，含有非法内容";
                return false;
            }

            // 4. 判断是否是1-12
            foreach (var item in numbers)
            {
                if (!(item >= 1 && item <= 12))
                {
                    msg = "生效月份，月份不在1-12";
                    return false;
                }
            }

            // 5. 核心验证逻辑 
            var uniqueNumbers = new HashSet<int>(numbers);
            if (uniqueNumbers.Count == 12 &&
                   numbers.All(n => n >= 1 && n <= 12))
            {
                msg = "符合1-12月";
                return true;
            }
            else
            {
                msg = "生效月份，配置重复";
                return false;
            }
        }

        public static bool ValidateInPolicy(In_Policy in_Policy,bool add, out string msg)
        {
            if (add)
            {
                if (in_Policy.OrgId == null)
                {
                    msg = "缺少OrgId";
                    return false;
                }
            }
            if (in_Policy.PolicyDetils == null || in_Policy.PolicyDetils.Count == 0)
            {
                msg = "生效月份，未配置满12个月";
                return false;
            }


            foreach (var item in in_Policy.PolicyDetils)
            {
                if (item.PolicyType == "1")
                {
                    if (item.JianPrice == null)
                    {
                        msg = "未设置尖价";
                        return false;
                    }
                    if (item.FengPrice == null)
                    {
                        msg = "未设置峰价";
                        return false;
                    }
                    if (item.PingPrice == null)
                    {
                        msg = "未设置平价";
                        return false;
                    }
                    if (item.GuPrice == null)
                    {
                        msg = "未设置谷价";
                        return false;
                    }
                    foreach (var jtem in item.PriceTimes)
                    {
                        if (!(jtem.TimePeriod == "1" || jtem.TimePeriod == "2" || jtem.TimePeriod == "3" || jtem.TimePeriod == "4"))
                        {
                            msg = "时段类型错误";
                            return false;
                        }
                    }
                }
                else if (item.PolicyType == "2")
                {
                    if (item.DayPrice == null)
                    {
                        msg = "未设置全天价";
                        return false;
                    }
                }
                else if (item.PolicyType == "3")
                {
                    if (!(item.CycleType == "1" || item.CycleType == "2" || item.CycleType == "3"))
                    {
                        msg = "计费周期错误";
                        return false;
                    }
                    if (item.PriceTiers == null)
                    {
                        msg = "未设置阶梯价格";
                        return false;
                    }
                    double MinKwh1 = 0;
                    double MaxKwh1 = 0;
                    double MinKwh2 = 0;
                    double MaxKwh2 = 0;
                    double MinKwh3 = 0;
                    double MaxKwh3 = 0;
                    foreach (var jtem in item.PriceTiers)
                    {
                        if (!(jtem.TierLevel == "1" || jtem.TierLevel == "2" || jtem.TierLevel == "3"))
                        {
                            msg = "阶梯级别不是1-3";
                            return false;
                        }
                        if (jtem.TierLevel == "1")
                        {
                            if (jtem.MinKwh != 0)
                            {
                                msg = "初始值不为0";
                                return false;
                            }
                            MinKwh1 = jtem.MinKwh;
                            MaxKwh1 = jtem.MaxKwh;
                        }
                        if (jtem.TierLevel == "2")
                        {
                            MinKwh2 = jtem.MinKwh;
                            MaxKwh2 = jtem.MaxKwh;
                        }
                        if (jtem.TierLevel == "3")
                        {
                            MinKwh3 = jtem.MinKwh;
                            MaxKwh3 = jtem.MaxKwh;
                        }

                        if (jtem.MinKwh >= jtem.MaxKwh)
                        {
                            msg = "最小用量不能大于最大用量";
                            return false;
                        }
                    }
                    if (MinKwh2 != MaxKwh1 + 1)
                    {
                        msg = "第二档的起始不是第一档结束";
                        return false;
                    }
                    if (MinKwh3 != MaxKwh2 + 1)
                    {
                        msg = "第三档的起始不是第二档结束";
                        return false;
                    }
                }
                else
                {
                    msg = "未知计费方式";
                    return false;
                }
            }
            msg = "";
            return true;
        }

        /// <summary>
        /// 新增电价政策
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddPolicy(In_Policy in_Policy)
        {
            string months = "";
            string msg = "";
            bool check = ValidateInPolicy(in_Policy, true, out msg);
            if (!check)
            {
                return BusResponse<string>.Error(500, msg);
            }

            foreach (var item in in_Policy.PolicyDetils)
            {
                months += "," + item.PolicyMonth;
            }

            bool isOK = Validate1To12Unique(months, out msg);
            if (!isOK)
            {
                return BusResponse<string>.Error(500, msg);
            }

            List<T_ENG_FactorType> factorTypes = await _factorDAL.SelectFactorType(in_Policy.EnergyType);
            if (factorTypes.Count == 0)
            {
                return BusResponse<string>.Error(500, "找不到能源类型");
            }

            var user = _provider.GetUser();

            T_Price_Policy policy = new T_Price_Policy();
            policy.Id = _snowflake.NextId().ToString();

            policy.OrgId = in_Policy.OrgId;
            policy.PolicyName = in_Policy.PolicyName;
            policy.EnergyType = in_Policy.EnergyType;
            policy.Unit = in_Policy.Unit;
            policy.LageUnit = in_Policy.LageUnit;
            policy.Memo = in_Policy.Memo;
            policy.Version = "1";
            policy.FactorId = in_Policy.FactorId;

            policy.del_flag = "0";
            policy.createId = user.UserId;
            policy.create_time = DateTime.Now;
            policy.updateId = user.UserId;
            policy.update_time = DateTime.Now;
            await _policyDAL.AddPolicy(policy);

            foreach (var item in in_Policy.PolicyDetils)
            {
                T_Price_PolicyDetil policyDetil = new T_Price_PolicyDetil();
                policyDetil.Id = _snowflake.NextId().ToString();

                policyDetil.PolicyId = policy.Id;
                policyDetil.PolicyMonth = item.PolicyMonth;
                policyDetil.PolicyType = item.PolicyType;
                policyDetil.Version = "1";

                policyDetil.del_flag = "0";
                policyDetil.createId = user.UserId;
                policyDetil.create_time = DateTime.Now;
                policyDetil.updateId = user.UserId;
                policyDetil.update_time = DateTime.Now;
                
                if (item.PolicyType == "1")
                {
                    policyDetil.CycleType = "";
                    T_Price_Period pricePeriod = new T_Price_Period();
                    pricePeriod.Id = _snowflake.NextId().ToString();

                    pricePeriod.DetilId = policyDetil.Id;
                    pricePeriod.TimePeriod = "1";
                    pricePeriod.Price = item.JianPrice;

                    pricePeriod.del_flag = "0";
                    pricePeriod.createId = user.UserId;
                    pricePeriod.create_time = DateTime.Now;
                    pricePeriod.updateId = user.UserId;
                    pricePeriod.update_time = DateTime.Now;
                    await _policyDAL.AddPeriod(pricePeriod);

                    pricePeriod.Id = _snowflake.NextId().ToString();
                    pricePeriod.TimePeriod = "2";
                    pricePeriod.Price = item.FengPrice;
                    await _policyDAL.AddPeriod(pricePeriod);

                    pricePeriod.Id = _snowflake.NextId().ToString();
                    pricePeriod.TimePeriod = "3";
                    pricePeriod.Price = item.PingPrice;
                    await _policyDAL.AddPeriod(pricePeriod);

                    pricePeriod.Id = _snowflake.NextId().ToString();
                    pricePeriod.TimePeriod = "4";
                    pricePeriod.Price = item.GuPrice;
                    await _policyDAL.AddPeriod(pricePeriod);


                    foreach (var jtem in item.PriceTimes)
                    {
                        T_Price_Time priceTime = new T_Price_Time();
                        priceTime.Id = _snowflake.NextId().ToString();

                        priceTime.DetilId = policyDetil.Id;
                        priceTime.TimePeriod = jtem.TimePeriod;
                        priceTime.StartTime = jtem.StartTime;
                        priceTime.EndTime = jtem.EndTime;

                        priceTime.del_flag = "0";
                        priceTime.createId = user.UserId;
                        priceTime.create_time = DateTime.Now;
                        priceTime.updateId = user.UserId;
                        priceTime.update_time = DateTime.Now;
                        await _policyDAL.AddTime(priceTime);
                    }
                }
                else if (item.PolicyType == "2")
                {
                    policyDetil.CycleType = "";
                    T_Price_Period pricePeriod = new T_Price_Period();
                    pricePeriod.Id = _snowflake.NextId().ToString();

                    pricePeriod.DetilId = policyDetil.Id;
                    pricePeriod.TimePeriod = "5";
                    pricePeriod.Price = item.DayPrice;

                    pricePeriod.del_flag = "0";
                    pricePeriod.createId = user.UserId;
                    pricePeriod.create_time = DateTime.Now;
                    pricePeriod.updateId = user.UserId;
                    pricePeriod.update_time = DateTime.Now;
                    await _policyDAL.AddPeriod(pricePeriod);
                }
                else if (item.PolicyType == "3")
                {
                    policyDetil.CycleType = item.CycleType;
                    foreach (var jtem in item.PriceTiers)
                    {
                        T_Price_Tier priceTier = new T_Price_Tier();
                        priceTier.Id = _snowflake.NextId().ToString();

                        priceTier.DetilId = policyDetil.Id;
                        priceTier.TierLevel = jtem.TierLevel;
                        priceTier.MinKwh = jtem.MinKwh;
                        priceTier.MaxKwh = jtem.MaxKwh;
                        priceTier.Price = jtem.Price;

                        priceTier.del_flag = "0";
                        priceTier.createId = user.UserId;
                        priceTier.create_time = DateTime.Now;
                        priceTier.updateId = user.UserId;
                        priceTier.update_time = DateTime.Now;
                        await _policyDAL.AddTier(priceTier);
                    }
                }

                await _policyDAL.AddPolicyDetil(policyDetil);
            }



            return BusResponse<string>.Success("新增成功");
        }

        /// <summary>
        /// 更新电价政策
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<int>> UpdatePolicy(In_Policy in_Policy)
        {
            string months = "";
            string msg = "";
            bool check = ValidateInPolicy(in_Policy, false, out msg);
            if (!check)
            {
                return BusResponse<int>.Error(500, msg);
            }

            foreach (var item in in_Policy.PolicyDetils)
            {
                months += "," + item.PolicyMonth;
            }

            bool isOK = Validate1To12Unique(months, out msg);
            if (!isOK)
            {
                return BusResponse<int>.Error(500, msg);
            }

            List<T_ENG_FactorType> factorTypes = await _factorDAL.SelectFactorType(in_Policy.EnergyType);
            if (factorTypes.Count == 0)
            {
                return BusResponse<int>.Error(500, "找不到能源类型");
            }

            var user = _provider.GetUser();

            T_Price_Policy policy = await _policyDAL.SelectPolicy(in_Policy.Id);
            if (policy == null)
            {
                return BusResponse<int>.Error(500, "未找到记录");
            }

            policy.OrgId = policy.OrgId;
            policy.PolicyName = in_Policy.PolicyName;
            policy.EnergyType = in_Policy.EnergyType;
            policy.Unit = in_Policy.Unit;
            policy.LageUnit = in_Policy.LageUnit;
            policy.Memo = in_Policy.Memo;
            policy.FactorId = in_Policy.FactorId;
            policy.Version = (int.Parse(policy.Version) + 1).ToString();

            policy.del_flag = "0";
            policy.createId = policy.createId;
            policy.create_time = policy.create_time;
            policy.updateId = user.UserId;
            policy.update_time = DateTime.Now;
            await _policyDAL.UpdatePolicy(policy);

            await _policyDAL.DeletePolicyDetil(policy.Id);//先删后增
            foreach (var item in in_Policy.PolicyDetils)
            {
                T_Price_PolicyDetil policyDetil = new T_Price_PolicyDetil();
                policyDetil.Id = _snowflake.NextId().ToString();

                policyDetil.PolicyId = policy.Id;
                policyDetil.PolicyMonth = item.PolicyMonth;
                policyDetil.PolicyType = item.PolicyType;
                policyDetil.Version = policy.Version;
               
                policyDetil.del_flag = "0";
                policyDetil.createId = user.UserId;
                policyDetil.create_time = DateTime.Now;
                policyDetil.updateId = user.UserId;
                policyDetil.update_time = DateTime.Now;

                if (item.PolicyType == "1")
                {
                    policyDetil.CycleType = "";
                    T_Price_Period pricePeriod = new T_Price_Period();
                    pricePeriod.Id = _snowflake.NextId().ToString();

                    pricePeriod.DetilId = policyDetil.Id;
                    pricePeriod.TimePeriod = "1";
                    pricePeriod.Price = item.JianPrice;

                    pricePeriod.del_flag = "0";
                    pricePeriod.createId = user.UserId;
                    pricePeriod.create_time = DateTime.Now;
                    pricePeriod.updateId = user.UserId;
                    pricePeriod.update_time = DateTime.Now;
                    await _policyDAL.AddPeriod(pricePeriod);

                    pricePeriod.Id = _snowflake.NextId().ToString();
                    pricePeriod.TimePeriod = "2";
                    pricePeriod.Price = item.FengPrice;
                    await _policyDAL.AddPeriod(pricePeriod);

                    pricePeriod.Id = _snowflake.NextId().ToString();
                    pricePeriod.TimePeriod = "3";
                    pricePeriod.Price = item.PingPrice;
                    await _policyDAL.AddPeriod(pricePeriod);

                    pricePeriod.Id = _snowflake.NextId().ToString();
                    pricePeriod.TimePeriod = "4";
                    pricePeriod.Price = item.GuPrice;
                    await _policyDAL.AddPeriod(pricePeriod);


                    foreach (var jtem in item.PriceTimes)
                    {
                        T_Price_Time priceTime = new T_Price_Time();
                        priceTime.Id = _snowflake.NextId().ToString();

                        priceTime.DetilId = policyDetil.Id;
                        priceTime.TimePeriod = jtem.TimePeriod;
                        priceTime.StartTime = jtem.StartTime;
                        priceTime.EndTime = jtem.EndTime;

                        priceTime.del_flag = "0";
                        priceTime.createId = user.UserId;
                        priceTime.create_time = DateTime.Now;
                        priceTime.updateId = user.UserId;
                        priceTime.update_time = DateTime.Now;
                        await _policyDAL.AddTime(priceTime);
                    }
                }
                else if (item.PolicyType == "2")
                {
                    policyDetil.CycleType = "";
                    T_Price_Period pricePeriod = new T_Price_Period();
                    pricePeriod.Id = _snowflake.NextId().ToString();

                    pricePeriod.DetilId = policyDetil.Id;
                    pricePeriod.TimePeriod = "5";
                    pricePeriod.Price = item.DayPrice;

                    pricePeriod.del_flag = "0";
                    pricePeriod.createId = user.UserId;
                    pricePeriod.create_time = DateTime.Now;
                    pricePeriod.updateId = user.UserId;
                    pricePeriod.update_time = DateTime.Now;
                    await _policyDAL.AddPeriod(pricePeriod);
                }
                else if (item.PolicyType == "3")
                {
                    policyDetil.CycleType = item.CycleType;
                    foreach (var jtem in item.PriceTiers)
                    {
                        T_Price_Tier priceTier = new T_Price_Tier();
                        priceTier.Id = _snowflake.NextId().ToString();

                        priceTier.DetilId = policyDetil.Id;
                        priceTier.TierLevel = jtem.TierLevel;
                        priceTier.MinKwh = jtem.MinKwh;
                        priceTier.MaxKwh = jtem.MaxKwh;
                        priceTier.Price = jtem.Price;

                        priceTier.del_flag = "0";
                        priceTier.createId = user.UserId;
                        priceTier.create_time = DateTime.Now;
                        priceTier.updateId = user.UserId;
                        priceTier.update_time = DateTime.Now;
                        await _policyDAL.AddTier(priceTier);
                    }
                }
                await _policyDAL.AddPolicyDetil(policyDetil);
            }

            return BusResponse<int>.Success();
        }

        /// <summary>
        /// 删除电价政策
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<int>> DeletePolicy(string Id)
        {
            await _policyDAL.DeletePolicy(Id);
            return BusResponse<int>.Success(0, "删除电价政策");
        }

        /// <summary>
        /// 查询电价政策
        /// </summary>
        /// <returns></returns>
        public async Task<In_Policy> SelectPolicy(string Id)
        {
            In_Policy data = new In_Policy();

            Out_Policy policy = await _policyDAL.SelectPolicy(Id);
            data.Id = policy.Id;
            data.OrgId = policy.OrgId;
            data.PolicyName = policy.PolicyName;
            data.EnergyType = policy.EnergyType;
            data.TypeName = policy.TypeName;
            data.Unit = policy.Unit;
            data.LageUnit = policy.LageUnit;
            data.Memo = policy.Memo;
            data.Version = policy.Version;
            data.del_flag = policy.del_flag;
            data.FactorId = policy.FactorId;
            List<In_PolicyDetil> PolicyDetils = new List<In_PolicyDetil>();
            List<T_Price_PolicyDetil> policyDetil = await _policyDAL.SelectPolicyDetil(policy.Id);
            foreach (var item in policyDetil)
            {
                In_PolicyDetil in_PolicyDetil = new In_PolicyDetil();
                in_PolicyDetil.PolicyMonth = item.PolicyMonth;
                in_PolicyDetil.PolicyType = item.PolicyType;
                in_PolicyDetil.CycleType = item.CycleType;

                if (item.PolicyType == "1")
                {
                    List<T_Price_Period> Periods = await _policyDAL.SelectPeriod(item.Id);
                    foreach (var jtem in Periods)
                    {
                        if (jtem.TimePeriod == "1")
                        {
                            in_PolicyDetil.JianPrice = jtem.Price;
                        }
                        if (jtem.TimePeriod == "2")
                        {
                            in_PolicyDetil.FengPrice = jtem.Price;
                        }
                        if (jtem.TimePeriod == "3")
                        {
                            in_PolicyDetil.PingPrice = jtem.Price;
                        }
                        if (jtem.TimePeriod == "4")
                        {
                            in_PolicyDetil.GuPrice = jtem.Price;
                        }
                    }
                    List<In_PriceTime> PriceTimes = new List<In_PriceTime>();
                    List<T_Price_Time> Times = await _policyDAL.SelectTime(item.Id);
                    foreach (var jtem in Times)
                    {
                        In_PriceTime priceTime = new In_PriceTime();
                        priceTime.TimePeriod = jtem.TimePeriod;
                        priceTime.StartTime = jtem.StartTime.ToString();
                        priceTime.EndTime = jtem.EndTime.ToString();
                        PriceTimes.Add(priceTime);
                    }
                    in_PolicyDetil.PriceTimes = PriceTimes;
                }
                else if (item.PolicyType == "2")
                {
                    List<T_Price_Period> Periods = await _policyDAL.SelectPeriod(item.Id);

                    foreach (var jtem in Periods)
                    {
                        if (jtem.TimePeriod == "5")
                        {
                            in_PolicyDetil.DayPrice = jtem.Price;
                        }
                    }
                }
                else if (item.PolicyType == "3")
                {
                    List<T_Price_Tier> Tiers = await _policyDAL.SelectTier(item.Id);
                    List<In_PriceTier> PriceTiers = new List<In_PriceTier>();
                    foreach (var jtem in Tiers)
                    {
                        In_PriceTier priceTier = new In_PriceTier();
                        priceTier.TierLevel = jtem.TierLevel;
                        priceTier.MinKwh = jtem.MinKwh;
                        priceTier.MaxKwh = jtem.MaxKwh;
                        priceTier.Price = jtem.Price;
                        PriceTiers.Add(priceTier);
                    }
                    in_PolicyDetil.PriceTiers = PriceTiers;
                }
                PolicyDetils.Add(in_PolicyDetil);
            }
            data.PolicyDetils = PolicyDetils;

            return data;
        }

        /// <summary>
        /// 分页电价政策
        /// </summary>
        /// <returns></returns>
        public async Task<PageObject<Out_Policy>> SelectPolicyList(In_PolicyList query)
        {
            PageObject<Out_Policy> data = await _policyDAL.SelectPolicy(query);
            return data;
        }


    }
}
