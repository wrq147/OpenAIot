using System;
namespace LLMService.Model
{
    public class Out_ChatMessage
    {
        /// <summary>
        /// 消息所属角色：用户user、助手assistant
        /// </summary>
        public string role { get; set; }
        /// <summary>
        /// 状态：0为完成，1为进行中
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string data { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public long time { get; set; }
    }
}
