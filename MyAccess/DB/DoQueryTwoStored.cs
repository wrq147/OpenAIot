using System;
using System.Data.Common;
namespace MyAccess.DB
{
    public class DoQueryTwoStored<T1, T2> : DoQueryStored where T1 : IDoResult<DbDataReader>, new() where T2 : IDoResult<DbDataReader>, new()
    {
        public DoQueryTwoStored() : base(new T1(), new T2())
        {
        }
        public T1 First
        {
            get
            {
                return GetCommand<T1>(0);
            }
        }
        public T2 Second
        {
            get
            {
                return GetCommand<T2>(1);
            }
        }
    }
}
