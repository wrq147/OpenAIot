using AuthService;
using AuthService.DAL;
using AuthService.Model;
using CardService.DAL;
using CardService.Model;
using Common.IdGenerator;
using Common.Share;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace CardService.Business
{
    public class CardBLL
    {
        private ITAServiceProvider _provider;
        private CardDAL _card;
        private CardAdminDAL _cardAdmin;
        private OrgDAL _orgDAL;
        private SnowflakeHelper _snowflake;
        private ITAContext _context;
        private AdminExtDAL _adminExt;
        public CardBLL(ITAServiceProvider serviceProvider, CardDAL card, CardAdminDAL cardAdmin, OrgDAL orgDAL, AdminExtDAL adminExt, SnowflakeHelper snowflake, ITAContext context)
        {
            _provider = serviceProvider;
            _card = card;
            _cardAdmin = cardAdmin;
            _orgDAL = orgDAL;
            _snowflake = snowflake;
            _context = context;
            _adminExt = adminExt;
        }

        public async Task<BusResponse<string>> Switch(long cardId)
        {
            try
            {
                MZ_AdminExt ext = new MZ_AdminExt();
                ext.UserId = Data_ServerTokenInfo.From(_context).UserId;
                ext.ExtField = "CardId";
                ext.ExtValue = cardId.ToString();
                await _adminExt.CreateOrUpdate(ext);

                return BusResponse<string>.Success();
            }
            catch
            {
                return BusResponse<string>.Error(112, "操作异常,请稍候重试");
            }
        }
        public async Task<long> SelectByCAId(long id)
        {
            return await _cardAdmin.SelectUsingCardId(id);
        }
        public async Task<MZ_Card> SelectById(long id)
        {
            return await _card.SelectDetailById(id);
        }
        public async Task<PageObject<MZ_Card>> SelectList(In_Card_List query)
        {
            query.UserId = Data_ServerTokenInfo.From(_context).UserId;
            return await _card.SelectList(query);
        }
        /// <summary>
        /// 添加名片
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<BusResponse<long>> Add(MZ_Card data)
        {
            try
            {
                IUserInfo user = Data_ServerTokenInfo.From(_context);
                data.Id = _snowflake.NextId();
                data.del_flag = "0";
                data.UserId = user.UserId;
                data.SetCreateBy(user);
                await _card.Add(data);

                var cardId = await _cardAdmin.SelectUsingCardId(user.UserId);
                if (cardId == 0)
                {
                    MZ_AdminExt ext = new MZ_AdminExt();
                    ext.UserId = user.UserId;
                    ext.ExtField = "CardId";
                    ext.ExtValue = data.Id.ToString();
                    await _adminExt.CreateOrUpdate(ext);
                }

                return BusResponse<long>.Success(data.Id.Value);
            }
            catch (Exception ex)
            {
                return BusResponse<long>.Error(102, ex.Message);
            }
        }
        /// <summary>
        /// 编辑名片
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<BusResponse<int>> Update(MZ_Card data)
        {
            try
            {
                IUserInfo user = Data_ServerTokenInfo.From(_context);
                if (!string.IsNullOrEmpty(data.Intro))
                {
                    var itemlist = Newtonsoft.Json.JsonConvert.DeserializeObject<Tx_Pro_Item[]>(data.Intro);
                    if (itemlist.Length == 0)
                    {
                        data.Intro = string.Empty;
                    }
                    else
                    {
                        var vrs = EditorHelper.ValidateDetail(itemlist);
                        if (!vrs.IsSuccess())
                        {
                            return vrs;
                        }
                    }

                }
                data.SetUpdateBy(user);
                await _card.Update(data);
                return BusResponse<int>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(102, ex.Message);
            }
        }
        public async Task<BusResponse<string>> Delete(long id)
        {
            try
            {
                Data_ServerTokenInfo user = Data_ServerTokenInfo.From(_context);
                var cardId = await _cardAdmin.SelectUsingCardId(user.UserId);
                if (cardId == id)
                {
                    return BusResponse<string>.Error(113, "无法删除使用中的名片");
                }

                var card = await _card.SelectById(id);
                if (card == null)
                {
                    return BusResponse<string>.Error(115, "名片不存在");
                }
                if (card.UserId != user.UserId)
                {
                    return BusResponse<string>.Error(114, "无删除权限");
                }

                await _card.Delete(id);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(102, ex.Message);
            }
        }
    }
}
