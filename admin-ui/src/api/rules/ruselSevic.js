import request from '@/utils/request'

// 查询规则引擎列表
export function rulesList(query) {
    return request({
        url: '/IoTRulesService/RuleFlow/ListPage',
        method: 'get',
        params: query
    })
}
//添加规则引擎
export function addRuselServe(data) {
    return request({
        url: '/IoTRulesService/RuleFlow/Add',
        method: 'post',
        data: data
    })
}
//修改规则引擎
export function editRuselServe(data) {
    return request({
        url: '/IoTRulesService/RuleFlow/Edit',
        method: 'post',
        data: data
    })
}
// 查询模板详情
export function getRuselDetail(query) {
    return request({
        url: '/IoTRulesService/RuleFlow/Info',
        method: 'get',
        params: query
    })
}
// 重置规则
export function resetRusel(id) {
    return request({
        url: '/IoTRulesService/RuleFlow/Reset',
        method: 'get',
        params: { id }
    })
}
//删除规则
export function delRusel(id) {
    return request({
        url: '/IoTRulesService/RuleFlow/Remove',
        method: 'get',
        params: { id }
    })
}
// 获取规则分组树
export function groupTree() {
    return request({
        url: '/IoTRulesService/RuleGroup/ListTree',
        method: 'get',
    })
}
// 获取设备分组树
export function groupSelectTree() {
    return request({
        url: '/IoTRulesService/RuleGroup/TreeSelect',
        method: 'get',
    })
}
// 获取在指定分组信息
export function groupInfo(query) {
    return request({
        url: '/IoTRulesService/RuleGroup/Info',
        method: 'get',
        params: query
    })
}
// 删除分组
export function removeGroup(query) {
    return request({
        url: '/IoTRulesService/RuleGroup/Remove',
        method: 'get',
        params: query
    })
}

//添加设备分组
export function addGroup(data) {
    return request({
        url: '/IoTRulesService/RuleGroup/Add',
        method: 'post',
        data: data
    })
}

//编辑设备分组
export function editGroup(data) {
    return request({
        url: '/IoTRulesService/RuleGroup/Edit',
        method: 'post',
        data: data
    })
}


//获取规则服务节点列表
export function nodeList() {
    return request({
        url: '/IoTRulesService/RuleFlow/NodeList',
        method: 'get'
    })
}

//启用规则调试
export function debugOn(id) {
    return request({
        url: '/IoTRulesService/RuleFlow/DebugOn',
        method: 'get',
        params: { id }
    })
}


//关闭规则调试
export function debugOff(id) {
    return request({
        url: '/IoTRulesService/RuleFlow/DebugOff',
        method: 'get',
        params: { id }
    })
}


//强制下线规则节点
export function forcedDown(name) {
    return request({
        url: '/IoTRulesService/RuleFlow/ForcedDown',
        method: 'get',
        params: {name}
    })
}

//上线指定名称的规则节点
export function regNode(name) {
    return request({
        url: '/IoTRulesService/RuleFlow/RegNode',
        method: 'get',
        params: {name}
    })
}