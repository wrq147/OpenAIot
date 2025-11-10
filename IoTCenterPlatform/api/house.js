import request from '@/common/request.js'


// 仓库列表
export function houseList(query) {
    return request.get({
        url: '/CRMService/House/List',
        query:query
    })
}

// 添加仓库
export function addHouse(data) {
    return request.post({
        url: '/CRMService/House/Add',
        data: data
    })
}

// 修改仓库
export function editHouse(data) {
    return request.post({
        url: '/CRMService/House/Edit',
        data: data
    })
}

// 删除仓库
export function delHouse(id) {
    return request.get({
        url: '/CRMService/House/Remove',
        query:{id}
    });
}

// 获取仓库
export function houseInfo(id) {
    return request.get({
        url: '/CRMService/House/Info',
        query:{id}
    })
}

// 修改仓库状态
export function changeStatus(id,status) {
    return request.get({
        url: '/CRMService/House/ChangeStaus',
        query:{id,status}
    });
}
