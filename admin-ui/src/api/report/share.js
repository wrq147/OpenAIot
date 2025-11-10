import request from '@/utils/request'

//获取分享列表
export function shareList(params) {
  return request({
    url: '/ReportService/Share/List',
    method: 'get',
    params
  })
}

//获取分享
export function infoShare(id) {
  return request({
    url: '/ReportService/Share/Info?id='+id,
    method: 'get',
  })
}

//添加分享
export function addShare(data) {
  return request({
    url: '/ReportService/Share/Add',
    method: 'post',
    data: data
  })
}
//编辑分享
export function editShare(data) {
  return request({
    url: '/ReportService/Share/Edit',
    method: 'post',
    data: data
  })
}

//删除分享
export function shareRemove(params) {
  return request({
    url: '/ReportService/Share/Remove',
    method: 'get',
    params
  })
}