using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.Model
{
    /// <summary>
    /// Bom树项
    /// </summary>
    public class Out_BomTreeItem
    {
        /// <summary>
        /// 产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 父产品Id
        /// </summary>
        public string ParentProductId { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 使用的计量
        /// </summary>
        public decimal? Quantity { get; set; }

        public List<Out_BomTreeItem> Childrens { get; set; }



        public static List<Out_BomTreeItem> BuildTree(List<Out_BomTreeItem> list)
        {
            List<Out_BomTreeItem> returnList = new List<Out_BomTreeItem>();
            List<string> tempList = new List<string>();
            foreach (Out_BomTreeItem cate in list)
            {
                tempList.Add(cate.ProductId);
            }
            foreach (Out_BomTreeItem cate in list)
            {
                // 如果是顶级节点, 遍历该父节点的所有子节点
                if (!tempList.Contains(cate.ParentProductId))
                {
                    _RecursionFn(list, cate);
                    returnList.Add(cate);
                }
            }

            if (returnList.Count == 0)
            {
                returnList = list;
            }
            return returnList;
        }


        /// <summary>
        /// 递归列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="t"></param>
        private static void _RecursionFn(List<Out_BomTreeItem> list, Out_BomTreeItem t)
        {
            // 得到子节点列表
            List<Out_BomTreeItem> childList = _GetChildList(list, t);
            t.Childrens = childList;
            foreach (Out_BomTreeItem tChild in childList)
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
        private static List<Out_BomTreeItem> _GetChildList(List<Out_BomTreeItem> list, Out_BomTreeItem t)
        {
            List<Out_BomTreeItem> tlist = new List<Out_BomTreeItem>();
            foreach (Out_BomTreeItem n in list)
            {
                if (n.ParentProductId == t.ProductId)
                {
                    tlist.Add(n);
                }
            }
            return tlist;
        }
    }
}
