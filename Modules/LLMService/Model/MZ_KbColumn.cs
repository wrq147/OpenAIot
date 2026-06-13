using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LLMService.Model
{
    [TableName("llm_kb_column")]
    public class MZ_KbColumn : BaseEntity
    {
        [ID]
        public string Id { get; set; }
        /// <summary>
        /// 知识库ID
        /// </summary>
        public string KbId { get; set; }
        /// <summary>
        /// 知识库
        /// </summary>
        [DataIgnore]
        public MZ_Knowledge Kb { get; set; }

        /// <summary>
        /// 栏目名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 父栏目ID，空表示顶级栏目
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int? SortOrder { get; set; }
        /// <summary>
        /// 分类层级
        /// </summary>
        [JsonIgnore]
        public string Path { get; set; }


        /// <summary>
        /// 子分类
        /// </summary>
        [DataIgnore]
        public List<MZ_KbColumn> Children { get; set; }


        public static List<MZ_KbColumn> BuildTree(List<MZ_KbColumn> categorys)
        {
            List<MZ_KbColumn> returnList = new List<MZ_KbColumn>();
            List<string> tempList = new List<string>();
            foreach (MZ_KbColumn cate in categorys)
            {
                tempList.Add(cate.Id);
            }
            foreach (MZ_KbColumn cate in categorys)
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
        private static void _RecursionFn(List<MZ_KbColumn> list, MZ_KbColumn t)
        {
            // 得到子节点列表
            List<MZ_KbColumn> childList = _GetChildList(list, t);
            t.Children = childList;
            foreach (MZ_KbColumn tChild in childList)
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
        private static List<MZ_KbColumn> _GetChildList(List<MZ_KbColumn> list, MZ_KbColumn t)
        {
            List<MZ_KbColumn> tlist = new List<MZ_KbColumn>();
            foreach (MZ_KbColumn n in list)
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