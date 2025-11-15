import request from '@/utils/request'

// 查询消息分页列表
export function messageList(query) {
    return request({
        url: '/MessageService/Message/ListPage',
        method: 'get',
        params: query
    })
}
// 查询前30条未读消息列表top
export function getNoRead(query) {
    return request({
        url: '/MessageService/Message/UnReadList',
        method: 'get',
        params: query
    })
}
//设置指定消息已读id
export function setRead(query) {
    return request({
        url: '/MessageService/Message/Read',
        method: 'get',
        params: query
    })
}
// 查询未读消息数量
export function noReadCount() {
    return request({
        url: '/MessageService/Message/UnreadCount',
        method: 'get'
    })
}
//设置全部已读
export function setReadAll() {
    return request({
        url: '/MessageService/Message/ReadAll',
        method: 'get'
    })
}
export default {
    messageList,
    getNoRead,
    setRead,
    setReadAll
}