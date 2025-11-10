import request from '@/utils/request'

export function doubleDirectionBIAnalysis(query) {
    return request({
        url: '/chart/BI/doubleDirection/analysis',
        method: 'post',
        data: query
    })
}