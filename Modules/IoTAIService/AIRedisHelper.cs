using ChannelUtility;
using Common.Redis;
using Common.Share;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService
{
    public class AIRedisHelper : RedisHelper
    {
        public AIRedisHelper(IOptions<GeneralOption> conf) : base(conf.Value.redisconn, 1) { }
    }
}
