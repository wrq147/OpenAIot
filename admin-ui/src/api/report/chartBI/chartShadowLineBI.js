import request from '@/utils/request'

export function shadowLineBIanalysis(query) {
    return request({
        url: '/chart/BI/shadowLine/analysis',
        method: 'post',
        data: query
    })
}