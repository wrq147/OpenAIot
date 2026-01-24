using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService
{
    public class IoTAIOption
    {
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
        /// <summary>
        /// 人脸检测模型
        /// </summary>
        public string FaceDetectionFile { get; set; }
        /// <summary>
        /// 人脸关键点模型
        /// </summary>
        public string FaceKeyPointsFile { get; set; }
        /// <summary>
        /// 人脸识别模型
        /// </summary>
        public string FaceRecogFile { get; set; }
        /// <summary>
        /// 人脸空间变换模型
        /// </summary>
        public string FaceSTNFile { get; set; }
    }
}
