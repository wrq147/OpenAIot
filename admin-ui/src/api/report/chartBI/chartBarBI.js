import request from '@/utils/request'

export function barBIanalysis(query) {
    return request({
        url: '/chart/BI/bar/analysis',
        method: 'post',
        data: query
    })
}