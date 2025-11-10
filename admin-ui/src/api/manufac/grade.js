import request from '@/utils/request'

// 获取级别列表
export function gradeList() {
    return request({
        url: '/ProducerService/Grade/List',
        method: 'get'
    })
}

// 添加级别
export function addGrade(data) {
    return request({
        url: '/ProducerService/Grade/Add',
        method: 'post',
        data: data
    })
}

// 编辑级别
export function editGrade(data) {
    return request({
        url: '/ProducerService/Grade/Edit',
        method: 'post',
        data: data
    })
}


// 级别排序
export function gradeSort(data) {
    return request({
        url: '/ProducerService/Grade/Sort',
        method: 'post',
        data: data
    })
}

// 删除级别
export function delGrade(id) {
    return request({
        url: '/ProducerService/Grade/Remove',
        method: 'get',
        params:{id}
    });
}
