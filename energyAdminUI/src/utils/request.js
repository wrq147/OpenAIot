import axios from 'axios'
import Vue from 'vue'
import store from '@/store'
import {
    setToken,
    getToken,
    getShareToken,
    getRefreshToken,
    setRefreshToken
} from '@/utils/auth'
import errorCode from '@/utils/errorCode'
import router from '@/router'
import {
    refreshToken
} from '@/api/login'
import qs from 'qs';
import { loginThemeInfo } from '@/utils/theme'
// 是否刷新令牌
let isRelogin = false;
//被挂起的请求数组
let refreshSubscribers = [];
let that = this
axios.defaults.headers['Content-Type'] = 'application/json;charset=utf-8'
    // 创建axios实例
const service = axios.create({
        // axios中请求配置有baseURL选项，表示请求URL公共部分
        baseURL: process.env.VUE_APP_BASE_API,
        // 超时
        timeout: 30000
    })
    // request拦截器
service.interceptors.request.use(config => {
    // 是否需要设置 token
    const isToken = (config.headers || {}).isToken === false
    if (!isToken) {
        if (getToken()) {
            config.headers['Authorization'] = getToken() // 让每个请求携带自定义token 请根据实际情况自行修改
        } else {
            if (getShareToken()) {
                config.headers['AuthOther'] = getShareToken()
            }
        }
    }
    config.headers['TZ'] = new Date().getTimezoneOffset();
    // get请求映射params参数

    if (config.method === 'get' && config.params) {
        config.paramsSerializer = function(params) {
            for (const propName of Object.keys(params)) {
                const value = params[propName];
                // console.log(value, 'value');

                if (value === null || typeof(value) === "undefined") {
                    delete params[propName]
                        // Vue.delete(params, propName)
                        // console.log("get方法传参dddd", params, value);
                }
            }
            // console.log("get方法传参", params);

            return qs.stringify(params, { arrayFormat: 'repeat' })
        }
    }
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
                    console.log("刷新令牌报错", err);
                    new Vue().$msgbox.alert('登录状态已过期，请重新登录', '系统提示', {
                        confirmButtonText: '确定',
                        callback: () => {
                            isRelogin = true;
                            store.dispatch('FedLogOut').then(async() => {
                                let loginUrl = await loginThemeInfo(store.state.user.orgId)
                                if (loginUrl) {
                                    router.replace(`${loginUrl}&redirect=${router.app.$route.fullPath}`);
                                } else {
                                    router.replace(`/login?redirect=${router.app.$route.fullPath}`);
                                }

                            })
                        }
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
                new Vue().$msgbox.alert(msg, '系统提示', {
                    confirmButtonText: '确定',
                    callback: () => {
                        isRelogin = true;
                        store.dispatch('FedLogOut').then(async() => {
                            let loginUrl = await loginThemeInfo(store.state.user.orgId)
                            if (loginUrl) {
                                router.replace(`${loginUrl}&redirect=${router.app.$route.fullPath}`);
                            } else {
                                router.replace(`/login?redirect=${router.app.$route.fullPath}`);
                            }
                        })
                    }
                });
            }

            return Promise.reject(msg);
        } else if (code !== 0) {
            if (code > 9) {
                new Vue().$message.error(msg);
            }

            return Promise.reject(res.data)
        } else {
            isRelogin = false;
            return res.data
        }
    },
    error => {

        let {
            message
        } = error;
        if (message == "Network Error") {
            message = "后端接口连接异常";
        } else if (message.includes("timeout")) {
            message = "系统接口请求超时";
        } else if (message.includes("Request failed with status code")) {
            message = "系统接口" + message.substr(message.length - 3) + "异常";
        }
        new Vue().$message({
            message: message,
            type: 'error',
            duration: 3000
        })
        return Promise.reject(error)
    }
)

export default service