import request from '@/utils/request'

export function pictorialBarBIanalysis(query) {
    return request({
        url: '/chart/BI/pictorialBar/analysis',
        method: 'post',
        data: query
    })
}