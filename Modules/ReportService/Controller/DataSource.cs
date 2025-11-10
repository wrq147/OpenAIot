using AuthService;
using AuthService.Controller;
using Common;
using Common.Share;
using ReportService.Business;
using ReportService.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace ReportService.Controller
{
    /// <summary>
    /// Sql数据源Api
    /// </summary>
    public class DataSource : AbstractLoginedController
    {
        private DataSourceBLL _sourceBLL;
        public DataSource(DataSourceBLL sourceBLL)
        {
            _sourceBLL = sourceBLL;
        }

        /// <summary>
        /// 获取当前企业的Sql数据源
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/ReportService/Source/List")]
        public async Task<DefaultAjaxResult<PageObject<MZ_DataSource>>> List(In_DataSourceListPage query)
        {
            return this.Success(await _sourceBLL.ListPage(query));
        }

        /// <summary>
        /// 获取Sql数据源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_DataSource>> Info(string id)
        {
            return (await _sourceBLL.Info(id)).ToAjaxResult();
        }

        /// <summary>
        /// 新增Sql数据源
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/ReportService/Source/Add")]
        public async Task<AjaxResult> Add(MZ_DataSource data)
        {
            return (await _sourceBLL.Add(data)).ToAjaxResult();
        }


        /// <summary>
        /// 修改Sql数据源
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/ReportService/Source/Edit")]
        public async Task<AjaxResult> Edit(MZ_DataSource data)
        {
            return (await _sourceBLL.Update(data)).ToAjaxResult();
        }
        /// <summary>
        /// 删除Sql数据源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/ReportService/Source/Remove")]
        public async Task<AjaxResult> Remove(string id)
        {
            return (await _sourceBLL.Remove(id)).ToAjaxResult();
        }
        private bool IsLocalIp(string ip)
        {
            ip = ip.ToLower();
            return ip == "127.0.0.1" || ip == "localhost" || ip == "::" || ip == "0:0:0:0:0:0:0:1";
        }

        /// <summary>
        /// 获取指定数据库数据源
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [ShareCheck]
        public async Task<DefaultAjaxResult<List<MZ_DataSource>>> GetDataSourceByIds(List<string> ids)
        {
            var shareUser = GetUser();
            var tlist = await _sourceBLL.GetDataSourceByIds(ids, shareUser.OrgId);
            return this.Success(tlist);
        }
        /// <summary>
        /// 解释执行数据源
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        [HttpPost]
        [ShareCheck]
        public async Task<AjaxResult> Analysis(In_Database database)
        {
            if (IsLocalIp(database.ipAdress))
            {
                return this.Error<DataTable>(12, "无法使用本地Ip地址");
            }
            return (await _sourceBLL.AnalysisToDataTable(database.type, database.ipAdress, database.port, database.username, database.password, database.baseName, database.executeSql, database.cacheTime)).ToAjaxResult();
        }
        /// <summary>
        /// 查询指定数据库的所有数据库列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> ShowDatabases(In_DatabaseNames data)
        {
            if (IsLocalIp(data.ipAdress))
            {
                return this.Error<DataTable>(12, "无法使用本地Ip地址");
            }
            return (await _sourceBLL.ShowDatabases(data.type, data.ipAdress, data.port, data.username, data.password)).ToAjaxResult();
        }
        /// <summary>
        /// 查询指定数据库的所有表名
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> TableNames(In_TableNames data)
        {
            if (IsLocalIp(data.ipAdress))
            {
                return this.Error<DataTable>(12, "无法使用本地Ip地址");
            }
            if (data.type == "mysql")
            {
                string exesql = $"SELECT TABLE_NAME,CASE WHEN TABLE_TYPE = 'BASE TABLE' THEN 'TABLE' WHEN TABLE_TYPE = 'VIEW' THEN 'VIEW' END AS TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES WHERE (TABLE_TYPE = 'BASE TABLE' or TABLE_TYPE='VIEW') and TABLE_SCHEMA='{data.baseName}' order by TABLE_NAME";
                return (await _sourceBLL.AnalysisToDataTable(data.type, data.ipAdress, data.port, data.username, data.password, data.baseName, exesql, 0)).ToAjaxResult();
            }
            else if (data.type == "sqlserver")
            {
                string exesql = $"SELECT TABLE_NAME,CASE WHEN TABLE_TYPE = 'BASE TABLE' THEN 'TABLE' WHEN TABLE_TYPE = 'VIEW' THEN 'VIEW' END AS TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES WHERE (TABLE_TYPE = 'BASE TABLE' or TABLE_TYPE='VIEW') and TABLE_SCHEMA='dbo' order by TABLE_NAME";
                return (await _sourceBLL.AnalysisToDataTable(data.type, data.ipAdress, data.port, data.username, data.password, data.baseName, exesql, 0)).ToAjaxResult();
            }
            else
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("TABLE_NAME");
                dt.Columns.Add("TABLE_TYPE");
                return this.Success<DataTable>(dt);
            }
        }
        /// <summary>
        /// 查询数据库的所有表的结构
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> AllTableStruct(In_AllTableStruct data)
        {
            if (IsLocalIp(data.ipAdress))
            {
                return this.Error<DataTable>(12, "无法使用本地Ip地址");
            }
            if (data.type == "sqlserver")
            {
                string exesql = $"SELECT COLUMN_NAME,DATA_TYPE,TABLE_NAME,p.value as COLUMN_COMMENT FROM INFORMATION_SCHEMA.COLUMNS c LEFT JOIN sys.extended_properties p ON p.major_id = object_id(c.TABLE_NAME) AND p.minor_id = c.ORDINAL_POSITION WHERE TABLE_CATALOG = '{data.baseName}'";
                return (await _sourceBLL.AnalysisToDataTable(data.type, data.ipAdress, data.port, data.username, data.password, data.baseName, exesql, 0)).ToAjaxResult();
            }
            else if (data.type == "mysql")
            {
                string exesql = $"SELECT COLUMN_NAME,DATA_TYPE,TABLE_NAME,COLUMN_COMMENT FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '{data.baseName}'";
                return (await _sourceBLL.AnalysisToDataTable(data.type, data.ipAdress, data.port, data.username, data.password, data.baseName, exesql, 0)).ToAjaxResult();
            }
            else
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("COLUMN_NAME");
                dt.Columns.Add("DATA_TYPE");
                dt.Columns.Add("TABLE_NAME");
                dt.Columns.Add("COLUMN_COMMENT");
                return this.Success<DataTable>(dt);
            }
        }
        /// <summary>
        /// 查询指定表的结构
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> TableStruct(In_TableStruct data)
        {
            if (IsLocalIp(data.ipAdress))
            {
                return this.Error<DataTable>(12, "无法使用本地Ip地址");
            }
            if (data.type == "sqlserver")
            {
                string exesql = $"SELECT COLUMN_NAME,DATA_TYPE,p.value as COLUMN_COMMENT FROM INFORMATION_SCHEMA.COLUMNS c LEFT JOIN sys.extended_properties p ON p.major_id = object_id(c.TABLE_NAME) AND p.minor_id = c.ORDINAL_POSITION WHERE TABLE_NAME = '{data.tableName}' and (name='MS_Description' or name is null)";
                return (await _sourceBLL.AnalysisToDataTable(data.type, data.ipAdress, data.port, data.username, data.password, data.baseName, exesql, 0)).ToAjaxResult();
            }
            else if (data.type == "mysql")
            {
                string exesql = $"SELECT COLUMN_NAME,DATA_TYPE,COLUMN_COMMENT FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{data.tableName}'";
                return (await _sourceBLL.AnalysisToDataTable(data.type, data.ipAdress, data.port, data.username, data.password, data.baseName, exesql, 0)).ToAjaxResult();
            }
            else
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("COLUMN_NAME");
                dt.Columns.Add("DATA_TYPE");
                dt.Columns.Add("COLUMN_COMMENT");
                return this.Success<DataTable>(dt);
            }
        }
    }
}
