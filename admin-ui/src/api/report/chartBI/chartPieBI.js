import request from '@/utils/request'

export function pieBIanalysis(query) {
    return request({
        url: '/chart/BI/pie/analysis',
        method: 'post',
        data: query
    })
}

export function ringPieBIanalysis(query) {
    return request({
        url: '/chart/BI/ringPie/analysis',
        method: 'post',
        data: query
    })
}
