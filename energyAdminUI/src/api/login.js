/*
 * @Author: error: git config user.name && git config user.email & please set dead value or install git
 * @Date: 2022-06-27 09:43:03
 * @LastEditors: error: error: git config user.name & please set dead value or install git && error: git config user.email & please set dead value or install git & please set dead value or install git
 * @LastEditTime: 2023-04-01 13:52:45
 * @FilePath: \admin-ui\src\api\login.js
 * @Description: 这是默认设置,请设置`customMade`, 打开koroFileHeader查看配置 进行设置: https://github.com/OBKoro1/koro1FileHeader/wiki/%E9%85%8D%E7%BD%AE
 */
import request from '@/utils/request'

// 登录方法
export function login(username, password, code, uuid) {
    const data = {
        username,
        password,
        code,
        uuid
    }
    return request({
        url: '/AuthService/SysLogin/Login',
        method: 'post',
        data: data
    })
}
export function LoginByWxCorp(data) {
    return request({
        url: '/WeiXinService/Login/FromWxCorp',
        method: 'post',
        data: data
    })
}
// 获取指定用户详情
export function wxBaseUrl(query) {
    return request({
        url: '/WeiXinService/Login/CreateWxCorpRedirectUrl',
        method: 'get',
        params: query
    })
}
// 获取用户详细信息
export function getInfo() {
    return request({
        url: '/AuthService/User/LoginInfo',
        method: 'get'
    })
}

// 获取指定用户详情
export function getUserInfo(query) {
    return request({
        url: '/AuthService/User/Info',
        method: 'get',
        params: query
    })
}

// 退出方法
export function logout() {
    return request({
        url: '/AuthService/User/Logout',
        method: 'post'
    })
}

// 获取验证码
export function getCodeImg() {
    return request({
        url: '/AuthService/SysLogin/CaptchaImage',
        method: 'get',
        timeout: 20000
    })
}

// 刷新令牌
export function refreshToken(data) {
    return request({
        url: '/AuthService/SysLogin/RefreshToken',
        method: 'get',
        params: data
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
export function CorpWxConfigJson(appid,url) {
	return request({
		url: '/WeiXinService/Login/CorpWxConfigJson',
        method: 'post',
		data: {
			appid: appid,
			url: url
		}
	})
}
