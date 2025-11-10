using Castle.Core.Logging;
using Common;
using Common.Share;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using ProducerService.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TemplateAction.Label;

namespace ProducerService.DAL
{
    public class ProductBatchDAL : BaseRepository<MZ_ProductBatch>
    {
        public virtual async Task<List<V_ProductBatch>> SelectListByIds(List<string> ids)
        {
            return await new SqlBuilder(help).Query<V_ProductBatch>().Where(x => ids.Contains(x.Id)).ToListAsync();
        }
        public virtual async Task<MZ_ProductBatch> SelectProductByOnlyNumber(long orgId, string number)
        {
            return await new SqlBuilder(help).Query<MZ_ProductBatch>().Where(x => x.OrgId == orgId && x.Number == number).ToFirstAsync();
        }
        public virtual async Task<V_ProductBatch> SelectProductVByNumber(long orgId, string number)
        {
            return await new SqlBuilder(help).Query<V_ProductBatch>().Where(x => x.OrgId == orgId && (x.Number == number || x.LNumber == number)).ToFirstAsync();
        }
        public virtual async Task<V_ProductBatch> SelectProductVById(string id)
        {
            return await new SqlBuilder(help).Query<V_ProductBatch>().Where(x => x.Id == id).ToFirstAsync();
        }
        public virtual async Task<PageObject<V_ProductBatch>> SelectByPage(In_ProductBatchList query, long orgId)
        {
            Expression<Func<V_ProductBatch, bool>> expression = (a) => a.OrgId == orgId;
            if (!string.IsNullOrEmpty(query.Key))
            {
                expression = expression.And(a => a.Number.Contains(query.Key) || a.LNumber.Contains(query.Key) || a.BatchName.Contains(query.Key) || a.ProductName.Contains(query.Key) || a.SkuNumber.Contains(query.Key));
            }
            return await new SqlBuilder(help).Query<V_ProductBatch>().Where(expression).GeneratePageObjectAsync(query);
        }
        public virtual async Task<T_IotDevice> SelectIOTNumber(long orgId, string number)
        {
            var dqs = await new SqlBuilder(help).Append("select Id,OrgId,Name,ProductId,DeviceNumber,DeviceId,PhotoUrl,Remark from mz_iot_device where OrgId=" + orgId + " and DeviceNumber=").AppendParam(number).DoAsync<DoQuerySql<T_IotDevice>>();
            return dqs.ToFirst();
        }
        public virtual async Task<List<T_IotDevice>> NoExistInIOT(long orgId)
        {
            var sql = new SqlBuilder(help).Append("select Id,OrgId,Name,ProductId,DeviceNumber,DeviceId,PhotoUrl,Remark from mz_iot_device where OrgId=" + orgId + " and not EXISTS(select Id from mz_product_batch where Number=mz_iot_device.DeviceNumber)");
            var rs = await sql.DoAsync<DoQuerySql<T_IotDevice>>();
            return rs.ToList();
        }
        public virtual async Task<List<T_IotDevice>> SelectIOTList(string[] ids)
        {
            var sql = new SqlBuilder(help).Append("select Id,OrgId,Name,ProductId,DeviceNumber,DeviceId,PhotoUrl,Remark from mz_iot_device where Id in (").AppendParam(ids).Append(")");
            var rs = await sql.DoAsync<DoQuerySql<T_IotDevice>>();
            return rs.ToList();
        }
        public virtual async Task<PageObject<T_IotDevice>> NoExistDevPage(In_NoExistDevParam query, long orgId)
        {
            Expression<Func<T_IotDevice, bool>> expression = (a) => a.OrgId == orgId && SonSqlFun.SqlCondition(" not EXISTS(select Id from mz_product_batch where OrgId=" + orgId + " and Number=mz_iot_device.DeviceNumber)");
            if (!string.IsNullOrEmpty(query.Key))
            {
                expression = expression.And(a => a.DeviceId.Contains(query.Key) || a.Name.Contains(query.Key) || a.DeviceNumber.Contains(query.Key));
            }
            return await new SqlBuilder(help).Query<T_IotDevice>().Where(expression).GeneratePageObjectAsync(query);
        }
        public virtual async Task<int> DeleteByNumber(string pid)
        {
            var sql = new SqlBuilder(help).Append("delete from mz_product_batch where ProductId=").AppendParam(pid).Append(" and EXISTS(select Id from mz_iot_device where DeviceNumber=mz_product_batch.Number)");
            return (await sql.DoAsync<DoExecSql>()).RowCount;
        }

    }
}
