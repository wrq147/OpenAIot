import request from '@/utils/request'

// 查询自定义组件库列表
export function listCustomchart(query) {
  return request({
    url: '/datav/customchart/list',
    method: 'get',
    params: query
  })
}

// 查询自定义组件库详细
export function getCustomchart(id) {
  return request({
    url: '/datav/customchart/' + id,
    method: 'get'
  })
}

// 新增自定义组件库
export function addCustomchart(data) {
  return request({
    url: '/datav/customchart',
    method: 'post',
    data: data
  })
}

// 修改自定义组件库
export function updateCustomchart(data) {
  return request({
    url: '/datav/customchart',
    method: 'put',
    data: data
  })
}

// 删除自定义组件库
export function delCustomchart(id) {
  return request({
    url: '/datav/customchart/' + id,
    method: 'delete'
  })
}

// 导出自定义组件库
export function exportCustomchart(query) {
  return request({
    url: '/datav/customchart/export',
    method: 'get',
    params: query
  })
}