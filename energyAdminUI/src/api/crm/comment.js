import request from '@/utils/request'
// 添加评论
export function commentAddClue(data) {
    return request({
        url: '/DiscussService/Comment/Add',
        method: 'post',
        data: data
    })
}
// 获取评论列表
export function commentList(params) {
    return request({
        url: '/DiscussService/comment/List',
        method: 'get',
        params: params
    })
}
export function removePinglun(id) {
    return request({
        url: '/DiscussService/Comment/Remove?id='+id,
        method: 'DELETE',
    })
}
