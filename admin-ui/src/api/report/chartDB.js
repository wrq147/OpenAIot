import request from '@/utils/request'

/**
 * 数据库测试链接接口
 * @param {object} database 
 */
export function chartDBConn(database) {
    return request({
        url: '/chart/DB/testConnection',
        method: 'post',
        data: database
    })
}

/**
 * 获取数据库下所有表
 * @param {object} query 
 */
export function getAllTable(query) {
    return request({
        url: '/chart/DB/getAllTable',
        method: 'post',
        data: query
    })
}

/**
 * 获取数据表下所有字段
 * @param {object} query 
 */
export function getAllField(query) {
    return request({
        url: '/chart/DB/getAllField',
        method: 'post',
        data: query
    })
}

/**
 * 获取字段下的所有统计数的筛选项
 * @param {object} query 
 * @returns 
 */
export function getDistinctOneField(query) {
    return request({
        url: '/chart/DB/getDistinctOneField',
        method: 'post',
        data: query
    })
}