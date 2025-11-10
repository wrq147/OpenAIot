import request from '@/utils/request'

// 查询数据大屏主题配置列表
export function listTheme(query) {
  return request({
    url: '/ReportService/Theme/List',
    method: 'get',
    params: query
  })
}
