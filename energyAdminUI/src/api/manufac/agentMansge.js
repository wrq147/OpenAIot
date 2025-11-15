import request from '@/utils/request'


// 生成生产商邀请码
export function factoryInvite(data) {
    return request({
        url: '/ProducerService/Agent/AddAllInvite',
        method: 'post',
        data: data
    })
}
// 生产商获取代理商列表
export function factorygetAgent(data) {
    return request({
        url: '/ProducerService/Agent/AllList',
        method: 'get',
        params: data
    })
}

// 生成生产商邀请码
export function joinInvite(data) {
    return request({
        url: '/ProducerService/Agent/JoinInvite',
        method: 'post',
        data: data
    })
}
//短信邀请注册
export function agentInviteReg(data) {
    return request({
        url: '/ProducerService/AgentInvite/Reg',
        method: 'post',
        data: data
    })
}
export function agentInviteLogin(data) { //短信邀请登录
    return request({
        url: '/ProducerService/AgentInvite/Login',
        method: 'post',
        data: data
    })
}

// 取消代理
export function CancelProxy(params) {
    return request({
        url: '/ProducerService/Agent/CancelProxy',
        method: 'get',
        params: params
    })
}
export function agentInviteInfo(params) {
    return request({
        url: '/ProducerService/AgentInvite/Info',
        method: 'get',
        params: params
    })
}
export function sendYqSms(params) {
    return request({
        url: '/SMSService/Sms/SendYqSms',
        method: 'get',
        params: params
    })
}
export function agentSendYqSms(params) {
    return request({
        url: '/ProducerService/Agent/SendYqSms',
        method: 'get',
        params: params
    })
}
export function yqRecord(params) { //邀请记录
    return request({
        url: '/ProducerService/Agent/List',
        method: 'get',
        params: params
    })
}