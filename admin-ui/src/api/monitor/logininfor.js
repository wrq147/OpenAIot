import request from '@/utils/request'
import download from '@/plugins/download'
// 查询登录日志列表
export function list(query) {
  return request({
    url: '/AuthService/LoginiLog/List',
    method: 'get',
    params: query
  })
}

// 删除登录日志
export function delLogininfor(query) {
  return request({
    url: '/AuthService/LoginiLog/Remove',
    method: 'get',
    params: query
  })
}

// 清空登录日志
export function cleanLogininfor() {
  return request({
    url: '/AuthService/LoginiLog/Clean',
    method: 'get'
  })
}

// 导出登录日志
export function exportLogininfor(query) {
  return download.resource('/AuthService/LoginiLog/Export', query);
}