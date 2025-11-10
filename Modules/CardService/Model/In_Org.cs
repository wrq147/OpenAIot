using AuthService;
using MyAccess.DB.Attr;
using System;
namespace CardService.Model
{
    [TableName("mz_org")]
    public class In_Org : MZ_Org
    {
        /// <summary>
        /// 绑定的外部Id
        /// </summary>
        [DataIgnore]
        public string BindId { get; set; }
    }
}
