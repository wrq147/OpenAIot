import request from '@/utils/request'
// 生成产品编码
export function factoryProductNumber() {
    return request({
        url: '/ProducerService/Product/GenerateNumber',
        method: 'get',
    })
}
// 产品列表
export function factoryProductListGet(params) {
    return request({
        url: '/ProducerService/Product/List',
        method: 'get',
        params: params
    })
}
// 代理商可见产品列表
export function agentProductListGet(params) {
    return request({
        url: '/ProducerService/Product/AgentList',
        method: 'get',
        params: params
    })
}
// 产品列表post
export function factoryProductListPost(data) {
    return request({
        url: '/ProducerService/Product/List',
        method: 'post',
        data: data
    })
}

// 删除产品
export function factoryProductRemove(params) {
    return request({
        url: '/ProducerService/Product/Remove',
        method: 'get',
        params: params
    })
}
// 产品详情
export function factoryProductInfo(params) {
    return request({
        url: '/ProducerService/Product/Info',
        method: 'get',
        params: params
    })
}
// 产品添加
export function addProductSave(data) {
    return request({
        url: '/ProducerService/Product/Add',
        method: 'post',
        data: data
    })
}
// 产品编辑
export function editProductSave(data) {
    return request({
        url: '/ProducerService/Product/Edit',
        method: 'post',
        data: data
    })
}
// 产品分类列表
export function factoryProductTypeListGet() {
    return request({
        url: '/ProducerService/ProductType/List',
        method: 'get',
    })
}
// 产品分类详情
export function factoryProductTypeInfo(params) {
    return request({
        url: '/ProducerService/ProductType/Info',
        method: 'get',
        params: params
    })
}
// 删除产品分类
export function factoryProductTypeRemove(params) {
    return request({
        url: '/ProducerService/ProductType/Remove',
        method: 'get',
        params: params
    })
}
// 产品分类添加
export function addProductTypeSave(data) {
    return request({
        url: '/ProducerService/ProductType/Add',
        method: 'post',
        data: data
    })
}
// 产品分类编辑
export function editProductTypeSave(data) {
    return request({
        url: '/ProducerService/ProductType/Edit',
        method: 'post',
        data: data
    })
}
//关联对象查询列表
export function factorySearchObject(params) {
    return request({
        url: 'AuthService/Org/SearchObject',
        method: 'get',
        params: params
    })
}