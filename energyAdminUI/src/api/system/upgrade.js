import request from '@/utils/request'

// 获取App升级列表
export function listUpgrade(query) {
    return request({
        url: '/AuthService/UpgradeMan/List',
        method: 'get',
        params: query
    })
}

// 获取指定升级详情
export function getUpgrade(id) {
    return request({
        url: '/AuthService/UpgradeMan/Info',
        method: 'get',
        params: {id}
    })
}

// 新增App升级
export function addUpgrade(data) {
    return request({
        url: '/AuthService/UpgradeMan/Add',
        method: 'post',
        data: data
    })
}

// 删除App升级
export function delUpgrade(ids) {

    return request({
        url: '/AuthService/UpgradeMan/Remove',
        method: 'get',
        params: {ids:ids instanceof Array?ids.join(','):ids}
    })
}
