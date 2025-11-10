import request from '@/utils/request'
import {
    praseStrEmpty
} from "@/utils/common";
import download from '@/plugins/download'
// 查询员工列表
export function listMember(query) {
    return request({
        url: '/AuthService/Member/List',
        method: 'get',
        params: query
    })
}
// 全局搜索指定用户
export function searchMember(query) {
    return request({
        url: '/AuthService/User/Search',
        method: 'get',
        params: query
    })
}
// 全局搜索指定组织
export function searchOrg(query) {
    return request({
        url: '/AuthService/Org/Search',
        method: 'get',
        params: query
    })
}
//邀请指定用户加入企业
export function memberJionOrg(data) {
    return request({
        url: '/AuthService/Member/Add',
        method: 'post',
        data: data
    })
}
//邀请用户手机号注册
export function memberLogon(data) {
    return request({
        url: '/SMSService/Visitor/Reg',
        method: 'post',
        data: data
    })
}
//邀请用户邮箱注册
export function emailReg(data) {
    return request({
        url: '/EmailService/Visitor/Reg',
        method: 'post',
        data: data
    })
}
// 导出员工
export function exportMember(query) {
    return download.resource('/AuthService/User/Export', query);
}

// 删除员工
export function delMember(query) {
    return request({
        url: '/AuthService/Member/Remove',
        method: 'get',
        params: query
    })
}
//生成邀请码
export function createCode(query) {
    return request({
        url: '/AuthService/Member/InviteLink',
        method: 'get',
        params: query
    })
}
//解释邀请码
export function explainCode(query) {
    return request({
        url: '/AuthService/SysLogin/InviteCode',
        method: 'get',
        params: query
    })
}
//员工加入邀请
export function staffJoinOrg(data) {
    return request({
        url: '/AuthService/Org/Join',
        method: 'post',
        data: data,
    })
}
//发送短信验证码
export function sendMobileCode(query) {
    return request({
        url: '/SMSService/Visitor/SendCode',
        method: 'get',
        params: query
    })
}
//发送邮箱验证码
export function sendEmailCode(query) {
    return request({
        url: '/EmailService/Email/SendCode',
        method: 'get',
        params: query
    })
}
//发送邮箱验证码 不需要token
export function visitorEmailCode(query) {
    return request({
        url: '/EmailService/Visitor/SendCode',
        method: 'get',
        params: query
    })
}
//发送图形验证码
export function sendImgCode() {
    return request({
        url: '/SMSService/Visitor/GetSMSCaptcha',
        method: 'get'
    })
}
//绑定手机号
export function bindTel(data) {
    return request({
        url: '/SMSService/Sms/Bind',
        method: 'post',
        data: data,
    })
}
//员工重置密码
export function memberResetPwd(data) {
    return request({
        url: 'AuthService/Member/ResetPwd',
        method: 'post',
        data: data,
    })
}

//绑定邮箱
export function bindEmail(query) {
    return request({
        url: '/EmailService/Email/BindEmail',
        method: 'get',
        params: query
    })
}
export function loginPhone(data) {
    return request({
        url: '/SMSService/Visitor/FromTel',
        method: 'post',
        data: data,
    })
}
export function loginEmails(data) {
    return request({
        url: '/EmailService/Visitor/FromEmail',
        method: 'post',
        data: data,
    })
}
export function forgetNewPass(data) {
    return request({
        url: '/AuthService/Profile/UpdatePwdByCode',
        method: 'post',
        data: data,
    })
}
// 
export function userDepts(query) {
    return request({
        url: '/AuthService/Member/UserDepts',
        method: 'get',
        params: query
    })
}
// 添加分身
export function addClone(data) {
    return request({
        url: '/AuthService/Member/AddClone',
        method: 'post',
        data: data,
    })
}
// 删除分身
export function delClone(data) {
    return request({
        url: '/AuthService/Member/DelClone',
        method: 'post',
        data: data,
    })
}