import request from '@/utils/request'
// 生命周期模型列表
export function modelPageList(data) {
    return request({
        url: '/EfficiencyService/Common/ModelPageList',
        method: 'post',
        data: data
    })
}

// 删除生命周期模型列表
export function removeModel(data) {
    return request({
        url: '/EfficiencyService/Common/RemoveModel',
        method: 'post',
        data: data
    })
}

// 新增生命周期模型列表
export function addModel(data) {
    return request({
        url: '/EfficiencyService/Common/AddModel',
        method: 'post',
        data: data
    })
}

// 编辑生命周期模型列表
export function editModel(data) {
    return request({
        url: '/EfficiencyService/Common/UpdateModel',
        method: 'post',
        data: data
    })
}

// 生命周期模型详情
export function modelInfo(data) {
    return request({
        url: '/EfficiencyService/Common/ModelInfo',
        method: 'post',
        data: data
    })
}

// 生命周期模型边界
export function modelSelectLink(data) {
    return request({
        url: '/EfficiencyService/Common/SelectLink',
        method: 'post',
        data: data
    })
}

// 生命周期模型边界
export function modelSelectShuRu(data) {
    return request({
        url: '/EfficiencyService/Common/SelectShuRu',
        method: 'post',
        data: data
    })
}

// 碳足迹列表
export function modelProductModelPage(data) {
    return request({
        url: '/EfficiencyService/Common/ModelProductModelPage',
        method: 'post',
        data: data
    })
}

// 新增碳足迹列表
export function addProductModel(data) {
    return request({
        url: '/EfficiencyService/Common/AddProductModel',
        method: 'post',
        data: data
    })
}

// 详情碳足迹列表
export function selectProductModelInfo(data) {
    return request({
        url: 'EfficiencyService/Common/SelectProductModelInfo',
        method: 'post',
        data: data
    })
}

// 删除碳足迹列表
export function removeProductModel(data) {
    return request({
        url: '/EfficiencyService/Common/RemoveProductModel',
        method: 'post',
        data: data
    })
}


