import request from '@/utils/request'
import download from '@/plugins/download'
// 查询定时任务调度列表
export function listJob(query) {
  return request({
    url: '/MonitorService/Job/List',
    method: 'get',
    params: query
  })
}

// 查询定时任务调度详细
export function getJob(jobId) {
  return request({
    url: '/MonitorService/Job/Info/' + jobId,
    method: 'get'
  })
}

// 新增定时任务调度
export function addJob(data) {
  return request({
    url: '/MonitorService/Job/Add',
    method: 'post',
    data: data
  })
}

// 修改定时任务调度
export function updateJob(data) {
  return request({
    url: '/MonitorService/Job/Edit',
    method: 'post',
    data: data
  })
}

// 删除定时任务调度
export function delJob(jobId) {
  return request({
    url: '/MonitorService/Job/Remove',
    method: 'get',
    params:{ id:[jobId]}
  })
}

// 导出定时任务调度
export function exportJob(query) {
  return download.resource('/MonitorService/Job/Export', query);
}

// 任务状态修改
export function changeJobStatus(jobId, status) {
  const data = {
    jobId,
    status
  }
  return request({
    url: '/MonitorService/Job/ChangeStatus',
    method: 'post',
    params: data
  })
}


// 定时任务立即执行一次
export function runJob(jobId) {
  return request({
    url: '/MonitorService/Job/Run/'+jobId,
    method: 'get'
  })
}


export function toCronDes(cron){
  return request({
    url: '/MonitorService/Job/CronToDes',
    method: 'get',
    params:{ cron}
  })
}

// 获取企业假期列表
export function calendarList(query) {
  return request({
    url: '/MonitorService/Calendar/List',
    method: 'get',
    params: query
  })
}

// 新增企业假期
export function calendarAdd(data) {
  return request({
    url: `/MonitorService/Calendar/Add?data=${data.data}`,
    method: 'post'
  })
}

// 删除企业假期
export function calendarRemove(query) {
  return request({
    url: '/MonitorService/Calendar/Remove',
    method: 'get',
    params: query
  })
}

// 节假日列表
export function HolidayTypeList(query) {
  return request({
    url: '/MonitorService/Calendar/HolidayTypeList',
    method: 'get',
    params: query
  })
}
// 新增节假日类型
export function HolidayTypeAdd(data) {
  return request({
    url: '/MonitorService/Calendar/AddType',
    method: 'post',
    data: data
  })
}

// 编辑节假日类型
export function HolidayTypeEdit(data) {
  return request({
    url: '/MonitorService/Calendar/EditType',
    method: 'post',
    data: data
  })
}

// 删除节假日类型
export function HolidayTypeDel(query) {
  return request({
    url: '/MonitorService/Calendar/DelType',
    method: 'get',
    params: query
  })
}