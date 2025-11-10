import request from '@/common/request.js'
// 生成产品编码
export function factoryProductNumber() {
    return request.get({
        url: '/ProducerService/Product/GenerateNumber',
    })
}
// 代理商可见产品列表
export function agentProductListGet(query) {
    return request.get({
        url: '/ProducerService/Product/AgentList',
        query: query
    })
}
// 产品列表post
export function factoryProductListPost(data) {
    return request.post({
        url: '/ProducerService/Product/List',
        data: data
    })
}

// 删除产品
export function factoryProductRemove(query) {
    return request.get({
        url: '/ProducerService/Product/Remove',
        query: query
    })
}
// 产品详情
export function factoryProductInfo(query) {
    return request.get({
        url: '/ProducerService/Product/Info',
        query: query
    })
}
// 产品添加
export function addProductSave(data) {
    return request.post({
        url: '/ProducerService/Product/Add',
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
// 产品分组列表
export function factoryProductTypeListGet() {
    return request.get({
        url: '/ProducerService/ProductType/List',
    })
}
// 产品分组详情
export function factoryProductTypeInfo(query) {
    return request.get({
        url: '/ProducerService/ProductType/Info',
        query: query
    })
}
// 产品分组添加
export function addProductTypeSave(data) {
    return request.post({
        url: '/ProducerService/ProductType/Add',
        data: data
    })
}
// 产品分组编辑
export function editProductTypeSave(data) {
    return request.post({
        url: '/ProducerService/ProductType/Edit',
        data: data
    })
}
// 删除产品分类
export function factoryProductTypeRemove(query) {
    return request.get({
        url: '/ProducerService/ProductType/Remove',
        query: query
    })
}
//关联对象查询列表
export function factorySearchObject(query) {
    return request.get({
        url: 'AuthService/Org/SearchObject',
        query: query
    })
}
// 获取关联对象的对应字段
export function orgFormFields(query) {
    return request.get({
        url: '/AuthService/Org/FormFields',
        query: query
    })
}
// 获取企业自定义字段
export function orgField(query) {
    return request.get({
        url: '/AuthService/Org/Field',
        query: query
    })
}
// 供应商列表get请求
export function factorySupplierListGet(query) {
    return request.get({
        url: '/ProducerService/Supplier/List',
        query: query
    })
}