import request from '@/common/request.js'
// CRM 数据 模块
export function crmData(query) {
    return request.get({
        url: '/CRMService/CrmReport/StatisticsInfo',
        query: query
    })
}
// CRM 客户列表（公海池 ）模块
//公海池----生成客户编号
export function CustomerNumber(query) {
    return request.get({
        url: '/CRMService/Customer/GenerateNumber',
        query: query
    })
}
//公海池----客户列表
export function DataPoolsList(query) {
    return request.get({
        url: '/CRMService/Customer/PubList',
        query: query
    })
}

//公海池----客户详情
export function DataPoolsDetails(query) {
    return request.get({
        url: '/CRMService/Customer/Info',
        query: query
    })
}
// CRM 客户列表（私海池 ）模块
//私海----客户列表
export function CustomerList(query) {
    return request.get({
        url: '/CRMService/Customer/List',
        query: query
    })
}
//线索池列表
export function ClueData(query) {
    return request.get({
        url: '/CRMService/Clue/PubList',
        query: query
    })
}
//线索池详情
export function ClueDataDetails(query) {
    return request.get({
        url: '/CRMService/Clue/Info',
        query: query
    })
}
//线索池列表
export function xianSuo(query) {
    return request.get({
        url: '/CRMService/Clue/PriList',
        query: query
    })
}
//联系人
export function contactData(query) {
    return request.get({
        url: '/CRMService/Contact/List',
        query: query
    })
}
//联系人详情1
export function CustomerInfo(query) {
    return request.get({
        url: '/CRMService/Contact/Info',
        query: query
    })
}
//商机列表
export function BusineData(query) {
    return request.get({
        url: '/CRMService/Opportunity/List',
        query: query
    })
}
//商机详情
export function BusineDataDetails(query) {
    return request.get({
        url: '/CRMService/Opportunity/Info',
        query: query
    })
}
//我的授权
export function AuthorizationData() {
    return request.get({
        url: '/CRMService/Agent/AuthList'
    })
}
//跟进计划
export function flowDataList(query) {
    return request.get({
        url: '/CRMService/Plan/List',
        query: query
    })
}

//跟进记录
export function flowDataRecord(query) {
    return request.get({
        url: '/CRMService/Follow/List',
        query: query
    })
}

//跟进记录详情
export function flowDataRecordDetails(query) {
    return request.get({
        url: '/CRMService/Follow/Info',
        query: query
    })
}
//字典  执行方式
export function ExecutionMode(query) {
    return request.get({
        url: '/DictService/DictData/List?pageNum=1&pageSize=10&dictType=follow_way',
        query: query
    })
}
///行业类型
export function IndustryType(query) {
    return request.get({
        url: '/AuthService/Code/IndustryList',
        query: query
    })
}

//修改公海客户
export function pubEditCustomer(data) {
    return request({
        url: '/CRMService/Customer/PubEdit',
        method: 'post',
        data: data
    })
}
// 添加公海客户
export function pubAddCustomer(data) {
    return request({
        url: '/CRMService/Customer/PubAdd',
        method: 'post',
        data: data
    })
}

//添加客户
export function pubAddKeHhu(data) {
    return request({
        url: '/CRMService/Customer/Add',
        method: 'post',
        data: data
    })
}
//修改客户
export function pubAddEditXiu(data) {
    return request({
        url: '/CRMService/Customer/Edit',
        method: 'post',
        data: data
    })
}
//新增线索池
export function AddCluePool(data) {
    return request({
        url: '/CRMService/Clue/PubAdd',
        method: 'post',
        data: data
    })
}
//编辑线索池
export function AddPubEdit(data) {
    return request({
        url: '/CRMService/Clue/PubEdit',
        method: 'post',
        data: data
    })
}
//线索新增
export function ClueAdd(data) {
    return request({
        url: '/CRMService/Clue/Add',
        method: 'post',
        data: data
    })
}
//线索编辑
export function ClueEdit(data) {
    return request({
        url: '/CRMService/Clue/Edit',
        method: 'post',
        data: data
    })
}

//联系人新增
export function addContact(data) {
    return request({
        url: '/CRMService/Contact/Add',
        method: 'post',
        data: data
    })
}
//联系人编辑
export function EditContact(data) {
    return request({
        url: '/CRMService/Contact/Edit',
        method: 'post',
        data: data
    })
}
//需求发现
export function periodList(query) {
    return request.get({
        url: '/CRMService/Period/List',
        query: query
    })
}
//商机编号
export function periodNumber(query) {
    return request.get({
        url: '/CRMService/Opportunity/GenerateNumber',
        query: query
    })
}

