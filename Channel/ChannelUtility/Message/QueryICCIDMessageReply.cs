using System;

namespace ChannelUtility.Message
{
    /// <summary>
    /// 查询ICCID的回复消息
    /// </summary>
    public class QueryICCIDMessageReply : BaseUpDeviceMessage
    {
        public string iccid { get; set; }
        public QueryICCIDMessageReply()
        {
            MsgType = "QueryICCIDReply";
        }
    }
}
