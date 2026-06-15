import request from '@/utils/request'

export function listColumn(kbId) {
  return request({
    url: '/LLMService/KbColumn/ListTree',
    method: 'get',
    params: {kbId}
  })
}

export function sortColumn(data) {
  return request({
    url: '/LLMService/KbColumn/Sort',
    method: 'post',
    data: data
  })
}

export function getColumn(id) {
  return request({
    url: `/LLMService/KbColumn/Info`,
    method: 'get',
    params: {id}
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
    method: 'post',
    data: data
  })
}

export function deleteColumn(id) {
  return request({
    url: `/LLMService/KbColumn/Delete`,
    method: 'get',
    params: {id}
  })
}

