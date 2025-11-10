using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using Newtonsoft.Json;
namespace Common.Share
{
    public class DefaultAjaxResult<T> : AjaxResult
    {
        /// <summary>
        /// 返回代码，为0为正确，1~9不提示，大于9提示
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 返回的值
        /// </summary>
        public T data { get; set; }
        public DefaultAjaxResult() { }
        public DefaultAjaxResult(int code, string message)
        {
            this.code = code;
            this.message = message;
        }
        public DefaultAjaxResult(int code, string message, T data)
        {
            this.code = code;
            this.message = message;
            this.data = data;
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Constants.ApiJsonSetting);
        }
    }
}
