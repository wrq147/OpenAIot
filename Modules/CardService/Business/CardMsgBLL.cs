using AuthService;
using CardService.DAL;
using CardService.Model;
using Common.IdGenerator;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace CardService.Business
{
    public class CardMsgBLL
    {
        private ITAServiceProvider _provider;
        private CardMsgDAL _cardMsg;
        private CardAdminDAL _cardAdmin;
        private SnowflakeHelper _snowflake;
        private CardDAL _card;
        private CardProDAL _cardPro;
        private CardCaseDAL _cardCase;
        private ITAContext _context;
        public CardMsgBLL(SnowflakeHelper snowflake, ITAServiceProvider serviceProvider, CardMsgDAL cardMsg, CardAdminDAL cardAdmin, CardDAL card, CardProDAL cardPro, CardCaseDAL cardCase, ITAContext context)
        {
            _snowflake = snowflake;
            _provider = serviceProvider;
            _cardMsg = cardMsg;
            _cardAdmin = cardAdmin;
            _card = card;
            _cardPro = cardPro;
            _cardCase = cardCase;
            _context = context;
        }
        public async Task<BusResponse<int>> UnreadAmount()
        {
            var user = Data_ServerTokenInfo.From(_context);
            return BusResponse<int>.Success(await _cardMsg.UnreadAmount(user.UserId));
        }
        public async Task<PageObject<MZ_Card_Record_V>> SelectList(In_Record_List query)
        {
            var usingCardId = await _cardAdmin.SelectUsingCardId(Data_ServerTokenInfo.From(_context).UserId);
            query.VisitCardId = usingCardId;
            return await _cardMsg.SelectList(query);
        }

        public async Task<PageObject<MZ_Card_Visited_V>> SelectVisitedList(In_Visited_List query)
        {
            if (query.InitTarget == true)
                query.ReceiveUserId = Data_ServerTokenInfo.From(_context).UserId;
            var rt = await _cardMsg.SelectVisitedList(query);
            if (query.InitTarget == true)
            {
                List<long> idCardList = new List<long>();
                List<long> idProList = new List<long>();
                List<long> idCaseList = new List<long>();
                foreach (MZ_Card_Visited_V cvv in rt.List)
                {
                    if (cvv.VisitType == 0)
                    {
                        idCardList.Add(cvv.TargetId.Value);
                    }
                    else if (cvv.VisitType == 1)
                    {
                        idProList.Add(cvv.TargetId.Value);
                    }
                    else if (cvv.VisitType == 2)
                    {
                        idCaseList.Add(cvv.TargetId.Value);
                    }
                }
                Dictionary<long, MZ_Card> cardDict = new Dictionary<long, MZ_Card>();
                Dictionary<long, MZ_Card_Pro> cardProDict = new Dictionary<long, MZ_Card_Pro>();
                Dictionary<long, MZ_Card_Case> cardCaseDict = new Dictionary<long, MZ_Card_Case>();
                if (idCardList.Count > 0)
                {
                    var cardlist = await _card.SelectListInIds(idCardList);
                    foreach (MZ_Card c in cardlist)
                    {
                        cardDict.Add(c.Id.Value, c);
                    }
                }
                if (idProList.Count > 0)
                {
                    var prolist = await _cardPro.SelectListInIds(idProList);
                    foreach (MZ_Card_Pro cp in prolist)
                    {
                        cardProDict.Add(cp.Id.Value, cp);
                    }
                }
                if (idCaseList.Count > 0)
                {
                    var caselist = await _cardCase.SelectListInIds(idCaseList);
                    foreach (MZ_Card_Case cc in caselist)
                    {
                        cardCaseDict.Add(cc.Id.Value, cc);
                    }
                }

                foreach (MZ_Card_Visited_V cvv in rt.List)
                {
                    if (cvv.VisitType == 0)
                    {
                        cvv.Target = cardDict[cvv.TargetId.Value];
                    }
                    else if (cvv.VisitType == 1)
                    {
                        cvv.Target = cardProDict[cvv.TargetId.Value];
                    }
                    else if (cvv.VisitType == 2)
                    {
                        cvv.Target = cardCaseDict[cvv.TargetId.Value];
                    }
                }
            }
            return rt;
        }

        public async Task<BusResponse<long>> Insert(MZ_Card_Msg data)
        {
            try
            {
                var user = Data_ServerTokenInfo.From(_context);
                data.UserId = user.UserId;
                if (data.ReceiveUserId.Value == user.UserId)
                {
                    return BusResponse<long>.Error(3, "不记录自己的访问");
                }

                var cardId = await _cardAdmin.SelectUsingCardId(user.UserId);
                if (cardId <= 0)
                {
                    return BusResponse<long>.Error(3, "未有使用中的名片");
                }
                data.VisitCardId = cardId;
                data.VisitNumber = await _cardMsg.CountOfVisit(cardId, data.TargetId.Value) + 1;
                data.Status = "0";
                data.CreatedOn = DateTime.Now;
                data.Id = _snowflake.NextId();
                await _cardMsg.Insert(data);
                return BusResponse<long>.Success(data.Id.Value);
            }
            catch (Exception ex)
            {
                return BusResponse<long>.Error(110, ex.Message);
            }
        }

        public async Task<BusResponse<int>> VisitEnd(long id)
        {
            var user = Data_ServerTokenInfo.From(_context);
            MZ_Card_Msg msg = await _cardMsg.SelectById(id);
            if (msg == null)
            {
                return BusResponse<int>.Error(110, "记录不存在");
            }
            if (user.UserId != msg.UserId)
            {
                return BusResponse<int>.Error(111, "无权限");
            }

            try
            {
                MZ_Card_Msg newupdate = new MZ_Card_Msg();
                newupdate.Id = id;
                newupdate.EndOn = DateTime.Now;
                await _cardMsg.Update(newupdate);
                return BusResponse<int>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(112, ex.Message);
            }
        }
        public async Task<BusResponse<int>> Readed(long id)
        {
            var user = Data_ServerTokenInfo.From(_context);
            if (id > 0)
            {
                MZ_Card_Msg msg = await _cardMsg.SelectById(id);
                if (msg == null)
                {
                    return BusResponse<int>.Error(110, "记录不存在");
                }
                if (user.UserId != msg.ReceiveUserId)
                {
                    return BusResponse<int>.Error(111, "无权限");
                }

                try
                {
                    MZ_Card_Msg newupdate = new MZ_Card_Msg();
                    newupdate.Id = id;
                    newupdate.Status = "1";
                    newupdate.ReadedOn = DateTime.Now;
                    await _cardMsg.Update(newupdate);
                    return BusResponse<int>.Success();
                }
                catch (Exception ex)
                {
                    return BusResponse<int>.Error(112, ex.Message);
                }
            }
            else
            {
                try
                {
                    await _cardMsg.ReadAll(user.UserId);
                    return BusResponse<int>.Success();
                }
                catch (Exception ex)
                {
                    return BusResponse<int>.Error(112, ex.Message);
                }
            }
        }
    }
}
