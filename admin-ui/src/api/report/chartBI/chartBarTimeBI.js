import request from '@/utils/request'

export function barTimeBIanalysis(query) {
    return request({
        url: '/chart/BI/bartime/analysis',
        method: 'post',
        data: query
    })
}