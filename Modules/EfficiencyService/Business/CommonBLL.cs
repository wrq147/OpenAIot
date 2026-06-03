using AfterService.DAL;
using AuthService;
using ChannelUtility.Tsl;
using Common;
using Common.IdGenerator;
using Common.Share;
using EfficiencyService.DAL;
using EfficiencyService.Model;
using EfficiencyService.Model.Common;
using EfficiencyService.Model.Production;
using TemplateAction.Core;

namespace EfficiencyService.Business
{
    public class CommonBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private CommonDAL _commonDAL;
        private OrgConfDAL _orgConfDAL;
        private FactorDAL _factorDAL;
        private PolicyDAL _policyDAL;
        private ProductionDAL _productionDAL;

        public CommonBLL(ITAServiceProvider provider, CommonDAL commonDAL, OrgConfDAL orgConfDAL, FactorDAL factorDAL, ProductionDAL productionDAL, SnowflakeHelper snowflake)
        {
            _provider = provider;
            _snowflake = snowflake;
            _commonDAL = commonDAL;
            _orgConfDAL = orgConfDAL;
            _factorDAL = factorDAL;
            _productionDAL = productionDAL;
        }

        #region 产品库
        /// <summary>
        /// 新增产品
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddProduct(In_Product in_Product)
        {
            if (!(in_Product.ProductType == "1" || in_Product.ProductType == "2"))
            {
                return BusResponse<string>.Error(500, "产品形态错误 1代表成品 2代表配套件");
            }

            if (!(in_Product.OnMarket == "是" || in_Product.OnMarket == "否")) 
            {
                return BusResponse<string>.Error(500, "是否上市错误 是/否");
            }
            DateTime MarketTime = DateTime.Now;
            if (in_Product.MarketTime != null)
            {
                MarketTime =  (DateTime)in_Product.MarketTime;
            }

            var user = _provider.GetUser();

            T_Com_Product product = new T_Com_Product();
            product.Id = _snowflake.NextId().ToString();

            product.OrgId = in_Product.OrgId;
            product.ProductModel = in_Product.ProductModel;
            product.ProductName = in_Product.ProductName;
            product.ShortName = in_Product.ShortName;
            product.BrandName = in_Product.BrandName;
            product.ProductType = in_Product.ProductType;
            product.ProductPrice = in_Product.ProductPrice;
            product.OnMarket = in_Product.OnMarket;
            product.MarketTime = MarketTime;
            product.Memo = in_Product.Memo;
            product.Unit = in_Product.Unit;

            product.del_flag = "0";
            product.createId = user.UserId;
            product.create_time = DateTime.Now;
            product.updateId = user.UserId;
            product.update_time = DateTime.Now;
            await _commonDAL.AddProduct(product);
            return BusResponse<string>.Success("新增成功");
        }

