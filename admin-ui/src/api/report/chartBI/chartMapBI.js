import request from '@/utils/request'

export function mapBIanalysis(query) {
    return request({
        url: '/chart/BI/map/analysis',
        method: 'post',
        data: query
    })
}