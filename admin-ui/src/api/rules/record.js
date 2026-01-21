import request from '@/utils/request'

export function recordList(query) {
    return request({
        url: '/IoTVideoService/Record/ListPage',
        method: 'get',
        params: query
    })
}