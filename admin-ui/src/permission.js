import router from './router'
import Vue from 'vue'
import store from './store'
import { Message } from 'element-ui'
import NProgress from 'nprogress'
import 'nprogress/nprogress.css'
import { getToken } from '@/utils/auth'
import { loadOrgTheme, judgeWhite } from '@/utils/theme'

NProgress.configure({ showSpinner: false })

const whiteList = ['/login', '/auth-redirect', '/bind', '/InviteRegister', '/loginInterface', '/emailLogin', '/wxworkLogin', '/ForgotPassword', '/crm/yaoqing/mobile', '/crm/yaoqing/choose', '/crm/yaoqing/creatUser', '/report/datav/datavRelease', '/report/spreadSheet/viewDataReport', '/report/datav/datavView']

router.beforeEach((to, from, next) => {
    NProgress.start()

    //主页跳转到其它页面
    if (to.path == "/index") {
        let pars = to.query;
        if (pars.code) {
            // next("/InviteRegister?code=" + pars.code);
            if (pars.hailogin) {
                next("/InviteRegister?code=" + pars.code);
            } else {
                if (getToken()) {
                    next("/crm/yaoqing/hailogin?code=" + pars.code);
                } else {
                    next("/crm/yaoqing/choose?code=" + pars.code)
                }
            }

            NProgress.done();
            return;
        }
        if (pars.yaoqingId) {
            if (pars.hailogin) {
                next("/crm/yaoqing/creatOrg?yaoqingId=" + pars.yaoqingId);


            } else {
                if (getToken()) {
                    next("/crm/yaoqing/hailogin?yaoqingId=" + pars.yaoqingId);
                } else {
                    next("/crm/yaoqing/choose?yaoqingId=" + pars.yaoqingId)
                }

            }
            NProgress.done();
            return;
        }
        if (pars.yqcode && pars.node) {
            // console.log('登录', pars.yqcode, pars.node);
            if (pars.hailogin) {
                next("/crm/yaoqing/creatOrg?yaoqingId=" + pars.yqcode + '&node=' + pars.node);


            } else {
                if (getToken()) {
                    next("/crm/yaoqing/hailogin?yaoqingId=" + pars.yqcode + '&node=' + pars.node);
                } else {
                    next("/crm/yaoqing/choose?yaoqingId=" + pars.yqcode + '&node=' + pars.node)
                }

            }
            NProgress.done();
            return;
        }
    }

    if (getToken()) {
        to.meta.title && store.dispatch('settings/setTitle', to.meta.title)
            /* has token*/
        if (to.path === '/login') {
            next({ path: '/' })
            NProgress.done()
        } else {
            if (store.getters.roles.length === 0) {
                new Vue().$cache.local.remove("layout-setting")
                    // 判断当前用户是否已拉取完user_info信息
                store.dispatch('GetInfo').then(() => {
                    loadOrgTheme(store.state.user.orgId).then(() => {
                        store.dispatch('GenerateRoutes').then(accessRoutes => {
                            // 根据roles权限生成可访问的路由表
                            router.addRoutes(accessRoutes) // 动态添加可访问路由表
                            next({...to, replace: true }) // hack方法 确保addRoutes已完成
                        })
                    })

                }).catch(err => {
                    store.dispatch('LogOut').then(() => {
                        Message.error(err)
                        next({ path: '/' })
                    })
                })
            } else {
                next()
            }
        }
    } else {
        // 没有token
        if (whiteList.indexOf(to.path) !== -1) {
            // 在免登录白名单，直接进入
            next()
        } else {
            //判断是否有主题登录页面
            if (store.state.settings && store.state.settings.loginUrl) {
                if (judgeWhite(whiteList, store.state.settings.loginUrl)) {
                    if (store.state.settings.loginUrl.indexOf('?') > -1) {
                        window.location.href = `${store.state.settings.loginUrl}&redirect=${to.fullPath}`
                        window.location.reload()
                    } else {
                        window.location.href = `${store.state.settings.loginUrl}?redirect=${to.fullPath}`
                        window.location.reload()
                    }
                } else {
                    next(`/login?redirect=${to.fullPath}`)
                }
            } else {
                next(`/login?redirect=${to.fullPath}`)
            }
            NProgress.done()
        }

    }
})

router.afterEach(() => {
    NProgress.done()
})