        /// <summary>
        /// 新增产品
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> ImportProduct(List<In_Product> list)
        {
            foreach (var in_Product in list)
            {
                if (!(in_Product.ProductType == "1" || in_Product.ProductType == "2"))
                {
                    return BusResponse<string>.Error(500, "产品形态错误 1代表成品 2代表配套件");
                }
                if (!(in_Product.OnMarket == "是" || in_Product.OnMarket == "否"))
                {
                    return BusResponse<string>.Error(500, "是否上市错误 是/否");
                }
                DateTime MarketTime = DateTime.Now;
                if (in_Product.MarketTime != null)
                {
                    MarketTime = (DateTime)in_Product.MarketTime;
                }
            }
            var user = _provider.GetUser();
            foreach (var in_Product in list)
            {
                DateTime MarketTime = DateTime.Now;
                if (in_Product.MarketTime != null)
                {
                    MarketTime = (DateTime)in_Product.MarketTime;
                }

                T_Com_Product com_Product = await _commonDAL.SelectProduct(in_Product.Id);

                if (com_Product == null)
                {
                    T_Com_Product product = new T_Com_Product();
                    product.Id = _snowflake.NextId().ToString();

                    product.OrgId = user.OrgId;
                    product.ProductModel = in_Product.ProductModel == null ? "" : in_Product.ProductModel;
                    product.ProductName = in_Product.ProductName == null ? "" : in_Product.ProductName;
                    product.ShortName = in_Product.ShortName == null ? "" : in_Product.ShortName;
                    product.BrandName = in_Product.BrandName == null ? "" : in_Product.BrandName;
                    product.ProductType = in_Product.ProductType == null ? "" : in_Product.ProductType;
                    product.ProductPrice = in_Product.ProductPrice;
                    product.OnMarket = in_Product.OnMarket;
                    product.MarketTime = MarketTime;
                    product.Memo = in_Product.Memo == null? "" : in_Product.Memo;
                    product.Unit = in_Product.Unit;

                    product.del_flag = "0";
                    product.createId = user.UserId;
                    product.create_time = DateTime.Now;
                    product.updateId = user.UserId;
                    product.update_time = DateTime.Now;

                    await _commonDAL.AddProduct(product);
                }
                else
                {
                    com_Product.ProductModel = in_Product.ProductModel == null ? "" : in_Product.ProductModel;
                    com_Product.ProductName = in_Product.ProductName == null ? "" : in_Product.ProductName;
                    com_Product.ShortName = in_Product.ShortName == null ? "" : in_Product.ShortName;
                    com_Product.BrandName = in_Product.BrandName == null ? "" : in_Product.BrandName;
                    com_Product.ProductType = in_Product.ProductType == null ? "" : in_Product.ProductType;
                    com_Product.ProductPrice = in_Product.ProductPrice;
                    com_Product.OnMarket = in_Product.OnMarket;
                    com_Product.MarketTime = MarketTime;
                    com_Product.Memo = in_Product.Memo == null ? "" : in_Product.Memo;
                    com_Product.Unit = in_Product.Unit;
                    com_Product.updateId = user.UserId;
                    com_Product.update_time = DateTime.Now;

                    await _commonDAL.UpdateProduct(com_Product);
                }
            }
            return BusResponse<string>.Success("导入成功");
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<int>> DeleteProduct(string Id)
        {
            await _commonDAL.DeleteProduct(Id);
            return BusResponse<int>.Success(0, "删除成功");
        }

        /// <summary>
        /// 修改产品
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateProduct(In_Product in_Product)
        {
            if (!(in_Product.ProductType == "1" || in_Product.ProductType == "2"))
            {
                return BusResponse<int>.Error(500, "产品形态错误 1代表成品 2代表配套件");
            }

            if (!(in_Product.OnMarket == "是" || in_Product.OnMarket == "否"))
            {
                return BusResponse<int>.Error(500, "是否上市错误 是/否");
            }

            DateTime MarketTime = DateTime.Now;
            if (in_Product.MarketTime != null)
            {
                MarketTime = (DateTime)in_Product.MarketTime;
            }

            var user = _provider.GetUser();

            T_Com_Product product = new T_Com_Product();
           
            product.Id = in_Product.Id;
            product.ProductModel = in_Product.ProductModel;
            product.ProductName = in_Product.ProductName;
            product.ShortName = in_Product.ShortName;
            product.BrandName = in_Product.BrandName;
            product.ProductType = in_Product.ProductType;
            product.ProductPrice = in_Product.ProductPrice;
            product.OnMarket = in_Product.OnMarket;
            product.MarketTime = MarketTime;
            product.Memo = in_Product.Memo;
            product.Unit = in_Product.Unit;
            product.updateId = user.UserId;
            product.update_time = DateTime.Now;

            int result = await _commonDAL.UpdateProduct(product);
            return BusResponse<int>.Success(result, "修改成功");
        }

        /// <summary>
        /// 分页查询产品列表
        /// </summary>
        /// <returns></returns>
        public async Task<PageObject<T_Com_Product>> SelectProductList(In_ProductPageList query)
        {
            return await _commonDAL.SelectProductPage(query);
        }

        /// <summary>
        /// 分页查询产品能效指标
        /// </summary>
        /// <returns></returns>
        public async Task<PageObject<Out_ProductEnergy>> SelectProductEnergyPage(In_ProductEnergyPageList query)
        {
            return await _commonDAL.SelectProductEnergyPage(query);
        }

        /// <summary>
        /// 查询产品信息
        /// </summary>
        /// <returns></returns>
        public async Task<T_Com_Product> SelectProduct(string Id)
        {
            return await _commonDAL.SelectProduct(Id);
        }

        /// <summary>
        /// 查询设施详情
        /// </summary>
        /// <returns></returns>
        public async Task<List<T_Com_Product>> SelectFacilityProduct(InFacilityProductList query)
        {
            string facilityIds = "";
            if (!string.IsNullOrEmpty(query.FacilityId) && query.OrgId != null)
            {
                List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree((long)query.OrgId);

                facilityIds = DeppIDS(query.FacilityId, list);

                query.FacilityId = facilityIds + query.FacilityId;
            }
            return await _commonDAL.SelectFacilityProduct(query);
        }

        #endregion

        #region 设施
        /// <summary>
        /// 新增设施
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddFacility(In_FacilityAdd in_Facility)
        {
            if (string.IsNullOrEmpty(in_Facility.ParentId))
            {
                in_Facility.ParentId = "-";
            }
            else
            {
                T_Com_Facility parentFacility = await _commonDAL.SelectFacility(in_Facility.ParentId);//父节点信息
                if (parentFacility == null)
                {
                    return BusResponse<string>.Error(500, "找不到父设施");
                }
                if (parentFacility.FacilityType)
                {
                    return BusResponse<string>.Error(500, "该设施绑定了设备，不能添加子设施");
                }
            }

            if (in_Facility.OrgId == null)
            {
                return BusResponse<string>.Error(500, "缺少企业ID");
            }

            var user = _provider.GetUser();

            T_Com_Facility facility = new T_Com_Facility();
            facility.Id = _snowflake.NextId().ToString();

            facility.OrgId = in_Facility.OrgId;
            facility.FacilityName = in_Facility.FacilityName;
            facility.Manager = in_Facility.Manager;
            facility.Contact = in_Facility.Contact;
            facility.ParentId = in_Facility.ParentId;
            facility.FacilityType = in_Facility.FacilityType;
            facility.FacilityCode = in_Facility.FacilityCode;

            facility.del_flag = "0";
            facility.createId = user.UserId;
            facility.create_time = DateTime.Now;
            facility.updateId = user.UserId;
            facility.update_time = DateTime.Now;
            await _commonDAL.AddFacility(facility);
            return BusResponse<string>.Success("新增成功");
        }

        /// <summary>
        /// 删除设施
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<int>> DeleteFacility(string Id)
        {
            await _commonDAL.DeleteFacility(Id);
            return BusResponse<int>.Success(0, "删除成功");
        }

        /// <summary>
        /// 修改设施
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateFacility(In_FacilityEdit in_Facility)
        {
            var user = _provider.GetUser();

            T_Com_Facility facility = await _commonDAL.SelectFacility(in_Facility.Id);
            if (facility == null) 
            {
                return BusResponse<int>.Error(500, "找不到设施");
            }
            if (in_Facility.ParentId != facility.ParentId)//更新父节点
            {
                if (in_Facility.ParentId == facility.Id)
                {
                    return BusResponse<int>.Error(500, "不能修改成自己");
                }
                T_Com_Facility Pfacility = await _commonDAL.SelectFacility(in_Facility.ParentId);
                if (Pfacility == null)
                {
                    return BusResponse<int>.Error(500, "找不到分组");
                }
                else
                {
                    if (Pfacility.FacilityType)
                    {
                        return BusResponse<int>.Error(500, "不能移动到设施底下");
                    }
                    else
                    {
                        facility.ParentId = in_Facility.ParentId;
                    }
                }
            }
            facility.FacilityName = in_Facility.FacilityName;
            facility.Manager = in_Facility.Manager;
            facility.Contact = in_Facility.Contact;
            facility.FacilityCode = in_Facility.FacilityCode;

            facility.updateId = user.UserId;
            facility.update_time = DateTime.Now;

            int result = await _commonDAL.UpdateFacility(facility);
            return BusResponse<int>.Success(result, "修改成功");
        }

        /// <summary>
        /// 查询设施详情
        /// </summary>
        /// <returns></returns>
        public async Task<T_Com_Facility> SelectFacility(string Id)
        {
            return await _commonDAL.SelectFacility(Id);
        }

        /// <summary>
        /// 查询设施树
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_Facility>> SelectFacilityTree(long OrgId)
        {
            List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree(OrgId);

            return DeppTree("-", list);
        }

        /// <summary>
        /// 查询子设施信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<T_Com_Facility>> SelectFacilityByParentId(string ParentId)
        {
            return await _commonDAL.SelectFacilityByParentId(ParentId);
        }

        /// <summary>
        /// 查询设施树带设备
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_Facility>> SelectFacilityEquipmentTree(long OrgId)
        {
            List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree(OrgId);

            List<T_Com_Equipment> equipments = await _commonDAL.SelectEquipmentList(OrgId);

            return DeppEquipmentTree("-", list, equipments);
        }

        private static List<Out_Facility> DeppEquipmentTree(string ParentId, List<T_Com_Facility> data, List<T_Com_Equipment> equipments)
        {
            List<Out_Facility> list = new List<Out_Facility>();
            foreach (var item in data)
            {
                if (item.ParentId == ParentId)
                {
                    if (item.Id == "371944411467845")
                    {

                    }

                    Out_Facility ft = new Out_Facility();
                    ft.Id = item.Id;
                    ft.FacilityName = item.FacilityName;
                    ft.FacilityCode = item.FacilityCode;
                    ft.Manager = item.Manager;
                    ft.Contact = item.Contact;
                    ft.ParentId = item.ParentId;
                    ft.FacilityType = item.FacilityType;
                    if (ft.FacilityType)
                    {
                        ft.Equipments = new List<T_Com_Equipment>();
                        foreach (var jtem in equipments)
                        {
                            if (jtem.FacilityId == item.Id)
                            {
                                ft.Equipments.Add(jtem);
                            }
                        }
                    }
                    ft.Children = DeppEquipmentTree(item.Id, data, equipments);

                    list.Add(ft);
                }
            }
            return list;
        }

        private static List<Out_Facility> DeppTree(string ParentId, List<T_Com_Facility> data)
        {
            List<Out_Facility> list = new List<Out_Facility>();
            foreach (var item in data)
            {
                if (item.ParentId == ParentId)
                {
                    Out_Facility ft = new Out_Facility();
                    ft.Id = item.Id;
                    ft.FacilityName = item.FacilityName;
                    ft.FacilityCode = item.FacilityCode;
                    ft.Manager = item.Manager;
                    ft.Contact = item.Contact;
                    ft.ParentId = item.ParentId;
                    ft.FacilityType = item.FacilityType;
                    ft.Children = DeppTree(item.Id, data);

                    list.Add(ft);
                }
            }
            return list;
        }

        private static string DeppIDS(string ParentId, List<T_Com_Facility> data)
        {
            string Ids = "";
            foreach (var item in data)
            {
                if (item.ParentId == ParentId)
                {
                    Ids += item.Id + ",";
                    Ids += DeppIDS(item.Id, data);
                }
            }
            return Ids;
        }

        /// <summary>
        /// 新增设施绑定设备
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddFacilityBind(In_FacilityBind in_FacilityBind)
        {  
            T_Com_Facility facility = await _commonDAL.SelectFacility(in_FacilityBind.FacilityId);//设施信息
            List<T_Com_Facility> children = await _commonDAL.SelectFacilityByParentId(in_FacilityBind.FacilityId);//子设施信息
            //设施下不能有子设施，有子设施的是目录
            if (children.Count > 0)
            {
                return BusResponse<string>.Error(500, "设施下有设施，不能绑定设备");
            }
            if (facility==null)
            {
                return BusResponse<string>.Error(500, "找不到设施");
            }
            //如果绑定了设备后，更新设施类型
            if (!facility.FacilityType)
            {
                facility.FacilityType = true;
                await _commonDAL.UpdateFacility(facility);
            }
            string[] FacilityIds = in_FacilityBind.EquipmentId.Split(',');
            foreach (var item in FacilityIds)
            {
                //检验设备Id
                T_Com_Equipment equipment = await _commonDAL.SelectEquipment(item);
                if (equipment == null)
                {
                    return BusResponse<string>.Error(500, "找不到设备");
                }

                var user = _provider.GetUser();

                equipment.FacilityId = in_FacilityBind.FacilityId;

                equipment.updateId = user.UserId;
                equipment.update_time = DateTime.Now;
                await _commonDAL.UpdateEquipment(equipment);
            }
            return BusResponse<string>.Success("绑定成功");
        }

        public virtual async Task<BusResponse<string>> GenerateNumber(string Id)
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            return BusResponse<string>.Success(await tmpredis.GenerateNumber(Id));
        }


