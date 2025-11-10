import request from '@/common/request.js'

// 生成流程单号
export function GenerateNumber() {
  return request.get({
      url: '/FlowService/Task/GenerateNumber'
  })
}

// 查询待办任务列表
export function todoList(query) {
  return request.get({
    url: '/FlowService/Task/TodoList',
    query: query
  })
}
//获取流程定义列表
export function definitionList(query) {
  return request.get({
    url: '/FlowService/Task/DefinitionList',
    query: query
  })
}
// 我的发起的流程
export function myProcessList(query) {
  return request.get({
    url: '/FlowService/Task/MyProcess',
    query: query
  })
}

// 查询已办任务列表
export function finishedList(query) {
  return request.get({
    url: '/FlowService/Task/FinishedList',
    query: query
  })
}

//查询抄送我的列表
export function csList(query) {
  return request.get({
    url: '/FlowService/Task/CSList',
    query: query
  })
}

// 发起流程实例
export function deployStart(data) {
  return request.post({
    url: '/FlowService/Task/Add',
    data: data
  })
}

// 执行任务
export function excuteTask(data) {
  return request.post({
    url: '/FlowService/Task/Excute',
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
  return request.get({
    url: '/FlowService/Task/Remove',
    query: {id}
  })
}

// 任务流转记录
export function flowRecord(id) {
  return request.get({
    url: '/FlowService/Task/GetUserActionForm',
    query: {id}
  })
}

// 发布者任务流转记录
export function flowRootRecord(id){
  return request.get({
    url: '/FlowService/Task/GetUserRootForm',
    query: {id}
  })
}

// 获取流程的输入参数
export function getQuery(query){
  return request.get({
    url: '/FlowService/Task/GetQuery',
    query: query
  })
}
// 查询流程表单详情
export function getFormDetail(id) {
    return request.get({
        url: '/FlowService/Flow/Info/' + id,
    })
}