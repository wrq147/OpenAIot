using AuthService;
using Common.Share;
using MonitorService.DAL;
using MonitorService.Model;
using Quartz;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MonitorService.Business
{
    public class CalendarBLL
    {
        private HolidayTypeDAL _holidayTypeDAL;
        private HolidayOrgDAL _holidayOrgDAL;
        private ITAServiceProvider _provider;
        public CalendarBLL(ITAServiceProvider provider, HolidayTypeDAL holidayTypeDAL, HolidayOrgDAL holidayOrgDAL)
        {
            _holidayTypeDAL = holidayTypeDAL;
            _holidayOrgDAL = holidayOrgDAL;
            _provider = provider;
        }
        public async Task<BusResponse<List<MZ_HolidayType>>> HolidayTypeList(IUserInfo user)
        {
            var tlist = await _holidayTypeDAL.SelectList(x => x.OrgId == user.OrgId, "CreatedOn asc");
            return BusResponse<List<MZ_HolidayType>>.Success(tlist);
        }

        public async Task<BusResponse<string>> AddHolidayType(MZ_HolidayType data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(111, "非企业用户无操作权限");
            }
            data.Id = MyAccess.Core.StringTool.GetGUID();
            data.OrgId = user.OrgId;
            data.CreatedOn = DateTime.Now;
            await GenerateHoliday(data);
            await _holidayTypeDAL.Insert(data);
            return BusResponse<string>.Success();
        }

        public async Task<BusResponse<int>> EditHolidayType(MZ_HolidayType data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(111, "非企业用户无操作权限");
            }
            var old = await _holidayTypeDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(112, "节日类型不存在");
            }
            data.OrgId = null;
            if ((data.CalendarType != null && data.CalendarType != old.CalendarType) || (data.HolidayStart != null && (data.HolidayStart.Value.Year != old.HolidayStart.Value.Year || data.HolidayStart.Value.Month != old.HolidayStart.Value.Month || data.HolidayStart.Value.Day != old.HolidayStart.Value.Day)) || (data.HolidayEnd != null && (data.HolidayEnd.Value.Year != old.HolidayEnd.Value.Year || data.HolidayEnd.Value.Month != old.HolidayEnd.Value.Month || data.HolidayEnd.Value.Day != old.HolidayEnd.Value.Day)))
            {
                await _holidayOrgDAL.Delete(x => x.HolidayTypeId == data.Id);
                await GenerateHoliday(data);
            }
            var rs = await _holidayTypeDAL.Update(data);
            return BusResponse<int>.Success(rs);
        }
        public async Task<BusResponse<int>> DelHolidayType(string id, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(111, "非企业用户无操作权限");
            }
            await _holidayOrgDAL.Delete(x => x.HolidayTypeId == id);
            var rs = await _holidayTypeDAL.Delete(x => x.Id == id);
            return BusResponse<int>.Success(rs);
        }
        public async Task<BusResponse<List<MZ_HolidayOrg>>> GetHolidayOrgList(long orgId)
        {
            if (orgId <= 0)
            {
                return BusResponse<List<MZ_HolidayOrg>>.Error(111, "非企业用户无操作权限");
            }
            var tlist = await _holidayOrgDAL.GetHolidayOrgList(orgId);
            return BusResponse<List<MZ_HolidayOrg>>.Success(tlist);
        }
        public async Task<BusResponse<string>> AddHoliday(DateTime dt, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(111, "非企业用户无操作权限");
            }
            var daystr = dt.ToString("yyyyMMdd");
            if (await _holidayOrgDAL.Some(x => x.OrgId == user.OrgId && x.DayStr == daystr))
            {
                return BusResponse<string>.Error(112, "已存在节日，无法添加");
            }
            MZ_HolidayOrg holidayOrg = new MZ_HolidayOrg();
            holidayOrg.Id = MyAccess.Core.StringTool.GetGUID();
            holidayOrg.OrgId = user.OrgId;
            holidayOrg.HolidayTypeId = string.Empty;
            holidayOrg.Holiday = dt;
            holidayOrg.DayStr = daystr;
            holidayOrg.TimeWay = 0;
            await _holidayOrgDAL.Insert(holidayOrg);

            return BusResponse<string>.Success();
        }

        public async Task<BusResponse<string>> DelHoliday(DateTime dt, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(111, "非企业用户无操作权限");
            }

            await _holidayOrgDAL.Delete(x => x.OrgId == user.OrgId && x.HolidayTypeId == "" && x.Holiday >= dt.Date && x.Holiday < dt.Date.AddDays(1).Date);

            return BusResponse<string>.Success();
        }


        private async Task GenerateHoliday(MZ_HolidayType data)
        {
            DateTime startTime = data.HolidayStart.Value;
            DateTime endTime = data.HolidayEnd.Value;
            if (data.CalendarType == 1)
            {
                ChineseLunisolarCalendar lunarCalendar = new ChineseLunisolarCalendar();
                startTime = lunarCalendar.ToDateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, startTime.Minute, startTime.Second, 0);
                endTime = lunarCalendar.ToDateTime(endTime.Year, endTime.Month, endTime.Day, endTime.Hour, endTime.Minute, endTime.Second, 0);
            }
            else if (data.CalendarType == 2)
            {
                int startWeek = (int)startTime.DayOfWeek;
                if (startWeek == 0)
                {
                    startWeek = 7;
                }

                int endWeek = (int)endTime.DayOfWeek;
                if (endWeek == 0)
                {
                    endWeek = 7;
                }
                List<MZ_HolidayOrg> tlist = new List<MZ_HolidayOrg>();
                DateTime dt = DateTime.Now;
                bool isrever = startWeek > endWeek;
                for (var i = 0; i <= 60; i++)
                {
                    int tmpweek = (int)dt.DayOfWeek;
                    if (tmpweek == 0)
                    {
                        tmpweek = 7;
                    }
                    bool isCondMin = false;
                    if (isrever)
                    {
                        isCondMin = (tmpweek > 0 && tmpweek < endWeek) || (tmpweek < 7 && tmpweek > startWeek);
                    }
                    else
                    {
                        isCondMin = tmpweek > startWeek && tmpweek < endWeek;
                    }
                    if (isCondMin)
                    {
                        MZ_HolidayOrg holidayOrg = new MZ_HolidayOrg();
                        holidayOrg.Id = MyAccess.Core.StringTool.GetGUID();
                        holidayOrg.OrgId = data.OrgId;
                        holidayOrg.Holiday = dt.Date;
                        holidayOrg.TimeWay = 0;
                        holidayOrg.DayStr = dt.ToString("yyyyMMdd");
                        holidayOrg.HolidayTypeId = data.Id;
                        tlist.Add(holidayOrg);
                    }
                    else
                    {
                        if (tmpweek == startWeek)
                        {
                            MZ_HolidayOrg startholidayOrg = new MZ_HolidayOrg();
                            startholidayOrg.Id = MyAccess.Core.StringTool.GetGUID();
                            startholidayOrg.OrgId = data.OrgId;
                            startholidayOrg.Holiday = new DateTime(dt.Year, dt.Month, dt.Day, startTime.Hour, startTime.Minute, startTime.Second);
                            startholidayOrg.TimeWay = 1;
                            startholidayOrg.DayStr = dt.ToString("yyyyMMdd");
                            startholidayOrg.HolidayTypeId = data.Id;
                            tlist.Add(startholidayOrg);
                        }
                        if (tmpweek == endWeek)
                        {
                            MZ_HolidayOrg endholidayOrg = new MZ_HolidayOrg();
                            endholidayOrg.Id = MyAccess.Core.StringTool.GetGUID();
                            endholidayOrg.OrgId = data.OrgId;
                            endholidayOrg.Holiday = new DateTime(dt.Year, dt.Month, dt.Day, endTime.Hour, endTime.Minute, endTime.Second);
                            endholidayOrg.TimeWay = 2;
                            endholidayOrg.DayStr = dt.ToString("yyyyMMdd");
                            endholidayOrg.HolidayTypeId = data.Id;
                            tlist.Add(endholidayOrg);
                        }
                    }

                    dt = dt.AddDays(1);
                }

                if (tlist.Count > 0)
                {
                    var dayStrList = tlist.Select(x => x.DayStr).ToList();
                    //清除日期内可能存在的假期
                    await _holidayOrgDAL.Delete(x => x.OrgId == data.OrgId && dayStrList.Contains(x.DayStr));
                    //添加假期
                    await _holidayOrgDAL.Insert(tlist);
                }
                data.NextTime = DateTime.Now.AddDays(50);
                return;
            }
            var addyear = DateTime.Now.Year - startTime.Year;
            if (addyear > 0)
            {
                startTime.AddYears(addyear);
                endTime.AddYears(addyear);
            }
            if (startTime > endTime)
            {
                var tmpstart = startTime;
                startTime = endTime;
                endTime = tmpstart;
            }
            if (DateTime.Now.AddDays(61) < startTime)
            {
                data.NextTime = startTime.AddDays(-60);
            }
            else
            {
                //执行生成假期
                List<MZ_HolidayOrg> tlist = new List<MZ_HolidayOrg>();

                MZ_HolidayOrg startholidayOrg = new MZ_HolidayOrg();
                startholidayOrg.Id = MyAccess.Core.StringTool.GetGUID();
                startholidayOrg.OrgId = data.OrgId;
                startholidayOrg.Holiday = startTime;
                startholidayOrg.TimeWay = 1;
                startholidayOrg.DayStr = startTime.ToString("yyyyMMdd");
                startholidayOrg.HolidayTypeId = data.Id;
                tlist.Add(startholidayOrg);

                var exeTime = startTime.AddDays(1);
                while (true)
                {
                    if (exeTime > endTime)
                    {
                        break;
                    }
                    if (exeTime.Year == endTime.Year && exeTime.Month == endTime.Month && exeTime.Day == endTime.Day)
                    {
                        break;
                    }
                    MZ_HolidayOrg holidayOrg = new MZ_HolidayOrg();
                    holidayOrg.Id = MyAccess.Core.StringTool.GetGUID();
                    holidayOrg.OrgId = data.OrgId;
                    holidayOrg.Holiday = exeTime;
                    holidayOrg.TimeWay = 0;
                    holidayOrg.DayStr = exeTime.ToString("yyyyMMdd");
                    holidayOrg.HolidayTypeId = data.Id;
                    tlist.Add(holidayOrg);
                    exeTime = exeTime.AddDays(1);
                }
                MZ_HolidayOrg endholidayOrg = new MZ_HolidayOrg();
                endholidayOrg.Id = MyAccess.Core.StringTool.GetGUID();
                endholidayOrg.OrgId = data.OrgId;
                endholidayOrg.Holiday = endTime;
                endholidayOrg.TimeWay = 2;
                endholidayOrg.DayStr = endTime.ToString("yyyyMMdd");
                endholidayOrg.HolidayTypeId = data.Id;
                tlist.Add(endholidayOrg);

                if (tlist.Count > 0)
                {
                    //清除日期内可能存在的假期
                    await _holidayOrgDAL.Delete(x => x.OrgId == data.OrgId && x.Holiday >= startTime.Date && x.Holiday < endTime.Date.AddDays(1));
                    //添加假期
                    await _holidayOrgDAL.Insert(tlist);
                }

                data.NextTime = startTime.AddMonths(12).AddDays(-60);
            }

        }
        /// <summary>
        /// 定时清除过期假日和每日生成假日
        /// </summary>
        /// <returns></returns>
        public virtual async Task Execute()
        {
            await _holidayOrgDAL.Delete(x => x.Holiday < DateTime.Now.AddDays(5).Date);

            var tlist = await _holidayTypeDAL.SelectList(x => x.NextTime < DateTime.Now.AddHours(24) && x.NextTime > DateTime.Now.AddHours(-24));
            foreach (var t in tlist)
            {
                await GenerateHoliday(t);
                t.HolidayEnd = null;
                t.HolidayName = null;
                t.HolidayStart = null;
                t.CreatedOn = null;
                t.OrgId = null;
                t.CalendarType = null;
                await _holidayTypeDAL.Update(t);
            }

        }
    }
}
