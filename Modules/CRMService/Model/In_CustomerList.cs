using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.Model
{
    public class In_CustomerList : BaseQueryParam
    {
        /// <summary>
        /// 为1表示公海，为2表示私海，其它为全部（不用传）
        /// </summary>
        public int? Belong { get; set; }
        /// <summary>
        /// 是否邀请：true,false
        /// </summary>
        public bool? IsInvite { get; set; }
        /// <summary>
        /// 跟进即将超时的
        /// </summary>
        public bool? IsFollowOver { get; set; }
    }
}
