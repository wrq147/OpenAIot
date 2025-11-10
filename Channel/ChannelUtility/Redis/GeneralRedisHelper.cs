namespace ChannelUtility.Redis
{
    public class GeneralRedisHelper : RedisHelper
    {
        public GeneralRedisHelper(ChannelOption conf) : base(conf.RedisConn, 1) { }
    }
}
