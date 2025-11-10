using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJointUI.Models
{
    public class ApiResult<T>
    {
        /// <summary>
        /// 返回代码，为0为正确，1~9不提示，大于9提示
        /// </summary>
        public int code { get; set; }
        public string message { get; set; }
        public T data { get; set; }
    }
    public class ListObject<T>
    {
        public List<T> List { get; set; }
        public int Total { get; set; }
    }
}
