using Common;
using Common.Share;
using IoTService.Models;
using MyAccess.Aop;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTService.DAL
{
    public class IotUpdateDAL : BaseRepository<MZ_IotUpdate>
    {
        public virtual async Task<int> InsertProductUpdate(string productId, int targetVersion, int level, long orgId)
        {
            if (this.GetSqlType() == SqlType.Sqlite)
            {
                var sqlbb = new SqlBuilder(help).Append("INSERT INTO mz_iot_update(Id,Status,UpdateCount,UpdateErr,Version,Level,OrgId) ").Append("select Id,0,0,'',")
   .AppendParam(targetVersion).Append(",").AppendParam(level).Append(",").AppendParam(orgId).Append(" from mz_iot_device where Online=1 and ProductId=").AppendParam(productId).Append(" ON CONFLICT(Id) DO UPDATE set Version=").AppendParam(targetVersion).Append(",Level=").AppendParam(level).Append(",OrgId=").AppendParam(orgId);
                var rs = await sqlbb.DoAsync<DoExecSql>();
                return rs.RowCount;
            }
            else
            {
                var sqlbb = new SqlBuilder(help).Append("INSERT INTO mz_iot_update(Id,Status,UpdateCount,UpdateErr,Version,Level,OrgId) ").Append("(select Id,0,0,'',")
    .AppendParam(targetVersion).Append(",").AppendParam(level).Append(",").AppendParam(orgId).Append(" from mz_iot_device where Online=1 and ProductId=").AppendParam(productId).Append(")").Append(" ON DUPLICATE KEY UPDATE Version=").AppendParam(targetVersion).Append(",Level=").AppendParam(level).Append(",OrgId=").AppendParam(orgId);
                var rs = await sqlbb.DoAsync<DoExecSql>();
                return rs.RowCount;
            }
        }
        public virtual async Task<int> InsertDeviceUpdate(string id, int targetVersion, int level, long orgId)
        {
            if (this.GetSqlType() == SqlType.Sqlite)
            {
                var rs = await new SqlBuilder(help).Append("INSERT INTO mz_iot_update(Id,Status,UpdateCount,UpdateErr,Version,Level,OrgId) VALUES(").AppendParam(id).Append(",0,0,'',")
.AppendParam(targetVersion).Append(",").AppendParam(level).Append(",").AppendParam(orgId).Append(") ON CONFLICT(Id) DO UPDATE set Version=").AppendParam(targetVersion).Append(",Level=").AppendParam(level).Append(",OrgId=").AppendParam(orgId).DoAsync<DoExecSql>();
                return rs.RowCount;
            }
            else
            {
                var rs = await new SqlBuilder(help).Append("INSERT INTO mz_iot_update(Id,Status,UpdateCount,UpdateErr,Version,Level,OrgId) VALUES(").AppendParam(id).Append(",0,0,'',")
.AppendParam(targetVersion).Append(",").AppendParam(level).Append(",").AppendParam(orgId).Append(") ON DUPLICATE KEY UPDATE Version=").AppendParam(targetVersion).Append(",Level=").AppendParam(level).Append(",OrgId=").AppendParam(orgId).DoAsync<DoExecSql>();
                return rs.RowCount;
            }
        }
        [Trans]
        public virtual async Task<List<MZ_IotUpdate>> SelectUpdateList(int top)
        {
            if (GetSqlType() == SqlType.Sqlite)
            {
                await new SqlBuilder(help).Append("update mz_iot_update set Status=1 where Id IN (SELECT Id FROM mz_iot_update WHERE Status = 0 ORDER BY Level DESC LIMIT " + top + ")").DoAsync<DoExecSql>();
                return await new SqlBuilder(help).Query<MZ_IotUpdate>().Where(x => x.Status == 1).ToListAsync();
            }
            else
            {
                await new SqlBuilder(help).Append("update mz_iot_update set Status=1 where Status=0 order by Level limit " + top).DoAsync<DoExecSql>();
                return await new SqlBuilder(help).Query<MZ_IotUpdate>().Where(x => x.Status == 1).ToListAsync();
            }
        }
        public virtual async Task<PageObject<Out_UpdateItem>> SelectUpdateListPage(In_UpdatePage query)
        {
            return await new SqlBuilder(help).Query<Out_UpdateItem>().Append("select u.*,d.Name,d.ProductId,p.Name as ProductName from mz_iot_update u left join mz_iot_device d on u.Id = d.Id left join mz_iot_product p on d.ProductId=p.Id where u.OrgId=")
    .AppendParam(query.OrgId)
    .Then(!string.IsNullOrEmpty(query.ProductId), sq => sq.Append(" and d.ProductId=").AppendParam(query.ProductId)).GeneratePageObjectAsync(query);
        }
        public virtual async Task UpdateToNewest(long orgId)
        {
            if (GetSqlType() == SqlType.Sqlite)
            {
                await new SqlBuilder(help).Append("UPDATE mz_iot_device SET ProductVer = (SELECT b.Version FROM mz_iot_update b WHERE b.Id=mz_iot_device.Id AND b.OrgId=" + orgId + " AND b.Status=2) WHERE Id IN (SELECT b.Id FROM mz_iot_update b WHERE mz_iot_device.Id=b.Id AND b.OrgId=" + orgId + " AND b.Status=2)").DoAsync<DoExecSql>();
            }
            else
            {
                await new SqlBuilder(help).Append("update mz_iot_device a INNER JOIN mz_iot_update b on a.Id=b.Id set a.ProductVer=b.Version where b.OrgId=" + orgId + " and b.Status=2").DoAsync<DoExecSql>();
            }
        }
    }
}
