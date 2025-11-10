
namespace CRMService.Model
{
    public class Out_CustomerInfo
    {
        public string Id { get; set; }
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }
        /// <summary>
        /// 客户唯一编号
        /// </summary>
        public string CustomerNumber { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 客户类型：0为代理，1为直销（新增必填）
        /// </summary>
        public int? CustomerType { get; set; }
        /// <summary>
        /// 客户对应的企业Id,未邀请则为0
        /// </summary>
        public long? BindOrgId { get; set; }

    }
}
