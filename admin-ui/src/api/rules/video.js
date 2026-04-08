import request from '@/utils/request'

export function videoSourceList(query) {
    return request({
        url: '/IoTVideoService/Source/ListPage',
        method: 'get',
        params: query
    })
}



export function addVideoSource(data) {
    return request({
        url: '/IoTVideoService/Source/Add',
        method: 'post',
        data: data
    })
}

export function editVideoSource(data) {
    return request({
        url: '/IoTVideoService/Source/Edit',
        method: 'post',
        data: data
    })
}

export function getVideoDetail(query) {
    return request({
        url: '/IoTVideoService/Source/Info',
        method: 'get',
        params: query
    })
}

export function removeVideoSource(query) {
    return request({
        url: '/IoTVideoService/Source/Remove',
        method: 'get',
        params: query
    })
}

export function getAIProjectList() {
    return request({
        url: '/IoTVideoService/Source/AIProjectList',
        method: 'get'
    })
}


export function getPresetList(sid) {
    return request({
        url: '/IoTVideoService/Ptz/GetPresetList',
        method: 'get',
        params: { sid }
    })
}

export function getPlayUrl(sid, cid, type) {
    return request({
        url: '/IoTVideoService/Ptz/GetPlayUrl',
        method: 'get',
        params: { "sid": sid, "cid": cid, "type": type }
    })
}


export function getChannelList(sid) {
    return request({
        url: '/IoTVideoService/Ptz/GetChannelList',
        method: 'get',
        params: { "sid": sid }
    })
}

export function controlPTZ(data) {
    return request({
        url: '/IoTVideoService/Ptz/ControlPTZ',
        method: 'post',
        data: data
    })
}


export function videoConfigList(query) {
    return request({
        url: '/IoTVideoService/Conf/ListPage',
        method: 'get',
        params: query
    })
}

export function addVideoConfig(data) {
    return request({
        url: '/IoTVideoService/Conf/Add',
        method: 'post',
        data: data
    })
}

export function editVideoConfig(data) {
    return request({
        url: '/IoTVideoService/Conf/Edit',
        method: 'post',
        data: data
    })
}

export function getVideoConfig(query) {
    return request({
        url: '/IoTVideoService/Conf/Info',
        method: 'get',
        params: query
    })
}

export function removeVideoConfig(query) {
    return request({
        url: '/IoTVideoService/Conf/Remove',
        method: 'get',
        params: query
    })
}