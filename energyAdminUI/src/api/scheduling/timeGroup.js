import request from '@/utils/request'


// 获取班组列表
export function groupList(data) {
    return request({
        url: '/PaiBanService/BanZu/List',
        method: 'post',
        data: data
    })
}

// 添加班组成员
export function addGroupPeople(data) {
    return request({
        url: '/PaiBanService/BanZu/AddBanZuChengYuan',
        method: 'post',
        data: data
    })
}


// 添加班组
export function addGroup(data) {
    return request({
        url: '/PaiBanService/BanZu/AddBanZu',
        method: 'post',
        data: data
    })
}

// 编辑班组
export function editGroup(data) {
    return request({
        url: '/PaiBanService/BanZu/EditBanZu',
        method: 'post',
        data: data
    })
}

// 删除班组
export function delGroup(data) {
    return request({
        url: '/PaiBanService/BanZu/RemoveBanZu',
        method: 'post',
        data: data
    });
}

// 删除班组人员
export function delGroupPeople(data) {
    return request({
        url: '/PaiBanService/BanZu/RemoveChengYuan',
        method: 'post',
        data: data
    });
}