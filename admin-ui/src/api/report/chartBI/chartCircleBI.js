import request from '@/utils/request'

export function circleBIanalysis(query) {
    return request({
        url: '/chart/BI/circle/analysis',
        method: 'post',
        data: query
    })
}