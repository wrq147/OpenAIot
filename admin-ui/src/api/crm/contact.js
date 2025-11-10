import request from '@/utils/request'

// 获取联系人列表
export function contactList(params) {
    return request({
        url: 'CRMService/Contact/List',
        method: 'get',
        params: params
    })
}
// 获取联系人信息
export function contactInfo(params) {
    return request({
        url: '/CRMService/Contact/Info',
        method: 'get',
        params: params
    })
}
// 删除联系人
export function delContact(params) {
    return request({
        url: '/CRMService/Contact/Remove',
        method: 'get',
        params: params
    })
}
// 添加联系人
export function addContact(data) {
    return request({
        url: '/CRMService/Contact/Add',
        method: 'post',
        data: data
    })
}

// 修改联系人
export function editContact(data) {
    return request({
        url: '/CRMService/Contact/Edit',
        method: 'post',
        data: data
    })
}