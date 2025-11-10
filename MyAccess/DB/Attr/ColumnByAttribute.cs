using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB.Attr
{
    /// <summary>
    /// 此字段关联的列
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class ColumnByAttribute : Attribute
    {
        private string _className;
        public string ClassName { get { return _className; } }
        private string _columnName;
        public string ColumnName { get { return _columnName; } }
        public ColumnByAttribute(Type classType, string columnName)
        {
            _className = classType.Name;
            _columnName = columnName;
        }
        public ColumnByAttribute(string columnName)
        {
            _className = string.Empty;
            _columnName = columnName;
        }
        public ColumnByAttribute(Type classType)
        {
            _className = classType.Name;
            _columnName = string.Empty;
        }
    }
}
