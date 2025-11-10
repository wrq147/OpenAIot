import request from '@/common/request.js'

// 获取耗材列表
export function partsList(data) {
    return request.get({
        url: '/CRMService/Parts/List',
        query:data
    })
}

//添加耗材
export function addParts(data) {
    return request.post({
        url: '/CRMService/Parts/Add',
        data: data
    })
}

//编辑耗材
export function editParts(data) {
    return request.post({
        url: '/CRMService/Parts/Edit',
        data: data
    })
}


// 删除耗材
export function delParts(id) {
    return request.get({
        url: '/CRMService/Parts/Remove',
        query:{id}
    });
}

// 获取耗材信息
export function partsInfo(id) {
    return request.get({
        url: '/CRMService/Parts/Info',
        query:{id}
    })
}


// 生成配件编码
export function generatePartsNumber(){
    return request.get({
        url:'/CRMService/Parts/GenerateNumber',
    })
}