using Common;
using Common.IdGenerator;
using Common.Share;
using IoTService.DAL;
using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.Business
{
    public class IotCodeBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private IotCodeDAL _codeDAL;
        private IotCodeGroupDAL _groupDAL;
        public IotCodeBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, IotCodeDAL codeDAL, IotCodeGroupDAL groupDAL)
        {
            _snowflake = snowflake;
            _provider = provider;
            _codeDAL = codeDAL;
            _groupDAL = groupDAL;
        }

        public virtual async Task<BusResponse<List<MZ_IotCodeGroup>>> ListTree(int t)
        {
            var cacheHelper = _provider.GetService<CacheHelper>();
            var rt = cacheHelper.GetCache<List<MZ_IotCodeGroup>>("IotCodeListTree" + t);
            if (rt == null)
            {
                var glist = await _groupDAL.SelectList(x => true, "Sort asc");
                var codelist = await _codeDAL.SelectList(x => x.CodeType == t, "Sort asc");
                var gdict = glist.ToDictionary(x => x.Id.Value);
                foreach (var c in codelist)
                {
                    if (gdict.TryGetValue(c.CodeGroup.Value, out MZ_IotCodeGroup g))
                    {
                        if (g.CodeList == null)
                        {
                            g.CodeList = new List<MZ_IotCode>();
                        }
                        g.CodeList.Add(c);
                    }
                }
                rt = BuildTree(glist);
                cacheHelper.SetCache("IotCodeListTree" + t, rt);
            }
            return BusResponse<List<MZ_IotCodeGroup>>.Success(rt);
        }


        private List<MZ_IotCodeGroup> BuildTree(List<MZ_IotCodeGroup> categorys)
        {
            List<MZ_IotCodeGroup> returnList = new List<MZ_IotCodeGroup>();
            List<int> tempList = new List<int>();
            foreach (MZ_IotCodeGroup cate in categorys)
            {
                tempList.Add(cate.Id.Value);
            }
            foreach (MZ_IotCodeGroup cate in categorys)
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
        private void _RecursionFn(List<MZ_IotCodeGroup> list, MZ_IotCodeGroup t)
        {
            // 得到子节点列表
            List<MZ_IotCodeGroup> childList = _GetChildList(list, t);
            t.Children = childList;
            foreach (MZ_IotCodeGroup tChild in childList)
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
        private List<MZ_IotCodeGroup> _GetChildList(List<MZ_IotCodeGroup> list, MZ_IotCodeGroup t)
        {
            List<MZ_IotCodeGroup> tlist = new List<MZ_IotCodeGroup>();
            foreach (MZ_IotCodeGroup n in list)
            {
                if (n.ParentId == t.Id)
                {
                    tlist.Add(n);
                }
            }
            return tlist;
        }
    }
}
