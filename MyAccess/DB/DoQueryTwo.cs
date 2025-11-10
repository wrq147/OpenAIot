using System;

namespace MyAccess.DB
{
    /// <summary>
    /// 混合查询两个IDoCommand
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class DoQueryTwo<T1, T2> : DoQueryGroup where T1 : IDoCommand,new() where T2 : IDoCommand, new()
    {
        public DoQueryTwo() : base(new T1(), new T2())
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
