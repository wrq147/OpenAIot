import request from '@/utils/request'

//人脸库列表
export function getFaceHouseList(data){
    return request({
        url: '/IoTAIService/Face/HouseList',
        method: 'get',
        params: data
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