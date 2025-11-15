import request from '@/utils/request'
// 获取企业自定义字段
export function orgField(params) {
    return request({
        url: '/AuthService/Org/Field',
        method: 'get',
        params: params
    })
}
// 获取企业自定义字段
export function orgFormFields(params) {
    return request({
        url: '/AuthService/Org/FormFields',
        method: 'get',
        params: params
    })
}
// 保持企业自定义字段
export function saveOrgField(data) {
    return request({
        url: '/AuthService/Org/SaveField',
        method: 'post',
        data: data
    })
}