        /// <summary>
        /// 删除设施绑定的设备
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<int>> DeleteFacilityBind(string Id)
        {
            //检验设备Id
            T_Com_Equipment equipment = await _commonDAL.SelectEquipment(Id);
            if (equipment == null)
            {
                return BusResponse<int>.Error(500, "找不到设备");
            }

            var user = _provider.GetUser();

            equipment.FacilityId = "-";

            equipment.updateId = user.UserId;
            equipment.update_time = DateTime.Now;
            await _commonDAL.UpdateEquipment(equipment);
            return BusResponse<int>.Success(0, "解绑成功");
        }

        #endregion

        #region 设备
        /// <summary>
        /// 新增设备
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddEquipment(In_Equipment in_Equipment)
        {
            if (string.IsNullOrEmpty(in_Equipment.DataState) || !(in_Equipment.DataState == "1" || in_Equipment.DataState == "2"))
            {
                return BusResponse<string>.Error(500, "数据状态错误（1代表纳入计算 2代表不纳入计算）");
            }

            if (in_Equipment.OrgId == null)
            {
                return BusResponse<string>.Error(500, "缺少企业ID");
            }
            if (string.IsNullOrEmpty(in_Equipment.PolicyId))
            {
                in_Equipment.PolicyId = "-";
            }
            if (string.IsNullOrEmpty(in_Equipment.ThirdId))
            {
                in_Equipment.ThirdId = "-";
            }
            if (string.IsNullOrEmpty(in_Equipment.FacilityId))
            {
                in_Equipment.FacilityId = "-";
            }

            var user = _provider.GetUser();

            T_Com_Equipment equipment = new T_Com_Equipment();
            equipment.Id = _snowflake.NextId().ToString();

            equipment.OrgId = in_Equipment.OrgId;
            equipment.EquipmentCode = in_Equipment.EquipmentCode;
            equipment.EquipmentName = in_Equipment.EquipmentName;
            equipment.DataState = in_Equipment.DataState;
            equipment.PolicyId = in_Equipment.PolicyId;
            equipment.ThirdId = in_Equipment.ThirdId;
            equipment.FacilityId = in_Equipment.FacilityId;


            equipment.del_flag = "0";
            equipment.createId = user.UserId;
            equipment.create_time = DateTime.Now;
            equipment.updateId = user.UserId;
            equipment.update_time = DateTime.Now;
            await _commonDAL.AddEquipment(equipment);

            T_Com_Facility facility = await _commonDAL.SelectFacility(equipment.FacilityId);

            if (facility != null && !facility.FacilityType)
            {
                facility.FacilityType = true;
                await _commonDAL.UpdateFacility(facility);
            }


            return BusResponse<string>.Success("新增成功");
        }
        /// <summary>
        /// 删除设备
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<int>> DeleteEquipment(string Id)
        {
            await _commonDAL.DeleteEquipment(Id);
            return BusResponse<int>.Success(0, "删除成功");
        }
        /// <summary>
        /// 修改设备
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateEquipment(In_Equipment in_Equipment)
        {
            if (string.IsNullOrEmpty(in_Equipment.DataState) || !(in_Equipment.DataState == "1" || in_Equipment.DataState == "2"))
            {
                return BusResponse<int>.Error(500, "数据状态错误（1代表纳入计算 2代表不纳入计算）");
            }
            if (string.IsNullOrEmpty(in_Equipment.PolicyId))
            {
                in_Equipment.PolicyId = "-";
            }
            if (string.IsNullOrEmpty(in_Equipment.ThirdId))
            {
                in_Equipment.ThirdId = "-";
            }
            if (string.IsNullOrEmpty(in_Equipment.FacilityId))
            {
                in_Equipment.FacilityId = "-";
            }

            var user = _provider.GetUser();

            T_Com_Equipment equipment = await _commonDAL.SelectEquipment(in_Equipment.Id);
            if (equipment == null)
            {
                return BusResponse<int>.Error(500, "找不到设备");
            }
            equipment.EquipmentCode = in_Equipment.EquipmentCode;
            equipment.EquipmentName = in_Equipment.EquipmentName;
            equipment.DataState = in_Equipment.DataState;
            equipment.PolicyId = in_Equipment.PolicyId;
            equipment.ThirdId = in_Equipment.ThirdId;
            equipment.DataState = in_Equipment.DataState;

            equipment.updateId = user.UserId;
            equipment.update_time = DateTime.Now;

            int result = await _commonDAL.UpdateEquipment(equipment);
            return BusResponse<int>.Success(result, "修改成功");
        }

        /// <summary>
        /// 分页查询设备
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<Out_Equipment>> SelectEquipmentPage(In_EquipmentList query)
        {
            string facilityIds = "";
            if (!string.IsNullOrEmpty(query.FacilityId) && query.OrgId != null)
            {
                List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree((long)query.OrgId);

                facilityIds = DeppIDS(query.FacilityId, list);

                query.FacilityId = facilityIds + query.FacilityId;
            }
            return await _commonDAL.SelectEquipmentPage(query);
        }