//商机新增
export function OpportunityAdd(data) {
    return request({
        url: '/CRMService/Opportunity/Add',
        method: 'post',
        data: data
    })
}

//商机详情
export function OpportunityInfo(query) {
    return request.get({
        url: '/CRMService/Opportunity/Info',
        query: query
    })
}
//商机编辑
export function OpportunityEdit(data) {
    return request({
        url: '/CRMService/Opportunity/Edit',
        method: 'post',
        data: data
    })
}
//跟进计划详情
export function PlanDetails(query) {
    return request.get({
        url: '/CRMService/Plan/Info',
        query: query
    })
}
//跟进计划新增
export function PlanAdd(data) {
    return request({
        url: '/CRMService/Plan/Add',
        method: 'post',
        data: data
    })
}
//跟进计划编辑
export function PlanEdit(data) {
    return request({
        url: '/CRMService/Plan/Edit',
        method: 'post',
        data: data
    })
}
//跟进记录新增
export function FollowRecord(data) {
    return request({
        url: '/CRMService/Follow/Add',
        method: 'post',
        data: data
    })
}
//跟进记录编辑
export function FollowEdit(data) {
    return request({
        url: '/CRMService/Follow/Edit',
        method: 'post',
        data: data
    })
}

//公海池删除 
export function PoolDelete(query) {
    return request.get({
        url: '/CRMService/Customer/PubRemove',
        query: query
    })
}
//公海池领取操作
export function ReceiveDraw(query) {
    return request.get({
        url: '/CRMService/Customer/Draw',
        query: query
    })
}

// 代理商生成客户或代理商邀请码
export function customerInvite(data) {
    return request({
        url: '/CRMService/Agent/AddInvite',
        method: 'post',
        data: data
    })
}
//客户删除
export function customerDelete(query) {
    return request.get({
        url: '/CRMService/Customer/Remove',
        query: query
    })
}
//退回
export function customerReturn(query) {
    return request.get({
        url: '/CRMService/Customer/Return',
        query: query
    })
}
//线索池领取接口
export function retrieval(query) {
    return request.get({
        url: '/CRMService/Clue/Draw',
        query: query
    })
}

//线索池删除
export function poolPubRemove(query) {
    return request.get({
        url: '/CRMService/Clue/PubRemove',
        query: query
    })
}
//线索删除
export function culeRemove(query) {
    return request.get({
        url: '/CRMService/Clue/Remove',
        query: query
    })
}
//线索退回
export function ClueReturn(query) {
    return request.get({
        url: '/CRMService/Clue/Return',
        query: query
    })
}
//跟进计划
export function FollowPlan(query) {
    return request.get({
        url: '/CRMService/Plan/Remove',
        query: query
    })
}
//跟进计划完成
export function completion(query) {
    return request.get({
        url: '/CRMService/Plan/Finish',
        query: query
    })
}
//快速跟进
export function FastTracking(data) {
    return request({
        url: '/CRMService/Follow/Add',
        method: 'post',
        data: data
    })
}
//删除 
export function ContactRemove(query) {
    return request.get({
        url: '/CRMService/Contact/Remove',
        query: query
    })
}

//评论添加
export function commentAdd(data) {
    return request({
        url: '/DiscussService/Comment/Add',
        method: 'post',
        data: data
    })
}
//评论列表
export function commentList(query) {
    return request.get({
        url: '/DiscussService/comment/List',
        query: query
    })
}
//销售阶段切换
export function phaseSwitching(query) {
    return request.get({
        url: '/CRMService/Opportunity/forward',
        query: query
    })
}
//商机删除
export function oppartRemove(query) {
    return request.get({
        url: '/CRMService/Opportunity/Remove',
        query: query
    })
}
//漏斗图
export function FunnelPlot(query) {
    return request.get({
        url: '/CRMService/CrmReport/PeriodCountList',
        query: query
    })
}

export function opportunityDetails(query) {
    return request.get({
        url: '/CRMService/Stock/List',
        query: query
    })
}
//取消邀请
export function cancelInvitation(query) {
    return request.get({
        url: '/CRMService/Customer/UnBind',
        query: query
    })
}