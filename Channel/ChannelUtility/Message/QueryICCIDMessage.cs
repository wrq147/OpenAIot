using System;

namespace ChannelUtility.Message
{
    /// <summary>
    /// 下发请求查询ICCID
    /// </summary>
    public class QueryICCIDMessage : RequestMessage
    {
        public QueryICCIDMessage()
        {
            MsgType = "QueryICCID";
        }
    }
}
