import request from '@/common/request.js'
// #ifdef H5
import {serverUrl} from '@/common/constVar.js'
// #endif
// #ifndef H5
import serverUrl from '@/common/constVar.js'
// #endif
let ser = ''
// #ifdef H5
ser = serverUrl
// #endif
// #ifndef H5
ser = serverUrl.getServerUrl()
// #endif
// 登录方法
export function login(username, password, code, uuid) {
    const data = {
        username,
        password,
        code,
        uuid
    }
    return request.post({
        url: '/AuthService/SysLogin/Login',
        data: data
    })
}

// 获取用户详细信息
export function getInfo() {
    return request.get({
        url: '/AuthService/User/LoginInfo'
    })
}

// 获取指定用户详情
export function getUserInfo(query) {
    return request.get({
        url: '/AuthService/User/Info',
		query:query
    })
}

// 退出方法
export function logout(isNotJump=false) {
    return request.post({
        url: '/AuthService/User/Logout',
		isNotJump:isNotJump
    })
}

// 获取验证码
export function getCodeImg() {
    return request.get({
        url: '/AuthService/SysLogin/CaptchaImage',
        timeout: 20000
    })
}
// 获取短信登录图形验证码
export function getSMSCodeImg() {
    return request.get({
        url: '/SMSService/Visitor/GetSMSCaptcha',
        timeout: 20000
    })
}
// 手机号登录
export function fromTelLogin(data) {
    return request.post({
        url: '/SMSService/Visitor/FromTel',
		data: data
    })
}

// 刷新令牌
export function refreshToken(query) {
    return request.get({
        url: '/AuthService/SysLogin/RefreshToken',
        query: query
    })
}
//注册
//解释邀请码
export function explainCode(query) {
    return request.get({
        url: '/AuthService/SysLogin/InviteCode',
        query: query
    })
}

//员工加入邀请
export function staffJoinOrg(data) {
    return request.post({
        url: '/AuthService/Org/Join',
        data: data
    })
}

//邮箱注册
export function emailReg(data) {
    return request.post({
        url: '/EmailService/Visitor/Reg',
        data: data
    })
}
//手机号注册
export function phoneReg(data) {
    return request.post({
        url: '/SMSService/Visitor/Reg',
        data: data
    })
}
//发送邮箱验证码
export function sendEmailCode(query) {
    return request.get({
        url: '/EmailService/Email/SendCode',
        query: query
    })
}
//发送短信验证码
export function sendMobileCode(query) {
    return request.get({
        url: '/SMSService/Visitor/SendCode',
        query: query
    })
}
export function forgetNewPass(data) {
    return request({
        url: '/AuthService/Profile/UpdatePwdByCode',
        method: 'post',
        data: data,
    })
}
export function agentInviteInfo(query) {
    return request({
        url: '/ProducerService/AgentInvite/Info',
        method: 'get',
        query: query
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

// export function sendMobileCode() {
// 	return new Promise((resolve, reject) => {
// 		uni.request({
// 			url: ser + '/SMSService/Visitor/SendCode',
// 			data: {},
// 			method: "GET",
// 			header: {
// 				'Content-Type': 'application/json;charset=utf-8', //自定义请求头信息
// 				"Access-Control-Allow-Origin":'*'
// 			},
// 			success: (res) => {
// 				resolve(res.data);
// 			},
// 			fail: function(err) {
// 				console.log("err",err);
// 				reject(err)
// 			}
// 		});
// 	})
// }