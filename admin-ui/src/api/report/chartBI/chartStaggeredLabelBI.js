import request from '@/utils/request'

export function staggeredlabelBIanalysis(query) {
    return request({
        url: '/chart/BI/staggeredlabel/analysis',
        method: 'post',
        data: query
    })
}