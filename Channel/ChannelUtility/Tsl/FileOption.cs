using System;

namespace ChannelUtility.Tsl
{
    public class FileOption : BaseValueOption
    {
        /// <summary>
        /// url,base64,binary
        /// </summary>
        public string bodyType { get; set; }
        public FileOption()
        {
            this.type = "file";
        }
    }
}
