using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AuthService.Model
{
    /// <summary>
    /// 区域代码实体
    /// </summary>
    [TableName("mz_area")]
    public class MZ_Area
    {
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 父区域
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 区域级别
        /// </summary>
        [JsonIgnore]
        public string LevelType { get; set; }
        /// <summary>
        /// 层级路径
        /// </summary>
        [JsonIgnore]
        public string ParentPath { get; set; }
        /// <summary>
        /// 所属省
        /// </summary>
        [JsonIgnore]
        public string Province { get; set; }
        /// <summary>
        /// 所属市
        /// </summary>
        [JsonIgnore]
        public string City { get; set; }
        /// <summary>
        /// 所属区
        /// </summary>
        [JsonIgnore]
        public string District { get; set; }
        /// <summary>
        /// 所属县
        /// </summary>
        [JsonIgnore]
        public string Street { get; set; }
        /// <summary>
        /// 拼音
        /// </summary>
        [JsonIgnore]
        public string Pinyin { get; set; }
        /// <summary>
        /// 首字母拼音
        /// </summary>
        [JsonIgnore]
        public string Jianpin { get; set; }
        /// <summary>
        /// 首字母
        /// </summary>
        [JsonIgnore]
        public string FirstChar { get; set; }
        /// <summary>
        /// 城市区号
        /// </summary>
        [JsonIgnore]
        public string CityCode { get; set; }
        /// <summary>
        /// 邮政编码
        /// </summary>
        [JsonIgnore]
        public string ZipCode { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        [JsonIgnore]
        public string Lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        [JsonIgnore]
        public string Lat { get; set; }
    }


    public class AreaDict
    {
        private Dictionary<string, MZ_Area> _dict = new Dictionary<string, MZ_Area>();
        public AreaDict(List<MZ_Area> codelist)
        {
            foreach (MZ_Area code in codelist)
            {
                _dict.Add(code.Id, code);
            }
        }
        /// <summary>
        /// 代码转名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string ToName(string id)
        {
            MZ_Area outitem;
            if (_dict.TryGetValue(id, out outitem))
            {
                return outitem.Name;
            }
            return null;
        }
        /// <summary>
        /// 多个代码转名称
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public string MultiToName(string ids)
        {
            var regionarr = ids.Split(',');
            string tmpstr = string.Empty;
            foreach (var region in regionarr)
            {
                tmpstr = tmpstr + "," + ToName(region.Trim());
            }
            return tmpstr.TrimStart(',');
        }
    }
}
