using AuthService;
using Common.IdGenerator;
using Common.Share;
using ReportService.DAL;
using ReportService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace ReportService.Business
{
    public class PrintBLL
    {
        private PrintDataDAL _dataDAL;
        private PrintTemplateDAL _printDAL;
        private SnowflakeHelper _snowflake;
        private ITAServiceProvider _provider;
        public PrintBLL(PrintDataDAL dataDAL, PrintTemplateDAL printDAL, SnowflakeHelper snowflake, ITAServiceProvider serviceProvider)
        {
            _dataDAL = dataDAL;
            _printDAL = printDAL;
            _snowflake = snowflake;
            _provider = serviceProvider;
        }

        public virtual async Task<PageObject<MZ_PrintTemplate>> ListPage(In_PrintList query)
        {
            var user = _provider.GetUser();
            return await _printDAL.SelectByPage(query, user);
        }
        public virtual async Task<BusResponse<MZ_PrintTemplate>> Info(string id)
        {
            var report = await _printDAL.Select(id);
            if (report == null)
            {
                return BusResponse<MZ_PrintTemplate>.Error(111, "打印模板不存在");
            }

            return BusResponse<MZ_PrintTemplate>.Success(report);
        }
        public virtual async Task<BusResponse<List<MZ_PrintData>>> DataList()
        {
            var datalist = await _dataDAL.SelectList(x => true);
            return BusResponse<List<MZ_PrintData>>.Success(datalist);
        }
        public virtual async Task<BusResponse<MZ_PrintData>> DataInfo(string id)
        {
            var printData = await _dataDAL.Select(id);
            return BusResponse<MZ_PrintData>.Success(printData);
        }
        public virtual async Task<BusResponse<string>> AddTemplate(MZ_PrintTemplate data)
        {
            try
            {
                var user = _provider.GetUser();
                if (user.OrgId <= 0)
                {
                    return BusResponse<string>.Error(112, "非企业用户无法添加打印模板");
                }
                data.Id = _snowflake.NextId().ToString();
                data.OrgId = user.OrgId;
                data.SetCreateBy(user);
                await _printDAL.Insert(data);
                return BusResponse<string>.Success(data.Id);
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }
        public virtual async Task<BusResponse<int>> UpdateTemplate(MZ_PrintTemplate data)
        {
            try
            {
                var user = _provider.GetUser();
                if (user.OrgId <= 0)
                {
                    return BusResponse<int>.Error(112, "非企业用户无法添加打印模板");
                }
                data.OrgId = null;
                data.DataId = null;
                data.SetUpdateBy(user);
                return BusResponse<int>.Success(await _printDAL.Update(data));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }
        }

        public virtual async Task<BusResponse<int>> Remove(string id)
        {
            var user = _provider.GetUser();
            return BusResponse<int>.Success(await _printDAL.Delete(x => x.Id == id && x.OrgId == user.OrgId));
        }

    }
}
