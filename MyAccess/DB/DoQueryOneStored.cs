using System;
using System.Data.Common;

namespace MyAccess.DB
{
    public class DoQueryOneStored<T1> : DoQueryStored where T1 : IDoResult<DbDataReader>, new()
    {
        public DoQueryOneStored() : base(new T1())
        {
        }
        public T1 First
        {
            get
            {
                return GetCommand<T1>(0);
            }
        }
    }
}
