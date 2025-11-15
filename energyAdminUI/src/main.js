import Vue from 'vue'

import Cookies from 'js-cookie'
import Vue2OrgTree from 'vue-tree-color'
Vue.use(Vue2OrgTree)
import './assets/styles/element-variables.scss'

import '@/assets/styles/index.scss' // global css
import '@/assets/styles/custom.scss' // 自定义的全局样式
import App from './App'
import store from './store'
import router from './router'
import directive from './directive' //directive
import plugins from './plugins' // plugins
import '@/assets/icons/customIcon/iconfont.css'
import './assets/icons' // icon
import './permission' // permission control
import { getDicts } from "@/api/system/dict/data";
import { getConfigKey } from "@/api/system/config";
import { parseTime, resetForm, addDateRange, selectDictLabel, selectDictLabels, handleTree, getDefalut, isNotEmpty } from "@/utils/common";
// 分页组件
let Pagination = () =>
    import ("@/components/Pagination")
    // 自定义表格工具组件
let RightToolbar = () =>
    import ("@/components/RightToolbar")
    // 富文本组件
let Editor = () =>
    import ("@/components/Editor")
    // 文件上传组件
let FileUpload = () =>
    import ("@/components/FileUpload")
    // 图片上传组件
let ImageUpload = () =>
    import ("@/components/ImageUpload")
    // 字典标签组件
let DictTag = () =>
    import ('@/components/DictTag')
    // 头部标签组件
import VueMeta from 'vue-meta'
// 字典数据组件
import DictData from '@/components/DictData'
// 图片预览组件
import Preview from "@/components/ImagePreview/preview.js";


import clipboard from 'clipboard'; //实现复制
Vue.prototype.clipboard = clipboard; //注册到vue原型上

// 全局方法挂载
Vue.prototype.getDicts = getDicts
Vue.prototype.getConfigKey = getConfigKey
Vue.prototype.parseTime = parseTime
Vue.prototype.resetForm = resetForm
Vue.prototype.addDateRange = addDateRange
Vue.prototype.selectDictLabel = selectDictLabel
Vue.prototype.selectDictLabels = selectDictLabels
Vue.prototype.handleTree = handleTree
Vue.prototype.$getDefalut = getDefalut
Vue.prototype.$isNotEmpty = isNotEmpty

// 全局组件挂载
Vue.component('DictTag', DictTag)
Vue.component('Pagination', Pagination)
Vue.component('RightToolbar', RightToolbar)
Vue.component('Editor', Editor)
Vue.component('FileUpload', FileUpload)
Vue.component('ImageUpload', ImageUpload)


Vue.use(Preview);
Vue.use(directive)
Vue.use(plugins)
Vue.use(VueMeta)
DictData.install()

//实现拖动滑动
import VueDragscroll from 'vue-dragscroll'
Vue.use(VueDragscroll)
Vue.directive("dragscroll", function(el) { //实现拖动滑动功能
    el.onmousedown = function(ev) {
        // console.log(el);
        const disX = ev.clientX;
        const disY = ev.clientY;
        const originalScrollLeft = el.scrollLeft;
        const originalScrollTop = el.scrollTop;
        const originalScrollBehavior = el.style["scroll-behavior"];
        const originalPointerEvents = el.style["pointer-events"];
        // auto: 默认值，表示滚动框立即滚动到指定位置。
        el.style["scroll-behavior"] = "auto";
        el.style["cursor"] = "grabbing";
        // 鼠标移动事件是监听的整个document，这样可以使鼠标能够在元素外部移动的时候也能实现拖动
        document.onmousemove = function(ev) {
            ev.preventDefault();
            // 计算拖拽的偏移距离
            const distanceX = ev.clientX - disX;
            const distanceY = ev.clientY - disY;

            el.scrollTo(
                originalScrollLeft - distanceX,
                originalScrollTop - distanceY
            );
            // console.log(
            //     originalScrollLeft - distanceX,
            //     originalScrollTop - distanceY
            // );
            // 在鼠标拖动的时候将点击事件屏蔽掉
            el.style["pointer-events"] = "none";
            document.body.style["cursor"] = "grabbing";
        };
        document.onmouseup = function() {
            document.onmousemove = null;
            document.onmouseup = null;
            el.style["scroll-behavior"] = originalScrollBehavior;
            el.style["pointer-events"] = originalPointerEvents;
            el.style["cursor"] = "grab";
        };
    };
});
//按需加载element组件
import element from './element'
Vue.use(element, {
    size: Cookies.get('size') || 'medium' // set element-ui default size
})
Vue.config.productionTip = false


new Vue({
    el: '#app',
    router,
    store,
    render: h => h(App)
})