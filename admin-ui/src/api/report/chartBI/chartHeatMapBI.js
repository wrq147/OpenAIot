import request from '@/utils/request'

export function heatMapBIanalysis(query) {
    return request({
        url: '/chart/BI/heatMap/analysis',
        method: 'post',
        data: query
    })
}