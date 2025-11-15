import request from '@/utils/request'
// 企业相关配置保存
export function orgConfSave(data) {
    return request({
        url: '/EfficiencyService/OrgConf/Save',
        method: 'post',
        data: data
    })
}
// 企业相关配置详情
export function orgConfInfo(query) {
    return request({
        url: '/EfficiencyService/OrgConf/Info',
        method: 'get',
        query: query
    })
}