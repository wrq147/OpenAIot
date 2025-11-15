import request from '@/utils/request'

// 获取路由
export const getRouters = () => {
  return request({
    url: '/AuthService/Permission/GetRouters',
    method: 'get'
  })
}