        /// <summary>
        /// 查询设备详情
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<Out_Equipment> SelectEquipmentInfo(string Id)
        {
            return await _commonDAL.SelectEquipmentInfo(Id);
        }

        #endregion

        #region 排放类型
        /// <summary>
        /// 新增排放类型
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddClass(In_Class in_Class)
        {
            if (!(in_Class.RangeId == "1" || in_Class.RangeId == "2" || in_Class.RangeId == "3"))
            {
                return BusResponse<string>.Error(500, "范围错误,1代表范围1 2代表范围2 3代表范围3");
            }

            var user = _provider.GetUser();

            T_Com_Class t_Class = new T_Com_Class();
            t_Class.Id = _snowflake.NextId().ToString();

            t_Class.OrgId = in_Class.OrgId;
            t_Class.ClassNo = in_Class.ClassNo;
            t_Class.ClassName = in_Class.ClassName;
            t_Class.RangeId = in_Class.RangeId;

            t_Class.del_flag = "0";
            t_Class.createId = user.UserId;
            t_Class.create_time = DateTime.Now;
            t_Class.updateId = user.UserId;
            t_Class.update_time = DateTime.Now;
            await _commonDAL.AddClass(t_Class);

            if (in_Class.SubClass != null)
            {
                foreach (var item in in_Class.SubClass)
                {
                    T_Com_SubClass t_SubClass = new T_Com_SubClass();
                    t_SubClass.Id = _snowflake.NextId().ToString();

                    t_SubClass.ClassId = t_Class.Id;
                    t_SubClass.SubClassName = item.SubClassName;

                    t_SubClass.del_flag = "0";
                    await _commonDAL.AddSubClass(t_SubClass);
                }
            }

            return BusResponse<string>.Success("新增成功");
        }

        /// <summary>
        /// 删除排放类型
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<int>> DeleteClass(string Id)
        {
            await _commonDAL.DeleteClass(Id);
            return BusResponse<int>.Success(0, "删除成功");
        }

        /// <summary>
        /// 修改排放类型
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateClass(In_Class in_Class)
        {
            if (!(in_Class.RangeId == "1" || in_Class.RangeId == "2" || in_Class.RangeId == "3"))
            {
                return BusResponse<int>.Error(500, "范围错误,1代表范围1 2代表范围2 3代表范围3");
            }

            T_Com_Class t_Class = await _commonDAL.SelectClass(in_Class.Id);

            var user = _provider.GetUser();

            t_Class.ClassNo = in_Class.ClassNo;
            t_Class.ClassName = in_Class.ClassName;
            t_Class.RangeId = in_Class.RangeId;

            t_Class.updateId = user.UserId;
            t_Class.update_time = DateTime.Now;

            int result = await _commonDAL.UpdateClass(t_Class);

            List<T_Com_SubClass> old_SubClass = await _commonDAL.SelectSubClass(in_Class.Id);
            List<string> updateId = new List<string>();

            if (in_Class.SubClass != null)
            {
                foreach (var item in in_Class.SubClass)
                {
                    if (string.IsNullOrEmpty(item.Id))//新增
                    {
                        T_Com_SubClass t_SubClass = new T_Com_SubClass();
                        t_SubClass.Id = _snowflake.NextId().ToString();

                        t_SubClass.ClassId = t_Class.Id;
                        t_SubClass.SubClassName = item.SubClassName;

                        t_SubClass.del_flag = "0";
                        await _commonDAL.AddSubClass(t_SubClass);
                    }
                    else
                    {
                        foreach (var jtem in old_SubClass)
                        {
                            if (item.Id == jtem.Id)
                            {
                                updateId.Add(jtem.Id);
                                jtem.SubClassName = item.SubClassName;
                                await _commonDAL.UpdateSubClass(jtem);
                            }
                        }
                    }
                }
                foreach (var item in old_SubClass)
                {
                    if (!updateId.Contains(item.Id))
                    {
                        await _commonDAL.DeleteSubClass(item.Id);
                    }
                }
            }

            return BusResponse<int>.Success(result, "修改成功");
        }

        /// <summary>
        /// 分页查询排放类型列表
        /// </summary>
        /// <returns></returns>
        public async Task<PageObject<Out_Class>> SelectClassList(In_ClassPageLis query)
        {
            return await _commonDAL.SelectClassPage(query);
        }

        /// <summary>
        /// 查询排放类型信息
        /// </summary>
        /// <returns></returns>
        public async Task<Out_Class> SelectClass(string Id)
        {
            Out_Class out_Class = await _commonDAL.SelectClass(Id);
            out_Class.SubClass = await _commonDAL.SelectSubClass(Id);
            return out_Class;
        }


        #endregion

        #region 碳排核算
        /// <summary>
        /// 查询企业碳排核算
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_OrgClass>> SelectOrgClass(In_OrgClassList query)
        {
            List<Out_OrgClass> out_OrgClass = new List<Out_OrgClass>();

            //把六大类型查出来
            List<Out_Class> out_Class = await _commonDAL.SelectClassAll();
            foreach (var item in out_Class)
            {
                Out_OrgClass orgClass = new Out_OrgClass();
                orgClass.Id = item.Id;
                orgClass.ClassNo = item.ClassNo;
                orgClass.ClassName = item.ClassName;
                orgClass.RangeId = item.RangeId;
                orgClass.OrgClasses = await _commonDAL.SelectOrgClass(query.OrgId, item.Id);

                out_OrgClass.Add(orgClass);
            }
            return out_OrgClass;
        }

        /// <summary>
        /// 新增企业排放类型
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddOrgClass(In_OrgClass in_OrgClass)
        {
            if (!(in_OrgClass.DataSource == "1" || in_OrgClass.DataSource == "2" || in_OrgClass.DataSource == "3"))
            {
                return BusResponse<string>.Error(500, "活动数据来源（1代表计量设备 2代表手工录入 3供应链数据）");
            }

            T_Com_SubClass subClass = await _commonDAL.SelectSubClassInfo(in_OrgClass.SubClassId);
            if (subClass == null)
            {
                return BusResponse<string>.Error(500, "找不到子排放类别");
            }

            T_ENG_Factor factor = await _factorDAL.SelectFactor(in_OrgClass.FactorId);
            if (factor == null)
            {
                return BusResponse<string>.Error(500, "找不到怕排放因子");
            }

            if (!string.IsNullOrEmpty(in_OrgClass.EquipmentIds))
            {
                string facilityIds = "";
                List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree((long)in_OrgClass.OrgId);

                facilityIds = DeppIDS(in_OrgClass.FacilityId, list) + in_OrgClass.FacilityId;

                string[] arr = facilityIds.Split(",");

                List<T_Com_Equipment> equipments = await _commonDAL.SelectEquipmentList((long)in_OrgClass.OrgId);
                string[] strings = in_OrgClass.EquipmentIds.Split(',');
                foreach (string str in strings)
                {
                    bool flg = false;
                    string name = "";
                    foreach (T_Com_Equipment item in equipments)
                    {
                        if (item.Id == str)//匹配到设备
                        {
                            if (!arr.Contains(item.FacilityId))//不在设施列表内提示
                            {
                                flg = true;
                                name = item.EquipmentName;
                            }
                        }
                    }
                    if (flg)
                    {

                        return BusResponse<string>.Error(500, "设备【" + name + "】不在设施下");
                    }
                }
            }

            var user = _provider.GetUser();

            T_Com_OrgClass t_OrgClass = new T_Com_OrgClass();
            t_OrgClass.Id = _snowflake.NextId().ToString();

            t_OrgClass.OrgId = in_OrgClass.OrgId;
            t_OrgClass.ClassId = subClass.ClassId;
            t_OrgClass.SubClassId = in_OrgClass.SubClassId;
            t_OrgClass.FacilityId = in_OrgClass.FacilityId;
            t_OrgClass.FactorId = in_OrgClass.FactorId;
            t_OrgClass.FactorType = factor.TypeId;
            t_OrgClass.DataSource = in_OrgClass.DataSource;
            t_OrgClass.EquipmentIds = in_OrgClass.EquipmentIds;

            t_OrgClass.del_flag = "0";
            t_OrgClass.createId = user.UserId;
            t_OrgClass.create_time = DateTime.Now;
            t_OrgClass.updateId = user.UserId;
            t_OrgClass.update_time = DateTime.Now;
            await _commonDAL.AddOrgClass(t_OrgClass);

            return BusResponse<string>.Success("新增成功");
        }

