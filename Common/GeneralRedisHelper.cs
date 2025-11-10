using Common;
using Common.Redis;
using System;
using Microsoft.Extensions.Options;
using Common.Share;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Common
{
    public class GeneralRedisHelper : RedisHelper
    {
        public GeneralRedisHelper(IOptions<GeneralOption> conf) : base(conf.Value.redisconn, 0) { }
        /// <summary>
        /// 生成每日自增长的编号
        /// </summary>
        /// <param name="pre"></param>
        /// <returns></returns>
        public async Task<string> GenerateNumber(string pre)
        {
            string tkey = pre + DateTime.Now.ToString("yyMMdd");
            long val = await this.StringIncrementLongAsync(tkey);
            await this.KeyExpireAsync(tkey, TimeSpan.FromHours(25));
            return tkey + (val + 1).ToString().PadLeft(7, '0');
        }
        /// <summary>
        /// 指生成自增长编号
        /// </summary>
        /// <param name="pre"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public async Task<List<string>> GenerateNumberList(string pre, int num)
        {
            string tkey = pre + DateTime.Now.ToString("yyMMdd");
            long val = await this.StringIncrementLongAsync(tkey, num);
            await this.KeyExpireAsync(tkey, TimeSpan.FromHours(25));
            long start = val - num;
            List<string> rt = new List<string>();
            for (long i = start; i <= val; i++)
            {
                rt.Add(tkey + (i + 1).ToString().PadLeft(10, '0'));
            }
            return rt;
        }

    }
}
