import request from '@/utils/request'

export function listArticle(query) {
  return request({
    url: '/LLMService/Article/List',
    method: 'get',
    params: query
  })
}

export function getArticle(id) {
  return request({
    url: `/LLMService/Article/${id}`,
    method: 'get'
  })
}

export function getArticlesByKbId(kbId) {
  return request({
    url: `/LLMService/Article/KbId/${kbId}`,
    method: 'get'
  })
}

export function addArticle(data) {
  return request({
    url: '/LLMService/Article/Add',
    method: 'post',
    data: data
  })
}

export function updateArticle(data) {
  return request({
    url: '/LLMService/Article/Update',
    method: 'put',
    data: data
  })
}

export function deleteArticle(id) {
  return request({
    url: `/LLMService/Article/Delete/${id}`,
    method: 'delete'
  })
}

export function changeArticleStatus(id, status) {
  return request({
    url: `/LLMService/Article/${id}/status`,
    method: 'post',
    data: status
  })
}