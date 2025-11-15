import request from '@/utils/request'


// 获取设备排班表信息
export function workScheduleInfo(data) {
    return request({
        url: '/PaiBanService/SheBeiBanCi/Info',
        method: 'post',
        data: data
    })
}
// 获取设备排班表日历
export function workScheduleCalculate(data) {
    return request({
        url: '/PaiBanService/SheBeiBanCi/Calculate',
        method: 'post',
        data: data
    })
}

// 编辑设备排班
export function editWorkSchedule(data) {
    return request({
        url: '/PaiBanService/SheBeiBanCi/Edit',
        method: 'post',
        data: data
    })
}
// 批量编辑排班
export function editWorkScheduleMore(data) {
    return request({
        url: '/PaiBanService/SheBeiBanCi/EditMany',
        method: 'post',
        data: data
    })
}

// 删除排班
export function delWorkSchedule(data) {
    return request({
        url: '/PaiBanService/SheBeiBanCi/Remove',
        method: 'post',
        data: data
    });
}

