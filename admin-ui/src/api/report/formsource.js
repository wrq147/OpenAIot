import request from '@/utils/request'

// 查询格文件数据源列表
export function listFormsource(query) {
  return request({
    url: '/ReportService/FileSource/List',
    method: 'get',
    params: query
})
}

// 查询格文件数据源详细
export function getFormsource(id) {
  return request({
    url: '/ReportService/FileSource/Info/' + id,
    method: 'get'
})
}

// 新增格文件数据源
export function addFormsource(data) {
  return request({
    url: '/ReportService/FileSource/Add',
    method: 'post',
    data: data
})
}

// 修改格文件数据源
export function updateFormsource(data) {
  return request({
    url: '/ReportService/FileSource/Edit',
    method: 'post',
    data: data
})
}

// 删除格文件数据源
export function delFormsource(id) {
  return request({
    url: '/ReportService/FileSource/Remove',
    method: 'get',
    params: {
      id
    }
  })
}
