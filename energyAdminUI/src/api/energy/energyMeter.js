import request from '@/utils/request';
import download from '@/plugins/download';
// 设备能耗数据采集列表
export function energyPageDateList(data) {
    return request({
        url: '/EfficiencyService/Production/EnergyPageDateList',
        method: 'post',
        data: data
    })
}

// 单元能耗数据采集列表
export function facilityEnergyPageDateList(data) {
    return request({
        url: '/EfficiencyService/Production/FacilityEnergyPageDateList',
        method: 'post',
        data: data
    })
}

// 设施设备树
export function selectFacilityEquipmentTree(data) {
    return request({
        url: '/EfficiencyService/Common/SelectFacilityEquipmentTree',
        method: 'post',
        data: data
    })
}

// 能流树
export function EnergyTree(data) {
    return request({
        url: '/EfficiencyService/Production/EnergyTree',
        method: 'post',
        data: data
    })
}
// 能效对标
export function benchEnergyDate(data) {
    return request({
        url: '/EfficiencyService/Production/BenchEnergyDate',
        method: 'post',
        data: data
    })
}
// 能效生产数据
export function productionPageDateList(data) {
    return request({
        url: '/EfficiencyService/Production/ProductionPageDateList',
        method: 'post',
        data: data
    })
}

// 导出单元数据
export function exportFacilityList(query) {
    return download.resource('/EfficiencyService/Production/ExportFacilityEnergy', query);
}

// 导出设备数据
export function exportEquipmentList(query) {
    return download.resource('/EfficiencyService/Production/ExportEnergy', query);
}
// 小时最大需量
export function selectEnergyHour(data) {
    return request({
        url: '/EfficiencyService/Common/SelectEnergyHour',
        method: 'post',
        data: data
    })
}
// 单元峰谷数据
export function selectFacilityEnergyHourList(data) {
    return request({
        url: '/EfficiencyService/Common/SelectFacilityEnergyHourList',
        method: 'post',
        data: data
    })
}
// 设备峰谷数据
export function selectEnergyHourList(data) {
    return request({
        url: '/EfficiencyService/Common/SelectEnergyHourList',
        method: 'post',
        data: data
    })
}
// 需量分析能源策略
export function selectEnergyStrategy(data) {
    return request({
        url: '/EfficiencyService/Common/SelectEnergyStrategy',
        method: 'post',
        data: data
    })
}