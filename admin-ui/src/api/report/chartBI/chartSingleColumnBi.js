import request from '@/utils/request'

export function singleColumnBIanalysis(query) {
    return request({
        url: '/chart/BI/singleColumn/analysis',
        method: 'post',
        data: query
    })
}