        /// <summary>
        /// 删除企业排放类型
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<int>> DeleteOrgClass(string Id)
        {
            await _commonDAL.DeleteOrgClass(Id);
            return BusResponse<int>.Success(0, "删除成功");
        }

        /// <summary>
        /// 修改排放类型
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateOrgClass(In_OrgClass in_OrgClass)
        {
            if (!(in_OrgClass.DataSource == "1" || in_OrgClass.DataSource == "2" || in_OrgClass.DataSource == "3"))
            {
                return BusResponse<int>.Error(500, "活动数据来源（1代表计量设备 2代表手工录入 3供应链数据）");
            }

            T_Com_SubClass subClass = await _commonDAL.SelectSubClassInfo(in_OrgClass.SubClassId);
            if (subClass == null)
            {
                return BusResponse<int>.Error(500, "找不到子排放类别");
            }

            T_ENG_Factor factor = await _factorDAL.SelectFactor(in_OrgClass.FactorId);
            if (factor == null)
            {
                return BusResponse<int>.Error(500, "找不到排放因子");
            }

            if (!string.IsNullOrEmpty(in_OrgClass.EquipmentIds))
            {
                string facilityIds = "";
                List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree((long)in_OrgClass.OrgId);

                facilityIds = DeppIDS(in_OrgClass.FacilityId, list) + in_OrgClass.FacilityId;

                string[] arr = facilityIds.Split(",");

                List<T_Com_Equipment> equipments = await _commonDAL.SelectEquipmentList((long)in_OrgClass.OrgId);
                string[] strings = in_OrgClass.EquipmentIds.Split(',');
                foreach (string str in strings)
                {
                    bool flg = false;
                    string name = "";
                    foreach (T_Com_Equipment item in equipments)
                    {
                        if (item.Id == str)//匹配到设备
                        {
                            if (!arr.Contains(item.FacilityId))//不在设施列表内提示
                            {
                                flg = true;
                                name = item.EquipmentName;
                            }
                        }
                    }
                    if (flg)
                    {
                        return BusResponse<int>.Error(500, "设备【" + name + "】不在设施下");
                    }
                }
            }

            T_Com_OrgClass t_OrgClass = await _commonDAL.SelectOrgClassInfo(in_OrgClass.Id);

            var user = _provider.GetUser();

            t_OrgClass.ClassId = subClass.ClassId;
            t_OrgClass.SubClassId = in_OrgClass.SubClassId;
            t_OrgClass.FacilityId = in_OrgClass.FacilityId;
            t_OrgClass.FactorId = in_OrgClass.FactorId;
            t_OrgClass.FactorType = factor.TypeId;
            t_OrgClass.DataSource = in_OrgClass.DataSource;
            t_OrgClass.EquipmentIds = in_OrgClass.EquipmentIds;

            t_OrgClass.updateId = user.UserId;
            t_OrgClass.update_time = DateTime.Now;

            int result = await _commonDAL.UpdateOrgClass(t_OrgClass);

            return BusResponse<int>.Success(result, "修改成功");
        }


        #endregion

        #region 产品生命周期模型
        /// <summary>
        /// 新增产品生命周期模型
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddModel(In_Model in_Model)
        {
            if (!(in_Model.ProductBorder == "1" || in_Model.ProductBorder == "2" || in_Model.ProductBorder == "3"))
            {
                return BusResponse<string>.Error(500, "产品生命周期边界错误:1,2,3");
            }

            T_Com_Product product = await _commonDAL.SelectProduct(in_Model.ProductId);

            if (product == null)
            {
                return BusResponse<string>.Error(500, "找不到产品");
            }

            var user = _provider.GetUser();

            T_Com_Model t_Model = new T_Com_Model();
            t_Model.Id = _snowflake.NextId().ToString();

            t_Model.OrgId = in_Model.OrgId;
            t_Model.ProductId = in_Model.ProductId;
            t_Model.ProductBorder = in_Model.ProductBorder;
            t_Model.ModelName = in_Model.ModelName;

            t_Model.del_flag = "0";
            t_Model.createId = user.UserId;
            t_Model.create_time = DateTime.Now;
            t_Model.updateId = user.UserId;
            t_Model.update_time = DateTime.Now;
            //添加
            await _commonDAL.AddModel(t_Model);

            List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree((long)in_Model.OrgId);

            //添加
            foreach (var item in in_Model.Processes)
            {
                T_Com_Process t_Process = new T_Com_Process();
                t_Process.Id = _snowflake.NextId().ToString();

                t_Process.ModelId = t_Model.Id;
                t_Process.LinkId = item.LinkId;
                t_Process.ProcessName = item.ProcessName;
                t_Process.ProcessNo = item.ProcessNo;

                await _commonDAL.AddProcess(t_Process);

                foreach (var jtem in item.ProcessItems)
                {
                    List<string> facilityIds = new List<string>();
                    string[] arr = jtem.FacilityId.Split(',');//用户选择的设施
                    foreach (var ktem in arr)
                    {
                        if (!facilityIds.Contains(ktem))//设施加进去
                        {
                            if (!string.IsNullOrEmpty(ktem))
                            {
                                facilityIds.Add(ktem);
                            }
                        }

                        string[] brr = DeppIDS(ktem, list).Split(',');//每个设施的子设施

                        foreach (var ltem in brr)
                        {
                            if (!facilityIds.Contains(ltem))//子设施加进去
                            {
                                if (!string.IsNullOrEmpty(ltem))
                                {
                                    facilityIds.Add(ltem);
                                }
                            }
                        }
                    }

                    string str = String.Join(",", facilityIds);

                    T_Com_ProcessItem t_ProcessItem = new T_Com_ProcessItem();
                    t_ProcessItem.ModelId = t_Model.Id;
                    t_ProcessItem.ProcessId = t_Process.Id;
                    t_ProcessItem.FacilityId = jtem.FacilityId;
                    t_ProcessItem.FacilityIds = str;
                    t_ProcessItem.ProductId = jtem.ProductId;

                    await _commonDAL.AddProcessItem(t_ProcessItem);
                }
            }


            if (in_Model.Materials != null)
            {
                foreach (var jtem in in_Model.Materials)
                {
                    jtem.ModelId = t_Model.Id;
                    await _productionDAL.AddBom(jtem);
                }
            }

            return BusResponse<string>.Success("新增成功");
        }

