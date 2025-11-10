using AuthService;
using Common.IdGenerator;
using Common.Share;
using ProducerService.DAL;
using ProducerService.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace ProducerService.Business
{
    public class ProductTypeBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private ProductTypeDAL _productTypeDAL;
        public ProductTypeBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, ProductTypeDAL productTypeDAL)
        {
            _snowflake = snowflake;
            _provider = provider;
            _productTypeDAL = productTypeDAL;
        }

        public virtual async Task<List<MZ_ProductType>> SelectProductTypeList(IUserInfo user)
        {
            return await _productTypeDAL.SelectList((x) => x.OrgId == user.OrgId, "Sort asc", "Id,OrgId,Name,PhotoUrl,Sort");
        }
        public virtual async Task<MZ_ProductType> Info(string id)
        {
            return await _productTypeDAL.Select(id);
        }

        public virtual async Task<BusResponse<int>> UpdateSort(List<string> idList)
        {
            try
            {
                return BusResponse<int>.Success(await _productTypeDAL.UpdateSort(idList));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(112, ex.Message);
            }
        }

        public virtual async Task<BusResponse<int>> Insert(MZ_ProductType data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(133, "请切换到企业账号");
            }

            if (string.IsNullOrEmpty(data.Name))
            {
                return BusResponse<int>.Error(113, "分组名称不能为空");
            }

            var tmplist = await _productTypeDAL.SelectList(x => x.OrgId == user.OrgId && x.Name == data.Name);
            if (tmplist.Count > 0)
            {
                return BusResponse<int>.Error(114, "分组名称已存在");
            }

            data.Id = _snowflake.NextId().ToString();
            data.PropList ??= string.Empty;
            data.ConditionJson ??= string.Empty;
            data.ListFieldsJson ??= string.Empty;
            data.PhotoUrl ??= string.Empty;
            data.OrgId = user.OrgId;
            if (data.Sort == null)
            {
                data.Sort = 0;
            }
            await _productTypeDAL.SortIncrease(data.OrgId.Value, data.Sort.Value);
            return BusResponse<int>.Success(await _productTypeDAL.Insert(data));
        }

        public virtual async Task<BusResponse<int>> Update(MZ_ProductType data, IUserInfo user)
        {
            var old = await _productTypeDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(113, "分组不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(114, "产品分组所属组织错误");
            }
            if (data.Name != null)
            {
                var tmplist = await _productTypeDAL.SelectList(x => x.OrgId == user.OrgId && x.Id != data.Id && x.Name == data.Name);
                if (tmplist.Count > 0)
                {
                    return BusResponse<int>.Error(115, "分组名称已存在");
                }
            }
            data.OrgId = null;
            if (data.Sort != null)
            {
                await _productTypeDAL.SortIncrease(old.OrgId.Value, data.Sort.Value);
            }
            return BusResponse<int>.Success(await _productTypeDAL.Update(data));
        }
        public virtual async Task<BusResponse<int>> Remove(string id, IUserInfo user)
        {
            var old = await _productTypeDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(113, "分组不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(114, "产品分组所属组织错误");
            }
            return BusResponse<int>.Success(await _productTypeDAL.Delete(x => x.Id == id));
        }
    }
}
