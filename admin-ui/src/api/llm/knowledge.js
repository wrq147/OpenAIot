import request from '@/utils/request'

export function listKnowledge(query) {
  return request({
    url: '/LLMService/KnowledgeBase/List',
    method: 'get',
    params: query
  })
}

export function getKnowledge(id) {
  return request({
    url: `/LLMService/KnowledgeBase/${id}`,
    method: 'get'
  })
}

export function addKnowledge(data) {
  return request({
    url: '/LLMService/KnowledgeBase/Add',
    method: 'post',
    data: data
  })
}

export function updateKnowledge(data) {
  return request({
    url: '/LLMService/KnowledgeBase/Update',
    method: 'put',
    data: data
  })
}

export function deleteKnowledge(id) {
  return request({
    url: `/LLMService/KnowledgeBase/Delete/${id}`,
    method: 'delete'
  })
}

export function changeKnowledgeStatus(id, status) {
  return request({
    url: `/LLMService/KnowledgeBase/${id}/status`,
    method: 'post',
    data: status
  })
}