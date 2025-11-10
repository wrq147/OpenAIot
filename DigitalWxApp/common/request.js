import ajax from 'uni-ajax'
import {
	getToken,
	getRefreshToken
} from './auth.js'
import {
	refreshToken
} from '../api/login.js'

export let isRelogin = {
	show: false
};
// export const serverUrl = 'http://127.0.0.1:880';
export const serverUrl = 'https://card-ai.huade-app.com';
//被挂起的请求数组
let refreshSubscribers = [];
// 创建请求实例
const instance = ajax.create({
	// 初始配置F
	baseURL: serverUrl,
	// 超时
	timeout: 10000
})

// 请求拦截器
instance.interceptors.request.use(config => {
	// 是否需要设置 token
	const isToken = (config.header || {}).isToken === false
	var tk = getToken();
	if (tk && !isToken) {
		config.header['Authorization'] = tk // 让每个请求携带自定义token 请根据实际情况自行修改
	}
	return config
})

//响应拦截器
instance.interceptors.response.use(response => {
		// 二进制数据则直接返回
		if (response.config.responseType === 'blob' || response.config.responseType === 'arraybuffer') {
			return response.data;
		}

		if (response.data.code == 401) {
			//没有登录或登录已超时
			if (!isRelogin.show) {
				isRelogin.show = true;

				//使用刷新令牌
				refreshToken({
					refreshtk: getRefreshToken(),
					clientToken: getToken()
				}).then(rsp => {
					setToken(rsp.data.token);
					setRefreshToken(rsp.data.refresh_token);
					//重新请求
					refreshSubscribers.forEach(cb => cb());
					refreshSubscribers = [];
					isRelogin.show = false;
				}).catch(err => {

					isRelogin.show = false;
					let pages = getCurrentPages();
					let route = pages[pages.length - 1]['$page']['fullPath'];
					uni.reLaunch({
						url: '/pages/index?t=' + encodeURIComponent(route)
					});

				});
			}
			return new Promise((resolve, reject) => {
				//挂起请求
				refreshSubscribers.push(() => {
					instance(response.config).then(data => {
						resolve(data);
					}).catch(err => {
						reject(err);
					})
				});
			});
		} else if (response.data.code == 50012) {
			//强制再次登录
			if (!isRelogin.show) {
				isRelogin.show = true;
				uni.showModal({
					title: '提示',
					content: response.data.message,
					showCancel: false,
					success: (res) => {
						if (res.confirm) {
							isRelogin.show = false;
							let pages = getCurrentPages();
							let route = pages[pages.length - 1]['$page']['fullPath'];
							uni.reLaunch({
								url: '/pages/index?t=' + encodeURIComponent(route)
							});
						}
					}
				});
			}

			return Promise.reject(response.data.message);
		}else if (response.data.code !== 0) {
			if (response.data.code > 9) {
				setTimeout(() => {
					uni.showToast({
						title: response.data.message,
						icon: "none",
						duration: 2000
					});
				}, 100);
			}
			return Promise.reject(response.data);
		}

		return response.data;
	},
	error => {
		// 对响应错误做些什么
		let errObj = {
			message: error.errMsg
		};

		if (errObj.message == "Network Error") {
			errObj.message = "后端接口连接异常";
		} else if (errObj.message.includes("timeout")) {
			errObj.message = "系统接口请求超时";
		} else if (errObj.message.includes("Request failed with status code")) {
			errObj.message = "系统接口" + errObj.message.substr(errObj.message.length - 3) + "异常";
		}
		setTimeout(() => {
			uni.showToast({
				title: errObj.message,
				icon: "none",
				duration: 2000
			});
		}, 50);
		return Promise.reject(errObj);
	})

export default instance
