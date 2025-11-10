using Common.Redis;
using Common.Share;
using Microsoft.Extensions.Options;
using TemplateAction.Core;


namespace FlowService
{
    public class DeviceRedisHelper : RedisHelper
    {
        private ITAServiceProvider _serviceProvider;
        public DeviceRedisHelper(IOptions<GeneralOption> conf, ITAServiceProvider serviceProvider) : base(conf.Value.redisconn, 1)
        {
            _serviceProvider = serviceProvider;
        }

    }
}
