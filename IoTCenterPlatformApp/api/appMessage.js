import request from '@/common/request.js'
// 上报ClientId
export function upClientId(clientId) {
    return request.get({
        url: '/MessageService/Client/UpClientId',
        query:{
			clientId:clientId
		}
    })
}
