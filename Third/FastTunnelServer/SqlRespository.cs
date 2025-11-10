using Microsoft.Extensions.Options;
using MyAccess.DB;
using MyAccess.MySql;

namespace FastTunnelServer
{
    public class SqlRespository
    {
        private IOptions<GeneralOption> _option;
        public SqlRespository(IOptions<GeneralOption> option)
        {
            _option = option;
        }
        private DbHelp CreateDB()
        {
            return new MySqlHelp(_option.Value.connstr);
        }

        public virtual async Task<MZ_IotDevice> InfoByDtuId(string dtuId)
        {
            return await new SqlBuilder(CreateDB()).Query<MZ_IotDevice>().Where(x => x.DeviceId == dtuId).ToFirstAsync();
        }
        public virtual async Task<MZ_Developer> InfoByOrg(long orgId)
        {
            return await new SqlBuilder(CreateDB()).Query<MZ_Developer>().Where(x => x.OrgId == orgId).ToFirstAsync();
        }
    }
}
