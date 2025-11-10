import request from '@/common/request.js'
//个人中心信息
export function dataList(query) {
    return request.get({
        url: '/AuthService/Profile/Info',
        query: query
    })
}
//流程报表接口
export function  ReportInterface(query) {
    return request.get({
        url: '/FlowService/FlowReport/StatisticsInfo',
        query: query
    })
}
//组织列表
export function  OrganizationList(query) {
    return request.get({
        url: '/AuthService/Profile/OrgList',
        query: query
    })
}
//切换企业
export function  SwitchEnterprise(query) {
    return request.get({
        url: '/AuthService/Profile/Switch',
        query: query
    })
}
//获取行业列表
export function  IndustryList(query) {
    return request.get({
        url: '/AuthService/Code/IndustryList',
        query: query
    })
}


//获取省市区列表
export function  provincesList(query) {
    return request.get({
        url: '/AuthService/Code/AreaList',
        query: query
    })
}
// 创建新企业
export function creatCompany(data) {
    return request({
        url: '/AuthService/Org/Create',
        method: 'post',
        data: data
    })
}
// 获取默认企业信息
export function GetDefaultData(query) {
    return request.get({
        url: '/AuthService/Org/Info',
        query: query
    })
}

//禁用账号默认
export function deleteAccountLogin(query) {
    return request.get({
        url: '/AuthService/Profile/Delete',
        query: query
    })
}


//企业详情
export function EnterpriseDetails(query) {
    return request.get({
        url: '/AuthService/Org/Info',
        query: query
    })
}
//企业编辑
export function BusinessEditors(data) {
    return request({
        url: '/AuthService/Org/Edit',
        method: 'post',
        data: data
    })
}

//解散企业/AuthService/Org/Remove
export function removeOrg(query) {
    return request({
        url: '/AuthService/Org/Remove',
        method: 'get',
        params: query
    })
}

// 查询员工列表
export function listMember(query) {
    return request({
        url: '/AuthService/Member/List',
        method: 'get',
        params: query
    })
}
//移交企业
export function handoverOrg(query) {
    return request({
        url: '/AuthService/Org/ChangeCreator',
        method: 'get',
        params: query
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


//发送邮箱验证码
export function sendEmailCode(query) {
    return request({
        url: '/EmailService/Email/SendCode',
        method: 'get',
        params: query
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

// 修改用户个人信息
export function updateUserProfile(data) {
    return request({
        url: '/AuthService/Profile/Edit',
        method: 'post',
        data: data
    })
}


//员工管理
export function yuanAarngemnt(query) {
    return request({
        url: '/AuthService/Member/List',
        method: 'get',
        params: query
    })
}
//员工详情
export function yuanAarngemntDetails(query) {
    return request({
        url: '/AuthService/User/Info',
        method: 'get',
        params: query
    })
}
//员工管理 删除员工
export function deleteArngemnt(query) {
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

//搜索部门
export function searchDeptSearch(query) {
    return request({
        url: '/AuthService/User/Search',
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

//部门列表
export function DeptamentList(query) {
    return request.get({
        url: '/AuthService/Dept/List',
        query: query
    })
}
//新增部门
export function DeptamentAdd(data) {
    return request({
        url: '/AuthService/Dept/Add',
        method: 'post',
        data: data
    })
}
//部门详情
export function DeptamentDetails(query) {
    return request.get({
        url: '/AuthService/Dept/Info/'+query,
        query: query
    })
}
//部门编辑
export function DeptamentEdit(data) {
    return request({
        url: '/AuthService/Dept/Edit',
        method: 'post',
        data: data
    })
}
//批量设置部门负责人
export function setLeaders(data) {
    return request({
        url: '/AuthService/Dept/setLeaders',
        method: 'post',
        data: data
    })
}
//批量设置部门负责人
export function deptMove(data) {
    return request({
        url: '/AuthService/Dept/Move',
        method: 'post',
        data: data
    })
}
//角色列表
export function RoleList(query) {
    return request.get({
        url: '/AuthService/Role/List',
        query: query
    })
}
//角色新增
export function RoleAdd(data) {
    return request({
        url: '/AuthService/Role/Add',
        method: 'post',
        data: data
    })
}
//角色详情
export function RoleDetails(query) {
    return request.get({
        url: '/AuthService/Role/Info/'+query,
        query: query
    })
}
//编辑
export function RoleEdit(data) {
    return request({
        url: '/AuthService/Role/Edit',
        method: 'post',
        data: data
    })
}
//角色删除
export function RoleDelete(query) {
    return request.get({
        url: '/AuthService/Role/Remove/'+query,
        query: query
    })
}
//角色权限
export function RolePermission(data) {
    return request.get({
        url: '/AuthService/Menu/TreeSelect',
        data: data
    })
}
//角色权限编辑

export function RolePermissionEdit(data) {
    return request.get({
        url: '/AuthService/Menu/RoleMenuTreeSelect/'+data,
        data: data
    })
}
//分配角色 列表
export function AssignRolesList(query) {
    return request.get({
        url: '/AuthService/User/AuthRole/'+query,
        query: query
    })
}
//分配角色提交保存
export function AssignRolesSave(data) {
    return request.post({
        url: '/AuthService/Member/UpdateAuthRole?userId='+data.userId+'&roleIds='+data.roleIds,
        data: data
    })
}
