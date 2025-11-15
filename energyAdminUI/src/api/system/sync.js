import request from '@/utils/request'
// 获取企业微信同步任务列表
export function corpTaskList(params) {
    return request({
      url: '/WeiXinService/Corp/TaskList',
      method: 'get',
      params:params
    })
}

//启动定时同步企业微信
export function wxStartSync(data) {
    return request({
      url: '/WeiXinService/Corp/StartSync',
      method: 'post',
      data: data
    })
}
//停止定时同步企业微信
export function wxStopSync(data) {
    return request({
      url: '/WeiXinService/Corp/StopSync',
      method: 'post',
      data: data
    })
}
//手动同步企业微信
export function wxManualSync(data) {
    return request({
      url: '/WeiXinService/Corp/ManualSync',
      method: 'post',
      data: data
    })
}
// 获取企业微信同步信息
export function corpInfo(id) {
    return request({
        url: '/WeiXinService/Corp/Info',
        method: 'get',
        params: {id}
    })
}
// 获取企业微信同步任务信息
export function corpTaskInfo(id) {
    return request({
        url: '/WeiXinService/Corp/TaskInfo',
        method: 'get',
        params: {id}
    })
}