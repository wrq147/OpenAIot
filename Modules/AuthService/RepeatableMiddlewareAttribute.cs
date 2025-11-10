using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Common;
using TemplateAction.Core;
using TemplateAction.Label;
using Common.Share;

namespace AuthService
{
    /// <summary>
    /// 防重复提交过滤器
    /// 10秒内不可重复提交算
    /// </summary>
    public class RepeatableMiddlewareAttribute : ActionFilterAttribute
    {
        public const string REPEAT_PARAMS = "RepeatParams";
        public const string REPEAT_TIME = "RepeatTime";
        /// <summary>
        /// 令牌键名
        /// </summary>
        private string _tokenKey;

        public RepeatableMiddlewareAttribute(string tokenKey = AuthConstant.CONFIG_TOKEN_KEY)
        {
            _tokenKey = tokenKey;
        }
        public override async Task<IResult> Excute(TAAction ac, FilterMiddlewareNode next)
        {

            //获取header令牌数据
            string submitKey = AuthConstant.CONFIG_REPEAT_SUBMIT_KEY + ac.Context.Request.Header[_tokenKey] ?? string.Empty + ac.Context.Request.Url.ToString();
            StreamReader sr = new StreamReader(ac.Context.Request.InputStream, Encoding.GetEncoding("UTF-8"));
            string nowParams = (await sr.ReadToEndAsync()) ?? string.Empty;

            GeneralRedisHelper redis = ac.Context.Application.ServiceProvider.GetService<GeneralRedisHelper>();
            string preParams = await redis.StringGetAsync<string>(submitKey);
            if (preParams != null)
            {
                if (nowParams == preParams)
                {
                    return new DefaultAjaxResult<string>(711, "请不要重复提交数据");
                }
            }

            await redis.StringSetAsync(submitKey, nowParams, TimeSpan.FromSeconds(10));
            return await next.Excute(ac);
        }
    }
}
