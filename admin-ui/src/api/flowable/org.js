import request from '@/utils/request'

// 查询组织架构树
export function getOrgTree(param) {
  return request({
    url: '/FlowService/Org/Tree',
    method: 'get',
    params: param
  })
}

// 搜索人员
export function getUserByName(param) {
  return request({
    url: '/FlowService/Org/Search',
    method: 'get',
    params: param
  })
}


export default {
  getOrgTree, getUserByName
}
