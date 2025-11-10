import ajax from 'uni-ajax'
// import Vue from 'vue'
import store from '@/store'
import Vue from 'vue'
import {
	setToken,
	getToken,
	getRefreshToken,
	setRefreshToken
} from '@/common/auth.js'
// #ifndef H5
import serverUrl from '@/common/constVar.js'
// #endif
// #ifdef H5
import {
	serverUrl
} from '@/common/constVar.js'
// #endif
import errorCode from '@/common/errorCode.js'
import {
	refreshToken
} from '@/api/login'
import {jumpLogin} from "@/common/utillib.js"
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
	// #ifndef H5
	baseURL: serverUrl.getServerUrl(),
	// #endif
	// #ifdef H5
	baseURL: serverUrl,
	// #endif
	// 超时
	// timeout: 10000
	timeout: 2000
})
// request拦截器
service.interceptors.request.use(config => {
	// 是否需要设置 token
	// config.header['Content-Type'] = 'application/json;charset=utf-8'
	// config.header["Access-Control-Allow-Origin"] = "*"
	// console.log("token",getToken());
	// #ifndef H5
	config.baseURL = serverUrl.getServerUrl()
	// #endif
	const isToken = (config.header || {}).isToken === false
	if (getToken() && !isToken) {
		config.header['Authorization'] = getToken() // 让每个请求携带自定义token 请根据实际情况自行修改
	}
	config.header['TZ'] = new Date().getTimezoneOffset();
	// console.log("config",config);
	return config
}, error => {
	console.log(error)
	Promise.reject(error)
})

// 响应拦截器
service.interceptors.response.use(res => {
	// isRelogin = store.state.isRelogin;
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
					// console.log("刷新令牌报错", err);
					isRelogin = true;
					let pages = getCurrentPages();
					// console.log('当前的页面地址', pages, pages[pages.length - 1].route);
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
							route) + '&noFirt=1' + urlStr,
						// #ifdef APP-PLUS
						success: () => {
							plus.navigator.closeSplashscreen();
						},
						// #endif
					});

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
				if(pages&&pages[pages.length - 1]){
					let route = pages[pages.length - 1].route;
					let options = pages[pages.length - 1].options;
					urlStr = ""
					for (var key in options) {
						if (key != 't') {
							urlStr += '&' + key + '=' + options[key]
						}
					}
					isRelogin = true;
					if(route&&route!='pages/index/login'){
						uni.reLaunch({
							url: '/page_register/msg_tips?t=' + encodeURIComponent(
								route) + '&noFirt=1' + urlStr,
							// #ifdef APP-PLUS
							success: () => {
								plus.navigator.closeSplashscreen();
							},
							// #endif
						});
					}
				}else{
					uni.reLaunch({
						url: '/page_register/msg_tips?noFirt=1',
						// #ifdef APP-PLUS
						success: () => {
							plus.navigator.closeSplashscreen();
						},
						// #endif
					});
				}
				
				
			}

			return Promise.reject(res.data);
		}else if(code == 400){
			if(!isRelogin){
				isRelogin=true
				// store.commit('SET_isRelogin', true)
				let pages = getCurrentPages();
				
				if(pages&&pages[pages.length - 1]){
					let route = pages[pages.length - 1].route;
					let options = pages[pages.length - 1].options;
					urlStr = ""
					for (var key in options) {
						if (key != 't') {
							urlStr += '&' + key + '=' + options[key]
						}
					}
					if(res.config&&res.config.isNotJump||route&&route=='pages/index/login'){}else{
						jumpLogin(route,urlStr)
					}
				}else{
					jumpLogin()
				}
			}
			
			
			
			return Promise.reject(res.data);
		} else if (code !== 0) {
			if (code > 9) {
				// uni.showToast({
				// 	title: msg,
				// 	icon: "none",
				// 	duration: 2000
				// });
				res.data.cusMsg = msg
			}

			return Promise.reject(res.data)
		} else {
			// if(getToken() && getRefreshToken()){
			// 	store.commit('SET_isRelogin', false)
			// }
			return res.data
		}
	},
	error => {
		// console.log("错误信息request", error);
		let {
			errMsg
		} = error;
		if (errMsg == "Network Error") {
			errMsg = "网络错误";
		} else if (errMsg.includes("timeout")) {
			errMsg = "系统接口请求超时";
			let isshow=store.state.isSetNotimeoutTips
			if(store.state.isSetNotimeoutTips||!store.state.isOpenApp){
				errMsg = null;
			}
		} else if (errMsg.includes("Request failed with status code")) {
			errMsg = "请求失败";
		} else if (errMsg.includes("Request failed")) {
			errMsg = "请求失败";
		}else if (errMsg.includes("request")&&errMsg.includes("fail")) {
			errMsg = "网络异常";
		}
		setTimeout(() => {
			if (errMsg&&errMsg.indexOf('request:ok')==-1) {
				// console.log('是否显示提示',store.state.isSetNotimeoutTips);
				if(!store.state.isSetNotimeoutTips){
					uni.showToast({
						title: errMsg,
						icon: "none",
						duration: 5 * 1000
					});
				}
				
			}

		}, 50);
		return Promise.reject(error)
	}
)

export default service