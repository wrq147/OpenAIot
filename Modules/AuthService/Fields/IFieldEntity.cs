using AuthService.Model;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Fields
{
    public interface IFieldEntity
    {
        string GetFormName();
        string GetFormId();
        /// <summary>
        /// 扩展对象
        /// </summary>
        Dictionary<string, object> ExtObjects { get; set; }
        /// <summary>
        /// 扩展值
        /// </summary>
        Dictionary<string, object> ExtVals { get; set; }
    }
}
