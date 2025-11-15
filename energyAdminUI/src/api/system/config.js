import request from '@/utils/request'
import download from '@/plugins/download'
// 查询参数列表
export function listConfig(query) {
    return request({
        url: '/AuthService/Config/List',
        method: 'get',
        params: query
    })
}

// 查询参数详细
export function getConfig(configId) {
    return request({
        url: '/AuthService/Config/Info/' + configId,
        method: 'get'
    })
}

// 根据参数键名查询参数值
export function getConfigKey(configKey) {
    return request({
        url: '/AuthService/Code/GetByKey/' + configKey,
        method: 'get'
    })
}

export function getConfigKeyList(list) {
    return request({
        url: '/AuthService/Code/GetConfigList',
        method: 'get',
        params: { data: list }
    })
}

// 新增参数配置
export function addConfig(data) {
    return request({
        url: '/AuthService/Config/Add',
        method: 'post',
        data: data
    })
}

// 修改参数配置
export function updateConfig(data) {
    return request({
        url: '/AuthService/Config/Edit',
        method: 'post',
        data: data
    })
}

// 删除参数配置
export function delConfig(configId) {
    return request({
        url: '/AuthService/Config/Remove',
        method: 'get',
        params: { id: configId instanceof Array ? configId.join(',') : configId }
    })
}

// 刷新参数缓存
export function refreshCache() {
    return request({
        url: '/AuthService/Config/RefreshCache',
        method: 'get'
    })
}

// 导出参数
export function exportConfig(query) {
    return download.resource('/AuthService/Config/Export', query);
}