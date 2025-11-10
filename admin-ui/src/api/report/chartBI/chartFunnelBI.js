import request from '@/utils/request'

export function funnelBIanalysis(query) {
    return request({
        url: '/chart/BI/funnel/analysis',
        method: 'post',
        data: query
    })
}