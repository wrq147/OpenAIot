using System;
namespace MyAccess.DB.Attr
{
    /// <summary>
    /// 映射的表名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class TableNameAttribute : Attribute
    {
        private string _name;

        public TableNameAttribute(string name)
        {
            _name = name;
        }
        /// <summary>
        /// 表名称
        /// </summary>
        public string Name { get { return _name; } }
    }
}
