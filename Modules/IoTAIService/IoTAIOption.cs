using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService
{
    public class IoTAIOption
    {
        /// <summary>
        /// 监听的帧发布源
        /// </summary>
        public string[] FramePushConns { get; set; }
        public string PythonHome { get; set; }
        /// <summary>
        /// 是否初始化Milvus表
        /// </summary>
        public bool InitMilvus { get; set; }
        /// <summary>
        /// Milvus的连接地址
        /// </summary>
        public string MilvusUrl { get; set; }
        /// <summary>
        /// Milvus数据库名称
        /// </summary>
        public string MilvusDatabase { get; set; }
    }
}
