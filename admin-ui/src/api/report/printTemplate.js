import request from '@/utils/request'

// 查询打印模板
export function listPrintTemplate(query) {
  return request({
        url: '/ReportService/Print/List',
        method: 'get',
        params: query
    })
}

// 查询打印数据源
export function getDataInfo(id) {
  return request({
      url: '/ReportService/Print/DataInfo/' + id,
      method: 'get'
  })
}

//添加打印模板
export function addPrintTemplate(data) {
  return request({
    url: '/ReportService/Print/Add',
    method: 'post',
    data: data
  })
}

//编辑打印模板
export function editPrintTemplate(data) {
  return request({
    url: '/ReportService/Print/Edit',
    method: 'post',
    data: data
  })
}

// 删除打印模板
export function deletePrintTemplate(id) {
  return request({
    url: '/ReportService/Print/Remove',
    method: 'get',
    params: {
      id
    }
  })
}

//获取指定打印模板
export function templateInfo(id) {
  return request({
    url: '/ReportService/Print/Info',
    method: 'get',
    params: {
      id
    }
  })
}

// 查询打印模板列表
export function dataList() {
  return request({
        url: '/ReportService/Print/DataList',
        method: 'get'
    })
}
