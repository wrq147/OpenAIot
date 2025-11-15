import request from '@/utils/request'
// 查询主题列表
export function styleManList(query) {
    return request({
        url: '/AuthService/StyleMan/List',
        method: 'get',
        params: query
    })
}

// 查询主题详细
export function getStyleMan(styleId) {
    return request({
        url: '/AuthService/StyleMan/Info?id=' + styleId,
        method: 'get'
    })
}

// 新增主题
export function addStyleMan(data) {
    return request({
        url: '/AuthService/StyleMan/Add',
        method: 'post',
        data: data
    })
}

// 修改主题
export function updateStyleMan(data) {
    return request({
        url: '/AuthService/StyleMan/Edit',
        method: 'post',
        data: data
    })
}

// 删除主题
export function delStyleMan(styleId) {
    return request({
        url: '/AuthService/StyleMan/Remove?id=' + styleId,
        method: 'get'
    })
}


// 查询已分配或未分配该主题的企业列表
export function allStyleOrgList(query) {
    return request({
        url: '/AuthService/StyleMan/StyleOrgList',
        method: 'get',
        params: query
    })
}


// 给指定企业分配主题
export function setOrgStyle(data) {
    return request({
        url: '/AuthService/StyleMan/SetOrgStyle',
        method: 'post',
        params: data
    })
}

// 删除企业的指定主题
export function delOrgStyle(data) {
    return request({
        url: '/AuthService/StyleMan/DelOrgStyle',
        method: 'post',
        params: data
    })
}
export function orgStyle(query) {
    return request({
        url: '/AuthService/Style/OrgStyle',
        method: 'get',
        params: query
    })
}
export function orgStyleList(query) {
    return request({
        url: '/AuthService/Org/StyleList',
        method: 'get',
        params: query
    })
}
export function changeOrgStyle(query) {
    return request({
        url: '/AuthService/Org/ChangeStyle',
        method: 'get',
        params: query
    })
}
