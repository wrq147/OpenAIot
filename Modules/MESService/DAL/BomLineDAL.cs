using Common;
using MESService.Model;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace MESService.DAL
{
    public class BomLineDAL : BaseRepository<MZ_BomLine>
    {
        public virtual async Task<List<Out_BomTreeItem>> ListTree(string id)
        {
            var tmpSql = new SqlBuilder(help).Append(@"WITH RECURSIVE TmpBomTree AS (
    SELECT Id as ProductId,ProductName,'' as ParentProductId,1 AS Level FROM mz_product WHERE Id=").AppendParam(id).Append(@"
    UNION ALL
    SELECT d.ProductId,p.ProductName,d.ParentProductId,sd.Level+1
    FROM mz_bom_line d left join mz_product p on d.ProductId=p.Id
    JOIN TmpBomTree sd ON d.ParentProductId = sd.ProductId where d.ProductId<>").AppendParam(id).Append(@"
)
SELECT * FROM TmpBomTree;");
            return (await tmpSql.DoAsync<DoQuerySql<Out_BomTreeItem>>()).ToList();
        }

    }
}
