import request from '@/utils/request'

//人脸库列表
export function getFaceHouseList(data){
    return request({
        url: '/IoTAIService/Face/HouseList',
        method: 'get',
        params: data
    })
}

//获取人脸库信息
export function getHouseInfo(id) {
    return request({
        url: '/IoTAIService/Face/HouseInfo',
        method: 'get',
        params: {id}
    })
}

//添加人脸库
export function addFaceHouse(data) {
    return request({
        url: '/IoTAIService/Face/AddHouse',
        method: 'post',
        data: data
    })
}

//编辑人脸库
export function updateFaceHouse(data) {
    return request({
        url: '/IoTAIService/Face/EditHouse',
        method: 'post',
        data: data
    })
}

//删除人脸库
export function removeFaceHouse(query) {
    return request({
        url: '/IoTAIService/Face/RemoveHouse',
        method: 'get',
        params: query
    })
}

//添加人脸
export function addFace(data) {
    return request({
        url: '/IoTAIService/Face/AddFace',
        method: 'post',
        data: data
    })
}