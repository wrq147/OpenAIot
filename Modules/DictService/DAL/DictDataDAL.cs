
using DictService.Model;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;

namespace DictService.DAL
{
    public class DictDataDAL : BaseDbSupport
    {
        /// <summary>
        /// 根据条件分页查询字典数据
        /// </summary>
        /// <param name="dictData"></param>
        /// <returns></returns>
        public virtual async Task<List<MZ_DictData>> SelectDictDataList(In_DictDataList dictData)
        {
            var sql = new SqlBuilder(help).Append("select dict_code, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, create_time, remark from mz_dict_data where 1=1 ")
                .Then(!string.IsNullOrEmpty(dictData.dictType), sql =>
                {
                    sql.Append(" AND dict_type = ").AppendParam(dictData.dictType);
                })
                .Then(!string.IsNullOrEmpty(dictData.dictName), sql =>
                {
                    sql.Append(" AND dict_label like concat('%', ").AppendParam(dictData.dictName).Append(", '%')");
                })
                .Then(!string.IsNullOrEmpty(dictData.status), sql =>
                {
                    sql.Append(" AND status = ").AppendParam(dictData.status);
                })
                .Append(" order by dict_sort asc");

            return (await sql.DoAsync<DoQuerySql<MZ_DictData>>()).ToList();
        }


        /// <summary>
        /// 根据字典类型查询字典数据
        /// </summary>
        /// <param name="dictType"></param>
        /// <returns></returns>
        public virtual async Task<List<MZ_DictData>> SelectDictDataByType(string dictType)
        {
            SqlBuilder sql = new SqlBuilder(help).Append("select dict_code, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, create_time, remark from mz_dict_data where status = '0' and dict_type = ")
                .AppendParam(dictType)
                .Append(" order by dict_sort asc");

            return (await help.DoCommandAsync<DoQuerySql<MZ_DictData>>(sql)).ToList();
        }


        /// <summary>
        /// 根据字典类型和字典键值查询字典数据信息
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public virtual async Task<string> SelectDictLabel(string dictType, string dictValue)
        {
            SqlBuilder sql = new SqlBuilder(help).Append("select dict_label from mz_dict_data where dict_type=").AppendParam(dictType).Append(" and dict_value = ").Append(dictValue);
            return (await help.DoCommandAsync<DoQuerySql<string>>(sql)).ToFirst();
        }

        /// <summary>
        /// 根据字典数据ID查询信息
        /// </summary>
        /// <param name="dictCode"></param>
        /// <returns></returns>
        public virtual async Task<MZ_DictData> SelectDictDataById(long dictCode)
        {
            return (await new SqlBuilder(help).Append("select dict_code, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, create_time, remark  from mz_dict_data where dict_code =")
                .AppendParam(dictCode).DoAsync<DoQuerySql<MZ_DictData>>()).ToFirst();
        }

        /// <summary>
        /// 查询字典数据
        /// </summary>
        /// <param name="dictType"></param>
        /// <returns></returns>
        public virtual async Task<int> CountDictDataByType(string dictType)
        {
            return (await new SqlBuilder(help).Append("select count(1) from mz_dict_data where dict_type=").AppendParam(dictType).DoAsync<DoQueryScalar>()).GetValueInt(0);
        }

        /// <summary>
        /// 通过字典ID删除字典数据信息
        /// </summary>
        /// <param name="dictCode"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteDictDataById(long dictCode)
        {
            try
            {
                return (await new SqlBuilder(help).Delete<MZ_DictData>("dict_code =").AppendParam(dictCode).DoAsync<DoExecSql>()).RowCount;
            }
            catch
            {
                return -1;
            }
        }


        /// <summary>
        /// 批量删除字典数据信息
        /// </summary>
        /// <param name="dictCodes"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteDictDataByIds(long[] dictCodes)
        {
            try
            {
                return (await new SqlBuilder(help).Delete<MZ_DictData>("dict_code in (").AppendParam(dictCodes).Append(")").DoAsync<DoExecSql>()).RowCount;
            }
            catch
            {
                return -1;
            }

        }

        /// <summary>
        /// 新增字典数据信息
        /// </summary>
        /// <param name="dictData"></param>
        /// <returns></returns>
        public virtual async Task<int> InsertDictData(MZ_DictData dictData)
        {
            try
            {
                return (await new SqlBuilder(help).Insert(dictData).DoAsync<DoExecSql>()).RowCount;
            }
            catch
            {
                return -1;
            }

        }


        /// <summary>
        /// 修改字典数据信息
        /// </summary>
        /// <param name="dictData"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateDictData(MZ_DictData dictData)
        {
            try
            {
                return (await new SqlBuilder(help).Update(dictData).DoAsync<DoExecSql>()).RowCount;
            }
            catch
            {
                return -1;
            }

        }

        /// <summary>
        /// 同步修改字典类型
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateDictDataType(string oldDictType, string newDictType)
        {
            try
            {
                return (await new SqlBuilder(help)
                      .Append("update mz_dict_data set dict_type = ").AppendParam(newDictType).Append(" where dict_type = ").AppendParam(oldDictType)
                      .DoAsync<DoExecSql>()).RowCount;
            }
            catch
            {
                return -1;
            }

        }
        /// <summary>
        /// 清除默认值
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual async Task<int> ClearDefault(string t)
        {
            return (await new SqlBuilder(help)
             .Append("update mz_dict_data set is_default='N' where dict_type=").AppendParam(t)
             .DoAsync<DoExecSql>()).RowCount;
        }

    }
}
