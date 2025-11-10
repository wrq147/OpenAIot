// export const serverUrl = 'http://localhost:81/'
 // #ifndef H5
 // export const serverUrl = 'http://125.124.98.180:7080' //后台根域名
// export const serverUrl = 'http://52.28.35.196'
// export const serverUrl = 'http://3.79.43.33'
// export const serverUrl = 'http://123.58.218.244'
var serverUrl = 'http://Airlinkiot.com'
export default {
	setServerUrl(url) {
		serverUrl = url;
	},
	getServerUrl(){
		return serverUrl;
	}
}
// export const serverUrl = 'https://52.28.35.196'//苹果
// export const serverUrl = 'https://airlinkiot.com/'
 // #endif
 // #ifdef H5
 // export const serverUrl = '/api' //后台根域名  这样写，会给所有的接口都添加上/api，然后走代理服务器
 var serverUrl = '/api'
 export default {
 	setServerUrl(url) {
 		serverUrl = url;
 	},
 	getServerUrl(){
 		return serverUrl;
 	}
 }
  // export const serverUrl = 'http://125.124.98.180:7080' //后台根域名
 // #endif