        /// <summary>
        /// 删除产品生命周期模型
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<int>> DeleteModel(string Id)
        {
            await _commonDAL.DeleteModel(Id);
            return BusResponse<int>.Success(0, "删除成功");
        }

        /// <summary>
        /// 修改产品生命周期模型
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateModel(In_Model in_Model)
        {
            if (!(in_Model.ProductBorder == "1" || in_Model.ProductBorder == "2" || in_Model.ProductBorder == "3"))
            {
                return BusResponse<int>.Error(500, "产品生命周期边界错误:1,2,3");
            }

            T_Com_Product product = await _commonDAL.SelectProduct(in_Model.ProductId);

            if (product == null)
            {
                return BusResponse<int>.Error(500, "找不到产品");
            }

            T_Com_Model t_Model = await _commonDAL.SelectModel(in_Model.Id);

            if (t_Model == null)
            {
                return BusResponse<int>.Error(500, "找不到生命周期模型");
            }

            var user = _provider.GetUser();

            t_Model.ProductId = in_Model.ProductId;
            t_Model.ProductBorder = in_Model.ProductBorder;
            t_Model.ModelName = in_Model.ModelName;

            t_Model.updateId = user.UserId;
            t_Model.update_time = DateTime.Now;

            //更新
            int result = await _commonDAL.UpdateModel(t_Model);

            //删除原来的
            await _commonDAL.DeleteProcess(t_Model.Id);
            await _commonDAL.DeleteProcessItem(t_Model.Id);
            await _productionDAL.DeleteBom(in_Model.Id);

            List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree((long)in_Model.OrgId);

            //新增
            foreach (var item in in_Model.Processes)
            {
                T_Com_Process t_Process = new T_Com_Process();
                t_Process.Id = _snowflake.NextId().ToString();

                t_Process.ModelId = in_Model.Id;
                t_Process.LinkId = item.LinkId;
                t_Process.ProcessName = item.ProcessName;
                t_Process.ProcessNo = item.ProcessNo;

                await _commonDAL.AddProcess(t_Process);

                foreach (var jtem in item.ProcessItems)
                {
                    List<string> facilityIds = new List<string>();
                    string[] arr = jtem.FacilityId.Split(',');//用户选择的设施
                    foreach (var ktem in arr)
                    {
                        if (!facilityIds.Contains(ktem))//设施加进去
                        {
                            if (!string.IsNullOrEmpty(ktem))
                            {
                                facilityIds.Add(ktem);
                            }
                        }

                        string[] brr = DeppIDS(ktem, list).Split(',');//每个设施的子设施

                        foreach (var ltem in brr)
                        {
                            if (!facilityIds.Contains(ltem))//子设施加进去
                            {
                                if (!string.IsNullOrEmpty(ltem))
                                {
                                    facilityIds.Add(ltem);
                                }
                            }
                        }
                    }

                    string str = String.Join(",", facilityIds);

                    T_Com_ProcessItem t_ProcessItem = new T_Com_ProcessItem();
                    t_ProcessItem.ModelId = in_Model.Id;
                    t_ProcessItem.ProcessId = t_Process.Id;
                    t_ProcessItem.FacilityId = jtem.FacilityId;
                    t_ProcessItem.FacilityIds = str;
                    t_ProcessItem.ProductId = jtem.ProductId;

                    await _commonDAL.AddProcessItem(t_ProcessItem);
                }
            }

            if (in_Model.Materials != null)
            {
                foreach (var jtem in in_Model.Materials)
                {
                    jtem.ModelId = t_Model.Id;
                    await _productionDAL.AddBom(jtem);
                }
            }

            return BusResponse<int>.Success(result, "修改成功");
        }

        /// <summary>
        /// 分页查询产品生命周期模型
        /// </summary>
        /// <returns></returns>
        public async Task<PageObject<Out_ModelList>> SelectModelList(In_ModelPageLis query)
        {
            return await _commonDAL.SelectModelPage(query);
        }

        /// <summary>
        /// 生命周期模型信息
        /// </summary>
        /// <returns></returns>
        public async Task<Out_Model> SelectModelInfo(string Id)
        {
            Out_Model model = new Out_Model();

            T_Com_Model t_model = await _commonDAL.SelectModel(Id);
            if (t_model == null)
            {
                return model;
            }
            T_Com_Product product = await _commonDAL.SelectProduct(t_model.ProductId);

            if (product == null)
            {
                return model;
            }
            model.Id = t_model.Id;
            model.OrgId = t_model.OrgId;
            model.ProductBorder = t_model.ProductBorder;

            model.ProductId = t_model.ProductId;
            model.ProductModel = product.ProductModel;
            model.ProductName = product.ProductName;
            model.ModelName = t_model.ModelName;

            //工序
            List<T_Com_Process> processes = await _commonDAL.SelectProcess(t_model.Id);

            model.Processes = new List<Out_Process>();

            //设施树
            List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree((long)t_model.OrgId);

            foreach (var item in processes)
            {
                Out_link link = await _commonDAL.SelectLinkInfo(item.LinkId);

                Out_Process process = new Out_Process();
                process.Id = item.Id;
                process.LinkId = item.LinkId;
                process.LinkName = link.LinkName;
                process.ProcessName = item.ProcessName;
                process.ProcessNo = item.ProcessNo;

                process.ProcessItems = new List<Out_ProcessItem>();

                List<string> facilityIds = new List<string>();
                //facilityIds = DeppIDS(query.FacilityId, list);
                //query.FacilityId = facilityIds + query.FacilityId;

                List<T_Com_ProcessItem> processItems = await _commonDAL.SelectProcessItem(item.Id);
                foreach (var jtem in processItems)
                {
                    Out_ProcessItem processItem = new Out_ProcessItem();
                    processItem.FacilityId = jtem.FacilityId;
                    processItem.FacilityIds = jtem.FacilityIds;

                   string[] arr =  processItem.FacilityId.Split(',');//用户选择的设施
                    foreach (var ktem in arr)
                    {
                        if (!facilityIds.Contains(ktem))//设施加进去
                        {
                            if (!string.IsNullOrEmpty(ktem))
                            {
                                facilityIds.Add(ktem);
                            }
                        }

                        string[] brr = DeppIDS(ktem, list).Split(',');//每个设施的子设施
                        
                        foreach (var ltem in brr)
                        {
                            if (!facilityIds.Contains(ltem))//子设施加进去
                            {
                                if (!string.IsNullOrEmpty(ltem))
                                {
                                    facilityIds.Add(ltem);
                                }
                            }
                        }
                    }

                    processItem.ProductId = jtem.ProductId;
                    T_Com_Product prod = await _commonDAL.SelectProduct(jtem.ProductId);

                    if (prod != null)
                    {
                        processItem.ProductName = prod.ProductName;
                        processItem.ProductModel = prod.ProductModel;
                    }
                    process.ProcessItems.Add(processItem);
                }
                process.ShuRus = await _commonDAL.SelectProcessEnergy(String.Join(", ", facilityIds));

                model.Processes.Add(process);
            }

            model.Materials = await _productionDAL.SelectBomList(t_model.Id);

            return model;
        }


