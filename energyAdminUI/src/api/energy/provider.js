import request from '@/utils/request'
import download from '@/plugins/download'
// 供应商列表
export function providerPageList(data) {
    return request({
        url: '/EfficiencyService/Production/ProviderPageList',
        method: 'post',
        data: data
    })
}

// 添加供应商
export function addProvider(data) {
    return request({
        url: '/EfficiencyService/Production/AddProvider',
        method: 'post',
        data: data
    })
}
// 修改供应商
export function updateProvider(data) {
    return request({
        url: '/EfficiencyService/Production/UpdateProvider',
        method: 'post',
        data: data
    })
}
// 下载导入模板
export function exportProviderModel(query) {
    return download.resource('/EfficiencyService/Production/ExportProvider', query);
}
// 导出数据
export function exportProviderList(query) {
    return download.resource('/EfficiencyService/Production/ExportProviderList', query);
}
// 导入数据
export function importProvider(data) {
    return request({
        url: '/EfficiencyService/Production/ImportProvider',
        method: 'post',
        data: data
    })
}
// 删除供应商
export function removeProvider(data) {
    return request({
        url: '/EfficiencyService/Production/RemoveProvider',
        method: 'post',
        data: data
    })
}
// 供应商详情
export function providerInfo(data) {
    return request({
        url: '/EfficiencyService/Production/Provider',
        method: 'post',
        data: data
    })
}
// 供应商编号
export function providerCode(data) {
    return request({
        url: '/EfficiencyService/Production/ProviderCode',
        method: 'post',
        data: data
    })
}