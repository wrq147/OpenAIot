import request from '@/utils/request'

// 创建新企业
export function creatCompany(data) {
    return request({
        url: '/AuthService/Org/Create',
        method: 'post',
        data: data
    })
}
// 更新企业
export function editCompany(data) {
    return request({
        url: '/AuthService/Org/Edit',
        method: 'post',
        data: data
    })
}
//获取用户管理的企业
export function manageOrg() {
    return request({
        url: '/AuthService/Org/List',
        method: 'get',
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

//获取用户管理的企业
export function orgInfo(query) {
    return request({
        url: '/AuthService/Org/Info',
        method: 'get',
        params: query
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

//上传文件
export function uploadPhoto(data) {
    return request({
        url: '/AuthService/File/Upload?withDomain=true',
        method: 'post',
        data: data
    });
}
//删除文件
export function delPhoto(query) {
    return request({
        url: '/AuthService/File/Delete',
        method: 'get',
        params: query
    });
}