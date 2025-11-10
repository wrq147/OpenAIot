using AuthService.DAL;
using AuthService.Model;
using Common;
using Common.Share;
using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace AuthService.Business
{
    public class UpgradeBLL
    {
        private ITAServiceProvider _provider;
        private UpgradeDAL _upgradeDAL;
        public UpgradeBLL(ITAServiceProvider provider, UpgradeDAL upgradeDAL)
        {
            _provider = provider;
            _upgradeDAL = upgradeDAL;
        }

        /// <summary>
        /// 查询App升级列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual async Task<PageObject<MZ_Upgrade>> SelectList(In_UpgradeList query)
        {
            var tpage = await _upgradeDAL.SelectByPage(query);
            if (tpage.List.Count > 0)
            {
                var tmpdict = await _provider.GetService<OrgDAL>().NavigateDict(tpage.List, x => true, x => x.OrgId);
                foreach (var item in tpage.List)
                {
                    if (tmpdict.TryGetValue(item.OrgId, out MZ_Org tmporg))
                    {
                        item.OrgName = tmporg.OrgName;
                    }
                }
            }

            return tpage;
        }

        public virtual async Task<MZ_Upgrade> SelectByVer(string ver, long orgId)
        {
            return (await _upgradeDAL.SelectList(x => x.OrgId == orgId && x.IsPublish == true && x.UpVersion == ver)).FirstOrDefault<MZ_Upgrade>();
        }
        public virtual async Task<MZ_Upgrade> SelectLast(int type, string platform, long orgId, string styleId)
        {
            return await _SelectLast(type, platform, orgId, styleId);
        }

        /// <summary>
        /// 获取指定包类型和平台的最新发行版本
        /// </summary>
        /// <param name="type">包类型</param>
        /// <param name="platform">平台</param>
        /// <param name="orgId">所属企业</param>
        /// <param name="styleId">所属主题</param>
        /// <returns></returns>
        private async Task<MZ_Upgrade> _SelectLast(int type, string platform, long orgId, string styleId)
        {
            Expression<Func<MZ_Upgrade, bool>> expression = x => x.IsPublish == true;
            if (orgId == 0 && string.IsNullOrEmpty(styleId))
            {
                expression = expression.And(x => x.OrgId == 0 && x.StyleId == "");
            }
            else
            {
                if (orgId > 0 && !string.IsNullOrEmpty(styleId))
                {
                    expression = expression.And(x => x.OrgId == orgId || x.StyleId == styleId);
                }
                else if (orgId > 0)
                {
                    expression = expression.And(x => x.OrgId == orgId);
                }
                else
                {
                    expression = expression.And(x => x.StyleId == styleId);
                }
            }

            if (type >= 0)
            {
                expression = expression.And(x => x.PackageType == type);
            }
            if (!string.IsNullOrEmpty(platform))
            {
                expression = expression.And(x => x.Platform.Contains(platform));
            }
            var tlist = await _upgradeDAL.SelectList(expression, "Id desc");
            if (tlist == null) { return null; }
            return tlist.FirstOrDefault();
        }
        public virtual async Task<MZ_Upgrade> SelectInfo(int id)
        {
            var upgrade = await _upgradeDAL.Select(id);
            if (upgrade != null)
            {
                var org = await _provider.GetService<OrgDAL>().Select(upgrade.OrgId);
                if (org != null)
                {
                    upgrade.OrgName = org.OrgName;
                }
            }
            return await _upgradeDAL.Select(id);
        }
        /// <summary>
        /// 新增App升级
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> InsertUpgrade(MZ_Upgrade data)
        {
            data.OrgId ??= 0;
            data.CreatedOn = DateTime.Now;
            data.StyleId ??= string.Empty;
            data.StyleId = data.StyleId.Trim();
            var unUpgradeList = await _upgradeDAL.SelectList(x => x.PackageType == data.PackageType && x.Platform.Contains(data.Platform) && x.IsPublish == false);
            if (unUpgradeList.Count > 0)
            {
                return BusResponse<int>.Error(111, "已存在同类型的未发行版‘" + unUpgradeList[0].Title + ",版本号：" + unUpgradeList[0].UpVersion + "'");
            }
            string[] plats = data.Platform.Split(',');
            if (plats.Length == 0)
            {
                return BusResponse<int>.Error(112, "发布平台不能为空");
            }
            var lastPack = await _SelectLast(data.PackageType.Value, data.PackageType.Value == 0 ? data.Platform : null, data.OrgId.Value, data.StyleId);
            if (lastPack != null && MyAccess.Core.StringTool.VersionCompare(lastPack.UpVersion, data.UpVersion) == -1)
            {
                return BusResponse<int>.Error(113, "当前包版本号，必须大于当前线上发行版本号");
            }


            return BusResponse<int>.Success(await _upgradeDAL.Insert(data));
        }

        /// <summary>
        /// 批量删除App升级
        /// </summary>
        /// <param name="ids"></param>
        public virtual async Task<BusResponse<int>> DeleteByIds(int[] ids)
        {
            var fileHelper = _provider.GetService<FileHelper>();
            foreach (int id in ids)
            {
                var upgradeItem = await _upgradeDAL.Select(id);
                if (upgradeItem == null)
                {
                    return BusResponse<int>.Error(112, "升级项不存在");
                }
                if (upgradeItem.PackageType == 1 || upgradeItem.Platform == "android")
                {
                    //删除本地资源
                    await fileHelper.DeleteFile(upgradeItem.UpUrl);
                }
                await _upgradeDAL.Delete(id);
            }
            return BusResponse<int>.Success();
        }
    }
}
