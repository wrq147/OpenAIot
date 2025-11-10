import request from '@/utils/request'

export function rainBIanalysis(query) {
    return request({
        url: '/chart/BI/rain/analysis',
        method: 'post',
        data: query
    })
}