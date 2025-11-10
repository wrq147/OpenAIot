import request from '@/utils/request'

// 查询部门列表
export function listDept(query) {
  return request({
    url: '/AuthService/Dept/List',
    method: 'get',
    params: query
  })
}

// 查询部门列表（排除节点）
export function listDeptExcludeChild(deptId) {
  return request({
    url: '/AuthService/Dept/ExcludeChild/' + deptId,
    method: 'get'
  })
}

// 查询部门详细
export function getDept(deptId) {
  return request({
    url: '/AuthService/Dept/Info/' + deptId,
    method: 'get'
  })
}

// 查询部门下拉树结构
export function treeselect(query) {
  return request({
    url: '/AuthService/Dept/TreeSelect',
    method: 'get',
    params:query
  })
}



// 新增部门
export function addDept(data) {
  return request({
    url: '/AuthService/Dept/Add',
    method: 'post',
    data: data
  })
}

// 修改部门
export function updateDept(data) {
  return request({
    url: '/AuthService/Dept/Edit',
    method: 'post',
    data: data
  })
}

// 删除部门
export function delDept(deptId) {
  return request({
    url: '/AuthService/Dept/Remove/' + deptId,
    method: 'get'
  })
}

// 单个设置部门负责人
export function setLeader(data) {
  return request({
    url: '/AuthService/Dept/SetLeader',
    method: 'post',
    data: data
  })
}

// 移动部门
export function DeptMove(data) {
  return request({
    url: '/AuthService/Dept/Move',
    method: 'post',
    data: data
  })
}