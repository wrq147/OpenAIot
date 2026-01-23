using Common.Attr;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;

namespace AfterService.Model
{
    /// <summary>
    /// 房间分类
    /// </summary>
    [TableName("mz_room_category")]
    public class MZ_RoomCategory
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }
        /// <summary>
        /// 房间所属企业Id，为0则无所属
        /// </summary>
        public long? TargetOrgId { get; set; }
        /// <summary>
        /// 房间所属企业名称
        /// </summary>
        [DataIgnore]
        public string TargetName { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        public string CustomerId { get; set; }
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序值：越小越前面
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 父分类Id
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 分类层级
        /// </summary>
        public string Path { get; set; }


        /// <summary>
        /// 子分类
        /// </summary>
        [DataIgnore]
        [OnlySeriaize]
        public List<MZ_RoomCategory> Children { get; set; }


        public static List<MZ_RoomCategory> BuildTree(List<MZ_RoomCategory> categorys)
        {
            List<MZ_RoomCategory> returnList = new List<MZ_RoomCategory>();
            List<string> tempList = new List<string>();
            foreach (MZ_RoomCategory cate in categorys)
            {
                tempList.Add(cate.Id);
            }
            foreach (MZ_RoomCategory cate in categorys)
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
        private static void _RecursionFn(List<MZ_RoomCategory> list, MZ_RoomCategory t)
        {
            // 得到子节点列表
            List<MZ_RoomCategory> childList = _GetChildList(list, t);
            t.Children = childList;
            foreach (MZ_RoomCategory tChild in childList)
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
        private static List<MZ_RoomCategory> _GetChildList(List<MZ_RoomCategory> list, MZ_RoomCategory t)
        {
            List<MZ_RoomCategory> tlist = new List<MZ_RoomCategory>();
            foreach (MZ_RoomCategory n in list)
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
