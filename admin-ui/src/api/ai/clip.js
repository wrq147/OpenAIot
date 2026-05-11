import request from '@/utils/request'


export function generateFeature(data) {
    return request({
        url: '/IoTAIService/Clip/GenerateFeature',
        method: 'post',
        data: data
    })
}

export function generateImgFeature(data) {
    return request({
        url: '/IoTAIService/Clip/GenerateImageFeature',
        method: 'post',
        data: data
    })
}