        /// <summary>
        /// 查询生命周期边界
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_link>> SelectLink(string ProductBorder)
        {
            return await _commonDAL.SelectLink(ProductBorder);
        }

        /// <summary>
        /// 查询工序输入源
        /// </summary>
        /// <returns></returns>
        public async Task<List<ShuRu>> SelectShuRu(string FacilityId, long OrgId)
        {
            List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree(OrgId);

            List<string> facilityIds = new List<string>();
            string[] arr = FacilityId.Split(',');//用户选择的设施
            foreach (var ktem in arr)
            {
                if (!facilityIds.Contains(ktem))//设施加进去
                {
                    if (!string.IsNullOrEmpty(ktem))
                    {
                        facilityIds.Add(ktem);
                    }
                }

                string[] brr = DeppIDS(ktem, list).Split(',');//每个设施的子设施

                foreach (var ltem in brr)
                {
                    if (!facilityIds.Contains(ltem))//子设施加进去
                    {
                        if (!string.IsNullOrEmpty(ltem))
                        {
                            facilityIds.Add(ltem);
                        }
                    }
                }
            }

            string str = String.Join(", ", facilityIds);

            return await _commonDAL.SelectProcessEnergy(str);
        }
        #endregion

        #region 产品碳足迹
        /// <summary>
        /// 新增产品碳足迹
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddProductModel(In_ProductModel in_Model)
        {
            DateTime beginDate;
            DateTime endDate;

            if (!DateTime.TryParse(in_Model.BeginDate, out beginDate))
            {
                return BusResponse<string>.Error(500, "统计开始日期错误");
            }

            if (!DateTime.TryParse(in_Model.EndDate, out endDate))
            {
                return BusResponse<string>.Error(500, "统计结束日期错误");
            }

            T_Com_Model t_model = await _commonDAL.SelectModel(in_Model.ModelId);
            if (t_model == null)
            {
                return BusResponse<string>.Error(500, "找不到生命周期模型");
            }

            var user = _provider.GetUser();

            T_Com_ProductModel t_Model = new T_Com_ProductModel();
            t_Model.Id = _snowflake.NextId().ToString();

            t_Model.OrgId = in_Model.OrgId;
            t_Model.ModelId = in_Model.ModelId;
            t_Model.BeginDate = beginDate;
            t_Model.EndDate = endDate;

            t_Model.del_flag = "0";
            t_Model.createId = user.UserId;
            t_Model.create_time = DateTime.Now;
            t_Model.updateId = user.UserId;
            t_Model.update_time = DateTime.Now;
            //添加
            await _commonDAL.AddProductModel(t_Model);

            return BusResponse<string>.Success("新增成功");
        }

        /// <summary>
        /// 删除产品碳足迹
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<int>> DeleteProductModel(string Id)
        {
            await _commonDAL.DeleteProductModel(Id);
            return BusResponse<int>.Success(0, "删除成功");
        }

        /// <summary>
        /// 修改产品碳足迹
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateProductModel(In_ProductModel in_Model)
        {
            DateTime beginDate;
            DateTime endDate;

            if (!DateTime.TryParse(in_Model.BeginDate, out beginDate))
            {
                return BusResponse<int>.Error(500, "统计开始日期错误");
            }

            if (!DateTime.TryParse(in_Model.EndDate, out endDate))
            {
                return BusResponse<int>.Error(500, "统计结束日期错误");
            }

            T_Com_Model t_model = await _commonDAL.SelectModel(in_Model.ModelId);
            if (t_model == null)
            {
                return BusResponse<int>.Error(500, "找不到生命周期模型");
            }

            T_Com_ProductModel t_ProductModel = await _commonDAL.SelectProductModel(in_Model.Id);
            if (t_ProductModel == null)
            {
                return BusResponse<int>.Error(500, "找不到产品碳足迹");
            }

            var user = _provider.GetUser();

            t_ProductModel.ModelId = in_Model.ModelId;
            t_ProductModel.BeginDate = beginDate;
            t_ProductModel.EndDate = endDate;

            t_ProductModel.updateId = user.UserId;
            t_ProductModel.update_time = DateTime.Now;

            //更新
            int result = await _commonDAL.UpdateProductModel(t_ProductModel);

            return BusResponse<int>.Success(result, "修改成功");
        }

