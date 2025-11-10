import request from '@/common/request.js'

//微信登录并绑定
export function wxLogin(data) {
	return request.post({
		url: '/WeiXinService/Login/FromWxApplet',
		header: {
			isToken: false
		},
		data: data
	});
}


export function refreshToken(data){
	return request.get({
		url: '/AuthService/SysLogin/RefreshToken',
		query: data
	});
}