import request from '@/utils/request'
import {
    praseStrEmpty
} from "@/utils/common";
import download from '@/plugins/download'
// 查询用户列表
export function listUser(query) {
    return request({
        url: '/AuthService/User/List',
        method: 'get',
        params: query
    })
}
// 查询用户的所有角色
export function listUserRole(uid) {
    return request({
        url: '/AuthService/User/UserRoleList',
        method: 'get',
        params: {uid}
    })
}
// 获取用户的所有企业
export function listUserOrg(uid) {
    return request({
        url: '/AuthService/User/UserOrgList',
        method: 'get',
        params: {uid}
    })
}
// 获取系统角色列表
export function listSystemRoles() {
    return request({
        url: '/AuthService/User/SysRoleList',
        method: 'get'
    })
}
export function AddSysRole(data){
    return request({
        url: '/AuthService/User/AddSysRole',
        method: 'post',
        data: data
    })
}
//删除用户系统角色
export function delUserRole(data) {
    return request({
        url: '/AuthService/User/DelSysRole',
        method: 'post',
        data: data
    })
}

// 查询用户详细
export function getUser(userId) {
    return request({
        url: '/AuthService/User/Info/' + praseStrEmpty(userId),
        method: 'get'
    })
}

// 新增用户
export function addUser(data) {
    return request({
        url: '/AuthService/User/Add',
        method: 'post',
        data: data
    })
}

// 修改用户
export function updateUser(data) {
    return request({
        url: '/AuthService/User/Edit',
        method: 'post',
        data: data
    })
}

// 删除用户
export function delUser(userId) {
    return request({
        url: '/AuthService/User/Remove/' + userId,
        method: 'get'
    })
}

// 导出用户
export function exportUser(query) {
    return download.resource('/AuthService/User/Export', query);
}

// 用户密码重置
export function resetUserPwd(userId, password) {
    const data = {
        userId,
        password
    }
    return request({
        url: '/AuthService/User/ResetPwd',
        method: 'post',
        data: data
    })
}

// 用户状态修改
export function changeUserStatus(userId, status) {
    const data = {
        userId,
        status
    }
    return request({
        url: '/AuthService/User/ChangeStatus',
        method: 'post',
        data: data
    })
}

// 查询用户个人信息
export function getUserProfile() {
    return request({
        url: '/AuthService/Profile/Info',
        method: 'get'
    })
}

// 修改用户个人信息
export function updateUserProfile(data) {
    return request({
        url: '/AuthService/Profile/Edit',
        method: 'post',
        data: data
    })
}

// 用户密码重置
export function updateUserPwd(oldPassword, newPassword) {
    const data = {
        oldPassword,
        newPassword
    }
    return request({
        url: '/AuthService/Profile/UpdatePwd',
        method: 'post',
        params: data
    })
}

// 用户头像上传
export function uploadAvatar(data) {
    return request({
        url: '/AuthService/Profile/Avatar',
        method: 'post',
        data: data
    })
}

// 下载用户导入模板
export function importTemplate() {
    return download.resource('/AuthService/User/ExportTemplate', {});
}

// 查询授权角色
export function getAuthRole(userId) {
    return request({
        url: '/AuthService/User/AuthRole/' + userId,
        method: 'get'
    })
}



//查询当前用户加入的组织列表
export function getJoinOrgList() {
    return request({
        url: '/AuthService/Profile/OrgList',
        method: 'get'
    })
}

//切换企业
export function switchOrg(query) {
    return request({
        url: '/AuthService/Profile/Switch',
        method: 'get',
        params: query
    })
}
export function LoginBySys(query) {//管理员登录账号
    return request({
        url: '/AuthService/User/LoginBySys',
        method: 'get',
        params: query
    })
}
