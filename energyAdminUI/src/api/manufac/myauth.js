import request from '@/utils/request'
// 我的授权列表（代理商）
export function AuthorizationList(query) {
    return request({
        url: '/ProducerService/Agent/AuthList',
        method: 'get',
        params: query
    })
}