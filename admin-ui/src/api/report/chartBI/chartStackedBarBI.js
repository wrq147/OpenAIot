import request from '@/utils/request'

export function stackedBarBIanalysis(query) {
    return request({
        url: '/chart/BI/stackedBar/analysis',
        method: 'post',
        data: query
    })
}