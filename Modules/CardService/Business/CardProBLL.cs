using AuthService;
using CardService.DAL;
using CardService.Model;
using Common.IdGenerator;
using Common.Json;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace CardService.Business
{
    public class CardProBLL
    {
        private SnowflakeHelper _snowflake;
        private CardProDAL _cardPro;
        private DeptDAL _dept;
        private CardDAL _card;
        private UserDAL _user;
        private ITAContext _context;
        private OrgDAL _orgDAL;
        public CardProBLL(SnowflakeHelper snowflake, CardProDAL cardPro, DeptDAL dept, CardDAL card, UserDAL user, OrgDAL orgDAL, ITAContext context)
        {
            _snowflake = snowflake;
            _cardPro = cardPro;
            _dept = dept;
            _card = card;
            _user = user;
            _context = context;
            _orgDAL = orgDAL;
        }
        #region 产品
        public async Task<BusResponse<MZ_Card_Pro>> SelectById(long id)
        {
            var pro = await _cardPro.SelectById(id);
            return BusResponse<MZ_Card_Pro>.Success(pro);
        }
        public async Task<PageObject<MZ_Card_Pro>> SelectList(In_Card_Pro query)
        {
            if (query.CardId != null)
            {
                MZ_Card card = await _card.SelectDetailById(query.CardId.Value);
                if (card == null)
                {
                    return PageObject<MZ_Card_Pro>.Empty();
                }
                if (card.OrgId == null || card.OrgId <= 0 || card.DeptId == null)
                {
                    return PageObject<MZ_Card_Pro>.Empty();
                }
                var deptInfo = await _dept.SelectById(card.DeptId.Value);
                if (deptInfo == null)
                {
                    return PageObject<MZ_Card_Pro>.Empty();
                }
                query.orgId = card.OrgId.Value;
                query.filterAncestors = deptInfo.ancestors;
            }
            else
            {
                var user = Data_ServerTokenInfo.From(_context);
                if (query.orgId == null)
                {
                    query.orgId = user.OrgId;
                }

                if (!await _orgDAL.CheckManOrg(user.UserId, query.orgId.Value))
                {
                    return PageObject<MZ_Card_Pro>.Empty();
                }
            }
            if (query.CategoryId != null)
            {
                var category = await _cardPro.SelectCategoryById(query.CategoryId.Value);
                query.CategoryPath = category.Path;
            }
            return await _cardPro.SelectList(query);
        }

        public async Task<BusResponse<int>> Insert(MZ_Card_Pro data)
        {
            var user = Data_ServerTokenInfo.From(_context);
            if (data.OrgId == null)
            {
                data.OrgId = user.OrgId;
            }
            if (!await _orgDAL.CheckManOrg(user.UserId, data.OrgId.Value))
            {
                return BusResponse<int>.Error(112, "权限不足");
            }
            if (!string.IsNullOrEmpty(data.Detail))
            {
                var itemlist = System.Text.Json.JsonSerializer.Deserialize<Tx_Pro_Item[]>(data.Detail, MyDefaultTextJsonConfig.DefaultOptions);
                if (itemlist.Length == 0)
                {
                    return BusResponse<int>.Error(114, "产品详情不能为空");
                }
                var vrs = EditorHelper.ValidateDetail(itemlist);
                if (!vrs.IsSuccess())
                {
                    return vrs;
                }
            }


            data.del_flag = "0";
            data.SetCreateBy(user);
            data.Id = _snowflake.NextId();

            return BusResponse<int>.Success(await _cardPro.Insert(data));
        }

        public async Task<BusResponse<int>> Update(MZ_Card_Pro data)
        {
            var user = Data_ServerTokenInfo.From(_context);
            MZ_Card_Pro old = await _cardPro.SelectById(data.Id.Value);
            if (old == null)
            {
                return BusResponse<int>.Error(110, "不存在");
            }
            if (old.del_flag != "0")
            {
                return BusResponse<int>.Error(111, "已被删除");
            }
            if (!await _orgDAL.CheckManOrg(user.UserId, old.OrgId.Value))
            {
                return BusResponse<int>.Error(112, "权限不足");
            }

            if (!string.IsNullOrEmpty(data.Detail))
            {
                var itemlist = System.Text.Json.JsonSerializer.Deserialize<Tx_Pro_Item[]>(data.Detail, MyDefaultTextJsonConfig.DefaultOptions);
                if (itemlist.Length == 0)
                {
                    return BusResponse<int>.Error(114, "产品详情不能为空");
                }
                var vrs = EditorHelper.ValidateDetail(itemlist);
                if (!vrs.IsSuccess())
                {
                    return vrs;
                }
            }


            data.SetCreateBy(user);
            data.OrgId = null;
            data.del_flag = null;
            return BusResponse<int>.Success(await _cardPro.Update(data));
        }
        public async Task<BusResponse<int>> Recover(long id)
        {
            var user = Data_ServerTokenInfo.From(_context);
            MZ_Card_Pro old = await _cardPro.SelectDeletedById(id);
            if (old == null)
            {
                return BusResponse<int>.Error(110, "不存在");
            }
            if (!await _orgDAL.CheckManOrg(user.UserId, old.OrgId.Value))
            {
                return BusResponse<int>.Error(112, "权限不足");
            }
            MZ_Card_Pro updatePro = new MZ_Card_Pro();
            updatePro.Id = id;
            updatePro.SetUpdateBy(user);
            updatePro.del_flag = "0";
            return BusResponse<int>.Success(await _cardPro.Update(updatePro));
        }
        public async Task<BusResponse<int>> Remove(long id)
        {
            var user = Data_ServerTokenInfo.From(_context);
            MZ_Card_Pro old = await _cardPro.SelectById(id);
            if (old == null)
            {
                return BusResponse<int>.Error(110, "不存在");
            }
            if (!await _orgDAL.CheckManOrg(user.UserId, old.OrgId.Value))
            {
                return BusResponse<int>.Error(112, "权限不足");
            }

            MZ_Card_Pro updatePro = new MZ_Card_Pro();
            updatePro.Id = id;
            updatePro.SetUpdateBy(user);
            updatePro.del_flag = "2";
            return BusResponse<int>.Success(await _cardPro.Update(updatePro));
        }
        #endregion

        #region 产品分类
        public virtual async Task<BusResponse<int>> UpdateCategorySort(List<long> idList, long orgId)
        {
            try
            {
                if (!await _orgDAL.CheckManOrg(Data_ServerTokenInfo.From(_context).UserId, orgId))
                {
                    return BusResponse<int>.Error(111, "无权操作指定分类");
                }
                return BusResponse<int>.Success(await _cardPro.UpdateCategorySort(idList, orgId));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(112, ex.Message);
            }
        }

        public async Task<List<MZ_Card_Category>> SelectCategoryList(In_Card_Category query)
        {
            if (query.CardId != null)
            {
                MZ_Card card = await _card.SelectDetailById(query.CardId.Value);
                if (card == null)
                {
                    return new List<MZ_Card_Category>();
                }
                if (card.OrgId == null || card.OrgId <= 0)
                {
                    return new List<MZ_Card_Category>();
                }
                query.OrgId = card.OrgId.Value;
            }
            return await _cardPro.SelectCategoryList(query);
        }
        public async Task<BusResponse<int>> UpdateCategory(MZ_Card_Category data)
        {
            data.OrgId = null;
            if (data.ParentId != null)
            {
                MZ_Card_Category old = await _cardPro.SelectCategoryById(data.ParentId.Value);
                if (old == null)
                {
                    return BusResponse<int>.Error(113, "父分类不存在");
                }
                data.Path = old.Path + data.Id + ",";
            }
            else
            {
                data.Path = null;
            }
            return BusResponse<int>.Success(await _cardPro.UpdateCategory(data));
        }
        public async Task<BusResponse<int>> InsertCategory(MZ_Card_Category data)
        {
            var user = Data_ServerTokenInfo.From(_context);
            data.OrgId = user.OrgId;
            data.Sort = 0;
            if (!await _orgDAL.CheckManOrg(user.UserId, data.OrgId.Value))
            {
                return BusResponse<int>.Error(112, "权限不足");
            }
            data.Id = _snowflake.NextId();
            data.ParentId ??= 0;
            if (data.ParentId > 0)
            {
                MZ_Card_Category old = await _cardPro.SelectCategoryById(data.ParentId.Value);
                if (old == null)
                {
                    return BusResponse<int>.Error(113, "父分类不存在");
                }
                data.Path = old.Path + data.Id + ",";
            }
            else
            {
                data.Path = data.Id + ",";
            }


            return BusResponse<int>.Success(await _cardPro.InsertCategory(data));
        }

        public async Task<BusResponse<int>> RemoveCategory(long id)
        {
            var user = Data_ServerTokenInfo.From(_context);
            MZ_Card_Category old = await _cardPro.SelectCategoryById(id);
            if (old == null)
            {
                return BusResponse<int>.Error(114, "分类不存在");
            }
            if (!await _orgDAL.CheckManOrg(user.UserId, old.OrgId.Value))
            {
                return BusResponse<int>.Error(112, "权限不足");
            }


            if (await _cardPro.ExistChildren(id, old.OrgId.Value))
            {
                return BusResponse<int>.Error(113, "无法删除,请先删除子分类");
            }

            return BusResponse<int>.Success(await _cardPro.DeleteCategory(id));
        }


        /// <summary>
        /// 分类树转选择树
        /// </summary>
        /// <param name="deptTree"></param>
        /// <returns></returns>
        public List<TreeSelect<long>> CategoryList2Tree(List<MZ_Card_Category> categoryTree)
        {
            List<TreeSelect<long>> treeList = new List<TreeSelect<long>>();
            foreach (MZ_Card_Category category in categoryTree)
            {
                TreeSelect<long> ts = new TreeSelect<long>();
                ts.parentId = category.ParentId.Value;
                ts.id = category.Id.Value;
                ts.label = category.CategoryName;
                if (category.children != null)
                {
                    ts.children = CategoryList2Tree(category.children);
                }
                treeList.Add(ts);
            }
            return treeList;
        }
        /// <summary>
        /// 构建前端所需要树结构
        /// </summary>
        /// <param name="depts"></param>
        /// <returns></returns>
        public List<MZ_Card_Category> BuildCategoryTree(List<MZ_Card_Category> categorys)
        {
            List<MZ_Card_Category> returnList = new List<MZ_Card_Category>();
            List<long> tempList = new List<long>();
            foreach (MZ_Card_Category cate in categorys)
            {
                tempList.Add(cate.Id.Value);
            }
            foreach (MZ_Card_Category cate in categorys)
            {
                // 如果是顶级节点, 遍历该父节点的所有子节点
                if (!tempList.Contains(cate.ParentId.Value))
                {
                    _RecursionFn(categorys, cate);
                    returnList.Add(cate);
                }
            }

            if (returnList.Count == 0)
            {
                returnList = categorys;
            }
            return returnList;
        }


        /// <summary>
        /// 递归列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="t"></param>
        private void _RecursionFn(List<MZ_Card_Category> list, MZ_Card_Category t)
        {
            // 得到子节点列表
            List<MZ_Card_Category> childList = _GetChildList(list, t);
            t.children = childList;
            foreach (MZ_Card_Category tChild in childList)
            {
                if (_GetChildList(list, tChild).Count > 0)
                {
                    _RecursionFn(list, tChild);
                }
            }
        }

        /// <summary>
        /// 得到子节点列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private List<MZ_Card_Category> _GetChildList(List<MZ_Card_Category> list, MZ_Card_Category t)
        {
            List<MZ_Card_Category> tlist = new List<MZ_Card_Category>();
            foreach (MZ_Card_Category n in list)
            {
                if (n.ParentId == t.Id)
                {
                    tlist.Add(n);
                }
            }
            return tlist;
        }


        #endregion
    }
}
