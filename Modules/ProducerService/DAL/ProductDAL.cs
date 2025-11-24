using AuthService;
using Common;
using Common.Share;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using ProducerService.Controller;
using ProducerService.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TemplateAction.Label;

namespace ProducerService.DAL
{
    public class ProductDAL : BaseRepository<MZ_Product>
    {
        public virtual async Task<PageObject<MZ_Product>> SelectByPage(In_ProductList query, long orgId, bool isAgent)
        {
            Expression<Func<MZ_Product, MZ_ProductType, bool>> expression = (a, b) => a.OrgId == orgId;
            if (isAgent == true)
            {
                expression = (a, b) => SonSqlFun.SqlCondition(" a.OrgId in (select FactoryId from mz_agent where OrgId = " + orgId + ") ");
            }
            else
            {
                expression = (a, b) => a.OrgId == orgId;
            }
            if (!string.IsNullOrEmpty(query.Key))
            {
                expression = expression.And((a, b) => a.ProductName.Contains(query.Key) || a.SkuNumber.Contains(query.Key));
            }
            if (!string.IsNullOrEmpty(query.TypeId))
            {
                expression = expression.And((a, b) => a.TypeId == query.TypeId);
            }
            if (!string.IsNullOrEmpty(query.Prop))
            {
                expression = expression.And((a, b) => a.Prop == query.Prop);
            }
            if (!string.IsNullOrEmpty(query.NoId))
            {
                expression = expression.And((a, b) => a.Id != query.NoId);
            }
            if (query.IsRoute == true)
            {
                expression = expression.And((a, b) => a.Route != "");
            }
            if (query.IsIot == true)
            {
                expression = expression.And((a, b) => a.IOTProductId != "");
            }
            if (query.beginTime != null)
            {
                expression = expression.And((a, b) => a.create_time >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And((a, b) => a.create_time <= query.endTime);
            }
            var tmpSql = new SqlBuilder(help).Query<MZ_Product>().LeftJoin<MZ_ProductType>((a, b) => a.TypeId == b.Id).Where(expression, "a.*,b.Name as TypeName");
            if (query.Items != null && query.Items.Length > 0)
            {
                //过滤扩展字段
                foreach (var item in query.Items)
                {
                    item.AppendFilter(tmpSql, "a.");
                }
            }

            return await tmpSql.GeneratePageObjectAsync(query, "a.create_time desc");
        }
        public virtual async Task<MZ_Product> SelectWithTypeById(string id)
        {
            return await new SqlBuilder(help).Query<MZ_Product>().LeftJoin<MZ_ProductType>((a, b) => a.TypeId == b.Id).Where((a, b) => a.Id == id, "a.*,b.Name as TypeName").ToFirstAsync();
        }
        public virtual async Task<T_IotProduct> SelectIotProduct(long orgId, string iotproid)
        {
            return await new SqlBuilder(help).Query<T_IotProduct>().Where(a => a.OrgId == orgId && a.Id == iotproid).ToFirstAsync();
        }
        public virtual async Task<T_IotProduct> SelectIotProductById(string iotproid)
        {
            return await new SqlBuilder(help).Query<T_IotProduct>().Where(a => a.Id == iotproid).ToFirstAsync();
        }
        public virtual async Task<List<T_IotProduct>> SelectIotProductList(List<string> ids)
        {
            return await new SqlBuilder(help).Query<T_IotProduct>().Where(a => ids.Contains(a.Id)).ToListAsync();
        }
        public virtual async Task<List<T_ProductRoute>> SelectProductRouteList(List<string> ids)
        {
            return await new SqlBuilder(help).Query<T_ProductRoute>().Where(a => ids.Contains(a.Id)).ToListAsync();
        }
        public virtual async Task<T_ProductRoute> SelectProductRouteById(string id)
        {
            return await new SqlBuilder(help).Query<T_ProductRoute>().Where(a => a.Id == id).ToFirstAsync();
        }

    }
}
