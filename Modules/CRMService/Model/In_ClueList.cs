using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.Model
{
    public class In_ClueList : BaseQueryParam
    {
        /// <summary>
        /// 为1表示公海，为2表示私海，其它为全部（不用传）
        /// </summary>
        public int? Belong { get; set; }
    }
}
