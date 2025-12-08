using AuthService;
using Common.IdGenerator;
using Common.Share;
using MESService.DAL;
using MESService.Model;
using Minio.DataModel;
using MyAccess.DB.Builder.WhereToSql;
using ProducerService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace MESService.Business
{
    public class BomBLL
    {
        private ITAServiceProvider _provider;
        public BomBLL(ITAServiceProvider provider)
        {
            _provider = provider;
        }
        public virtual async Task<List<Out_BomTreeItem>> SelectBomTree(string id)
        {
            BomLineDAL bomLineDAL = _provider.GetService<BomLineDAL>();
            var tmplist = await bomLineDAL.ListTree(id);
            return Out_BomTreeItem.BuildTree(tmplist);
        }
        public virtual async Task<PageObject<MZ_BomHeader>> SelectList(In_BomList query, IUserInfo user)
        {
            BomHeaderDAL bomHeaderDAL = _provider.GetService<BomHeaderDAL>();
            var pagelist = await bomHeaderDAL.SelectByPage(query, user.OrgId);
            var createdIds = pagelist.List.Select(x => x.createId.Value).ToList();
            var updatedIds = pagelist.List.Select(x => x.updateId.Value).ToList();
            List<long> concatList = createdIds.Concat(updatedIds).ToList();
            if (concatList.Count > 0)
            {
                var tmpUsers = await _provider.GetService<UserDAL>().GetUserListByIds(concatList);
                foreach(var item in pagelist.List)
                {
                    item.updateName = tmpUsers.Where(x => x.Id == item.updateId).FirstOrDefault()?.RealName;
                    item.createName = tmpUsers.Where(x => x.Id == item.createId).FirstOrDefault()?.RealName;
                }
            }
            return pagelist;
        }
        public virtual async Task<BusResponse<MZ_BomHeader>> Info(string id)
        {
            BomHeaderDAL bomHeaderDAL = _provider.GetService<BomHeaderDAL>();
            BomLineDAL bomLineDAL = _provider.GetService<BomLineDAL>();
            var info = await bomHeaderDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_BomHeader>.Error(111, "物料清单不存在");
            }
            info.Items = await bomLineDAL.SelectList(x => x.HeaderId == id);
            var pids = info.Items.Select(x => x.ProductId).ToList();
            if (pids.Count > 0)
            {
                var prolist = await _provider.GetService<ProductDAL>().SelectList(x => pids.Contains(x.Id));
                foreach (var iitem in info.Items)
                {
                    iitem.ProInfo = prolist.FirstOrDefault(x => x.Id == iitem.ProductId);
                }
            }
            var operids = info.Items.Select(x => x.ProcessStepId).ToList();
            if (operids.Count > 0)
            {
                var operlist = await _provider.GetService<OperDAL>().SelectList(x => operids.Contains(x.Id));
                foreach (var iitem in info.Items)
                {
                    iitem.ProcessStepInfo = operlist.FirstOrDefault(x => x.Id == iitem.ProcessStepId);
                }
            }
            return BusResponse<MZ_BomHeader>.Success(info);
        }
        public virtual async Task<BusResponse<int>> Update(MZ_BomHeader data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(111, "非企业用户无法修改物料清单");
            }
            var bomHeaderDAL = _provider.GetService<BomHeaderDAL>();
            var old = await bomHeaderDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(112, "物料清单不存在");
            }

            var snowflake = _provider.GetService<SnowflakeHelper>();
            BomLineDAL bomLineDAL = _provider.GetService<BomLineDAL>();
            await bomLineDAL.Delete(x => x.HeaderId == data.Id);
            if (data.Items != null && data.Items.Count > 0)
            {
                foreach (var item in data.Items)
                {
                    item.OrgId = user.OrgId;
                    item.Id = snowflake.NextId().ToString();
                    item.HeaderId = data.Id;
                    item.ParentProductId = data.ProductId;
                }
                await bomLineDAL.Insert(data.Items);
            }

            data.OrgId = null;
            data.SetUpdateBy(user);
            return BusResponse<int>.Success(await bomHeaderDAL.Update(data));
        }
        public virtual async Task<BusResponse<string>> Insert(MZ_BomHeader data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<string>.Error(111, "非企业用户无法添加物料清单");
            }
            if (string.IsNullOrEmpty(data.ProductId))
            {
                return BusResponse<string>.Error(112, "所属产品不能为空");
            }
            var snowflake = _provider.GetService<SnowflakeHelper>();
            data.Id = snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            if (data.Items != null && data.Items.Count > 0)
            {
                BomLineDAL bomLineDAL = _provider.GetService<BomLineDAL>();
                foreach (var item in data.Items)
                {
                    item.OrgId = user.OrgId;
                    item.Id = snowflake.NextId().ToString();
                    item.HeaderId = data.Id;
                    item.ParentProductId = data.ProductId;
                }
                await bomLineDAL.Insert(data.Items);
            }
            data.SetCreateBy(user);
            await _provider.GetService<BomHeaderDAL>().Insert(data);
            return BusResponse<string>.Success(data.Id);
        }
        public virtual async Task<BusResponse<int>> Delete(string id, IUserInfo user)
        {
            var bomHeaderDAL = _provider.GetService<BomHeaderDAL>();
            MZ_BomHeader old = await bomHeaderDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(111, "物料清单不存在");
            }
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<int>.Error(112, "所属组织错误");
            }
            var rs = await bomHeaderDAL.Delete(id);
            BomLineDAL bomLineDAL = _provider.GetService<BomLineDAL>();
            await bomLineDAL.Delete(x => x.HeaderId == id);
            return BusResponse<int>.Success(rs);
        }
    }
}
