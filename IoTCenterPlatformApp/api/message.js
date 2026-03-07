import request from '@/common/request.js'

// 查询消息分页列表
export function messageList(query) {
    return request.get({
        url: '/MessageService/Message/ListPage',
        query: query
    })
}
// 查询前30条未读消息列表top
export function getNoRead(query) {
    return request.get({
        url: '/MessageService/Message/UnReadList',
        query: query
    })
}
//设置指定消息已读id
export function setRead(query) {
    return request.get({
        url: '/MessageService/Message/Read',
        query: query
    })
}
// 查询未读消息数量
export function noReadCount() {
    return request.get({
        url: '/MessageService/Message/UnreadCount',
    })
}
//设置全部已读
export function setReadAll() {
    return request.get({
        url: '/MessageService/Message/ReadAll',
    })
}
// 获取评论列表
export function commentList(query) {
    return request.get({
        url: '/DiscussService/comment/List',
        query: query
    })
}
export default {
    messageList,
    getNoRead,
    setRead,
    setReadAll,
	commentList
}