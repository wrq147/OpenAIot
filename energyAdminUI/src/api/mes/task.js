import request from '@/utils/request'

// 生产计划列表
export function taskList(params) {
    return request({
        url: '/MESService/Task/List',
        method: 'get',
        params: params
    })
}