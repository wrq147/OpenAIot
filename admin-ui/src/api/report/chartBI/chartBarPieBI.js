import request from '@/utils/request'

export function barPieBIanalysis(query) {
    return request({
        url: '/chart/BI/barpie/analysis',
        method: 'post',
        data: query
    })
}