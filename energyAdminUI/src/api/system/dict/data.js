import request from '@/utils/request'
import download from '@/plugins/download'
// 查询字典数据列表
export function listData(query) {
  return request({
    url: '/DictService/DictData/List',
    method: 'get',
    params: query
  })
}

// 查询字典数据详细
export function getData(dictCode) {
  return request({
    url: '/DictService/DictData/Info/' + dictCode,
    method: 'get'
  })
}

// 根据字典类型查询字典数据信息
export function getDicts(dictType) {
  return request({
    url: '/DictService/DictData/DictType/' + dictType,
    method: 'get'
  })
}

// 新增字典数据
export function addData(data) {
  return request({
    url: '/DictService/DictData/Add',
    method: 'post',
    data: data
  })
}

// 修改字典数据
export function updateData(data) {
  return request({
    url: '/DictService/DictData/Edit',
    method: 'post',
    data: data
  })
}

// 删除字典数据
export function delData(dictCode) {
  return request({
    url: '/DictService/DictData/Remove/' + dictCode,
    method: 'get'
  })
}

// 导出字典数据
export function exportData(query) {
  return download.resource('/DictService/DictData/Export', query);
}

//导出默认设置
export function setDefault(dictCode){
  return request({
    url: '/DictService/DictData/Default/' + dictCode,
    method: 'get'
  })
}