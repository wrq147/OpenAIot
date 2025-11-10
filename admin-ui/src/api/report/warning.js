import request from '@/utils/request'
// 查询报表预警列表
export function reportWarnList(query) {
    return request({
        url: '/ReportService/ReportWarn/List',
        method: 'get',
        params: query
    })
}
// 查询报表预警信息
export function reportWarnInfo(id) {
    return request({
      url: '/ReportService/ReportWarn/Info',
      method: 'get',
      params: {
        id
      }
    })
  }
  
// 添加报表预警
export function addReportWarn(data) {
    return request({
        url: '/ReportService/ReportWarn/Add',
        method: 'post',
        data: data
    })
}
// 修改报表预警
export function editReportWarn(data) {
    return request({
        url: '/ReportService/ReportWarn/Edit',
        method: 'post',
        data: data
    })
}

// 删除指定报表预警
export function deleteReportWarn(id) {
    return request({
        url: '/ReportService/ReportWarn/Remove',
        method: 'get',
        params: {
            id
        }
    })
}