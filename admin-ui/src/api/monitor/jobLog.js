import request from '@/utils/request'
import download from '@/plugins/download'
// 查询调度日志列表
export function listJobLog(query) {
  return request({
    url: '/MonitorService/JobLog/List',
    method: 'get',
    params: query
  })
}

// 删除调度日志
export function delJobLog(jobLogId) {
  return request({
    url: '/MonitorService/JobLog/Remove/' + jobLogId,
    method: 'get'
  })
}

// 清空调度日志
export function cleanJobLog(name,group) {
  return request({
    url: '/MonitorService/JobLog/Clean',
    method: 'get',
    params:{name,group}
  })
}

// 导出调度日志
export function exportJobLog(query) {
  return download.resource('/MonitorService/JobLog/Export', query);
}

export function retryJobLog(id) {
  return request({
    url: '/MonitorService/JobLog/Retry',
    method: 'get',
    params:{id}
  })
}