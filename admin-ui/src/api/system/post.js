import request from '@/utils/request'

// 查询岗位列表
export function listPost(query) {
  return request({
    url: '/AuthService/Post/List',
    method: 'get',
    params: query
  })
}

// 查询岗位详细
export function getPost(postId) {
  return request({
    url: '/AuthService/Post/Info/' + postId,
    method: 'get'
  })
}

// 新增岗位
export function addPost(data) {
  return request({
    url: '/AuthService/Post/Add',
    method: 'post',
    data: data
  })
}

// 修改岗位
export function updatePost(data) {
  return request({
    url: '/AuthService/Post/Edit',
    method: 'post',
    data: data
  })
}

// 删除岗位
export function delPost(postId) {
  return request({
    url: '/AuthService/Post/Remove',
    method: 'get',
    data: {"id":postId}
  })
}

// 导出岗位
export function exportPost(query) {
  return request({
    url: '/AuthService/Post/Export',
    method: 'get',
    params: query
  })
}