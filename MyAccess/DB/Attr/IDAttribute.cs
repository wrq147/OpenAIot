using System;

namespace MyAccess.DB.Attr
{
    /// <summary>
    /// 标识ID作用
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class IDAttribute : Attribute
    {
        private bool _auto;
        /// <summary>
        /// 判断是否为自增ID
        /// </summary>
        public bool IsAuto { get { return _auto; } }
        private string _seqName;

        /// <summary>
        /// 序列名
        /// </summary>
        public string SeqName { get { return _seqName; } }
        /// <summary>
        /// 可设置是否为自增
        /// </summary>
        /// <param name="auto"></param>
        /// <param name="seqName">oracle序列用</param>
        public IDAttribute(bool auto = false, string seqName = "")
        {
            _auto = auto;
            _seqName = seqName;
        }
    }
}
