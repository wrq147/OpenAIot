import request from '@/utils/request'

export function noticeCalProp(time){
    return request({
        url: '/IoTService/IotData/NoticeCalProp',
        method: 'get',
        params: {Time:time}
    })
}

export function copyData(data){
    return request({
        url: '/IoTService/IotData/CopyData',
        method: 'post',
        data: data
    })
}