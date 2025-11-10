using MyAccess.DB.Attr;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AuthService.Model
{
    /// <summary>
    /// 行业实体
    /// </summary>
    [TableName("mz_industry")]
    public class MZ_Industry
    {
        /// <summary>
        /// 行业编码
        /// </summary>
        [ID(false)]
        public int Id { get; set; }
        /// <summary>
        /// 行业名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 父Id
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// 排序
        /// 越小越前
        /// </summary>
        [JsonIgnore]
        public int Sort { get; set; }
    }


    public class IndustryDict
    {
        private Dictionary<int, MZ_Industry> _dict = new Dictionary<int, MZ_Industry>();
        public IndustryDict(List<MZ_Industry> codelist)
        {
            foreach (MZ_Industry code in codelist)
            {
                _dict.Add(code.Id, code);
            }
        }
        /// <summary>
        /// 代码转名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string ToName(int id)
        {
            MZ_Industry outitem;
            if (_dict.TryGetValue(id, out outitem))
            {
                return outitem.Name;
            }
            return null;
        }
    }
}
