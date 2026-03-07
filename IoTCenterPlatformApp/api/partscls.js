import request from '@/common/request.js'

// 获取耗材分类树
export function classTree() {
    return request({
        url: '/ProducerService/PartsClass/ListTree',
        method: 'get',
    })
}
// 获取在指定分类信息
export function classInfo(query) {
    return request.get({
        url: '/ProducerService/PartsClass/Info',
        query: query
    })
}
// 删除分类
export function removeClass(query) {
    return request.get({
        url: '/ProducerService/PartsClass/Remove',
        query: query
    })
}

//添加耗材分类
export function addClass(data) {
    return request.post({
        url: '/ProducerService/PartsClass/Add',
        data: data
    })
}
//耗材分类排序
export function classSort(data) {
    return request.post({
        url: '/ProducerService/PartsClass/Sort',
        data: data
    })
}
//编辑耗材分类
export function editClass(data) {
    return request.post({
        url: '/ProducerService/PartsClass/Edit',
        data: data
    })
}