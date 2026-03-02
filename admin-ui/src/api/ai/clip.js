import request from '@/utils/request'


export function generateFeature(data) {
    return request({
        url: '/IoTAIService/Clip/GenerateFeature',
        method: 'post',
        data: data
    })
}