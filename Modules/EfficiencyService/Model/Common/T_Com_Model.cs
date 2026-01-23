using Common.Share;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyService.Model
{
    [TableName("t_com_model")]
    public class T_Com_Model : BaseEntity
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ID(false)]
        public string Id { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 产品生命周期边界
        /// </summary>
        public string ProductBorder { get; set; }

        /// <summary>
        /// 模型名称
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// 删除标志（0代表存在 2代表删除）
        /// </summary>
        public string del_flag { get; set; }
    }

    public class Out_ModelList
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 模型名称
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProductModel { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 产品生命周期边界
        /// </summary>
        public string ProductBorder { get; set; }

        /// <summary>
        /// 边界标题
        /// </summary>
        public string BorderTitle { get; set; }

        /// <summary>
        /// 边界名称
        /// </summary>
        public string BorderName { get; set; }

        /// <summary>
        /// 创建者Id（不用传）
        /// </summary>
        public long? createId { get; set; }
        /// <summary>
        /// 创建者名称
        /// </summary>
        public string createName { get; set; }
        /// <summary>
        /// 更新者Id（不用传）
        /// </summary>
        public long? updateId { get; set; }
        /// <summary>
        /// 更新者名称
        /// </summary>
        public string updateName { get; set; }

        /// <summary>
        /// 创建时间（不用传）
        /// </summary>
        public DateTime? create_time { get; set; }

        /// <summary>
        /// 更新时间（不用传）
        /// </summary>
        public DateTime? update_time { get; set; }
    }

    public class Out_Model
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 模型名称
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProductModel { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 产品生命周期边界
        /// </summary>
        public string ProductBorder { get; set; }

        /// <summary>
        /// 边界标题
        /// </summary>
        public string BorderTitle { get; set; }

        /// <summary>
        /// 边界名称
        /// </summary>
        public string BorderName { get; set; }

        /// <summary>
        /// 工序
        /// </summary>
        public List<Out_Process> Processes { get; set; }


        /// <summary>
        /// 物料清单
        /// </summary>
        public List<Out_Bom> Materials { get; set; }
    }

    public class In_Model
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 产品生命周期边界
        /// </summary>
        public string ProductBorder { get; set; }

        /// <summary>
        /// 模型名称
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// 生命周期建模
        /// </summary>
        public List<In_Process> Processes { get; set; }

        /// <summary>
        /// 物料清单
        /// </summary>
        public List<T_Prod_Bom> Materials { get; set; }
    }

    public class In_Process
    {
        /// <summary>
        /// 环节编码
        /// </summary>
        public string LinkId { get; set; }

        /// <summary>
        /// 工序名称
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 工序序号
        /// </summary>
        public string ProcessNo { get; set; }

        /// <summary>
        /// 产出物
        /// </summary>
        public List<In_ProcessItem> ProcessItems { get; set; }

    }

    public class In_ProcessItem
    {
        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 设施编码
        /// </summary>
        public string FacilityId { get; set; }
    }

    public class Out_Process
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 环节编码
        /// </summary>
        public string LinkId { get; set; }

        /// <summary>
        /// 环节名称
        /// </summary>
        public string LinkName { get; set; }

        /// <summary>
        /// 工序名称
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 工序序号
        /// </summary>
        public string ProcessNo { get; set; }

        /// <summary>
        /// 产出物
        /// </summary>
        public List<Out_ProcessItem> ProcessItems { get; set; }

        /// <summary>
        /// 输入能源
        /// </summary>
        public List<ShuRu> ShuRus { get; set; }
    }

    public class Out_ProcessItem
    {
        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProductModel { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 设施编码
        /// </summary>
        public string FacilityId { get; set; }

        /// <summary>
        /// 包含子设施编码
        /// </summary>
        public string FacilityIds { get; set; }
    }

    public class ShuRu
    {
        /// <summary>
        /// 能源类型
        /// </summary>
        public string EnergyType { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 因子单位
        /// </summary>
        public string FactorUnit { get; set; }
    }

    public class Out_ProductModelList
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProductModel { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }


        /// <summary>
        /// 产品类型
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 产量
        /// </summary>
        public double OutPut { get; set; }

        /// <summary>
        /// 产品生命周期边界
        /// </summary>
        public string ProductBorder { get; set; }

        /// <summary>
        /// 边界标题
        /// </summary>
        public string BorderTitle { get; set; }

        /// <summary>
        /// 边界名称
        /// </summary>
        public string BorderName { get; set; }

        /// <summary>
        /// 创建者Id（不用传）
        /// </summary>
        public long? createId { get; set; }
        /// <summary>
        /// 创建者名称
        /// </summary>
        public string createName { get; set; }
        /// <summary>
        /// 更新者Id（不用传）
        /// </summary>
        public long? updateId { get; set; }
        /// <summary>
        /// 更新者名称
        /// </summary>
        public string updateName { get; set; }

        /// <summary>
        /// 创建时间（不用传）
        /// </summary>
        public DateTime? create_time { get; set; }

        /// <summary>
        /// 更新时间（不用传）
        /// </summary>
        public DateTime? update_time { get; set; }

        /// <summary>
        /// 统计开始时间
        /// </summary>
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// 统计结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 碳排放量
        /// </summary>
        public double CarbonEmission { get; set; }
    }

    public class Out_ProductModelInfo
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 所属企业Id
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProductModel { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }


        /// <summary>
        /// 产品类型
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 产值
        /// </summary>
        public double OutValue { get; set; }

        /// <summary>
        /// 产量
        /// </summary>
        public double OutPut { get; set; }

        /// <summary>
        /// 产品生命周期边界
        /// </summary>
        public string ProductBorder { get; set; }

        /// <summary>
        /// 边界标题
        /// </summary>
        public string BorderTitle { get; set; }

        /// <summary>
        /// 边界名称
        /// </summary>
        public string BorderName { get; set; }

        /// <summary>
        /// 统计开始时间
        /// </summary>
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// 统计结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 工序
        /// </summary>
        public List<ModelProcess> Processes { get; set; }

        /// <summary>
        /// 物料清单
        /// </summary>
        public List<Out_Bom> Materials { get; set; }
    }

    public class ModelProcess
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 环节编码
        /// </summary>
        public string LinkId { get; set; }

        /// <summary>
        /// 环节名称
        /// </summary>
        public string LinkName { get; set; }

        /// <summary>
        /// 工序名称
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 工序序号
        /// </summary>
        public string ProcessNo { get; set; }

        /// <summary>
        /// 模型编码
        /// </summary>
        public string ModelId { get; set; }

        /// <summary>
        /// 工序
        /// </summary>
        public List<ModelProcessItem> ProcessItems { get; set; }

        /// <summary>
        /// 输入
        /// </summary>
        public List<ShuRu1> shuRus { get; set; }
    }

    public class ModelProcessItem
    {
        /// <summary>
        /// 模型编码
        /// </summary>
        public string ModelId { get; set; }
        /// <summary>
        /// 工序编码
        /// </summary>
        public string ProcessId { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 设施编码
        /// </summary>
        public string FacilityId { get; set; }

        /// <summary>
        /// 包含子设施编码
        /// </summary>
        public string FacilityIds { get; set; }

       /// <summary>
       /// 碳排放量
       /// </summary>
       public double CarbonEmission { get; set; }

       /// <summary>
       /// 折标准煤
       /// </summary>
       public double ConvertCoal { get; set; }
       
    }

    public class ShuRu1
    {
        /// <summary>
        /// 能源类型
        /// </summary>
        public string EnergyType { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 能源消耗量
        /// </summary>
        public double UseVale { get; set; }

        /// <summary>
        /// 一级单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 二级单位
        /// </summary>
        public string LageUnit { get; set; }
    }
}
