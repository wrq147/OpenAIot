using Common;
using Common.Share;
using IoTService.Models;
using MyAccess.DB;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTService.DAL
{
    public class IotProductDAL : BaseRepository<MZ_IotProduct>
    {
        public virtual async Task<List<MZ_IotProduct>> SelectProductTSL(string[] ids)
        {
            if (ids.Length == 0)
            {
                return new List<MZ_IotProduct>();
            }
            return await new SqlBuilder(help).Query<MZ_IotProduct>().Where(x => ids.Contains(x.Id)).ToListAsync();
        }
        public virtual async Task<MZ_IotProduct> SelectWithNoTsl(string id)
        {
            return await new SqlBuilder(help).Query<MZ_IotProduct>().Append("select createId,updateId,create_time,update_time,Id,OrgId,Name,PhotoUrl,Remark,ClassifiedId,NetworkWay,NoticeWay,Status,PhysicsWay,Version,PublicTime,StorageConfig from mz_iot_product where Id=").AppendParam(id).ToFirstAsync();
        }
        public virtual async Task<PageObject<MZ_IotProduct>> SelectWithClassPage(In_ProductListPage query)
        {
            return await new SqlBuilder(help).Query<MZ_IotProduct>().Append("select p.createId,p.updateId,p.create_time,p.update_time,p.Id,p.OrgId,p.Name,p.PhotoUrl,p.Remark,p.ClassifiedId,p.NetworkWay,p.NoticeWay,p.Status,p.PhysicsWay,p.Version,p.PublicTime,c.Name as ClassName from mz_iot_product p left join mz_iot_class c on p.ClassifiedId = c.Id where p.OrgId=")
                .AppendParam(query.OrgId)
            .Then(query.Name != null, sql =>
            {
                sql.Append(" and p.Name like ").AppendParam("%" + query.Name.SqlLikeFilter() + "%");
            })
            .Then(query.Ids != null && query.Ids.Length > 0, sq => sq.Append(" and p.Id in (").AppendParam(query.Ids).Append(")"))
            .Then(query.IsNet == true, sq => sq.Append(" and NetworkWay<>''"))
            .Then(query.IsNet == false, sq => sq.Append(" and NetworkWay=''"))
            .Then(query.ClassPath != null, sq => sq.Append(" and c.Path like ").AppendParam(query.ClassPath + "%"))
            .Then(query.Status != null, sq => sq.Append(" and p.Status=").AppendParam(query.Status))
            .Then(query.beginTime != null, sq => sq.Append(" and p.create_time >= ").AppendParam(query.beginTime))
            .Then(query.endTime != null, sq => sq.Append(" and p.create_time <= ").AppendParam(query.endTime))
            .GeneratePageObjectAsync(query, "p.update_time desc");
        }
        public virtual async Task<PageObject<Out_ProductName>> SelectNames(In_ProductNamePage query, IUserInfo user)
        {
            return await new SqlBuilder(help).Query<Out_ProductName>().Append("select Id,Name,PhotoUrl from mz_iot_product where OrgId=").AppendParam(user.OrgId)
                .Then(!string.IsNullOrEmpty(query.ClassId), sql =>
                {
                    sql.Append(" and ClassifiedId=").AppendParam(query.ClassId);
                }).GeneratePageObjectAsync(query);
        }
        public virtual async Task<MZ_IotProduct> SelectProductView(string id)
        {
            return await new SqlBuilder(help).Query<MZ_IotProduct>().Append("select * from mz_iot_product_v where Id=").AppendParam(id).ToFirstAsync();
        }
    }
}
