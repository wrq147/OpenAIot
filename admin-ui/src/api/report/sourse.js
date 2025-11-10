import request from '@/utils/request'

// 数据源信息列表
export function listSourse(query) {
    return request({
        url: '/ReportService/DataSource/List',
        method: 'get',
        params: query
    })
}

// 数据库数据源列表
export function TableNames(query) {
    return request({
        url: '/ReportService/DataSource/TableNames',
        method: 'get',
        params: query
    })
}

// 数据库数据源列表
export function AllTableStruct(query) {
    return request({
        url: '/ReportService/DataSource/AllTableStruct',
        method: 'get',
        params: query
    })
}

// 数据库数据源子表结构
export function TableStruct(query) {
    return request({
        url: '/ReportService/DataSource/TableStruct',
        method: 'get',
        params: query
    })
}

// 查询数据源详细
export function getSourse(id) {
    return request({
        url: '/ReportService/DataSource/Info/' + id,
        method: 'get'
    })
}

// 新增数据源
export function addSourse(data) {
    return request({
        url: '/ReportService/DataSource/Add',
        method: 'post',
        data: data
    })
}

// 修改数据源信息
export function updateSourse(data) {
    return request({
        url: '/ReportService/DataSource/Edit',
        method: 'post',
        data: data
    })
}

// 删除数据源信息
export function delSourse(params) {
    return request({
        url: '/ReportService/DataSource/Remove',
        method: 'get',
        params
    })
}

// 解释执行数据源
export function chartBIanalysis(query) {
    return request({
        url: '/ReportService/DataSource/Analysis',
        method: 'post',
        data: query
    })
}

// 验证数据源是否正确
export function showDatabases(query) {
    return request({
        url: '/ReportService/DataSource/ShowDatabases',
        method: 'get',
        params: query
    })
}

// 用于初始化验证数据源(数据库)
export function GetDataSourceByIds(data) {
    return request({
        url: '/ReportService/DataSource/GetDataSourceByIds',
        method: 'post',
        data: data
    })
}

// 用于初始化验证数据源(api)
export function GetApiSourceByIds(query) {
    return request({
        url: '/ReportService/DataSource/GetApiSourceByIds',
        method: 'post',
        data: data
    })
}