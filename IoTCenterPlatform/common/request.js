import ajax from 'uni-ajax'
// import Vue from 'vue'
import store from '@/store'
import {
	setToken,
	getToken,
	getRefreshToken,
	setRefreshToken
} from '@/common/auth.js'
import serverUrl from '@/common/constVar.js'
import errorCode from '@/common/errorCode.js'
import {
	refreshToken
} from '@/api/login'
// import qs from 'qs';
var urlStr = ''
// 是否刷新令牌
var isRelogin = false;
//被挂起的请求数组
let refreshSubscribers = [];
let that = this
// ajax.defaults.headers['Content-Type'] = 'application/json;charset=utf-8'
// 创建ajax实例
const service = ajax.create({
	// ajax中请求配置有baseURL选项，表示请求URL公共部分
	baseURL: serverUrl.getServerUrl(),
	// 超时
	timeout: 20000
})
// request拦截器
service.interceptors.request.use(config => {
	// 是否需要设置 token
	config.baseURL = serverUrl.getServerUrl()
	// config.header['Content-Type'] = 'application/json;charset=utf-8'
	// config.header["Access-Control-Allow-Origin"] = "*"
	// console.log("token",getToken());
	const isToken = (config.header || {}).isToken === false
	if (getToken() && !isToken) {
		config.header['Authorization'] = getToken() // 让每个请求携带自定义token 请根据实际情况自行修改
	}
	config.header['TZ'] = new Date().getTimezoneOffset();
	// console.log("config",config);
	// get请求映射params参数
	// if (config.method === 'get' && config.params) {
	//     config.paramsSerializer = function(params) {
	//         for (const propName of Object.keys(params)) {
	//             const value = params[propName];
	//             // console.log(value, 'value');

	//             if (value === null || typeof(value) === "undefined") {
	//                 delete params[propName]
	//                     // Vue.delete(params, propName)
	//                     // console.log("get方法传参dddd", params, value);
	//             }
	//         }
	//         // console.log("get方法传参", params);

	//         return qs.stringify(params, { arrayFormat: 'repeat' })
	//     }
	// }

	return config
}, error => {
	console.log(error)
	Promise.reject(error)
})

// 响应拦截器
service.interceptors.response.use(res => {
		// 未设置状态码则默认成功状态
		const code = res.data.code || 0;
		// 获取错误信息
		const msg = errorCode[code] || res.data.message || errorCode['default']
		if (code === 401) {
			if (!isRelogin) {
				isRelogin = true;
				//使用刷新令牌
				refreshToken({
					refreshtk: getRefreshToken(),
					clientToken: getToken()
				}).then(data => {
					setToken(data.data.token);
					setRefreshToken(data.data.refresh_token);
					//重新请求
					refreshSubscribers.forEach(cb => cb());
					refreshSubscribers = [];
					isRelogin = false;
				}).catch(err => {
					isRelogin = true;
					let pages = getCurrentPages();
					let route = pages[pages.length - 1].route;
					let options = pages[pages.length - 1].options;
					urlStr = ""
					for (var key in options) {
						if (key != 't') {
							urlStr += '&' + key + '=' + options[key]
						}
					}
					uni.reLaunch({
						url: '/page_register/msg_tips?t=' + encodeURIComponent(
							route) + '&noFirt=1' + urlStr
					});
					// uni.showModal({
					// 	title: '系统提示',
					// 	content: '登录状态已过期，请重新登录',
					// 	showCancel: false,
					// 	success: (res) => {
					// 		if (res.confirm) {
					// 			isRelogin = false;
					// 			let pages = getCurrentPages();
					// 			console.log('当前的页面地址',pages, pages[pages.length - 1].route);
					// 			let route = pages[pages.length - 1].route;
					// 			uni.reLaunch({
					// 				url: '/pages/index/login?t=' + encodeURIComponent(
					// 					route) + '&noFirt=1'
					// 			});
					// 		}
					// 	}
					// });

				});
			}

			return new Promise((resolve, reject) => {
				//挂起请求
				refreshSubscribers.push(() => {
					service(res.config).then(data => {
						resolve(data);
					}).catch(err => {
						reject(err);
					})
				});
			});
		} else if (code == 50012) {
			//强制再次登录
			if (!isRelogin) {
				isRelogin = true;
				let pages = getCurrentPages();
				console.log('当前的页面地址', pages, pages[pages.length - 1].route);
				let route = pages[pages.length - 1].route;
				let options = pages[pages.length - 1].options;
				urlStr = ""
				for (var key in options) {
					if (key != 't') {
						urlStr += '&' + key + '=' + options[key]
					}
				}
				if (route && route != 'pages/index/login') {
					uni.reLaunch({
						url: '/page_register/msg_tips?t=' + encodeURIComponent(
							route) + '&noFirt=1' + urlStr
					});
				}
			}

			return Promise.reject(res.data);
		} else if (code == 400) {
			
			if (!isRelogin) {
				isRelogin = true;
				let pages = getCurrentPages();
				let route = pages[pages.length - 1].route;
				let options = pages[pages.length - 1].options;
				urlStr = ""
				for (var key in options) {
					if (key != 't') {
						urlStr += '&' + key + '=' + options[key]
					}
				}
				if (res.config && res.config.isNotJump || route && route == 'pages/index/login') {} else {
					uni.reLaunch({
						url: '/pages/index/login?t=' + encodeURIComponent(route) + '&noFirt=1' + urlStr,
						// #ifdef APP-PLUS
						success: () => {
							plus.navigator.closeSplashscreen();
						},
						// #endif
					});
				}
			}
			return Promise.reject(res.data)
		} else if (code !== 0) {
			if (code > 9) {
				// new Vue().$message.error(msg);
				// uni.showModal({
				// 	title: '系统提示',
				// 	content: msg,
				// 	showCancel: false,
				// 	success: (res) => {
				// 		if (res.confirm) {

				// 		}
				// 	}
				// });
				// uni.showToast({
				// 	title: msg,
				// 	icon: "none",
				// 	duration: 2000
				// });
				res.data.cusMsg = msg
			}

			return Promise.reject(res.data)
		} else {
			isRelogin = false;
			return res.data
		}
	},
	error => {
		console.log("错误信息", error,store.state.isSetNotimeoutTips);
		let {
			errMsg
		} = error;
		if(store.state.isSetNotimeoutTips){
			errMsg = null;
		}
		if (errMsg == "Network Error") {
			errMsg = "Network Error";
		} else if (errMsg.includes("timeout")) {
			errMsg = "System interface request timeout";
		} else if (errMsg.includes("Request failed with status code")) {
			errMsg = "Request failed with status code";
		}
		setTimeout(() => {
			if (errMsg) {
				uni.showToast({
					title: errMsg,
					icon: "none",
					duration: 5 * 1000
				});
			}

		}, 50);
		return Promise.reject(error)
	}
)

export default service