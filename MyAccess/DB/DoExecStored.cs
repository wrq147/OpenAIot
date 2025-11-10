using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    /// <summary>
    /// 执行指定存储过程
    /// </summary>
    public class DoExecStored : IDoCommand
    {
        protected int mRowCount;

        /// <summary>
        /// 影响的行数
        /// </summary>
        public int RowCount
        {
            get { return mRowCount; }
        }
   
        public void Excute(ExcuteParam p)
        {
            p.Command.CommandType = CommandType.StoredProcedure;
            p.Command.CommandText = p.Sql;
            mRowCount = p.Command.ExecuteNonQuery();
        }

        public async Task ExcuteAsync(ExcuteParam p)
        {
            p.Command.CommandType = CommandType.StoredProcedure;
            p.Command.CommandText = p.Sql;
            mRowCount = await p.Command.ExecuteNonQueryAsync();
        }
    }
}
