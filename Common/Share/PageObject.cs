using System;
using System.Collections.Generic;

namespace Common.Share
{
    public class PageObject<T>
    {
        public List<T> List { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 返回空
        /// </summary>
        /// <returns></returns>
        public static PageObject<T> Empty()
        {
            var e = new PageObject<T>();
            e.List = new List<T>();
            e.Total = 0;
            return e;
        }
    }
}
