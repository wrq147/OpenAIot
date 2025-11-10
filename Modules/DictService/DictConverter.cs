using DictService.Model;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictService
{
    public class DictConverter
    {
        private Dictionary<string, MZ_DictData> _dict = new Dictionary<string, MZ_DictData>();
        public DictConverter(List<MZ_DictData> list)
        {
            foreach (MZ_DictData data in list)
            {
                _dict.Add(data.dict_value, data);
            }
        }
        /// <summary>
        /// 字典值转名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string ToName(string id)
        {
            MZ_DictData outitem;
            if (_dict.TryGetValue(id, out outitem))
            {
                return outitem.dict_label;
            }
            return null;
        }
    }
}
