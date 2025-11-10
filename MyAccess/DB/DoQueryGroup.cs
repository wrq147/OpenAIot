using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    /// <summary>
    /// 多实体返回查询
    /// </summary>
    public class DoQueryGroup : IDoCommand
    {
        private IDoCommand[] _commands;
        public T GetCommand<T>(int index) where T : IDoCommand
        {
            if (index < _commands.Length)
            {
                return (T)_commands[index];
            }
            else
            {
                return default(T);
            }
        }


        public DoQueryGroup(params IDoCommand[] commands)
        {
            _commands = commands;
        }


        public void Excute(ExcuteParam p)
        {
            p.Command.CommandType = CommandType.Text;
            p.Command.CommandText = p.Sql;
            DbDataReader dataReader = p.Command.ExecuteReader();
            using (dataReader)
            {
                bool drbl = true;
                int i = 0;
                while (drbl && _commands.Length > i)
                {
                    IDoResult<DbDataReader> res = _commands[i] as IDoResult<DbDataReader>;
                    if (res == null)
                    {
                        ++i;
                        continue;
                    }
                    res.SetNavigate(p.Sql.SubMaps, p.Sql.SubTypeMaps);
                    while (dataReader.Read())
                    {
                        res.SetResult(dataReader);
                    }
                    drbl = dataReader.NextResult();
                    ++i;
                }
            }
        }

        public async Task ExcuteAsync(ExcuteParam p)
        {
            p.Command.CommandType = CommandType.Text;
            p.Command.CommandText = p.Sql;
            DbDataReader dataReader = await p.Command.ExecuteReaderAsync();
            using (dataReader)
            {
                bool drbl = true;
                int i = 0;
                while (drbl && _commands.Length > i)
                {
                    IDoResult<DbDataReader> res = _commands[i] as IDoResult<DbDataReader>;
                    if (res == null)
                    {
                        ++i;
                        continue;
                    }
                    res.SetNavigate(p.Sql.SubMaps, p.Sql.SubTypeMaps);
                    while (await dataReader.ReadAsync())
                    {
                        await res.SetResultAsync(dataReader);
                    }
                    drbl = dataReader.NextResult();
                    ++i;
                }
            }
        }

    }
}
