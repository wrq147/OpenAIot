import request from '@/utils/request'

export function pieMoreBIanalysis(query) {
    return request({
        url: '/chart/BI/piemore/analysis',
        method: 'post',
        data: query
    })
}