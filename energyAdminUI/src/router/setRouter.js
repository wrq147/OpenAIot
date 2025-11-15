import Vue from 'vue'
import Router from 'vue-router'

Vue.use(Router)

/* Layout */
import Layout from '@/layout'

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
export const othersRoutes = [{
        name: "business",
        path: "/business",
        hidden: false,
        redirect: "noRedirect",
        component: Layout,
        query: "",
        alwaysShow: true,
        meta: {
            title: "企业管理",
            icon: "qiyeguanli",
        },
        children: [{
            name: "archives",
            path: "archives",
            hidden: false,
            component: (resolve) => require(['@/views/business/archives'], resolve),
            query: "",
            meta: {
                title: "企业档案",
                icon: "peoples",
                noCache: true,
                link: ""
            }
        }, {
            name: "productLibrary",
            path: "productLibrary",
            hidden: false,
            component: (resolve) => require(['@/views/business/productLibrary'], resolve),
            meta: {
                title: "产品库",
                icon: "peoples",
                noCache: true,
                link: ""
            }
        }, {
            name: "deviceManagement",
            path: "deviceManagement",
            hidden: false,
            component: (resolve) => require(['@/views/business/deviceManagement'], resolve),
            meta: {
                title: "设备管理",
                icon: "peoples",
                noCache: true,
                link: ""
            }
        }, {
            name: "facility",
            path: "facility",
            hidden: false,
            component: (resolve) => require(['@/views/business/facility'], resolve),
            meta: {
                title: "设施管理",
                icon: "peoples",
                noCache: true,
                link: ""
            }
        }, {
            name: "freightBasis",
            path: "freightBasis",
            hidden: false,
            component: (resolve) => require(['@/views/business/freightBasis'], resolve),
            meta: {
                title: "计费标准",
                icon: "peoples",
                noCache: true,
                link: ""
            }
        }]
    },
    {
        name: "gatherManage",
        path: "/gatherManage",
        hidden: false,
        redirect: "noRedirect",
        component: Layout,
        alwaysShow: true,
        meta: {
            title: "采集管理",
            icon: "caijiguanli",
        },
        children: [{
                name: "energyGather",
                path: "energyGather",
                hidden: false,
                component: (resolve) => require(['@/views/gatherManage/energyGather'], resolve),
                query: "",
                meta: {
                    title: "能源数据采集",
                    icon: "peoples",
                    noCache: true,
                    link: ""
                }
            },
            {
                name: "productionGather",
                path: "productionGather",
                hidden: false,
                component: (resolve) => require(['@/views/gatherManage/productionGather'], resolve),
                query: "",
                meta: {
                    title: "生产数据采集",
                    icon: "peoples",
                    noCache: true,
                    link: ""
                }
            },
        ]
    },
    {
        name: "energyMeter",
        path: "/energyMeter",
        hidden: false,
        redirect: "noRedirect",
        component: Layout,
        alwaysShow: true,
        meta: {
            title: "能耗计量",
            icon: "nenghaojisuan",
        },
        children: [{
                name: "energyFlowAnalysis",
                path: "/energyFlowAnalysis",
                hidden: false,
                component: (resolve) => require(['@/views/energyMeter/energyFlowAnalysis'], resolve),
                query: "",
                meta: {
                    title: "能流分析",
                    icon: "peoples",
                    noCache: true,
                    link: ""
                }
            },
            {
                name: "energyStatis",
                path: "/energyStatis",
                hidden: false,
                component: (resolve) => require(['@/views/energyMeter/energyStatis'], resolve),
                query: "",
                meta: {
                    title: "设备能耗统计",
                    icon: "peoples",
                    noCache: true,
                    link: ""
                }
            },
            {
                name: "energyCellStatis",
                path: "/energyCellStatis",
                hidden: false,
                component: (resolve) => require(['@/views/energyMeter/energyCellStatis'], resolve),
                query: "",
                meta: {
                    title: "单元能耗统计",
                    icon: "peoples",
                    noCache: true,
                    link: ""
                }
            },
            {
                name: "energyUnitInquiry",
                path: "/energyUnitInquiry",
                hidden: false,
                component: (resolve) => require(['@/views/energyMeter/energyUnitInquiry'], resolve),
                query: "",
                meta: {
                    title: "单元能耗查询",
                    icon: "peoples",
                    noCache: true,
                    link: ""
                }
            },
            {
                name: "energyInquiry",
                path: "/energyInquiry",
                hidden: false,
                component: (resolve) => require(['@/views/energyMeter/energyInquiry'], resolve),
                query: "",
                meta: {
                    title: "设备能耗查询",
                    icon: "peoples",
                    noCache: true,
                    link: ""
                }
            },
            {
                name: "energyCompositeSearch",
                path: "/energyCompositeSearch",
                hidden: false,
                component: (resolve) => require(['@/views/energyMeter/energyCompositeSearch'], resolve),
                query: "",
                meta: {
                    title: "综合能耗查询",
                    icon: "peoples",
                    noCache: true,
                    link: ""
                }
            }, {
                name: "balance",
                path: "/energyMeter/balance",
                hidden: false,
                component: (resolve) => require(['@/views/energyMeter/balance'], resolve),
                query: "",
                meta: {
                    title: "能效平衡与优化",
                    icon: "peoples",
                    noCache: true,
                    link: ""
                }
            }, {
                name: "energyBenchmarking",
                path: "/energyMeter/energyBenchmarking",
                hidden: false,
                component: (resolve) => require(['@/views/energyMeter/energyBenchmarking'], resolve),
                query: "",
                meta: {
                    title: "能耗对标分析",
                    icon: "peoples",
                    noCache: true,
                    link: ""
                }
            }
        ]
    },
    {
        name: "energyPerformance",
        path: "/energyPerformance",
        hidden: false,
        redirect: "noRedirect",
        component: Layout,
        alwaysShow: true,
        meta: {
            title: "能源绩效",
            icon: "nengyuanfenxi",
        },
        children: [{
            name: "energyAnalyse",
            path: "/energyPerformance/energyAnalyse",
            hidden: false,
            component: (resolve) => require(['@/views/energyPerformance/energyAnalyse'], resolve),
            query: "",
            meta: {
                title: "能效分析",
                icon: "peoples",
                noCache: true,
                link: ""
            }
        }, {
            name: "efficiencyBenchmarking",
            path: "/energyPerformance/efficiencyBenchmarking",
            hidden: false,
            component: (resolve) => require(['@/views/energyPerformance/efficiencyBenchmarking'], resolve),
            query: "",
            meta: {
                title: "能效对标分析",
                icon: "peoples",
                noCache: true,
                link: ""
            }
        }]
    },
    {
        name: "supplyChain",
        path: "/supplyChain",
        hidden: false,
        redirect: "noRedirect",
        component: Layout,
        alwaysShow: true,
        meta: {
            title: "供应链碳管理",
            icon: "gongyingliantanguanli",
        },
        children: [{
            name: "supplier",
            path: "/supplyChain/supplier",
            hidden: false,
            component: (resolve) => require(['@/views/supplyChain/supplier'], resolve),
            query: "",
            meta: {
                title: "供应商",
                icon: "peoples",
                noCache: true,
                link: ""
            }
        }, {
            name: "materialManagement",
            path: "/supplyChain/materialManagement",
            hidden: false,
            component: (resolve) => require(['@/views/supplyChain/materialManagement'], resolve),
            query: "",
            meta: {
                title: "物料管理",
                icon: "peoples",
                noCache: true,
                link: ""
            }
        }, {
            name: "materialCarbonFootprint",
            path: "/supplyChain/materialCarbonFootprint",
            hidden: false,
            component: (resolve) => require(['@/views/supplyChain/materialCarbonFootprint'], resolve),
            query: "",
            meta: {
                title: "物料碳足迹",
                icon: "peoples",
                noCache: true,
                link: ""
            }
        }]
    },
    {
        name: "carbonManage",
        path: "/carbonManage",
        hidden: false,
        redirect: "noRedirect",
        component: Layout,
        alwaysShow: true,
        meta: {
            title: "碳排管理",
            icon: "tanpaiguanli",
        },
        children: [{
                name: "emissionCategory",
                path: "emissionCategory",
                hidden: false,
                component: (resolve) => require(['@/views/carbonManage/emissionCategory'], resolve),
                query: "",
                meta: {
                    title: "排放类别管理",
                    icon: "peoples",
                    noCache: true,
                    link: ""
                }
            },
            {
                name: "factorLibrary",
                path: "factorLibrary",
                hidden: false,
                component: (resolve) => require(['@/views/carbonManage/factorLibrary'], resolve),
                query: "",
                meta: {
                    title: "碳因子库",
                    icon: "peoples",
                    noCache: true,
                    link: ""
                }
            },
            {
                name: "checkComputation",
                path: "checkComputation",
                hidden: false,
                component: (resolve) => require(['@/views/carbonManage/checkComputation'], resolve),
                query: "",
                meta: {
                    title: "碳排核算",
                    icon: "peoples",
                    noCache: true,
                    link: ""
                }
            },
            {
                name: "checkAnalyse",
                path: "checkAnalyse",
                hidden: false,
                component: (resolve) => require(['@/views/carbonManage/checkAnalyse'], resolve),
                query: "",
                meta: {
                    title: "碳排分析",
                    icon: "peoples",
                    noCache: true,
                    link: ""
                }
            },
            {
                name: "cyclicalModel",
                path: "cyclicalModel",
                hidden: false,
                component: (resolve) => require(['@/views/carbonManage/cyclicalModel'], resolve),
                query: "",
                meta: {
                    title: "产品生命周期模型",
                    icon: "peoples",
                    noCache: true,
                    link: ""
                }
            },
            {
                name: "carbonFootprint",
                path: "carbonFootprint",
                hidden: false,
                component: (resolve) => require(['@/views/carbonManage/carbonFootprint'], resolve),
                query: "",
                meta: {
                    title: "产品碳足迹",
                    icon: "peoples",
                    noCache: true,
                    link: ""
                }
            },
            {
                name: "checkPlan",
                path: "checkPlan",
                hidden: false,
                component: (resolve) => require(['@/views/carbonManage/checkPlan'], resolve),
                query: "",
                meta: {
                    title: "碳排计划",
                    icon: "peoples",
                    noCache: true,
                    link: ""
                }
            },
            {
                name: "carbonAssets",
                path: "carbonAssets",
                hidden: false,
                component: (resolve) => require(['@/views/carbonManage/carbonAssets'], resolve),
                query: "",
                meta: {
                    title: "碳资产",
                    icon: "peoples",
                    noCache: true,
                    link: ""
                }
            },
        ]
    }
]

export default othersRoutes