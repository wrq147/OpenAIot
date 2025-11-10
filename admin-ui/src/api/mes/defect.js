import request from '@/utils/request'

// 不良品项列表
export function defectList(params) {
    return request({
        url: '/MESService/Defect/List',
        method: 'get',
        params: params
    })
}

// 添加不良品项
export function defectAdd(data) {
    return request({
        url: '/MESService/Defect/Add',
        method: 'post',
        data: data
    })
}

// 修改不良品项
export function defectEdit(data) {
    return request({
        url: '/MESService/Defect/Edit',
        method: 'post',
        data: data
    })
}

// 删除不良品项
export function defectRemove(params) {
    return request({
        url: '/MESService/Defect/Remove',
        method: 'get',
        params: params
    })
}

