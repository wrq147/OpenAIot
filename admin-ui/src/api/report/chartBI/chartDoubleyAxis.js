import request from '@/utils/request'

export function doubleyAxisBIanalysis(query) {
    return request({
        url: '/chart/BI/doubleyAxis/analysis',
        method: 'post',
        data: query
    })
}