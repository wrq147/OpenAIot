import request from '@/utils/request'

// 物料清单列表
export function bomList(params) {
    return request({
        url: '/MESService/Bom/List',
        method: 'get',
        params: params
    })
}
// 物料清单详情
export function bomInfo(params) {
    return request({
        url: '/MESService/Bom/Info',
        method: 'get',
        params: params
    })
}
// 添加物料清单
export function bomAdd(data) {
    return request({
        url: '/MESService/Bom/Add',
        method: 'post',
        data: data
    })
}

// 修改物料清单
export function bomEdit(data) {
    return request({
        url: '/MESService/Bom/Edit',
        method: 'post',
        data: data
    })
}

// 删除物料清单
export function bomRemove(params) {
    return request({
        url: '/MESService/Bom/Remove',
        method: 'get',
        params: params
    })
}