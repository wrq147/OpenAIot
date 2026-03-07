import request from '@/common/request.js'
// 根据参数键名查询参数值
export function getConfigKey(configKey) {
    return request.get({
        url: '/AuthService/Code/GetByKey/' + configKey,
    })
}
// 获取Stock设置
export function getStockConfig(id) {
    return request.get({
        url: '/CRMService/Config/StockInfo',
        query: { id }
    })
}