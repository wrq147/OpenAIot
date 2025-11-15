import request from '@/utils/request'

// 查询在线用户列表
export function list(query) {
  return request({
    url: '/MonitorService/Online/List',
    method: 'get',
    params: query
  })
}

// 强退用户
export function forceLogout(tokenId) {
  return request({
    url: '/MonitorService/Online/ForceLogout/' + tokenId,
    method: 'get'
  })
}
