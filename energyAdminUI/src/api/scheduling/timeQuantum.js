import request from '@/utils/request'


// 获取时间段列表
export function timeQuantumList(data) {
    return request({
        url: '/PaiBanService/ShiJianDuan/List',
        method: 'post',
        data: data
    })
}
// 添加时间段
export function addTimeQuantum(data) {
    return request({
        url: '/PaiBanService/ShiJianDuan/Add',
        method: 'post',
        data: data
    })
}

// 编辑时间段
export function editTimeQuantum(data) {
    return request({
        url: '/PaiBanService/ShiJianDuan/Edit',
        method: 'post',
        data: data
    })
}

// 删除时间段
export function delTimeQuantum(data) {
    return request({
        url: '/PaiBanService/ShiJianDuan/Remove',
        method: 'post',
        data: data
    });
}