import request from '@/utils/request'

// 查询报表列表
export function rptList(query) {
    return request({
        url: '/ReportService/Report/List',
        method: 'get',
        params: query
    })
}


// 查询报表列表分组
export function treeSelectList() {
    return request({
        url: '/ReportService/ReportGroup/ListTree',
        method: 'get'
    })
}

// 查询报表列表分组信息
export function treeSelectInfo(query) {
    return request({
        url: '/ReportService/ReportGroup/Info',
        method: 'get',
        params: query
    })
}

// 新增报表列表分组
export function treeSelectAdd(data) {
    return request({
        url: '/ReportService/ReportGroup/Add',
        method: 'post',
        data: data
    })
}

// 编辑报表列表分组
export function treeSelectEdit(data) {
    return request({
        url: '/ReportService/ReportGroup/Edit',
        method: 'post',
        data: data
    })
}

// 删除报表列表分组
export function treeSelectRemove(query) {
    return request({
        url: '/ReportService/ReportGroup/Remove',
        method: 'get',
        params: query
    })
}

// 查询报表信息
export function rptInfo(id) {
    return request({
        url: '/ReportService/Report/Info',
        method: 'get',
        params: {
            id
        }
    })
}

// 复制指定报表
export function rptCopy(id) {
    return request({
        url: '/ReportService/Report/Copy',
        method: 'get',
        params: {
            id
        }
    })
}

// 添加报表
export function addRpt(data) {
    return request({
        url: '/ReportService/Report/Add',
        method: 'post',
        data: data
    })
}

// 修改报表
export function editRpt(data) {
    return request({
        url: '/ReportService/Report/Edit',
        method: 'post',
        data: data
    })
}

// 删除指定报表
export function deleteRpt(id) {
    return request({
        url: '/ReportService/Report/Remove',
        method: 'get',
        params: {
            id
        }
    })
}


// 查询分享信息
export function shareInfo(id) {
    return request({
        url: '/ReportService/Share/Info',
        method: 'get',
        params: {
            id
        }
    })
}


export function checkShare(id, pwd) {
    return request({
        url: '/ReportService/Preview/CheckShare',
        method: 'get',
        params: {
            id: id,
            pwd: pwd
        }
    })
}
// 大屏更新
export function reportUpdateTime(query) {
    return request({
        url: '/ReportService/Preview/ReportUpdateTime',
        method: 'get',
        params: query
    })
}