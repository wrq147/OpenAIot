using DictService.Model;
using System.Collections.Generic;
using MyAccess.DB;
using System;
using Common.Share;
using AuthService;
using Common;
using System.Threading.Tasks;

namespace DictService.DAL
{
    public class DictTypeDAL : BaseDbSupport
    {
        /// <summary>
        /// 根据条件分页查询字典类型
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual async Task<PageObject<MZ_DictType>> SelectDictTypeList(In_DictTypeList query)
        {
            var sb = new SqlBuilder(help).Query<MZ_DictType>().Append("select dict_id, dict_name, dict_type, status, create_time, remark from mz_dict_type where 1=1");
            if (!string.IsNullOrEmpty(query.dictName))
            {
                help.AddParam("@dictName", query.dictName);
                sb.Append(" AND dict_name like concat('%', @dictName, '%')");
            }
            if (!string.IsNullOrEmpty(query.status))
            {
                help.AddParam("@status", query.status);
                sb.Append(" AND status = @status");
            }
            if (!string.IsNullOrEmpty(query.dictType))
            {
                help.AddParam("@dictType", query.dictType);
                sb.Append(" AND dict_type like concat('%', @dictType, '%')");
            }
            if (query.beginTime != null)
            {
                help.AddParam("@beginTime", query.beginTime);
                sb.Append(" and create_time >=@beginTime");
            }
            if (query.endTime != null)
            {
                help.AddParam("@endTime", query.endTime);
                sb.Append(" and create_time <= @endTime");
            }
            return await sb.GeneratePageObjectAsync(query);

        }

        /// <summary>
        /// 根据所有字典类型
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<MZ_DictType>> SelectDictTypeAll()
        {
            SqlBuilder sql = new SqlBuilder(help).Append("select dict_id, dict_name, dict_type, status, create_time, remark from mz_dict_type");
            return (await help.DoCommandAsync<DoQuerySql<MZ_DictType>>(sql)).ToList();
        }

        /// <summary>
        /// 根据字典类型ID查询信息
        /// </summary>
        /// <param name="dictId"></param>
        /// <returns></returns>
        public virtual async Task<MZ_DictType> SelectDictTypeById(long dictId)
        {
            help.AddParam("@dictId", dictId);
            SqlBuilder sql = new SqlBuilder(help).Append("select dict_id, dict_name, dict_type, status, create_time, remark from mz_dict_type where dict_id = @dictId");
            return (await help.DoCommandAsync<DoQuerySql<MZ_DictType>>(sql)).ToFirst();
        }

        /// <summary>
        /// 根据字典类型查询信息
        /// </summary>
        /// <param name="dictType"></param>
        /// <returns></returns>
        public virtual async Task<MZ_DictType> SelectDictTypeByType(string dictType)
        {
            help.AddParam("@dictType", dictType);
            SqlBuilder sql = new SqlBuilder(help).Append("select dict_id, dict_name, dict_type, status, create_time, remark from mz_dict_type where dict_type = @dictType");
            return (await help.DoCommandAsync<DoQuerySql<MZ_DictType>>(sql)).ToFirst();
        }

        /// <summary>
        /// 通过字典ID删除字典信息
        /// </summary>
        /// <param name="dictId"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteDictTypeById(long dictId)
        {
            try
            {
                return (await new SqlBuilder(help).Delete<MZ_DictType>("dict_id = ").AppendParam(dictId).DoAsync<DoExecSql>()).RowCount;
            }
            catch
            {
                return -1;
            }
        }
        /// <summary>
        /// 批量删除字典类型信息
        /// </summary>
        /// <param name="dictIds"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteDictTypeByIds(long[] dictIds)
        {
            try
            {
                return (await new SqlBuilder(help).Delete<MZ_DictType>("dict_id in (").AppendParam(dictIds).Append(")").DoAsync<DoExecSql>()).RowCount;
            }
            catch
            {
                return -1;
            }
        }


        /// <summary>
        /// 新增字典类型信息
        /// </summary>
        /// <param name="dictType"></param>
        /// <returns></returns>
        public virtual async Task<int> InsertDictType(MZ_DictType dictType)
        {
            try
            {
                dictType.create_time = DateTime.Now;
                dictType.update_time = dictType.create_time;
                dictType.remark ??= string.Empty;

                return (await new SqlBuilder(help).Insert(dictType).DoAsync<DoExecSql>()).RowCount;
            }
            catch(Exception ex)
            {
                return -1;
            }

        }

        /// <summary>
        /// 修改字典类型信息
        /// </summary>
        /// <param name="dictType"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateDictType(MZ_DictType dictType)
        {
            try
            {
                dictType.update_time = DateTime.Now;

                return (await new SqlBuilder(help).Update(dictType).DoAsync<DoExecSql>()).RowCount;
            }
            catch
            {
                return -1;
            }

        }


        /// <summary>
        /// 校验字典类型称是否唯一
        /// </summary>
        /// <param name="dictType"></param>
        /// <returns>true表示唯一</returns>
        public virtual async Task<bool> CheckDictTypeUnique(string dictType, long filterId = -1)
        {
            var sql = new SqlBuilder(help).Append("select dict_id from mz_dict_type where dict_type =")
                .AppendParam(dictType)
                .Then(filterId > 0, tsql =>
                {
                    tsql.Append(" and dict_id<>").AppendParam(filterId);
                })
                .Append(" limit 1");
            return (await sql.DoAsync<DoQuerySql<MZ_DictType>>()).Count == 0;
        }
    }
}
