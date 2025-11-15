import { constantRoutes } from '@/router'
import { getRouters } from '@/api/menu'
import Layout from '@/layout/index'
import ParentView from '@/components/ParentView';
import InnerLink from '@/layout/components/InnerLink'

const permission = {
    state: {
        routes: [],
        addRoutes: [],
        defaultRoutes: [],
        topbarRouters: [],
        sidebarRouters: [],
        affixTags: null,
    },
    mutations: {
        SET_AFFIXTAGS: (state, routes) => {
            state.affixTags = routes
        },
        SET_ROUTES: (state, routes) => {
            state.addRoutes = routes
            state.routes = constantRoutes.concat(routes)
        },
        SET_DEFAULT_ROUTES: (state, routes) => {
            state.defaultRoutes = constantRoutes.concat(routes)
        },
        SET_TOPBAR_ROUTES: (state, routes) => {
            state.topbarRouters = routes;
        },
        SET_SIDEBAR_ROUTERS: (state, routes) => {
            state.sidebarRouters = routes
        },
    },
    actions: {
        // 生成路由
        GenerateRoutes({ commit }) {
            return new Promise(resolve => {
                // 向后端请求路由数据
                getRouters().then(res => {
                    // console.log("侧边列表", JSON.parse(JSON.stringify(res)))
                    res.data = res.data.filter(row => row.meta.title == '系统管理');
                    // || row.meta.title == '企业管理'
                    const sdata = JSON.parse(JSON.stringify(res.data));
                    const rdata = JSON.parse(JSON.stringify(res.data));

                    // let lis = {
                    //     meta: {}
                    // }
                    // lis.alwaysShow = false
                    // lis.name = 'RulesEngine'
                    // lis.parentPath = '/flowable'
                    // lis.path = '/rulesEngine/index'
                    // lis.hidden = false
                    // lis.meta.icon = 'user'
                    // lis.meta.link = ''
                    // lis.meta.noCache = false
                    // lis.meta.title = '规则引擎'
                    // sdata[3].children.push(lis)
                    // lis = {
                    //     meta: {}
                    // }
                    // lis.alwaysShow = false
                    // lis.name = 'RulesEngineAdd'
                    // lis.parentPath = '/flowable'
                    // lis.path = '/rulesEngine/add'
                    // lis.hidden = false
                    // lis.meta.icon = 'user'
                    // lis.meta.link = ''
                    // lis.meta.noCache = false
                    // lis.meta.title = '规则新增'
                    // sdata[3].children.push(lis)
                    const sidebarRoutes = filterAsyncRouter(sdata);
                    const rewriteRoutes = filterAsyncRouter(rdata, false, true);
                    rewriteRoutes.push({ path: '*', redirect: '/404', hidden: true });
                    // console.log("路由图标", rewriteRoutes);
                    // console.log(constantRoutes, 'constantRoutes');
                    commit('SET_ROUTES', rewriteRoutes);
                    commit('SET_SIDEBAR_ROUTERS', constantRoutes.concat(sidebarRoutes));
                    commit('SET_DEFAULT_ROUTES', sidebarRoutes);
                    commit('SET_TOPBAR_ROUTES', sidebarRoutes);
                    resolve(rewriteRoutes);
                })
            })
        }
    }
}

// 遍历后台传来的路由字符串，转换为组件对象
function filterAsyncRouter(asyncRouterMap, lastRouter = false, type = false) {
    return asyncRouterMap.filter(route => {
        if (route) {}
        if (type && route.children) {
            route.children = route.children.filter(row => row.meta.title != '自定义字段' && row.meta.title != '我的授权');
            route.children = filterChildren(route.children)
        }
        if (route.component) {
            // Layout ParentView 组件特殊处理
            if (route.component === 'Layout') {
                route.component = Layout
            } else if (route.component === 'ParentView') {
                route.component = ParentView
            } else if (route.component === 'InnerLink') {
                route.component = InnerLink
            } else {
                route.component = loadView(route.component)
            }
        }
        if (route.children != null && route.children && route.children.length) {
            route.children = route.children.filter(row => row.meta.title != '自定义字段' && row.meta.title != '我的授权' && row.meta.title != '企业信息');
            route.children = filterAsyncRouter(route.children, route, type)
        } else {
            delete route['children']
            delete route['redirect']
        }
        return true
    })
}

function filterChildren(childrenMap, lastRouter = false) {
    var children = []
    childrenMap.forEach((el, index) => {
        if (el.children && el.children.length) {
            if (el.component === 'ParentView' && !lastRouter) {
                el.children.forEach(c => {
                    c.path = el.path + '/' + c.path
                    if (c.children && c.children.length) {
                        children = children.concat(filterChildren(c.children, c))
                        return
                    }
                    children.push(c)
                })
                return
            }
        }
        if (lastRouter) {
            el.path = lastRouter.path + '/' + el.path
        }
        children = children.concat(el)
    })
    return children
}

export const loadView = (view) => {
    if (process.env.NODE_ENV === 'development') {
        return (resolve) => require([`@/views/${view}`], resolve)
    } else {
        // 使用 import 实现生产环境的路由懒加载
        return () =>
            import (`@/views/${view}`)
    }
}

export default permission