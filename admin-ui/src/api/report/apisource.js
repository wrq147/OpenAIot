import request from '@/utils/request'

// 查询接口数据源列表
export function listApiSource(query) {
  return request({
    url: '/ReportService/ApiSource/List',
    method: 'get',
    params: query
})
}

// 查询接口数据源详细
export function getApiSource(id) {
  return request({
    url: '/ReportService/ApiSource/Info/' + id,
    method: 'get'
})
}

// 新增接口数据源
export function addApiSource(data) {
  return request({
    url: '/ReportService/ApiSource/Add',
    method: 'post',
    data: data
})
}

// 修改接口数据源
export function updateApiSource(data) {
  return request({
    url: '/ReportService/ApiSource/Edit',
    method: 'post',
    data: data
})
}

// 删除接口数据源
export function delApiSource(id) {
  return request({
    url: '/ReportService/ApiSource/Remove',
    method: 'get',
    params: {
      id
    }
  })
}
