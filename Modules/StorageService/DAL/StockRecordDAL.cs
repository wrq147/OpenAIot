using AuthService;
using Common;
using Common.Share;
using StorageService.Model;
using MyAccess.DB;
using System.Linq.Expressions;
using MyAccess.DB.Builder.WhereToSql;
using System.Threading.Tasks;
using System;
namespace StorageService.DAL
{
    public class StockRecordDAL : BaseRepository<MZ_StockRecord>
    {
        public virtual async Task<PageObject<MZ_StockRecord>> SelectByPage(In_StockRecordPage query)
        {
            Expression<Func<MZ_StockRecord, bool>> expression = x => x.HouseId == query.HouseId && x.TargetType == query.TargetType && x.TargetId == query.TargetId;

            if (query.beginTime != null)
            {
                expression = expression.And(x => x.CreatedOn >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And(x => x.CreatedOn <= query.endTime);
            }
            return await new SqlBuilder(help).Query<MZ_StockRecord>()
                .Where(expression).GeneratePageObjectAsync(query, "CreatedOn desc");
        }
        public virtual async Task<PageObject<Out_StockRecord>> SelectDetailByPage(In_DetailRecordPage query, Data_ServerTokenInfo user, DataScope scope)
        {
            var sqlbuilder = new SqlBuilder(help).Query<Out_StockRecord>().Append("select * from mz_stockrecord_v where OrgId=").AppendParam(user.OrgId)
                .Then(query.beginTime != null, sq => sq.Append(" and CreatedOn >= ").AppendParam(query.beginTime))
                .Then(query.endTime != null, sq => sq.Append(" and CreatedOn <= ").AppendParam(query.endTime))
                .Then(scope != null, sql => sql.Append(" and HouseId in (select Id from mz_store_house where OrgId=" + user.OrgId + scope.GenerateFilter("DeptId", "LeaderId") + ")"));

            return await sqlbuilder.GeneratePageObjectAsync(query, "CreatedOn desc");
        }
        public virtual async Task<int> InsertRecord(long orgId, string houseId, int t, string id, int formType, string formId, decimal quantity, decimal price)
        {
            string curtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return (await new SqlBuilder(help).Append(@"INSERT INTO mz_stock_record(OrgId,HouseId,TargetType,TargetId,Remnant,LockRemnant,FormType,FormId,Quantity,StockPrice,Price,CreatedOn)
            (select " + orgId + ",'" + houseId + "'," + t + ",'" + id + "',Quantity,LockQuantity," + formType + ",'" + formId + "'," + quantity + ",Price," + price + ",'"+ curtime + "' from mz_stock_pile where HouseId=")
                .AppendParam(houseId).Append(" and TargetType=").AppendParam(t).Append(" and TargetId=").AppendParam(id)
                .Append(" UNION select " + orgId + ",'" + houseId + "'," + t + ",'" + id + "',0,0," + formType + ",'" + formId + "'," + quantity + ",0," + price + ",'"+ curtime + "' WHERE NOT EXISTS (SELECT 1 FROM mz_stock_pile WHERE  HouseId=").AppendParam(houseId).Append(" and TargetType=").AppendParam(t).Append(" and TargetId=").AppendParam(id).Append("))")
                .DoAsync<DoExecSql>()).RowCount;
        }
    }
}
