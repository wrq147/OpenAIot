import request from '@/utils/request'

export function listColumn(query) {
  return request({
    url: '/LLMService/KbColumn/List',
    method: 'get',
    params: query
  })
}

export function getColumn(id) {
  return request({
    url: `/LLMService/KbColumn/${id}`,
    method: 'get'
  })
}

export function getColumnsByKbId(kbId) {
  return request({
    url: `/LLMService/KbColumn/KbId/${kbId}`,
    method: 'get'
  })
}

export function getColumnsByParentId(parentId) {
  return request({
    url: `/LLMService/KbColumn/ParentId/${parentId}`,
    method: 'get'
  })
}

export function addColumn(data) {
  return request({
    url: '/LLMService/KbColumn/Add',
    method: 'post',
    data: data
  })
}

export function updateColumn(data) {
  return request({
    url: '/LLMService/KbColumn/Update',
    method: 'put',
    data: data
  })
}

export function deleteColumn(id) {
  return request({
    url: `/LLMService/KbColumn/Delete/${id}`,
    method: 'delete'
  })
}

export function changeColumnStatus(id, status) {
  return request({
    url: `/LLMService/KbColumn/${id}/status`,
    method: 'post',
    data: status
  })
}