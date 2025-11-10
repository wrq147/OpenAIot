using System;
using TemplateAction.Core;

namespace AuthService
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Data_ServerTokenInfo GetUser(this ITAServiceProvider provider)
        {
            var context = provider.GetService<ITAContext>();
            return Data_ServerTokenInfo.From(context);
        }
    }
}
