using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;

namespace IoTService.Models
{
    [TableName("mz_iot_group")]
    public class MZ_IotGroup : BaseEntity
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
        public List<MZ_IotGroup> Children { get; set; }

        public static List<MZ_IotGroup> BuildTree(List<MZ_IotGroup> categorys)
        {
            List<MZ_IotGroup> returnList = new List<MZ_IotGroup>();
            List<string> tempList = new List<string>();
            foreach (MZ_IotGroup cate in categorys)
            {
                tempList.Add(cate.Id);
            }
            foreach (MZ_IotGroup cate in categorys)
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
        private static void _RecursionFn(List<MZ_IotGroup> list, MZ_IotGroup t)
        {
            // 得到子节点列表
            List<MZ_IotGroup> childList = _GetChildList(list, t);
            t.Children = childList;
            foreach (MZ_IotGroup tChild in childList)
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
        private static List<MZ_IotGroup> _GetChildList(List<MZ_IotGroup> list, MZ_IotGroup t)
        {
            List<MZ_IotGroup> tlist = new List<MZ_IotGroup>();
            foreach (MZ_IotGroup n in list)
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
        public static List<TreeSelect<string>> GroupList2Tree(List<MZ_IotGroup> groupTree)
        {
            List<TreeSelect<string>> treeList = new List<TreeSelect<string>>();
            foreach (MZ_IotGroup group in groupTree)
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
