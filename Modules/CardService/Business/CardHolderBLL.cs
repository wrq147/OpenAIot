using AuthService;
using CardService.DAL;
using CardService.Model;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace CardService.Business
{
    public class CardHolderBLL
    {
        private ITAServiceProvider _provider;
        private CardHolderDAL _holderDAL;
        private CardDAL _cardDAL;
        private ITAContext _context;
        public CardHolderBLL(ITAServiceProvider serviceProvider, CardHolderDAL holderDAL, CardDAL cardDAL, ITAContext context)
        {
            _provider = serviceProvider;
            _holderDAL = holderDAL;
            _cardDAL = cardDAL;
            _context = context;
        }

        public async Task<PageObject<MZ_Card_Holder_V>> SelectList(In_Card_Holder query)
        {
            query.UserId = Data_ServerTokenInfo.From(_context).UserId;
            return await _holderDAL.SelectList(query);
        }

        public async Task<BusResponse<string>> Add(long id)
        {
            IUserInfo user = Data_ServerTokenInfo.From(_context);
            MZ_Card card = await _cardDAL.SelectById(id);
            if (card == null)
            {
                return BusResponse<string>.Error(112, "名片不存在");
            }
            if (card.UserId == user.UserId)
            {
                return BusResponse<string>.Error(114, "无法添加自己的名片");
            }
            if (await _holderDAL.Exist(user.UserId, id))
            {
                return BusResponse<string>.Error(113, "名片已经存在");
            }

            MZ_CardHolder cardHolder = new MZ_CardHolder();
            cardHolder.CardId = id;
            cardHolder.CreatedOn = DateTime.Now;
            cardHolder.UserId = user.UserId;
            try
            {
                await _holderDAL.Insert(cardHolder);
                return BusResponse<string>.Success();
            }
            catch
            {
                return BusResponse<string>.ErrorBusy();
            }
        }
        public async Task<BusResponse<string>> Remove(long id)
        {
            try
            {
                IUserInfo user = Data_ServerTokenInfo.From(_context);
                await _holderDAL.Delete(user.UserId, id);
                return BusResponse<string>.Success();
            }
            catch
            {
                return BusResponse<string>.ErrorBusy();
            }
        }
    }
}
