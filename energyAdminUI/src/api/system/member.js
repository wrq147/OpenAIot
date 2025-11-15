import request from '@/utils/request'

// 查询员工列表
export function listMembers(query) {
    return request({
      url: '/AuthService/Member/List',
      method: 'get',
      params: query
    })
  }
  
  // 给员工授权角色
export function updateAuthRole(data) {
  return request({
      url: '/AuthService/Member/UpdateAuthRole',
      method: 'post',
      params: data
  })
}