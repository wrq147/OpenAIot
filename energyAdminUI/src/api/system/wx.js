import request from '@/utils/request'

// 查询微信应用列表
export function listWx() {
    return request({
      url: '/WeiXinService/Account/List',
      method: 'get'
    })
}

// 添加微信应用
export function AddWx(data) {
    return request({
      url: '/WeiXinService/Account/Add',
      method: 'post',
      data: data
    })
}
  
// 删除微信应用
export function delWx(appid) {
    return request({
        url: '/WeiXinService/Account/Remove',
        method: 'get',
        params: {appid}
    })
}
