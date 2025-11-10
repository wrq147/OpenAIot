using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;

namespace IoTRulesService.Model
{
    [TableName("mz_rule_group")]
    public class MZ_RuleGroup : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 所属组织ID
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 排序值：越小越前面
        /// </summary> 
        public int? Sort { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 父分组Id
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 分组层级
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 子分组
        /// </summary>
        [DataIgnore]
        public List<MZ_RuleGroup> Children { get; set; }

        public static List<MZ_RuleGroup> BuildTree(List<MZ_RuleGroup> categorys)
        {
            List<MZ_RuleGroup> returnList = new List<MZ_RuleGroup>();
            List<string> tempList = new List<string>();
            foreach (MZ_RuleGroup cate in categorys)
            {
                tempList.Add(cate.Id);
            }
            foreach (MZ_RuleGroup cate in categorys)
            {
                // 如果是顶级节点, 遍历该父节点的所有子节点
                if (!tempList.Contains(cate.ParentId))
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
        private static void _RecursionFn(List<MZ_RuleGroup> list, MZ_RuleGroup t)
        {
            // 得到子节点列表
            List<MZ_RuleGroup> childList = _GetChildList(list, t);
            t.Children = childList;
            foreach (MZ_RuleGroup tChild in childList)
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
        private static List<MZ_RuleGroup> _GetChildList(List<MZ_RuleGroup> list, MZ_RuleGroup t)
        {
            List<MZ_RuleGroup> tlist = new List<MZ_RuleGroup>();
            foreach (MZ_RuleGroup n in list)
            {
                if (n.ParentId == t.Id)
                {
                    tlist.Add(n);
                }
            }
            return tlist;
        }
        /// <summary>
        /// 分组树转选择树
        /// </summary>
        /// <param name="groupTree"></param>
        /// <returns></returns>
        public static List<TreeSelect<string>> GroupList2Tree(List<MZ_RuleGroup> groupTree)
        {
            List<TreeSelect<string>> treeList = new List<TreeSelect<string>>();
            foreach (MZ_RuleGroup group in groupTree)
            {
                TreeSelect<string> ts = new TreeSelect<string>();
                ts.parentId = group.ParentId;
                ts.id = group.Id;
                ts.label = group.GroupName;
                if (group.Children != null)
                {
                    ts.children = GroupList2Tree(group.Children);
                }
                treeList.Add(ts);
            }
            return treeList;
        }
    }
}
