import request from '@/utils/request'


// 获取生产商列表
export function factoryList(data) {
    return request({
        url: '/ProducerService/Factory/List',
        method: 'get',
        params:data
    })
}

// 添加新生产商
export function addFactory(data) {
    return request({
        url: '/ProducerService/Factory/Add',
        method: 'post',
        data: data
    })
}

// 获取生产商信息
export function factoryInfo(id) {
    return request({
        url: '/ProducerService/Factory/Info',
        method: 'get',
        params:{id}
    })
}

// 删除生产商
export function delFactory(id) {
    return request({
        url: '/ProducerService/Factory/Remove',
        method: 'get',
        params:{id}
    });
}

// 生产商配置专用
export function setFactoryConfig(data) {
    return request({
        url: '/ProducerService/Factory/Set',
        method: 'post',
        data: data
    })
}