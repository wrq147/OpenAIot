import request from '@/utils/request'

export function listKnowledge(query) {
  return request({
    url: '/LLMService/Knowledge/List',
    method: 'get',
    params: query
  })
}

export function getKnowledge(id) {
  return request({
    url: `/LLMService/Knowledge/Info`,
    method: 'get',
    params: {id}
  })
}

export function addKnowledge(data) {
  return request({
    url: '/LLMService/Knowledge/Add',
    method: 'post',
    data: data
  })
}

export function updateKnowledge(data) {
  return request({
    url: '/LLMService/Knowledge/Update',
    method: 'post',
    data: data
  })
}

export function deleteKnowledge(id) {
  return request({
    url: `/LLMService/Knowledge/Delete`,
    method: 'get',
    params: {id}
  })
}

export function enableKnowledge(id) {
  return request({
    url: `/LLMService/Knowledge/Enable`,
    method: 'get',
    params: {id}
  })
}

export function disableKnowledge(id) {
  return request({
    url: `/LLMService/Knowledge/Disable`,
    method: 'get',
    params: {id}
  })
}