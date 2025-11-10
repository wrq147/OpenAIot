import request from '@/common/request.js'
// 盘点列表
export function invList(query) {
    return request.get({
        url: '/CRMService/Inventory/List',
        query: query
    })
}
// 盘点任务
export function invTask(query) {
    return request.get({
        url: '/CRMService/Inventory/Task',
        query: query
    })
}

//记录盘点数据
export function confirmItem(id, number, count) {
    return request.post({
        url: '/CRMService/Inventory/ConfirmItem',
        data: { id, number, count }
    })
}

// 盘点项列表
export function invItemList(query) {
    return request.get({
        url: '/CRMService/Inventory/ItemList',
        query: query
    })
}


// 获取盘点
export function invInfo(id) {
    return request.get({
        url: '/CRMService/Inventory/Info',
        query: { id }
    })
}


// 获取盘点项信息
export function itemInfo(id, number) {
    return request.get({
        url: '/CRMService/Inventory/ItemInfo',
        query: { id, number }
    })
}

// 添加盘点
export function addInv(data) {
    return request.post({
        url: '/CRMService/Inventory/Add',
        data: data
    })
}

// 修改盘点
export function editInv(data) {
    return request.post({
        url: '/CRMService/Inventory/Edit',
        data: data
    })
}

// 提交盘点
export function submitInv(id) {
    return request.post({
        url: '/CRMService/Inventory/Submit',
        data: { id }
    })
}

// 开始盘点单
export function startInv(ids) {
    return request.post({
        url: '/CRMService/Inventory/Start',
        data: ids
    })
}

// 开始盘点单
export function checkInv(ids) {
    return request.post({
        url: '/CRMService/Inventory/Check',
        data: { ids }
    })
}

// 完成盘点单
export function finishInv(ids) {
    return request.post({
        url: '/CRMService/Inventory/Finish',
        data: ids
    })
}

// 修正盘点单
export function repairInv(id, items) {
    return request.post({
        url: '/CRMService/Inventory/Repair',
        data: { id, items }
    })
}


// 取消盘点单
export function cancelInv(ids) {
    return request.post({
        url: '/CRMService/Inventory/Cancel',
        data: ids
    })
}

// 删除盘点
export function delInv(ids) {
    return request.post({
        url: '/CRMService/Inventory/Remove',
        data: ids
    })
}

//删除指定盘点项
export function delItems(id, ids) {
    return request.post({
        url: '/CRMService/Inventory/DelItems',
        data: { id, ids }
    })
}

//添加盘点项
export function addItems(id, list) {
    return request.post({
        url: '/CRMService/Inventory/AddItems',
        data: { id, list }
    })
}

//添加全部
export function addItemsAll(data) {
    return request.post({
        url: '/CRMService/Inventory/AddItemsAll',
        data: data
    })
}

//移除全部
export function delItemsAll(id) {
    return request.post({
        url: '/CRMService/Inventory/DelItemsAll',
        data: { id }
    })
}