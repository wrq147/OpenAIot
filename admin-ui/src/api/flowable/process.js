import request from '@/utils/request'


// 生成流程单号
export function GenerateNumber() {
  return request({
      url: '/FlowService/Task/GenerateNumber',
      method: 'get'
  })
}

// 查询待办任务列表
export function todoList(query) {
  return request({
    url: '/FlowService/Task/TodoList',
    method: 'get',
    params: query
  })
}
//获取流程定义列表
export function definitionList(query) {
  return request({
    url: '/FlowService/Task/DefinitionList',
    method: 'get',
    params: query
  })
}
// 我的发起的流程
export function myProcessList(query) {
  return request({
    url: '/FlowService/Task/MyProcess',
    method: 'get',
    params: query
  })
}

// 查询已办任务列表
export function finishedList(query) {
  return request({
    url: '/FlowService/Task/FinishedList',
    method: 'get',
    params: query
  })
}

//查询抄送我的列表
export function csList(query) {
  return request({
    url: '/FlowService/Task/CSList',
    method: 'get',
    params: query
  })
}

// 发起流程实例
export function deployStart(data) {
  return request({
    url: '/FlowService/Task/Add',
    method: 'post',
    data: data
  })
}

// 执行任务
export function excuteTask(data) {
  return request({
    url: '/FlowService/Task/Excute',
    method: 'post',
    data: data
  })
}

// 取消申请
export function canProcess(id) {
  return request({
    url: '/FlowService/Task/Cancel',
    method: 'post',
    data: {id}
  })
}
//删除我的流程
export function delProcess(id) {
  return request({
    url: '/FlowService/Task/Remove',
    method: 'get',
    params: {id}
  })
}

// 任务流转记录
export function flowRecord(id) {
  return request({
    url: '/FlowService/Task/GetUserActionForm',
    method: 'get',
    params: {id}
  })
}

// 发布者任务流转记录
export function flowRootRecord(id){
  return request({
    url: '/FlowService/Task/GetUserRootForm',
    method: 'get',
    params: {id}
  })
}

// 获取流程的输入参数
export function getQuery(query){
  return request({
    url: '/FlowService/Task/GetQuery',
    method: 'get',
    params: query
  })
}