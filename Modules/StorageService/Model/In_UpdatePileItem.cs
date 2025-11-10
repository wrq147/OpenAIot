

namespace StorageService.Model
{
    public class In_UpdatePileItem
    {
        /// <summary>
        /// 所在仓库
        /// </summary>
        public string HouseId { get; set; }
        /// <summary>
        /// 存储类型：0半成品、1成品
        /// </summary>
        public int TargetType { get; set; }
        /// <summary>
        /// 耗材或设备Id
        /// </summary>
        public string TargetId { get; set; }
        /// <summary>
        /// 库存下限，-1不预警
        /// </summary>
        public int MinNum { get; set; }
        /// <summary>
        /// 库存上限，-1不预警
        /// </summary>
        public int MaxNum { get; set; }
    }
}
