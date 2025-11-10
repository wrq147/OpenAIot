import request from '@/utils/request'

export function progressBIanalysis(query) {
    return request({
        url: '/chart/BI/progress/analysis',
        method: 'post',
        data: query
    })
}