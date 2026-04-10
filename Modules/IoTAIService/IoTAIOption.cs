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
        /// AI任务线程数量（默认4）
        /// </summary>
        public int runer_count { get; set; }
        /// <summary>
        /// 绑定端口，等待所有发布端连接
        /// </summary>
        public string AIBind { get; set; }
        /// <summary>
        /// Python的根目录
        /// </summary>
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
