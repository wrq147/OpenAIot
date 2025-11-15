import request from '@/utils/request'
import download from '@/plugins/download'
// 物料列表
export function materialPageList(data) {
    return request({
        url: '/EfficiencyService/Production/MaterialPageList',
        method: 'post',
        data: data
    })
}

// 添加物料
export function addMaterial(data) {
    return request({
        url: '/EfficiencyService/Production/AddMaterial',
        method: 'post',
        data: data
    })
}
// 修改物料
export function updateMaterial(data) {
    return request({
        url: '/EfficiencyService/Production/UpdateMaterial',
        method: 'post',
        data: data
    })
}
// 下载导入模板
export function exportMaterialModel(query) {
    return download.resource('/EfficiencyService/Production/ExportMaterial', query);
}
// 导出数据
export function exportMaterialList(query) {
    return download.resource('/EfficiencyService/Production/ExportMaterialList', query);
}
// 导入数据
export function importMaterial(data) {
    return request({
        url: '/EfficiencyService/Production/ImportMaterial',
        method: 'post',
        data: data
    })
}
// 删除物料
export function removeMaterial(data) {
    return request({
        url: '/EfficiencyService/Production/RemoveMaterial',
        method: 'post',
        data: data
    })
}
// 物料详情
export function materialInfo(id) {
    return request({
        url: '/EfficiencyService/Production/Material?id=' + id,
        method: 'post',
    })
}
// 物料编号
export function materialCode(data) {
    return request({
        url: '/EfficiencyService/Production/MaterialCode',
        method: 'post',
        data: data
    })
}
// 物料碳足迹列表
export function materialCarbonPageList(data) {
    return request({
        url: '/EfficiencyService/Production/MaterialCarbonPageList',
        method: 'post',
        data: data
    })
}

// 添加物料碳足迹
export function addMaterialCarbon(data) {
    return request({
        url: '/EfficiencyService/Production/AddMaterialCarbon',
        method: 'post',
        data: data
    })
}
// 修改物料碳足迹
export function updateMaterialCarbon(data) {
    return request({
        url: '/EfficiencyService/Production/UpdateMaterialCarbon',
        method: 'post',
        data: data
    })
}
// 删除物料碳足迹
export function removeMaterialCarbon(data) {
    return request({
        url: '/EfficiencyService/Production/RemoveMaterialCarbon',
        method: 'post',
        data: data
    })
}
// 物料碳足迹详情
export function materialCarbonInfo(data) {
    return request({
        url: '/EfficiencyService/Production/MaterialCarbon',
        method: 'post',
        data: data
    })
}
// 下载导入碳足迹模板
export function exportMaterialCarbonModel(query) {
    return download.resource('/EfficiencyService/Production/ExportMaterialCarbon', query);
}
// 导出碳足迹数据
export function exportMaterialCarbonList(query) {
    return download.resource('/EfficiencyService/Production/ExportMaterialCarbonList', query);
}
// 导入碳足迹数据
export function importMaterialCarbon(data) {
    return request({
        url: '/EfficiencyService/Production/ImportMaterialCarbon',
        method: 'post',
        data: data
    })
}