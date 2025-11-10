import request from '@/common/request.js'
// 盘点列表
export function invList(query) {
    return request.get({
        url: '/StorageService/Inventory/List',
        query: query
    })
}
// 盘点任务
export function invTask(query) {
    return request.get({
        url: '/StorageService/Inventory/Task',
        query: query
    })
}

//记录盘点数据
export function confirmItem(id, number, count) {
    return request.post({
        url: '/StorageService/Inventory/ConfirmItem',
        data: { id, number, count }
    })
}

// 盘点项列表
export function invItemList(query) {
    return request.get({
        url: '/StorageService/Inventory/ItemList',
        query: query
    })
}


// 获取盘点
export function invInfo(id) {
    return request.get({
        url: '/StorageService/Inventory/Info',
        query: { id }
    })
}


// 获取盘点项信息
export function itemInfo(id, number) {
    return request.get({
        url: '/StorageService/Inventory/ItemInfo',
        query: { id, number }
    })
}

// 添加盘点
export function addInv(data) {
    return request.post({
        url: '/StorageService/Inventory/Add',
        data: data
    })
}

// 修改盘点
export function editInv(data) {
    return request.post({
        url: '/StorageService/Inventory/Edit',
        data: data
    })
}

// 提交盘点
export function submitInv(id) {
    return request.post({
        url: '/StorageService/Inventory/Submit',
        data: { id }
    })
}

// 开始盘点单
export function startInv(ids) {
    return request.post({
        url: '/StorageService/Inventory/Start',
        data: ids
    })
}

// 开始盘点单
export function checkInv(ids) {
    return request.post({
        url: '/StorageService/Inventory/Check',
        data: { ids }
    })
}

// 完成盘点单
export function finishInv(ids) {
    return request.post({
        url: '/StorageService/Inventory/Finish',
        data: ids
    })
}

// 修正盘点单
export function repairInv(id, items) {
    return request.post({
        url: '/StorageService/Inventory/Repair',
        data: { id, items }
    })
}


// 取消盘点单
export function cancelInv(ids) {
    return request.post({
        url: '/StorageService/Inventory/Cancel',
        data: ids
    })
}

// 删除盘点
export function delInv(ids) {
    return request.post({
        url: '/StorageService/Inventory/Remove',
        data: ids
    })
}

//删除指定盘点项
export function delItems(id, ids) {
    return request.post({
        url: '/StorageService/Inventory/DelItems',
        data: { id, ids }
    })
}

//添加盘点项
export function addItems(id, list) {
    return request.post({
        url: '/StorageService/Inventory/AddItems',
        data: { id, list }
    })
}

//添加全部
export function addItemsAll(data) {
    return request.post({
        url: '/StorageService/Inventory/AddItemsAll',
        data: data
    })
}

//移除全部
export function delItemsAll(id) {
    return request.post({
        url: '/StorageService/Inventory/DelItemsAll',
        data: { id }
    })
}