// export const serverUrl = 'http://localhost:81/'
//// #ifndef H5
// export const serverUrl = 'http://125.124.182.225:880' //后台根域名
// export const serverUrl1 = 'http://125.124.98.180:7080'
// export const serverUrl1 = 'http://52.28.35.196'
// export const serverUrl = 'http://123.58.218.244'
// export const serverUrl = 'https://52.28.35.196'//苹果
// export const serverUrl = 'https://airlinkiot.com/'
//// #endif

// #ifdef H5
// export const serverUrl = '/api' //后台根域名  这样写，会给所有的接口都添加上/api，然后走代理服务器
// export const serverUrl = 'http://wxwx.huade-app.com:880' //
// export const serverUrl = 'http://tpm.honggangtex.com:3880' //
 // export const serverUrl = 'http://52.28.35.196' //
 // var serverUrl1 = 'http://123.58.218.244'
 // export const serverUrl = 'http://125.124.98.180:7080'
 // var serverUrl1 = 'http://125.124.98.180:7080'
 export const serverUrl = 'http://iot.wookongcloud.com'
 var serverUrl1 = 'http://iot.wookongcloud.com'
 // export const serverUrl = 'https://e.orangeiot.com'
 // var serverUrl1 = 'https://e.orangeiot.com'
 // export const serverUrl = 'http://123.58.218.244' //
 // var serverUrl1 = 'http://123.58.218.244'
 // export const serverUrl = 'https://c.lida-iot.com' //电信LD
 // var serverUrl1 = 'https://c.lida-iot.com'
 // export const serverUrl = '/' //后台根域名  这样写，会给所有的接口都添加上/api，然后走代理服务器
 // var serverUrl1 = '/'
// var	serverUrl1='http://wxwx.huade-app.com:880';//企业微信测试
// var	serverUrl1='http://tpm.honggangtex.com:3880';//企业微信测试
export default {
	setServerUrl(url) {
		serverUrl1 = url;
	},
	getServerUrl(){
		return serverUrl1;
	},
	returnSetWeather(){
		var cansetWeather=true//是否可以设置天气
		// var cansetWeather=false
		return cansetWeather;
	}
}
// #endif
// #ifndef H5
  // var serverUrl = 'http://123.58.218.244'
  // var serverUrl = 'http://125.124.98.180:7080'
  var serverUrl = 'http://iot.wookongcloud.com'
  // var serverUrl = 'https://c.lida-iot.com'
  // var serverUrl = '/'
// var	serverUrl='http://wxwx.huade-app.com:880';//企业微信测试
// var	serverUrl='https://e.orangeiot.com';
export default {
	setServerUrl(url) {
		serverUrl = url;
	},
	getServerUrl(){
		return serverUrl;
	},
	returnSetWeather(){
		var cansetWeather=true//是否可以设置天气
		// var cansetWeather=false
		return cansetWeather;
	}
}
// #endif