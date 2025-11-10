import App from './App'
import Vue from 'vue'
import store from './store'
import validate from '@/common/ys-validate.js'
import '@/common/date.js'
import './style/iconfont.css'

import tabBar from "./components/card-tab-bar.vue";

Vue.component('tabBar',tabBar);

Vue.config.productionTip = false
App.mpType = 'app'
Vue.prototype.$store = store
Vue.prototype.$validate = validate

const app = new Vue({
	store,
    ...App
})
app.$mount()