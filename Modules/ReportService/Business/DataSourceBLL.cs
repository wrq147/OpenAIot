using AuthService;
using Common;
using Common.IdGenerator;
using Common.Share;
using ReportService.DAL;
using ReportService.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TemplateAction.Core;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using MonitorService.Controller;
using TemplateAction.Cache;
namespace ReportService.Business
{
    public class DataSourceBLL
    {
        private DataSourceDAL _dataSource;
        private SnowflakeHelper _snowflake;
        private ITAServiceProvider _provider;
        public DataSourceBLL(DataSourceDAL dataSource, SnowflakeHelper snowflake, ITAServiceProvider serviceProvider)
        {
            _dataSource = dataSource;
            _snowflake = snowflake;
            _provider = serviceProvider;
        }
        public async Task<List<MZ_DataSource>> GetDataSourceByIds(List<string> ids, long orgId)
        {
            return await _dataSource.SelectList(x => ids.Contains(x.Id) && x.OrgId == orgId);
        }
        public async Task<PageObject<MZ_DataSource>> ListPage(In_DataSourceListPage query)
        {
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);
            return await _dataSource.SelectByPage(query, user.OrgId);
        }

        public virtual async Task<BusResponse<MZ_DataSource>> Info(string id)
        {
            var source = await _dataSource.Select(id);
            if (source == null)
            {
                return BusResponse<MZ_DataSource>.Error(111, "数据源不存在");
            }

            return BusResponse<MZ_DataSource>.Success(source);
        }


        public virtual async Task<BusResponse<string>> Add(MZ_DataSource data)
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

        public virtual async Task<BusResponse<int>> Update(MZ_DataSource data)
        {
            try
            {
                MZ_DataSource old = await _dataSource.Select(data.Id);
                if (old == null)
                {
                    return BusResponse<int>.Error(103, "当前数据源不存在");
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
        public virtual async Task<BusResponse<DataTable>> ShowDatabases(string type, string ipAddress, int port, string username, string password)
        {
            DataSet ds = new DataSet();
            if (type == "sqlserver")
            {
                using SqlConnection con = new SqlConnection(string.Format("server={0},{1};User Id={2};Password={3};Database={4};", ipAddress, port, username, password, "master"));
                await con.OpenAsync();
                SqlCommand cmd = new SqlCommand("SELECT name FROM sys.databases", con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds);
                return BusResponse<DataTable>.Success(ds.Tables[0]);
            }
            else if (type == "mysql")
            {
                using MySqlConnection con = new MySqlConnection(string.Format("server={0};port={1};user={2};password={3};database={4};Pooling=false", ipAddress, port, username, password, "mysql"));
                await con.OpenAsync();
                DataTable dt = con.GetSchema("Databases");
                DataTable newdt = new DataTable();
                newdt.Columns.Add("name");
                foreach (DataRow dr in dt.Rows)
                {
                    var newrow = newdt.NewRow();
                    newrow["name"] = dr[1];
                    newdt.Rows.Add(newrow);
                }
                return BusResponse<DataTable>.Success(newdt);
            }
            else
            {
                return BusResponse<DataTable>.Error(110, string.Format("不支持数据库类型：{0}", type));
            }
        }
        private async Task<DataTable> AnalysisTo(string type, string ipAddress, int port, string username, string password, string baseName, string executeSql, int cacheTime)
        {
            DataSet ds = new DataSet();
            if (type == "sqlserver")
            {
                using SqlConnection con = new SqlConnection(string.Format("server={0},{1};User Id={2};Password={3};Database={4};", ipAddress, port, username, password, baseName));
                await con.OpenAsync();
                SqlCommand cmd = new SqlCommand(executeSql, con);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds);
            }
            else if (type == "mysql")
            {
                using MySqlConnection con = new MySqlConnection(string.Format("server={0};port={1};user={2};password={3};database={4};Pooling=false", ipAddress, port, username, password, baseName));
                await con.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(executeSql, con);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(ds);
            }
            else
            {
                return null;
            }
            string cacheKey = type + "-" + ipAddress + "-" + port + "-" + baseName + "-" + executeSql;
            CacheHelper cache = _provider.GetService<CacheHelper>();
            if (ds.Tables.Count > 0)
            {
                if (cache != null)
                {
                    cache.SetCache(cacheKey, new CacheDataTable()
                    {
                        dt = ds.Tables[0],
                        expire = DateTime.Now.AddSeconds(cacheTime).AddMinutes(-1)
                    }, DateTime.Now.AddSeconds(cacheTime));
                }
                return ds.Tables[0];
            }
            else
            {
                var dt = new DataTable();
                if (cache != null)
                {
                    cache.SetCache(cacheKey, new CacheDataTable()
                    {
                        dt = dt,
                        expire = DateTime.Now.AddSeconds(cacheTime).AddMinutes(-1)
                    }, DateTime.Now.AddSeconds(cacheTime));
                }
                return dt;
            }
        }
        public virtual async Task<BusResponse<DataTable>> AnalysisToDataTable(string type, string ipAddress, int port, string username, string password, string baseName, string executeSql, int cacheTime)
        {
            try
            {
                string cacheKey = null;
                CacheHelper cache = null;
                if (cacheTime > 0)
                {
                    cacheKey = type + "-" + ipAddress + "-" + port + "-" + baseName + "-" + executeSql;
                    cache = _provider.GetService<CacheHelper>();
                    var tmpcache = cache.GetCache<CacheDataTable>(cacheKey);
                    if (tmpcache != null)
                    {
                        if (tmpcache.expire < DateTime.Now)
                        {
                            //另起线程缓存
                            Task t = AnalysisTo(type, ipAddress, port, username, password, baseName, executeSql, cacheTime);
                        }
                        return BusResponse<DataTable>.Success(tmpcache.dt);
                    }
                }
                var dt = await AnalysisTo(type, ipAddress, port, username, password, baseName, executeSql, cacheTime);
                if (dt == null)
                {
                    return BusResponse<DataTable>.Error(110, string.Format("不支持数据库类型：{0}", type));
                }
                return BusResponse<DataTable>.Success(dt);
            }
            catch (Exception ex)
            {
                return BusResponse<DataTable>.Error(111, string.Format("执行Sql查询异常：{0}", ex.Message));
            }

        }
    }
}
