import request from '@/utils/request'


// CRM设置
export function setCrmConfig(data) {
    return request({
        url: '/CRMService/Config/SetCrm',
        method: 'post',
        data: data
    })
}

// 获取CRM设置
export function getCrmConfig(id) {
    return request({
        url: '/CRMService/Config/CrmInfo',
        method: 'get',
        params: { id }
    })
}


//添加阶段信息
export function addPeriod(data) {
    return request({
        url: '/CRMService/Period/Add',
        method: 'post',
        data: data
    })
}
//修改阶段信息
export function editPeriod(data) {
    return request({
        url: '/CRMService/Period/Edit',
        method: 'post',
        data: data
    })
}

//删除阶段信息
export function removePeriod(id) {
    return request({
        url: '/CRMService/Period/remove',
        method: 'get',
        params: { id }
    })
}