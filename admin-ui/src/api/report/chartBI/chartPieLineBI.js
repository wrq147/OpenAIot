import request from '@/utils/request'

export function pieLineBIanalysis(query) {
    return request({
        url: '/chart/BI/pieline/analysis',
        method: 'post',
        data: query
    })
}