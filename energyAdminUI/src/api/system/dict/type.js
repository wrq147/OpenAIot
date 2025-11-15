import request from '@/utils/request'
import download from '@/plugins/download'
// 查询字典类型列表
export function listType(query) {
  return request({
    url: '/DictService/DictType/List',
    method: 'get',
    params: query
  })
}

// 查询字典类型详细
export function getType(dictId) {
  return request({
    url: '/DictService/DictType/Info/' + dictId,
    method: 'get'
  })
}

// 新增字典类型
export function addType(data) {
  return request({
    url: '/DictService/DictType/Add',
    method: 'post',
    data: data
  })
}

// 修改字典类型
export function updateType(data) {
  return request({
    url: '/DictService/DictType/Edit',
    method: 'post',
    data: data
  })
}

// 删除字典类型
export function delType(dictId) {
  return request({
    url: '/DictService/DictType/Remove/' + dictId,
    method: 'get'
  })
}

// 刷新字典缓存
export function refreshCache() {
  return request({
    url: '/DictService/DictType/RefreshCache',
    method: 'get'
  })
}

// 导出字典类型
export function exportType(query) {
  return download.resource('/DictService/DictType/Export', query);
}

// 获取字典选择框列表
export function optionselect() {
  return request({
    url: '/DictService/DictType/OptionSelect',
    method: 'get'
  })
}