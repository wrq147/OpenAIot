using System;
using Common.Share;
using Microsoft.Extensions.Options;

namespace Common.IdGenerator
{
    /// <summary>
    /// 雪花算法生成ID
    /// </summary>
    public class SnowflakeHelper
    {
        private IIdGenerator _IdGenInstance = null;
        public SnowflakeHelper(IOptions<GeneralOption> option)
        {
            //初始化ID生成器
            var options = new IdGeneratorOptions(option.Value.idgenerator_workid);
            _IdGenInstance = new DefaultIdGenerator(options);
        }
   
        /// <summary>
        /// 生成新的Id
        /// </summary>
        /// <returns></returns>
        public long NextId()
        {
            return _IdGenInstance.NewLong();
        }
    }
}
