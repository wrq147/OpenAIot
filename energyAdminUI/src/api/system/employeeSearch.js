import request from '@/utils/request'

// 查询组织架构树
export function getEmployeeTree(param) {
  return request({
    url: 'AuthService/Member/Tree',
    method: 'get',
    params: param
  })
}

// 搜索人员
export function getEmployeeUserByName(param) {
  return request({
    url: 'AuthService/Member/Search',
    method: 'get',
    params: param
  })
}


export default {
    getEmployeeTree, getEmployeeUserByName
}
