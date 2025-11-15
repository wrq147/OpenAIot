import request from '@/utils/request'


// 获取班次列表
export function classesList(data) {
    return request({
        url: '/PaiBanService/BanCi/List',
        method: 'post',
        data: data
    })
}

// 获取周期单位列表
export function classesPeriod(data) {
    return request({
        url: '/PaiBanService/BanCi/ZhouQiDanWei',
        method: 'post',
        data: data
    })
}

// 获取周期列表
export function periodList(data) {
    return request({
        url: '/PaiBanService/BanCi/ZhouQiList',
        method: 'post',
        data: data
    })
}

// 设置班次时段
export function editClassesTime(data) {
    return request({
        url: '/PaiBanService/BanCi/EditBanCiShiDuan',
        method: 'post',
        data: data
    })
}

// 设置班次时段
export function editClassesTimeMore(data) {
    return request({
        url: '/PaiBanService/BanCi/EditBanCiShiDuans',
        method: 'post',
        data: data
    })
}

// 添加班次
export function addClasses(data) {
    return request({
        url: '/PaiBanService/BanCi/AddBanCi',
        method: 'post',
        data: data
    })
}

// 编辑班次
export function editClasses(data) {
    return request({
        url: '/PaiBanService/BanCi/EditBanCi',
        method: 'post',
        data: data
    })
}

// 删除班次
export function delClasses(data) {
    return request({
        url: '/PaiBanService/BanCi/RemoveBanci',
        method: 'post',
        data: data
    });
}