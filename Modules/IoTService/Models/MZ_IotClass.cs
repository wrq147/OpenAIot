using Common.Attr;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IoTService.Models
{
    /// <summary>
    /// 物联网协议分类
    /// </summary>
    [TableName("mz_iot_class")]
    public class MZ_IotClass
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
        /// 分类名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        [JsonConverter(typeof(ImageUrl))]
        public string PhotoUrl { get; set; }
        /// <summary>
        /// 排序值：越小越前面
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 父分类Id
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 分类层级
        /// </summary>
        [JsonIgnore]
        public string Path { get; set; }
        /// <summary>
        /// 子分类
        /// </summary>
        [DataIgnore]
        public List<MZ_IotClass> Children { get; set; }


        public static List<MZ_IotClass> BuildTree(List<MZ_IotClass> categorys)
        {
            List<MZ_IotClass> returnList = new List<MZ_IotClass>();
            List<string> tempList = new List<string>();
            foreach (MZ_IotClass cate in categorys)
            {
                tempList.Add(cate.Id);
            }
            foreach (MZ_IotClass cate in categorys)
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
        private static void _RecursionFn(List<MZ_IotClass> list, MZ_IotClass t)
        {
            // 得到子节点列表
            List<MZ_IotClass> childList = _GetChildList(list, t);
            t.Children = childList;
            foreach (MZ_IotClass tChild in childList)
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
        private static List<MZ_IotClass> _GetChildList(List<MZ_IotClass> list, MZ_IotClass t)
        {
            List<MZ_IotClass> tlist = new List<MZ_IotClass>();
            foreach (MZ_IotClass n in list)
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
