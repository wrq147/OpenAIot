import request from '@/utils/request'


// 获取产品批次列表
export function BatchList(data) {
    return request({
        url: '/ProducerService/ProductBatch/List',
        method: 'get',
        params:data
    })
}
// 添加产品批次
export function addBatch(data) {
    return request({
        url: '/ProducerService/ProductBatch/Add',
        method: 'post',
        data: data
    })
}

// 编辑产品批次
export function editBatch(data) {
    return request({
        url: '/ProducerService/ProductBatch/Edit',
        method: 'post',
        data: data
    })
}

// 设备列表中批量导入
export function AddListBatch(data) {
    return request({
        url: '/ProducerService/ProductBatch/AddList',
        method: 'post',
        data: data
    })
}

// 设备自动同步到批次
export function SyncDeviceToBatch(data) {
    return request({
        url: '/ProducerService/ProductBatch/SyncFromDevice',
        method: 'get'
    })
}

// 获取导入的设备列表
export function devicePageBatch(params) {
    return request({
        url: '/ProducerService/ProductBatch/DevicePage',
        method: 'get',
        params: params
    });
}

// 删除产品批次
export function delBatch(params) {
    return request({
        url: '/ProducerService/ProductBatch/Remove',
        method: 'get',
        params: params
    });
}

// 批量删除产品批次
export function delListBatch(query) {
    return request({
        url: '/ProducerService/ProductBatch/RemoveList',
        method: 'get',
        params: query
    })
}
