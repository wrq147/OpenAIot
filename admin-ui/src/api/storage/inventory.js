import request from '@/utils/request'

// 盘点列表
export function invList(query) {
    return request({
        url: '/StorageService/Inventory/List',
        method: 'get',
        params: query
    })
}
// 盘点任务
export function invTask(query) {
    return request({
        url: '/StorageService/Inventory/Task',
        method: 'get',
        params: query
    })
}

//记录盘点数据
export function confirmItem(id, number, count) {
    return request({
        url: '/StorageService/Inventory/ConfirmItem',
        method: 'post',
        data: { id, number, count }
    })
}

// 盘点项列表
export function invItemList(query) {
    return request({
        url: '/StorageService/Inventory/ItemList',
        method: 'get',
        params: query
    })
}


// 获取盘点
export function invInfo(id) {
    return request({
        url: '/StorageService/Inventory/Info',
        method: 'get',
        params: { id }
    })
}


// 获取盘点项信息
export function itemInfo(id, number) {
    return request({
        url: '/StorageService/Inventory/ItemInfo',
        method: 'get',
        params: { id, number }
    })
}

// 添加盘点
export function addInv(data) {
    return request({
        url: '/StorageService/Inventory/Add',
        method: 'post',
        data: data
    })
}

// 修改盘点
export function editInv(data) {
    return request({
        url: '/StorageService/Inventory/Edit',
        method: 'post',
        data: data
    })
}

// 提交盘点
export function submitInv(id) {
    return request({
        url: '/StorageService/Inventory/Submit',
        method: 'post',
        data: { id }
    })
}

// 开始盘点单
export function startInv(ids) {
    return request({
        url: '/StorageService/Inventory/Start',
        method: 'post',
        data: ids
    })
}

// 开始盘点单
export function checkInv(ids) {
    return request({
        url: '/StorageService/Inventory/Check',
        method: 'post',
        data: { ids }
    })
}

// 完成盘点单
export function finishInv(ids) {
    return request({
        url: '/StorageService/Inventory/Finish',
        method: 'post',
        data: ids
    })
}

// 修正盘点单
export function repairInv(id, items) {
    return request({
        url: '/StorageService/Inventory/Repair',
        method: 'post',
        data: { id, items }
    })
}


// 取消盘点单
export function cancelInv(ids) {
    return request({
        url: '/StorageService/Inventory/Cancel',
        method: 'post',
        data: ids
    })
}

// 删除盘点
export function delInv(ids) {
    return request({
        url: '/StorageService/Inventory/Remove',
        method: 'post',
        data: ids
    })
}

//删除指定盘点项
export function delItems(id, ids) {
    return request({
        url: '/StorageService/Inventory/DelItems',
        method: 'post',
        data: { id, ids }
    })
}

//添加盘点项
export function addItems(id, list) {
    return request({
        url: '/StorageService/Inventory/AddItems',
        method: 'post',
        data: { id, list }
    })
}

//添加全部
export function addItemsAll(data) {
    return request({
        url: '/StorageService/Inventory/AddItemsAll',
        method: 'post',
        data: data
    })
}

//移除全部
export function delItemsAll(id) {
    return request({
        url: '/StorageService/Inventory/DelItemsAll',
        method: 'post',
        data: { id }
    })
}