using Common.Share;
using Microsoft.Extensions.Options;
using MyAccess.DB;
using System;
namespace Common
{
    /// <summary>
    /// 系统数据库仓储（无通用API）
    /// </summary>
    public class BaseDbSupport : SqlSupport
    {
        public BaseDbSupport() : base(Constants.General.connstr)
        {
            if (!string.IsNullOrEmpty(Constants.General.sqltype))
            {
                this.SetSqlType(Enum.Parse<SqlType>(Constants.General.sqltype));
            }
        }
    }
}
