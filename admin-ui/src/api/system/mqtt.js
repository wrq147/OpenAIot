import request from '@/utils/request'
import {
  getToken,
} from '@/utils/auth'
// 获取Mqtt的web连接地址
export function getWebIp(ssl) {
    return request({
      url: '/MqttService/Server/WebIp',
      method: 'get',
      params: {ssl}
    })
}
  
// 返回服务端格林威治时间和本地时间之间的时差
export function getTimezoneOffset(ssl) {
  return request({
    url: '/MqttService/Server/GetTimezoneOffset',
    method: 'get'
  })
}

// 获取用户的mqtt连接clientId
export function getClientId() {
  return request({
    url: '/MqttService/User/ClientId',
    method: 'get'
  });
}
