import request from '@/utils/request'

export function topbarBIAnalysis(query) {
    return request({
        url: '/chart/BI/topbar/analysis',
        method: 'post',
        data: query
    })
}