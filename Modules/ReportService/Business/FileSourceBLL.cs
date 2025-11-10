using AuthService;
using Common.IdGenerator;
using Common.Share;
using MyAccess.Core;
using MyAccess.DB.Builder.WhereToSql;
using NPOI.OpenXmlFormats.Dml.Chart;
using ReportService.DAL;
using ReportService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
namespace ReportService.Business
{
    public class FileSourceBLL
    {
        private FileSourceDAL _dataSource;
        private SnowflakeHelper _snowflake;
        private ITAServiceProvider _provider;
        public FileSourceBLL(FileSourceDAL dataSource, SnowflakeHelper snowflake, ITAServiceProvider serviceProvider)
        {
            _dataSource = dataSource;
            _snowflake = snowflake;
            _provider = serviceProvider;
        }

        public async Task<PageObject<MZ_FileSource>> ListPage(In_FileSourceListPage query)
        {
            return await _dataSource.SelectByPage(query);
        }

        public virtual async Task<BusResponse<MZ_FileSource>> Info(string id)
        {
            var source = await _dataSource.Select(id);
            if (source == null)
            {
                return BusResponse<MZ_FileSource>.Error(111, "数据源不存在");
            }

            return BusResponse<MZ_FileSource>.Success(source);
        }


        public virtual async Task<BusResponse<string>> Add(MZ_FileSource data)
        {
            try
            {
                var context = _provider.GetService<ITAContext>();
                var user = Data_ServerTokenInfo.From(context);
                if (user.OrgId <= 0)
                {
                    return BusResponse<string>.Error(112, "非企业用户无法添加数据源");
                }

                data.SetCreateBy(user);
                data.Id = _snowflake.NextId().ToString();
                data.OrgId = user.OrgId;

                await _dataSource.Insert(data);
                return BusResponse<string>.Success(data.Id);
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }

        public virtual async Task<BusResponse<int>> Update(MZ_FileSource data)
        {
            try
            {
                MZ_FileSource old = await _dataSource.Select(data.Id);
                if (old == null)
                {
                    return BusResponse<int>.Error(103, "数据源不存在");
                }
                var context = _provider.GetService<ITAContext>();
                var user = Data_ServerTokenInfo.From(context);
                data.SetUpdateBy(user);
                return BusResponse<int>.Success(await _dataSource.Update(data, x => x.Id == data.Id && x.OrgId == user.OrgId));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }
        }

        public virtual async Task<BusResponse<int>> Remove(string id)
        {
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);
            return BusResponse<int>.Success(await _dataSource.Delete(x => x.Id == id && x.OrgId == user.OrgId));
        }
    }
}
