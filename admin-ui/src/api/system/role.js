import request from '@/utils/request'
import download from '@/plugins/download'
// 查询角色列表
export function listRole(query) {
    return request({
        url: '/AuthService/Role/List',
        method: 'get',
        params: query
    })
}

// 查询角色详细
export function getRole(roleId) {
    return request({
        url: '/AuthService/Role/Info/' + roleId,
        method: 'get'
    })
}

// 新增角色
export function addRole(data) {
    return request({
        url: '/AuthService/Role/Add',
        method: 'post',
        data: data
    })
}

// 修改角色
export function updateRole(data) {
    return request({
        url: '/AuthService/Role/Edit',
        method: 'post',
        data: data
    })
}

//角色数据权限修改
export function dataScope(scopeData) {
    return request({
        url: '/AuthService/Role/EditScope',
        method: 'post',
        data: scopeData
    })
}

// 角色状态修改
export function changeRoleStatus(roleId, status) {
    const data = {
        roleId,
        status
    }
    return request({
        url: '/AuthService/Role/Edit',
        method: 'post',
        data: data
    })
}

// 删除角色
export function delRole(roleId) {
    return request({
        url: '/AuthService/Role/Remove/' + roleId,
        method: 'get'
    })
}

// 导出角色
export function exportRole(query) {
    return download.resource('/AuthService/Role/Export', query);
}

// 查询角色已授权用户列表
export function allocatedUserList(query) {
    return request({
        url: '/AuthService/Role/AllocatedList',
        method: 'get',
        params: query
    })
}

// 查询角色未授权用户列表
export function unallocatedUserList(query) {
    return request({
        url: '/AuthService/Role/UnallocatedList',
        method: 'get',
        params: query
    })
}

// 取消用户授权角色
export function authUserCancel(data) {
    return request({
        url: '/AuthService/Role/Cancel',
        method: 'post',
        data: data
    })
}

// 批量取消用户授权角色
export function authUserCancelAll(data) {
    return request({
        url: '/AuthService/Role/CancelAll',
        method: 'post',
        params: data
    })
}

// 授权用户选择
export function authUserSelectAll(data) {
    return request({
        url: '/AuthService/Role/SelectAll',
        method: 'post',
        params: data
    })
}

// 根据角色域查询部门树结构
export function roleScopeTreeselect(roleId, menuId) {
    return request({
        url: '/AuthService/Role/RoleScopeTreeSelect',
        method: 'get',
        params: { id: roleId, menuId: menuId }
    })
}