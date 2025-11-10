import request from '@/utils/request'
import download from '@/plugins/download'
// 查询操作日志列表
export function list(query) {
  return request({
    url: '/MonitorService/OperLog/List',
    method: 'get',
    params: query
  })
}

// 删除操作日志
export function delOperlog(operId) {
  return request({
    url: '/MonitorService/OperLog/' + operId,
    method: 'get'
  })
}

// 清空操作日志
export function cleanOperlog() {
  return request({
    url: '/MonitorService/OperLog/Clean',
    method: 'get'
  })
}

// 导出操作日志
export function exportOperlog(query) {
  return download.resource('/MonitorService/OperLog/Export', query);
}