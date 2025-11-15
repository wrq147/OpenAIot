import moment from 'moment'
export function mergeData(useData, factorData) {
    // 创建映射表，方便通过Id快速查找factor数据
    const factorMap = {};
    factorData.forEach(factor => {
        factorMap[factor.Id] = factor;
    });

    // 处理useData为空数组的情况
    if (useData.length === 0) {
        // 基于factorData生成默认数据，数值字段为'-'
        const emptyResult = factorData.map(factor => ({
            FactorId: factor.Id,
            FactorName: factor.TypeName,
            TypeName: factor.TypeName,
            UseVale: '-',
            CostVale: '-',
            ConvertCoal: '-',
            CarbonEmission: '-',
            Unit: '',
            LageUnit: '',
            ProductUnit: '',
            OutPut: '-',
            UnitProductEfficien: '-',
            UnitProductCostEfficien: '-',
        }));
        
        // 添加合计行（全部为'-'）
        emptyResult.push({
            FactorId: 'total',
            FactorName: '合计',
            TypeName: '合计',
            UseVale: '-',
            CostVale: '-',
            ConvertCoal: '-',
            CarbonEmission: '-',
            Unit: '',
            LageUnit: '',
            ProductUnit: '',
            OutPut: '-',
            UnitProductEfficien: '-',
            UnitProductCostEfficien: '-',
        });
        
        return emptyResult;
    }

    // 创建结果对象，用于累加数据（useData非空时）
    const resultMap = {};
    // 初始化合计值
    const total = {
        UseVale: 0,
        CostVale: 0,
        ConvertCoal: 0,
        CarbonEmission: 0,
        OutPut: 0,
        UnitProductEfficien: 0,
        UnitProductCostEfficien: 0,
    };

    // 在 mergeData 函数内部添加工具函数
    const safeFormat = (value, fixed = 2) => {
        // 若值为 0 或有效数字，格式化保留小数；否则返回 '-'
        if (value === 0 || (!isNaN(value) && isFinite(value))) {
            return Number(Number(value).toFixed(fixed));
        }
        return '-';
    };

    // 遍历使用数据，进行累加
    useData.forEach(item => {
        const key = item.FactorId;
        // 如果结果中已有该FactorId的数据，则累加
        if (resultMap[key]) {
            resultMap[key].UseVale += item.UseVale;
            resultMap[key].CostVale += item.CostVale;
            resultMap[key].ConvertCoal += item.ConvertCoal;
            resultMap[key].CarbonEmission += item.CarbonEmission;
            resultMap[key].OutPut = item.OutPut;
            resultMap[key].UnitProductEfficien += item.UnitProductEfficien;
            resultMap[key].UnitProductCostEfficien += item.UnitProductCostEfficien;
        } else {
            // 如果是新的FactorId，创建新条目
            resultMap[key] = {
                ...item,
                // 关联factor数据的TypeName
                TypeName: factorMap[key]?.TypeName || ''
            };
        }
        
        // 累加合计值
        // total.UseVale += item.UseVale;
        total.CostVale += item.CostVale;
        total.ConvertCoal += item.ConvertCoal;
        total.CarbonEmission += item.CarbonEmission;
        total.OutPut = item.OutPut;
        total.UnitProductEfficien += item.UnitProductEfficien;
        total.UnitProductCostEfficien += item.UnitProductCostEfficien;
    });

    // 将结果转换为数组，并对数值字段保留两位小数
    const result = Object.values(resultMap).map(item => ({
        ...item,
        UseVale: Number(item.UseVale.toFixed(2)),
        CostVale: Number(item.CostVale.toFixed(2)),
        ConvertCoal: Number(item.ConvertCoal.toFixed(2)),
        CarbonEmission: Number(item.CarbonEmission.toFixed(2)),
        OutPut: item.OutPut,
        UnitProductEfficien: item.UnitProductEfficien ? safeFormat(item.UnitProductEfficien) : '-',
        UnitProductCostEfficien: item.UnitProductCostEfficien ? safeFormat(item.UnitProductCostEfficien) : '-',
    }));

    // 添加合计行（保留两位小数）
    result.push({
        FactorId: 'total',
        FactorName: '合计',
        TypeName: '合计',
        // UseVale: Number(total.UseVale.toFixed(2)),
        UseVale: '-',
        CostVale: Number(total.CostVale.toFixed(2)),
        ConvertCoal: Number(total.ConvertCoal.toFixed(2)),
        CarbonEmission: Number(total.CarbonEmission.toFixed(2)),
        Unit: result[0]?.Unit || '', // 复用第一个数据的单位
        LageUnit: result[0]?.LageUnit || '',
        ProductUnit: result[0]?.ProductUnit || '',
        OutPut: total.OutPut,
        UnitProductEfficien: total.UnitProductEfficien ? safeFormat(total.UnitProductEfficien) : '-',
        UnitProductCostEfficien: total.UnitProductCostEfficien ? safeFormat(total.UnitProductCostEfficien) : '-',
    });

    return result;
};

export function processEnergyData(energyData, productData) {
    // 检查输入数据是否有效（仅验证energyData）
    if (!Array.isArray(energyData)) {
        console.error('能源数据格式不正确，必须为数组');
        return [];
    }

    // 循环处理能源数据数组
    return energyData.map(item => {
        // 复制原始数据，避免修改原对象
        const newItem = { ...item };
        
        // 判断productData是否为空（null/undefined/空对象）
        const isProductDataEmpty = !productData || Object.keys(productData).length === 0;
        
        if (isProductDataEmpty) {
            // productData为空时，相关字段显示为'-'
            newItem.OutPut = '-';
            newItem.OutValue = '-';
            newItem.ProductUnit = '-';
            newItem.UnitProductEfficien = '-';
            newItem.UnitProductCostEfficien = '-';
        } else {
            // productData有效时，正常添加字段并计算
            newItem.OutPut = productData.OutPut ?? '-'; // 兼容0的情况
            newItem.OutValue = productData.OutValue ?? '-';
            newItem.ProductUnit = productData.Unit || '-';
            
            // 计算单位产品能耗效率 (UseVale/OutPut)
            if (productData.OutPut > 0 && item.UseVale !== '-' && !isNaN(item.UseVale)) {
                newItem.UnitProductEfficien = parseFloat((item.UseVale / productData.OutPut).toFixed(2));
            } else {
                newItem.UnitProductEfficien = '-';
            }
            
            // 计算单位产品成本效率 (CostVale/OutPut)
            if (productData.OutPut > 0 && item.CostVale !== '-' && !isNaN(item.CostVale)) {
                newItem.UnitProductCostEfficien = parseFloat((item.CostVale / productData.OutPut).toFixed(2));
            } else {
                newItem.UnitProductCostEfficien = '-';
            }
        }
        
        return newItem;
    });
}

