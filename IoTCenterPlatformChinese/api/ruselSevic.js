import request from '@/common/request.js'

// 查询规则引擎列表
export function rulesListFun(query) {
    return request.get({
        url: '/IoTRulesService/RuleFlow/ListPage',
        query: query
    })
}
//添加规则引擎
export function addRuselServe(data) {
    return request.post({
        url: '/IoTRulesService/RuleFlow/Add',
        data: data
    })
}
//修改规则引擎
export function editRuselServe(data) {
    return request.post({
        url: '/IoTRulesService/RuleFlow/Edit',
        data: data
    })
}
// 查询模板详情
export function getRuselDetail(query) {
    return request.get({
        url: '/IoTRulesService/RuleFlow/Info',
        query: query
    })
}

//删除规则
export function delRusel(id) {
    return request.get({
        url: '/IoTRulesService/RuleFlow/Remove',
        query: { id }
    })
}
//获取单个产品信息
export function productInfo(query) {
    return request.get({
        url: '/IoTService/IotProduct/Info',
        query: query
    })
}
//获取物联网产品列表信息
export function iotProductList(query) {
    return request.get({
        url: '/IoTService/IotProduct/ListPage',
        query: query
    })
}
export function toCronDes(cron){
  return request({
    url: '/MonitorService/Job/CronToDes',
    method: 'get',
    params:{ cron}
  })
}