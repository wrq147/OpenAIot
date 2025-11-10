using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    /// <summary>
    /// 获取唯一返回值
    /// </summary>
    public class DoQueryScalar : IDoCommand
    {
        private object mValue;

        public double GetValueDouble(double def = 0)
        {
            double rt = def;
            if (mValue != null)
            {
                if (!double.TryParse(mValue.ToString(), out rt))
                {
                    rt = def;
                }
            }
            return rt;
        }
        public decimal GetValueDecimal(decimal def = 0)
        {
            decimal rt = def;
            if (mValue != null)
            {
                if (!decimal.TryParse(mValue.ToString(), out rt))
                {
                    rt = def;
                }
            }
            return rt;
        }
        public long GetValueLong(long def = 0)
        {
            long rt = def;
            if (mValue != null)
            {
                if (!long.TryParse(mValue.ToString(), out rt))
                {
                    rt = def;
                }
            }
            return rt;
        }
        public int GetValueInt(int def = 0)
        {
            int rt = def;
            if (mValue != null)
            {
                if (!int.TryParse(mValue.ToString(), out rt))
                {
                    rt = def;
                }
            }
            return rt;
        }
        public string GetValue(string def = null)
        {
            if (mValue != null)
            {
                return mValue.ToString();
            }
            return def;
        }
        public DoQueryScalar() { }
    
        public void Excute(ExcuteParam p)
        {
            p.Command.CommandType = CommandType.Text;
            p.Command.CommandText = p.Sql;
            mValue = p.Command.ExecuteScalar();
        }

        public async Task ExcuteAsync(ExcuteParam p)
        {
            p.Command.CommandType = CommandType.Text;
            p.Command.CommandText = p.Sql;
            mValue = await p.Command.ExecuteScalarAsync();
        }

    }
}
