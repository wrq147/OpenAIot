using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ChannelUtility.Buffers;
using ChannelUtility.Config;
using ChannelUtility.Redis;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Jint;
namespace ChannelUtility
{
    public class ChannelRegister : IDisposable
    {
        protected GeneralRedisHelper _redis;
        public GeneralRedisHelper RedisHelper { get { return _redis; } }
        protected ChannelOption _option;
        public ChannelOption Option
        {
            get { return _option; }
        }

        public ChannelRegister(IServiceProvider provider)
        {
            _option = provider.GetService<ChannelOption>();
            _redis = provider.GetService<GeneralRedisHelper>();
        }
        //供程序员显式调用的Dispose方法
        public void Dispose()
        {
            //调用带参数的Dispose方法，释放托管和非托管资源
            Dispose(true);
            //手动调用了Dispose释放资源，那么析构函数就是不必要的了，这里阻止GC调用析构函数
            System.GC.SuppressFinalize(this);
        }

        //protected的Dispose方法，保证不会被外部调用。
        //传入bool值disposing以确定是否释放托管资源
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                //TODO:在这里加入清理"托管资源"的代码，应该是xxx.Dispose();
            }
            //TODO:在这里加入清理"非托管资源"的代码
        }

        //供GC调用的析构函数
        ~ChannelRegister()
        {
            Dispose(false);//释放非托管资源
        }
    }
}
