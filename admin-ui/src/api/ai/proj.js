import request from '@/utils/request'

export function drawBoxs(data) {
    return request({
        url: '/IoTAIService/Project/DrawBoxs',
        method: 'post',
        data: data
    })
}