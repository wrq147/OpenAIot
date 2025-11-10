import request from '@/utils/request'

export function tableBIanalysis(query) {
    return request({
        url: '/chart/BI/table/analysis',
        method: 'post',
        data: query
    })
}