using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    public class DoQueryStored : IDoCommand
    {
        private Dictionary<string, object> mOutDict;
        private IDoResult<DbDataReader>[] _results;
        public T GetCommand<T>(int index) where T : IDoResult<DbDataReader>
        {
            if (index < _results.Length)
            {
                return (T)_results[index];
            }
            else
            {
                return default(T);
            }
        }
        public DoQueryStored(params IDoResult<DbDataReader>[] results)
        {
            mOutDict = new Dictionary<string, object>();
            _results = results;
        }
        public int OutInt(string key)
        {
            if (mOutDict.ContainsKey(key))
            {
                string inputval = mOutDict[key].ToString();
                int rt = 0;
                if (int.TryParse(inputval, out rt))
                {
                    return rt;
                }
            }
            return 0;
        }
        public string OutStr(string key)
        {
            if (mOutDict.ContainsKey(key))
            {
                return mOutDict[key].ToString();
            }
            return "";
        }
        /// <summary>
        /// 获取输出参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get { return mOutDict[key]; }
        }

        private void AfterExcute(DbCommand command)
        {
            foreach (DbParameter dbp in command.Parameters)
            {
                if (dbp.Direction == ParameterDirection.InputOutput || dbp.Direction == ParameterDirection.Output)
                {
                    mOutDict.Add(dbp.ParameterName, dbp.Value);
                }
            }
        }
        public void Excute(ExcuteParam p)
        {
            p.Command.CommandType = CommandType.StoredProcedure;
            p.Command.CommandText = p.Sql;
            if (_results.Length > 0)
            {
                DbDataReader dataReader = p.Command.ExecuteReader();
                using (dataReader)
                {
                    bool drbl = true;
                    int i = 0;
                    while (drbl && _results.Length > i)
                    {
                        while (dataReader.Read())
                        {
                            _results[i].SetResult(dataReader);
                        }
                        drbl = dataReader.NextResult();
                        ++i;
                    }
                }
            }
   
            AfterExcute(p.Command);
        }

        public async Task ExcuteAsync(ExcuteParam p)
        {
            p.Command.CommandType = CommandType.StoredProcedure;
            p.Command.CommandText = p.Sql;
            if (_results.Length > 0)
            {
                DbDataReader dataReader = await p.Command.ExecuteReaderAsync();
                using (dataReader)
                {
                    bool drbl = true;
                    int i = 0;
                    while (drbl && _results.Length > i)
                    {
                        while (await dataReader.ReadAsync())
                        {
                            await _results[i].SetResultAsync(dataReader);
                        }
                        drbl = dataReader.NextResult();
                        ++i;
                    }
                }
            }
       
            AfterExcute(p.Command);
        }
    }
}
