import request from '@/utils/request'

// 查询缓存详细
export function getCache() {
  return request({
    url: '/MonitorService/Cache/Info',
    method: 'get'
  })
}

// 清理缓存
export function delCache(key) {
  return request({
    url: '/MonitorService/Cache/Del',
    method: 'get',
    params: {
      key
    }
  })
}