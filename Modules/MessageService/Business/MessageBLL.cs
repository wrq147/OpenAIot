using AuthService;
using Common.IdGenerator;
using MessageService.DAL;
using MessageService.Model;
using MyAccess.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using Common.Share;
namespace MessageService.Business
{
    public class MessageBLL
    {
        private MessageLogDAL _logDAL;
        private MessageDAL _msgDAL;
        private SnowflakeHelper _snowflake;
        protected ITAServiceProvider _provider;
        public MessageBLL(MessageLogDAL logDAL, MessageDAL msgDAL, SnowflakeHelper snowflake, ITAServiceProvider provider)
        {
            _logDAL = logDAL;
            _msgDAL = msgDAL;
            _snowflake = snowflake;
            _provider = provider;
        }
        public virtual async Task AddMessage(MZ_Message msg)
        {
            msg.id = _snowflake.NextId();
            msg.create_time = DateTime.Now;
            await _msgDAL.Insert(msg);
        }
        public virtual async Task AddMessageLog(MZ_Message_Log log)
        {
            await _logDAL.Insert(log);
        }
        public virtual async Task<BusResponse<int>> UnreadCount()
        {
            ITAContext context = _provider.GetService<ITAContext>();
            Data_ServerTokenInfo user = Data_ServerTokenInfo.From(context);
            int rs = await _msgDAL.UnreadCount(user);
            return BusResponse<int>.Success(rs);
        }
        public virtual async Task<BusResponse<int>> ReadAll()
        {
            ITAContext context = _provider.GetService<ITAContext>();
            Data_ServerTokenInfo user = Data_ServerTokenInfo.From(context);
            int rs = await _logDAL.ReadAll(user);
            return BusResponse<int>.Success(rs);
        }
        public virtual async Task<BusResponse<int>> Read(long msgId)
        {
            ITAContext context = _provider.GetService<ITAContext>();
            Data_ServerTokenInfo user = Data_ServerTokenInfo.From(context);
            MZ_Message_Log msglog = new MZ_Message_Log();
            msglog.read_time = DateTime.Now;
            msglog.status = 1;
            return BusResponse<int>.Success(await _logDAL.Update(msglog, x => x.messsage_id == msgId && x.receiver_id == user.UserId && x.status == 0));
        }
        public virtual async Task<List<MZ_Message>> SelectUnread(int top)
        {
            ITAContext context = _provider.GetService<ITAContext>();
            Data_ServerTokenInfo user = Data_ServerTokenInfo.From(context);
            return await _msgDAL.SelectUnread(user, top);
        }
        public virtual async Task<PageObject<MZ_Message>> SelectPage(In_MessageList query)
        {
            ITAContext context = _provider.GetService<ITAContext>();
            Data_ServerTokenInfo user = Data_ServerTokenInfo.From(context);
            return await _msgDAL.SelectPage(query, user);
        }
    }
}
