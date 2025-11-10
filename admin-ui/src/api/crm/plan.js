import request from '@/utils/request'
// 跟进计划列表
export function followPlanList(query) {
    return request({
        url: '/CRMService/Plan/List',
        method: 'get',
        params: query
    })
}
//添加计划列表---跟进人客户列表
export function PlanListCustomers(query) {
    return request({
        url: '/CRMService/Customer/List',
        method: 'get',
        params: query
    })
}
//归属部门列表
export function BelongingDepartments(query) {
    return request({
        url: '/AuthService/Dept/List',
        method: 'get',
        params: query
    })
}

// 查询部门列表
export function listDept(query) {
    return request({
        url: '/AuthService/Dept/List',
        method: 'get',
        params: query
    })
}
//添加跟进计划
export function AddFlowPlan(data) {
    return request({
        url: '/CRMService/Plan/Add',
        method: 'post',
        data: data
    })
}
//删除跟进计划
export function DeletePlan(query) {
    return request({
        url: '/CRMService/Plan/Remove',
        method: 'get',
        params: query
    })
}
//完成跟进计划

export function CompleteFollow(query) {
    return request({
        url: '/CRMService/Plan/Finish',
        method: 'get',
        params: query
    })
}
//获取员工列表
export function ObtainEmployee(query) {
    return request({
        url: '/AuthService/Member/List',
        method: 'get',
        params: query
    })
}
//详情接口
export function ObtainDetails(query) {
    return request({
        url: '/CRMService/Plan/Info',
        method: 'get',
        params: query
    })
}
//编辑接口
export function ObtainEdit(data) {
    return request({
        url: '/CRMService/Plan/Edit',
        method: 'post',
        data: data
    })
}