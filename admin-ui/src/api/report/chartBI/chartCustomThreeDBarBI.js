import request from '@/utils/request'

export function customThreeDBarBIanalysis(query) {
    return request({
        url: '/chart/BI/customThreeDBar/analysis',
        method: 'post',
        data: query
    })
}