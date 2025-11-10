using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    public interface IDbHelp
    {
        /// <summary>
        /// 是否正在事务中
        /// </summary>
        /// <returns></returns>
        bool IsTrans();
        void BeginTran();
        Task BeginTranAsync();
        void Commit();
        Task CommitAsync();
        void RollBack();
        Task RollBackAsync();
        void Close();
        Task CloseAsync();
    }
}
