import request from '@/utils/request'

export function handsonTableBIanalysis(query) {
    return request({
        url: '/chart/BI/handsontable/analysis',
        method: 'post',
        data: query
    })
}