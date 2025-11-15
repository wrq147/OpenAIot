using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Share;

namespace EfficiencyService.Model
{
    /// <summary>
    ///分页查询设备
    /// </summary>
    public class In_EquipmentList : BaseQueryParam
    {
        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 设备代码
        /// </summary>
        public string EquipmentCode { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentName { get; set; }

        /// <summary>
        /// 数据状态（1代表纳入计算 2代表不纳入计算）
        /// </summary>
        public string DataState { get; set; }

        /// <summary>
        /// 绑定计费状态（1代表绑定 2代表未绑定）
        /// </summary>
        public string PolicyState { get; set; }

        /// <summary>
        /// 绑定第三方编码状态（1代表绑定 2代表未绑定）
        /// </summary>
        public string ThirdState { get; set; }

        /// <summary>
        /// 设备状态（1代表在线 2代表离线）
        /// </summary>
        public string EquipmentState { get; set; }

        /// <summary>
        /// 设施状态（1代表绑定 2代表未绑定）
        /// </summary>
        public string FacilityState { get; set; }

        /// <summary>
        /// 设施编码
        /// </summary>
        public string FacilityId { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string TypeId { get; set; }
    }

    /// <summary>
    /// 设备
    /// </summary>
    public class In_Equipment
    {
        /// <summary>
        /// 编码 新增不传
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 所属企业Id，新增必传，修改不传
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 设备代码
        /// </summary>
        public string EquipmentCode { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentName { get; set; }

        /// <summary>
        /// 数据状态（1代表纳入计算 2代表不纳入计算）
        /// </summary>
        public string DataState { get; set; }

        /// <summary>
        /// 计费政策编码
        /// </summary>
        public string PolicyId { get; set; }

        /// <summary>
        /// 第三方编码
        /// </summary>
        public string ThirdId { get; set; }

        /// <summary>
        /// 设施编码
        /// </summary>
        public string FacilityId { get; set; }
    }
}