        /// <summary>
        /// 查询产品碳足迹详情
        /// </summary>
        /// <returns></returns>
        public async Task<Out_ProductModelInfo> SelectProductModelInfo(string Id)
        {
            Out_ProductModelInfo out_Product = new Out_ProductModelInfo();

            //产品足迹
            T_Com_ProductModel t_ProductModel = await _commonDAL.SelectProductModel(Id);
            out_Product.Id = t_ProductModel.Id;
            out_Product.OrgId = t_ProductModel.OrgId;
            out_Product.BeginDate = t_ProductModel.BeginDate;
            out_Product.EndDate = t_ProductModel.EndDate;

            //生命周期模型
            T_Com_Model model = await _commonDAL.SelectModel(t_ProductModel.ModelId);
            if (model == null)
            {
                return out_Product;
            }
            //产品信息
            T_Com_Product product = await _commonDAL.SelectProduct(model.ProductId);
            if (product != null)
            {
                out_Product.ProductModel = product.ProductModel;
                out_Product.ProductName = product.ProductName;
                out_Product.ProductType = product.ProductType;
                out_Product.Unit = product.Unit;
            }
            out_Product.OutValue = 0;
            out_Product.OutPut = 0;

            //能耗数据
            InEnergyTree inEnergyTree = new InEnergyTree();
            inEnergyTree.OrgId = t_ProductModel.OrgId;
            inEnergyTree.BeginDate = t_ProductModel.BeginDate.ToString("yyyy-MM-dd");
            inEnergyTree.EndDate = t_ProductModel.EndDate.ToString("yyyy-MM-dd");
            List<Out_EnergyList> listEnergies = await _productionDAL.SelectEnergyList(inEnergyTree);
            //边界
            out_Product.ProductBorder = model.ProductBorder;
            List<Out_link> links = (await _commonDAL.SelectLink(model.ProductBorder));
            foreach (var item in links)
            {
                out_Product.BorderName = item.BorderName;
                out_Product.BorderTitle = item.BorderTitle;
            }
            List<string> fList = new List<string>();
            //工序
            out_Product.Processes = new List<ModelProcess>();
            List<T_Com_Process> processes = await _commonDAL.SelectProcess(t_ProductModel.ModelId);
            foreach (var item in processes)
            {
                Out_link link = await _commonDAL.SelectLinkInfo(item.LinkId);
                ModelProcess modelProcess = new ModelProcess();
                modelProcess.Id = item.Id;
                modelProcess.ModelId = item.ModelId;
                modelProcess.LinkId = item.LinkId;
                modelProcess.ProcessName = item.ProcessName;
                modelProcess.ProcessNo = item.ProcessNo;
                modelProcess.LinkName = link.LinkName;
                modelProcess.shuRus = new List<ShuRu1>();

                modelProcess.ProcessItems = new List<ModelProcessItem>();
                List<T_Com_ProcessItem> processItems = await _commonDAL.SelectProcessItem(item.Id);
                foreach (var jtem in processItems)
                {
                    ModelProcessItem processItem = new ModelProcessItem();
                    processItem.ModelId = jtem.ModelId;
                    processItem.ProcessId = jtem.ProcessId;
                    processItem.ProductId = jtem.ProductId;
                    processItem.FacilityId = jtem.FacilityId;
                    processItem.FacilityIds = jtem.FacilityIds;

                    double CarbonEmission = 0;
                    double ConvertCoal = 0;
                    string[] arrF = processItem.FacilityIds.Split(',');
                    foreach (var ktem in arrF)
                    {
                        if (!fList.Contains(ktem))
                        {
                            fList.Add(ktem);
                        }
                    }
                    foreach (var ktem in listEnergies)
                    {
                        if (arrF.Contains(ktem.FacilityId))
                        {
                            CarbonEmission += ktem.CarbonEmission;
                            ConvertCoal += ktem.ConvertCoal;
                            bool flg = true;
                            foreach (var wtem in modelProcess.shuRus)
                            {
                                if (wtem.EnergyType == ktem.FactorId)
                                {
                                    wtem.UseVale += ktem.UseVale;
                                    wtem.UseVale = Math.Round(wtem.UseVale,2);
                                    flg = false;
                                }
                            }
                            if (flg)
                            {
                                ShuRu1 shuRu = new ShuRu1();
                                shuRu.Unit = ktem.Unit;
                                shuRu.LageUnit = ktem.LageUnit;
                                shuRu.EnergyType = ktem.FactorId;
                                shuRu.TypeName = ktem.FactorName;
                                shuRu.UseVale = Math.Round(ktem.UseVale, 2);
                                modelProcess.shuRus.Add(shuRu);
                            }
                        }
                    }
                    processItem.CarbonEmission = Math.Round(CarbonEmission, 2);
                    processItem.ConvertCoal = Math.Round(ConvertCoal, 2);

                    modelProcess.ProcessItems.Add(processItem);
                }

                out_Product.Processes.Add(modelProcess);
            }

            //产值数据
            InProductionPageList inProduction = new InProductionPageList();
            inProduction.OrgId = t_ProductModel.OrgId;
            inProduction.BeginDate = t_ProductModel.BeginDate.ToString("yyyy-MM-dd");
            inProduction.EndDate = t_ProductModel.EndDate.ToString("yyyy-MM-dd");
            inProduction.ProductId = model.ProductId;
            List<Out_Production> listProductions = (await _productionDAL.SelectProductionPage(inProduction)).List;
            foreach (var item in listProductions)
            {
                if (fList.Contains(item.FacilityId))
                {
                    out_Product.OutValue += item.OutValue;
                    out_Product.OutPut += item.OutPut;
                }
            }
            out_Product.OutValue = Math.Round(out_Product.OutValue, 2);
            out_Product.OutPut = Math.Round(out_Product.OutPut, 2);

            out_Product.Materials = await _productionDAL.SelectBomList(model.Id);
            foreach (var jtem in out_Product.Materials)
            {
                jtem.Dosage = jtem.Dosage * (int)out_Product.OutPut;
                jtem.CarbonEmission = Math.Round(jtem.CarbonEmission * jtem.Dosage, 2);
            }

            return out_Product;
        }

        /// <summary>
        /// 分页查询产品碳足迹详情
        /// </summary>
        /// <returns></returns>
        public async Task<PageObject<Out_ProductModelList>> SelectProductModelPage(In_ModelPageLis query)
        {
            return await _commonDAL.SelectProductModelPage(query);
        }
        #endregion

        #region 小时数据
        /// <summary>
        /// 查询能耗时段
        /// </summary>
        /// <returns></returns>
        public async Task<List<T_Prod_Energy_Hour>> SelectEnergyHour(InEnergyHour query)
        {
            DateTime beginTime = DateTime.Parse(query.beginDate);
            DateTime endTime = DateTime.Parse(query.endDate);

            //设施和子设施拼接的字符串
            string facilityIds = "";
            if (!string.IsNullOrEmpty(query.FacilityId) && query.orgId != null)
            {
                List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree((long)query.orgId);

                facilityIds = DeppIDS(query.FacilityId, list);

                query.FacilityId = facilityIds + query.FacilityId;
            }

            List<T_Prod_Energy_Hour> list_Hour = new List<T_Prod_Energy_Hour>();
            while (beginTime <= endTime)
            {
                T_Prod_Energy_Hour hour = await _productionDAL.SelectEnergyHour(query.FacilityId, beginTime.ToString("yyyy-MM-dd"));
                if (hour == null)
                {
                    hour = new T_Prod_Energy_Hour();
                    hour.DDate = beginTime;
                    hour.TTime = "-";
                    hour.UseVale = 0;
                }
                else
                {
                    hour.DDate = beginTime;
                }
                list_Hour.Add(hour);
                beginTime = beginTime.AddDays(1);
            }

            return list_Hour;
        }

        /// <summary>
        /// 查询能耗时段
        /// </summary>
        /// <returns></returns>
        public async Task<List<T_Prod_Energy_Hour>> SelectEnergyHourList(InEnergyHourList query)
        {
            List<T_Prod_Energy_Hour> list = new List<T_Prod_Energy_Hour>();
            if (string.IsNullOrEmpty(query.EquipmentId))
            {
                return list;
            }
            return list = (await _productionDAL.SelectEnergyHourList(query.EquipmentId, query.beginDate, query.endDate)).ToList();
        }

        /// <summary>
        /// 查询能耗时段
        /// </summary>
        /// <returns></returns>
        public async Task<List<T_Prod_Energy_T>> SelectEnergyTimeList(InEnergyHourList query)
        {
            List<T_Prod_Energy_T> list = new List<T_Prod_Energy_T>();
            if (string.IsNullOrEmpty(query.EquipmentId))
            {
                return list;
            }
            return list = (await _productionDAL.SelectEnergyTimeList(query.EquipmentId, query.beginDate, query.endDate, query.FactorId)).ToList();
        }
        /// <summary>
        /// 查询能耗时段
        /// </summary>
        /// <returns></returns>
        public async Task<List<T_Prod_Energy_T2>> SelectEnergyTimeList2(InEnergyHourList query)
        {
            //设施和子设施拼接的字符串
            string facilityIds = "";
            if (!string.IsNullOrEmpty(query.FacilityId) && query.orgId != null)
            {
                List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree((long)query.orgId);

                facilityIds = DeppIDS(query.FacilityId, list);

                query.FacilityId = facilityIds + query.FacilityId;
            }
            return await _productionDAL.SelectEnergyTimeList2(query.FacilityId, query.beginDate, query.endDate, query.FactorId);
        }


        /// <summary>
        /// 查询能耗时段
        /// </summary>
        /// <returns></returns>
        public async Task<T_Prod_Energy_H> SelectEnergyTimeList3(InEnergyHour query)
        {
            //设施和子设施拼接的字符串
            string facilityIds = "";
            if (!string.IsNullOrEmpty(query.FacilityId) && query.orgId != null)
            {
                List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree((long)query.orgId);

                facilityIds = DeppIDS(query.FacilityId, list);

                query.FacilityId = facilityIds + query.FacilityId;
            }
            return await _productionDAL.SelectEnergyTimeList3(query.FacilityId, query.beginDate, query.endDate);
        }
        #endregion
    }
}
