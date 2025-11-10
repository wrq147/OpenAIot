using AuthService;
using CardService.DAL;
using CardService.Model;
using Common.IdGenerator;
using Common.Share;
using MyAccess.Aop;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace CardService.Business
{
    public class CardExchangeBLL
    {
        private ITAServiceProvider _provider;
        private CardExchangeDAL _exchangeDAL;
        private CardHolderDAL _holderDAL;
        private CardAdminDAL _cardAdmin;
        private CardDAL _cardDAL;
        private SnowflakeHelper _snowflake;
        private ITAContext _context;
        public CardExchangeBLL(ITAServiceProvider serviceProvider, CardExchangeDAL exchangeDAL, CardHolderDAL holderDAL, CardDAL cardDAL, SnowflakeHelper snowflake, CardAdminDAL cardAdmin, ITAContext context)
        {
            _provider = serviceProvider;
            _exchangeDAL = exchangeDAL;
            _holderDAL = holderDAL;
            _cardDAL = cardDAL;
            _snowflake = snowflake;
            _cardAdmin = cardAdmin;
            _context = context;
        }
        public async Task<Out_Exchange_Info> SelectExchangeInfo(long targetUserId, long targetCardId, long userCardId)
        {
            var user = Data_ServerTokenInfo.From(_context);
            Out_Exchange_Info outInfo = new Out_Exchange_Info();

            outInfo.InHolder = await _holderDAL.Exist(user.UserId, targetCardId);
            outInfo.Exchanging = await _exchangeDAL.ExistExchanging(targetUserId, userCardId);
            return outInfo;
        }
        public async Task<PageObject<MZ_CardExchange>> SelectList(In_CardExchange query)
        {
            var user = Data_ServerTokenInfo.From(_context);
            query.ReceiveUserId = user.UserId;
            return await _exchangeDAL.SelectList(query);
        }

        public async Task<int> SelectPendingCount(long recvId)
        {
            return await _exchangeDAL.SelectPendingCount(recvId);
        }

        public async Task<BusResponse<int>> Add(long cardId)
        {
            IUserInfo user = Data_ServerTokenInfo.From(_context);
            MZ_Card card = await _cardDAL.SelectById(cardId);
            if (card == null)
            {
                return BusResponse<int>.Error(112, "名片不存在");
            }
            if (card.UserId == user.UserId)
            {
                return BusResponse<int>.Error(114, "无法交换自己的名片");
            }


            bool targetInHolder = await _holderDAL.Exist(card.UserId.Value, cardId);
            if (targetInHolder)
            {
                return BusResponse<int>.Error(115, "对方已添加你的名片");
            }

            //未添加对方的名片，则先添加
            if (!await _holderDAL.Exist(user.UserId, cardId))
            {
                MZ_CardHolder cardHolder = new MZ_CardHolder();
                cardHolder.CardId = cardId;
                cardHolder.CreatedOn = DateTime.Now;
                cardHolder.UserId = user.UserId;
                await _holderDAL.Insert(cardHolder);
            }


            var usingCardId = await _cardAdmin.SelectUsingCardId(user.UserId);
            MZ_CardExchange cardExchange = new MZ_CardExchange();
            cardExchange.Id = _snowflake.NextId();
            cardExchange.SendCardId = usingCardId;
            cardExchange.ReceiveCardId = cardId;
            cardExchange.ReceiveUserId = card.UserId;
            cardExchange.Status = "0";
            cardExchange.SetCreateBy(user);
            try
            {
                await _exchangeDAL.Insert(cardExchange);
                return BusResponse<int>.Success();
            }
            catch
            {
                return BusResponse<int>.ErrorBusy();
            }
        }
        public async Task<BusResponse<string>> Refuse(long id, IUserInfo user)
        {
            MZ_CardExchange exchange = await _exchangeDAL.SelectById(id);
            if (exchange == null)
            {
                return BusResponse<string>.Error(110, "请求不存在");
            }
            if (exchange.Status != "0")
            {
                return BusResponse<string>.Error(111, "请求已被处理");
            }

            MZ_CardExchange cex = new MZ_CardExchange();
            cex.SetUpdateBy(user);
            cex.Id = id;
            cex.Status = "2";
            await _exchangeDAL.Update(cex);
            return BusResponse<string>.Success();
        }
        public virtual async Task<BusResponse<string>> Agree(long id, IUserInfo user)
        {
            MZ_CardExchange exchange = await _exchangeDAL.SelectById(id);
            if (exchange == null)
            {
                return BusResponse<string>.Error(110, "请求不存在");
            }
            if (exchange.Status != "0")
            {
                return BusResponse<string>.Error(111, "请求已被处理");
            }

            using (BLLTranScope scope = new BLLTranScope())
            {
                try
                {
                    MZ_CardHolder newHolder = new MZ_CardHolder();
                    newHolder.CreatedOn = DateTime.Now;
                    newHolder.CardId = exchange.SendCardId.Value;
                    newHolder.UserId = exchange.ReceiveUserId.Value;
                    await _holderDAL.InsertTrans(newHolder);

                    MZ_CardExchange cex = new MZ_CardExchange();
                    cex.SetUpdateBy(user);
                    cex.Id = id;
                    cex.Status = "1";
                    await _exchangeDAL.UpdateTrans(cex);

                    // 完成
                    await scope.CompleteAsync();
                    return BusResponse<string>.Success();
          
                }
                catch
                {
                    return BusResponse<string>.ErrorBusy();
                }
        
            }
     
        }


    }
}
