import request from '@/utils/request'

// 获取生产商MES配置信息
export function factoryMesConfig() {
    return request({
        url: '/MESService/Config/Info',
        method: 'get'
    })
}
// 设置生产商MES配置信息
export function saveFactoryMesConfig(data) {
    return request({
        url: '/MESService/Config/Set',
        method: 'post',
        data: data
    })
}