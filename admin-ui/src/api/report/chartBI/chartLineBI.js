import request from '@/utils/request'

export function lineBIanalysis(query) {
    return request({
        url: '/chart/BI/line/analysis',
        method: 'post',
        data: query
    })
}