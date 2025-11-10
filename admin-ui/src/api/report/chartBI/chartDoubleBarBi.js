import request from '@/utils/request'

export function doubleBarBIanalysis(query) {
    return request({
        url: '/chart/BI/doubleBar/analysis',
        method: 'post',
        data: query
    })
}