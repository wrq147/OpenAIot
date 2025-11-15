import request from '@/utils/request'

// 查询设备基本
export function deviceStatistics(query) {
    return request({
        url: '/IoTService/IotReport/StatisticsInfo',
        method: 'get',
        params: query
    })
}
//获取报警列表
export function warningList(query) {
    return request({
        url: '/IoTService/IotWarning/ListPage',
        method: 'get',
        params: query
    })
}

//获取客户统计数据
export function crmStatisticsInfo(query) {
    return request({
        url: '/CRMService/CrmReport/StatisticsInfo',
        method: 'get',
        params: query
    })
}


//获取销售漏斗数据
export function periodCountList(query) {
    return request({
        url: '/CRMService/CrmReport/PeriodCountList',
        method: 'get',
        params: query
    })
}


//获取流程信息
export function followStatisticsInfo(query) {
    return request({
        url: '/FlowService/FlowReport/StatisticsInfo',
        method: 'get',
        params: query
    })
}

//获取库存统计信息
export function StockStatisticsInfo(query) {
    return request({
        url: '/StorageService/StReport/StockInfo',
        method: 'get',
        params: query
    })
}
//获取库存出入口记录
export function StockDetailRecords(query) {
    return request({
        url: '/StorageService/StReport/DetailRecords',
        method: 'get',
        params: query
    })
}
//获取本人操作日志
export function personOperLogList(query) {
    return request({
        url: 'MonitorService/OperLog/PersonList',
        method: 'get',
        params: query
    })
}
// 设备统计
export function homeDevStatisticsInfo(data) {
    return request({
        url: '/EfficiencyService/Production/StatisticsInfo',
        method: 'post',
        data: data
    })
}
// 故障告警统计
export function homeWarningListPage(query) {
    return request({
        url: '/EfficiencyService/Production/WarningListPage',
        method: 'get',
        params: query
    })
}
// 能效指标
export function selectProductEnergyPage(data) {
    return request({
        url: '/EfficiencyService/Common/SelectProductEnergyPage',
        method: 'post',
        data: data
    })
}