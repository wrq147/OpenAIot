using MyAccess.DB;
using Common.Share;
using Common;
using System.Threading.Tasks;

namespace AuthService
{
    public class ConfigDAL : BaseRepository<MZ_Config>
    {
        /// <summary>
        /// 查询参数配置信息
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task<MZ_Config> SelectConfig(MZ_Config config)
        {
            using DbHelp db = CreateDB();
            return (await new SqlBuilder(db).Append("select * from mz_config where 1=1")
                .Then(config.config_id != null, sql =>
                {
                    sql.Append(" and config_id = ").AppendParam(config.config_id);
                })
                .Then(!string.IsNullOrEmpty(config.config_key), sql =>
                {
                    sql.Append(" AND config_key = ").AppendParam(config.config_key);
                })
                .DoAsync<DoQuerySql<MZ_Config>>()).ToFirst();
        }


        /// <summary>
        /// 查询参数配置列表
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task<PageObject<MZ_Config>> SelectConfigList(In_ConfigList query)
        {
            using DbHelp db = CreateDB();

            return await new SqlBuilder(db).Query<MZ_Config>().Append("select * from mz_config where 1=1")
                .Then(!string.IsNullOrEmpty(query.configName), sql =>
                {
                    sql.Append("	AND config_name like concat('%', ").AppendParam(query.configName).Append(", '%')");
                })
                .Then(!string.IsNullOrEmpty(query.configType), sql =>
                {
                    sql.Append(" AND config_type = ").AppendParam(query.configType);
                })
                .Then(!string.IsNullOrEmpty(query.configKey),sql=> {
                    sql.Append("	AND config_key like concat('%', ").AppendParam(query.configKey).Append(", '%')");
                })
                .Then(query.beginTime != null, sql=> {
                    sql.Append(" and create_time >= ").AppendParam(query.beginTime);
                })
                .Then(query.endTime != null, sql=> {
                    sql.Append(" and create_time <= ").AppendParam(query.endTime);
                })
                .GeneratePageObjectAsync(query, "config_id desc");
        }


        /// <summary>
        /// 根据键名查询参数配置信息
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        public async Task<MZ_Config> CheckConfigKeyUnique(string configKey)
        {
            using DbHelp db = CreateDB();
            return (await new SqlBuilder(db).Append("select config_id from mz_config where config_key=").AppendParam(configKey).Append(" limit 1")
                .DoAsync<DoQuerySql<MZ_Config>>()).ToFirst();
        }


        /// <summary>
        /// 删除参数配置
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public async Task<int> DeleteConfigById(int configId)
        {
            using DbHelp db = CreateDB();
            return (await new SqlBuilder(db).Delete<MZ_Config>("config_id=").AppendParam(configId)
                .DoAsync<DoExecSql>()).RowCount;

        }


        /// <summary>
        /// 批量删除参数信息
        /// </summary>
        /// <param name="configIds"></param>
        /// <returns></returns>
        public int DeleteConfigByIds(int[] configIds)
        {
            using DbHelp db = CreateDB();
            return new SqlBuilder(db).Delete<MZ_Config>("config_id in (").AppendParam(configIds).Append(")")
                .Do<DoExecSql>().RowCount;
        }
    }
}
