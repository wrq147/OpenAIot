import request from '@/utils/request'


// 仓库列表
export function houseList(query) {
    return request({
        url: '/StorageService/House/List',
        method: 'get',
        params:query
    })
}

// 添加仓库
export function addHouse(data) {
    return request({
        url: '/StorageService/House/Add',
        method: 'post',
        data: data
    })
}

// 修改仓库
export function editHouse(data) {
    return request({
        url: '/StorageService/House/Edit',
        method: 'post',
        data: data
    })
}

// 删除仓库
export function delHouse(id) {
    return request({
        url: '/StorageService/House/Remove',
        method: 'get',
        params:{id}
    });
}

// 获取仓库
export function houseInfo(id,showTemplateName) {
    return request({
        url: '/StorageService/House/Info',
        method: 'get',
        params:{id,showTemplateName}
    })
}

// 修改仓库状态
export function changeStatus(id,status) {
    return request({
        url: '/StorageService/House/ChangeStaus',
        method: 'get',
        params:{id,status}
    });
}
