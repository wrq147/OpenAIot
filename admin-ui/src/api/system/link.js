import request from '@/utils/request'


export function getLinkList(query) {
  return request({
    url: '/ShortLinkService/LinkMan/List',
    method: 'get',
    params: query
  })
}


export function addLink(data) {
  return request({
    url: '/ShortLinkService/LinkMan/Add',
    method: 'post',
    data: data
  })
}


export function delLink(query) {
  return request({
    url: '/ShortLinkService/LinkMan/Remove',
    method: 'get',
    params: query
  })
}