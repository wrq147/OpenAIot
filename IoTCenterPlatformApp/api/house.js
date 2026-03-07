import request from '@/common/request.js'


// 仓库列表
export function houseList(query) {
    return request.get({
        url: '/StorageService/House/List',
        query:query
    })
}

// 添加仓库
export function addHouse(data) {
    return request.post({
        url: '/StorageService/House/Add',
        data: data
    })
}

// 修改仓库
export function editHouse(data) {
    return request.post({
        url: '/StorageService/House/Edit',
        data: data
    })
}

// 删除仓库
export function delHouse(id) {
    return request.get({
        url: '/StorageService/House/Remove',
        query:{id}
    });
}

// 获取仓库
export function houseInfo(id,showTemplateName) {
    return request.get({
        url: '/StorageService/House/Info',
        query:{id,showTemplateName}
    })
}

// 修改仓库状态
export function changeStatus(id,status) {
    return request.get({
        url: '/StorageService/House/ChangeStaus',
        query:{id,status}
    });
}
