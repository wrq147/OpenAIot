import request from '@/utils/request'

export function cylinderBIanalysis(query) {
    return request({
        url: '/chart/BI/cylinder/analysis',
        method: 'post',
        data: query
    })
}