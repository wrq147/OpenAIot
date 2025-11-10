import request from '@/utils/request'

// 查询部门列表
export function listDev(query) {
  return request({
    url: '/DeveloperService/Developer/ListPage',
    method: 'get',
    params: query
  })
}
//获取开发者详情
export function devInfo(id) {
  return request({
      url: '/DeveloperService/Developer/Info',
      method: 'get',
      params: {id}
  })
}


// 新增部门
export function addDev(data) {
  return request({
    url: '/DeveloperService/Developer/Add',
    method: 'post',
    data: data
  })
}

// 修改部门
export function updateDev(data) {
  return request({
    url: '/DeveloperService/Developer/Edit',
    method: 'post',
    data: data
  })
}

// 删除部门
export function delDev(id) {
  return request({
    url: '/DeveloperService/Developer/Remove/' + id,
    method: 'get'
  })
}



//获取当前开发者信息
export function devProfile() {
  return request({
      url: '/DeveloperService/Center/Profile',
      method: 'get'
  })
}
