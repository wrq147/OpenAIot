import request from '@/utils/request'

export function popsBIanalysis(query) {
    return request({
        url: '/chart/BI/pops/analysis',
        method: 'post',
        data: query
    })
}