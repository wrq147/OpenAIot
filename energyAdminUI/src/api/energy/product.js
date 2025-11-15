import request from '@/utils/request';
import download from '@/plugins/download';
// 产品列表
export function productPageList(data) {
    return request({
        url: '/EfficiencyService/Common/ProductPageList',
        method: 'post',
        data: data
    })
}

// 产品列表
export function facilityProductList(data) {
    return request({
        url: '/EfficiencyService/Common/FacilityProductList',
        method: 'post',
        data: data
    })
}

// 添加产品
export function addProduct(data) {
    return request({
        url: '/EfficiencyService/Common/AddProduct',
        method: 'post',
        data: data
    })
}
// 修改产品
export function updateProduct(data) {
    return request({
        url: '/EfficiencyService/Common/UpdateProduct',
        method: 'post',
        data: data
    })
}
// 删除产品
export function removeProduct(data) {
    return request({
        url: '/EfficiencyService/Common/RemoveProduct',
        method: 'post',
        data: data
    })
}
// 产品详情
export function productInfo(data) {
    return request({
        url: '/EfficiencyService/Common/ProductInfo',
        method: 'post',
        data: data
    })
}

// 下载产品数据导入模板
export function importProductTemplate() {
    return download.resource('/EfficiencyService/Common/ExportProduct', {});
}

// 导出产品数据
export function exportProductList(query) {
    return download.resource('/EfficiencyService/Common/ExportProductList', query);
}