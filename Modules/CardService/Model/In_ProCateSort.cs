using System;
using System.Collections.Generic;

namespace CardService.Model
{
    public class In_ProCateSort
    {
        /// <summary>
        /// 要排序的分类，按排序放在列表里
        /// </summary>
        public List<long> idList { get; set; }
        public long orgId { get; set; }
    }
}
