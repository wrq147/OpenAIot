import Vue from 'vue'
import Router from 'vue-router'

Vue.use(Router)

/* Layout */
import Layout from '@/layout'
import { othersRoutes } from './setRouter'
/**
 * Note: 路由配置项
 *
 * hidden: true                     // 当设置 true 的时候该路由不会再侧边栏出现 如401，login等页面，或者如一些编辑页面/edit/1
 * alwaysShow: true                 // 当你一个路由下面的 children 声明的路由大于1个时，自动会变成嵌套的模式--如组件页面
 *                                  // 只有一个时，会将那个子路由当做根路由显示在侧边栏--如引导页面
 *                                  // 若你想不管路由下面的 children 声明的个数都显示你的根路由
 *                                  // 你可以设置 alwaysShow: true，这样它就会忽略之前定义的规则，一直显示根路由
 * redirect: noRedirect             // 当设置 noRedirect 的时候该路由在面包屑导航中不可被点击
 * name:'router-name'               // 设定路由的名字，一定要填写不然使用<keep-alive>时会出现各种问题
 * query: '{"id": 1, "name": "ry"}' // 访问路由的默认传递参数
 * meta : {
    noCache: true                   // 如果设置为true，则不会被 <keep-alive> 缓存(默认 false)
    title: 'title'                  // 设置该路由在侧边栏和面包屑中展示的名字
    icon: 'svg-name'                // 设置该路由的图标，对应路径src/assets/icons/svg
    breadcrumb: false               // 如果设置为false，则不会在breadcrumb面包屑中显示
    activeMenu: '/system/user'      // 当路由设置了该属性，则会高亮相对应的侧边栏。
  }
 */

