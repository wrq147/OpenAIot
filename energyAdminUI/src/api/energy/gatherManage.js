import request from '@/utils/request';
import download from '@/plugins/download'
// 能源数据采集列表
export function selectEnergyPageList(data) {
    return request({
        url: '/EfficiencyService/Production/EnergyPageList',
        method: 'post',
        data: data
    })
}
// 能源数据采集新增
export function addEnergy(data) {
    return request({
        url: '/EfficiencyService/Production/AddEnergyHand',
        method: 'post',
        data: data
    })
}

// 能源数据采集修改
export function editEnergy(data) {
    return request({
        url: '/EfficiencyService/Production/UpdateEnergy',
        method: 'post',
        data: data
    })
}

// 能源数据采集删除
export function removeEnergy(data) {
    return request({
        url: '/EfficiencyService/Production/RemoveEnergy',
        method: 'post',
        data: data
    })
}

// 生产数据采集列表
export function selectProductionPageList(data) {
    return request({
        url: '/EfficiencyService/Production/ProductionPageList',
        method: 'post',
        data: data
    })
}

// 生产数据采集新增
export function addProduction(data) {
    return request({
        url: '/EfficiencyService/Production/AddProduction',
        method: 'post',
        data: data
    })
}
// 生产数据采集修改
export function editProduction(data) {
    return request({
        url: '/EfficiencyService/Production/UpdateProduction',
        method: 'post',
        data: data
    })
}
// 生产数据采集删除
export function removeProduction(data) {
    return request({
        url: '/EfficiencyService/Production/RemoveProduction',
        method: 'post',
        data: data
    })
}

// 查询企业的能源类型
export function selectFactorOrg(data) {
    return request({
        url: '/EfficiencyService/Factor/SelectFactorTypeOrg',
        method: 'post',
        data: data
    })
}

// 获取历史数据
export function reCalculationEnergyDay(data) {
    return request({
        url: '/EfficiencyService/Production/ReCalculationEnergyDay',
        method: 'post',
        data: data
    })
}

// 下载能源数据导入模板
export function importEnergyTemplate() {
    return download.resource('EfficiencyService/Production/ExportEnergy', {});
}

// 下载生产数据导入模板
export function importProductionTemplate() {
    return download.resource('/EfficiencyService/Production/ExportProduction', {});
}






