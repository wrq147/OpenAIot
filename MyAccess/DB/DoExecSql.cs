using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    /// <summary>
    /// 执行删除、更改时使用，返回被影响记录数
    /// </summary>
    public class DoExecSql : IDoCommand
    {
        private int mRowCount;
        /// <summary>
        /// 影响的行数
        /// </summary>
        public int RowCount
        {
            get { return mRowCount; }
        }

        public DoExecSql()
        {
            mRowCount = -1;
        }

        public virtual void Excute(ExcuteParam p)
        {
            p.Command.CommandType = CommandType.Text;
            p.Command.CommandText = p.Sql;
            mRowCount = p.Command.ExecuteNonQuery();
        }

        public virtual async Task ExcuteAsync(ExcuteParam p)
        {
            p.Command.CommandType = CommandType.Text;
            p.Command.CommandText = p.Sql;
            mRowCount = await p.Command.ExecuteNonQueryAsync();
        }

    }
}