// 公共路由
export const constantRoutes = [{
        path: '/redirect',
        component: Layout,
        hidden: true,
        children: [{
            path: '/redirect/:path(.*)',
            component: (resolve) => require(['@/views/redirect'], resolve)
        }]
    },
    {
        path: '/newOrg',
        component: (resolve) => require(['@/views/newOrg'], resolve),
        hidden: true
    },
    {
        path: '/login',
        component: (resolve) => require(['@/views/login'], resolve),
        hidden: true
    },
    {
        path: '/404',
        component: (resolve) => require(['@/views/error/404'], resolve),
        hidden: true
    },
    {
        path: '/401',
        component: (resolve) => require(['@/views/error/401'], resolve),
        hidden: true
    },
    {
        path: '',
        component: Layout,
        redirect: 'index',
        children: [{
            path: 'index',
            component: (resolve) => require(['@/views/index'], resolve),
            name: 'Index',
            meta: { title: '首页', icon: 'shouye', affix: true }
        }]
    },
    {
        path: '/user',
        component: Layout,
        hidden: true,
        redirect: 'noredirect',
        children: [{
            path: 'profile',
            component: (resolve) => require(['@/views/system/user/profile/index'], resolve),
            name: 'Profile',
            meta: { title: '个人中心', icon: 'user' }
        }]
    },
    {
        path: '/org/user-auth',
        component: Layout,
        hidden: true,
        children: [{
            path: 'role/:userId(\\d+)',
            component: (resolve) => require(['@/views/system/Employee/authRole'], resolve),
            name: 'AuthRole',
            meta: { title: '分配角色' }
        }]
    },
    {
        path: '/org/role-auth',
        component: Layout,
        hidden: true,
        children: [{
            path: 'user/:roleId(\\d+)',
            component: (resolve) => require(['@/views/system/role/authUser'], resolve),
            name: 'AuthUser',
            meta: { title: '分配用户' }
        }]
    }, {
        path: '/org/theme-auth',
        component: Layout,
        hidden: true,
        children: [{
            path: 'org/:themeId(\\d+)',
            component: (resolve) => require(['@/views/system/theme/authOrg'], resolve),
            name: 'AuthOrg',
            meta: { title: '分配企业' }
        }]
    },
    {
        path: '/system/dict-data',
        component: Layout,
        hidden: true,
        children: [{
            path: 'index/:dictId(\\d+)',
            component: (resolve) => require(['@/views/system/dict/data'], resolve),
            name: 'Data',
            meta: { title: '字典数据', activeMenu: '/system/dict' }
        }]
    },
    {
        path: '/monitor/job-log',
        component: Layout,
        hidden: true,
        children: [{
            path: 'index',
            component: (resolve) => require(['@/views/monitor/job/log'], resolve),
            name: 'JobLog',
            meta: { title: '调度日志', activeMenu: '/monitor/job' }
        }]
    },
    {
        path: '/InviteRegister', //邀请注册
        component: (resolve) => require(['@/views/InviteRegister'], resolve),
        hidden: true
    },
    {
        path: '/loginInterface',
        component: (resolve) => require(['@/views/loginInterface'], resolve),
        hidden: true
    },
    {
        path: '/emailLogin',
        component: (resolve) => require(['@/views/emailLogin'], resolve),
        hidden: true
    },
    {
        path: '/wxworkLogin',
        component: (resolve) => require(['@/views/wxworkLogin'], resolve),
        hidden: true
    },
    {
        path: '/ForgotPassword',
        component: (resolve) => require(['@/views/ForgotPassword'], resolve),
        hidden: true
    },
    {
        path: '/iot/deviceManage',
        component: Layout,
        redirect: 'deviceDetail',
        hidden: true,
        meta: { title: '物联设备', icon: 'fuwuguanli' },
        children: [{
            path: '/iot/deviceManage/deviceDetail',
            component: (resolve) => require(['@/views/iot/deviceManage/deviceDetail'], resolve),
            name: 'Index',
            meta: { title: '设备详情', icon: 'dashboard', affix: false }
        }]
    }, {
        path: '/crm/yaoqing/choose',
        component: (resolve) => require(['@/views/crm/yaoqing/choose'], resolve),
        name: 'choose',
        hidden: true,
        meta: { title: '邀请选择', activeMenu: '/crm/yaoqing/choose' }

    }, {
        path: '/crm/yaoqing/choose_addorg',
        component: (resolve) => require(['@/views/crm/yaoqing/choose_addorg'], resolve),
        name: 'choose_addorg',
        hidden: true,
        meta: { title: '加入企业', activeMenu: '/crm/yaoqing/choose_addorg' }

    }, {
        path: '/crm/yaoqing/analyzeLlink',
        component: (resolve) => require(['@/views/crm/yaoqing/analyzeLlink'], resolve),
        name: 'analyzeLlink',
        hidden: true,
        meta: { title: '解析链接', activeMenu: '/crm/yaoqing/analyzeLlink' }

    }, {
        path: '/crm/yaoqing/hailogin',
        component: (resolve) => require(['@/views/crm/yaoqing/hailogin'], resolve),
        name: 'hailogin',
        hidden: true,
        meta: { title: '加入邀请', activeMenu: '/crm/yaoqing/hailogin' }
    }, {
        path: '/crm/yaoqing/creatUser',
        component: (resolve) => require(['@/views/crm/yaoqing/creatUser'], resolve),
        name: 'creatUser',
        hidden: true,
        meta: { title: '创建用户', activeMenu: '/crm/yaoqing/creatUser' }
    }, {
        path: '/crm/yaoqing/creatOrg',
        component: (resolve) => require(['@/views/crm/yaoqing/creatOrg'], resolve),
        name: 'creatOrg',
        hidden: true,
        meta: { title: '创建企业', activeMenu: '/crm/yaoqing/creatOrg' }
    }, {
        path: '/jxc/',
        component: Layout,
        hidden: true,
        meta: { title: '数字仓储', icon: 'fuwuguanli' },
        children: [{
            path: '/jxc/leave/stockApplylist',
            component: (resolve) => require(['@/views/storage/leave/stockApplylist'], resolve),
            name: 'stockApply',
            meta: { title: '待出库申请单' }
        }]
    },
    ...othersRoutes
]
let routeArr = []
export default new Router({
    mode: 'hash', // history去掉url中的#  hash
    scrollBehavior: () => ({ y: 0 }),
    routes: constantRoutes
})