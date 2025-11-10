import request from '@/utils/request'

export function parallelBarBIanalysis(query) {
    return request({
        url: '/chart/BI/parallelBar/analysis',
        method: 'post',
        data: query
    })
}