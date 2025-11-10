using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    public class DoQuerySql<T> : QueryResult<T>, IDoCommand
    {
        public DoQuerySql() { }
        public void Excute(ExcuteParam p)
        {
            this.SetNavigate(p.Sql.SubMaps, p.Sql.SubTypeMaps);
            p.Command.CommandType = CommandType.Text;
            p.Command.CommandText = p.Sql;
            DbDataReader dataReader = p.Command.ExecuteReader();
            using (dataReader)
            {
                bool drbl = true;
                while (drbl)
                {
                    while (dataReader.Read())
                    {
                        SetResult(dataReader);
                    }
                    drbl = dataReader.NextResult();
                }
            }
        }

        public async Task ExcuteAsync(ExcuteParam p)
        {
            this.SetNavigate(p.Sql.SubMaps, p.Sql.SubTypeMaps);
            p.Command.CommandType = CommandType.Text;
            p.Command.CommandText = p.Sql;
            DbDataReader dataReader = await p.Command.ExecuteReaderAsync();
            using (dataReader)
            {
                bool drbl = true;
                while (drbl)
                {
                    while (await dataReader.ReadAsync())
                    {
                        await SetResultAsync(dataReader);
                    }
                    drbl = dataReader.NextResult();
                }
            }
        }

    }
}
