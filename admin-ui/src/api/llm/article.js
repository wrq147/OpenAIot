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
    url: `/LLMService/Article/Info`,
    method: 'get',
    params: {id}
  })
}

export function getArticleItems(kbId) {
  return request({
    url: `/LLMService/Article/ArticleItems`,
    method: 'get',
    params: {kbId}
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
    method: 'post',
    data: data
  })
}

export function deleteArticle(id) {
  return request({
    url: `/LLMService/Article/Delete`,
    method: 'get',
    params: {id}
  })
}

export function viewArticle(id) {
  return request({
    url: `/LLMService/Article/View`,
    method: 'get',
    params: {id}
  })
}