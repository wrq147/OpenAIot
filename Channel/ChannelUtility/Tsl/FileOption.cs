using System;

namespace ChannelUtility.Tsl
{
    public class FileOption : BaseValueOption
    {
        /// <summary>
        /// url,base64
        /// </summary>
        public string bodyType { get; set; }
        /// <summary>
        /// 是否是图片
        /// </summary>
        public bool isimg { get; set; }
        public FileOption()
        {
            this.type = "file";
        }
    }
}
