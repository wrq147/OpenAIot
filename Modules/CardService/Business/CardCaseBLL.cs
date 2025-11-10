using AuthService;
using CardService.DAL;
using CardService.Model;
using Common.IdGenerator;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace CardService.Business
{
    public class CardCaseBLL
    {
        private SnowflakeHelper _snowflake;
        private CardCaseDAL _cardCase;
        private CardDAL _card;
        private DeptDAL _dept;
        private OrgDAL _org;
        private ITAContext _context;
        public CardCaseBLL(SnowflakeHelper snowflake, CardCaseDAL cardCase, DeptDAL dept, CardDAL card, OrgDAL org, ITAContext context)
        {
            _snowflake = snowflake;
            _cardCase = cardCase;
            _card = card;
            _dept = dept;
            _org = org;
            _context = context;
        }
        public async Task<BusResponse<MZ_Card_Case>> SelectById(long id)
        {
            var casex = await _cardCase.SelectById(id);
            return BusResponse<MZ_Card_Case>.Success(casex);
        }
        public async Task<PageObject<MZ_Card_Case>> SelectList(In_Card_Case query)
        {
            MZ_Card card = await _card.SelectDetailById(query.CardId.Value);
            if (card == null)
            {
                return PageObject<MZ_Card_Case>.Empty();
            }
            if (card.OrgId == null || card.OrgId <= 0)
            {
                return PageObject<MZ_Card_Case>.Empty();
            }

            query.OrgId = card.OrgId.Value;
            return await _cardCase.SelectList(query);
        }
        public async Task<PageObject<MZ_Card_Case>> SelectListM(In_Card_Case_M query)
        {
            var user = Data_ServerTokenInfo.From(_context);
            if (!await _org.CheckManOrg(user.UserId, query.OrgId.Value))
            {
                return PageObject<MZ_Card_Case>.Empty();
            }

            return await _cardCase.SelectListM(query);
        }
        public async Task<BusResponse<int>> Insert(MZ_Card_Case data)
        {
            var user = Data_ServerTokenInfo.From(_context);
            if (!await _org.CheckManOrg(user.UserId, user.OrgId))
            {
                return BusResponse<int>.Error(112, "权限不足");
            }

            data.OrgId = user.OrgId;
            data.Id = _snowflake.NextId();
            data.SetCreateBy(user);
            return BusResponse<int>.Success(await _cardCase.Insert(data));
        }

        public async Task<BusResponse<int>> Update(MZ_Card_Case data)
        {
            var user = Data_ServerTokenInfo.From(_context);
            MZ_Card_Case old = await _cardCase.SelectById(data.Id.Value);
            if (old == null)
            {
                return BusResponse<int>.Error(110, "不存在");
            }

            if (!await _org.CheckManOrg(user.UserId, old.OrgId.Value))
            {
                return BusResponse<int>.Error(112, "权限不足");
            }

            data.OrgId = null;
            data.SetUpdateBy(user);
            return BusResponse<int>.Success(await _cardCase.Update(data));
        }
        public async Task<BusResponse<int>> Remove(long id)
        {
            var user = Data_ServerTokenInfo.From(_context);
            MZ_Card_Case old = await _cardCase.SelectById(id);
            if (old == null)
            {
                return BusResponse<int>.Error(110, "不存在");
            }
            if (!await _org.CheckManOrg(user.UserId, old.OrgId.Value))
            {
                return BusResponse<int>.Error(112, "权限不足");
            }

            return BusResponse<int>.Success(await _cardCase.Delete(id));
        }
    }
}
