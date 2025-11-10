
// #ifndef VUE3
import Vue from 'vue'
import App from './App'
import VueCompositionAPI from '@vue/composition-api'
Vue.use(VueCompositionAPI)
import store from './store'
Vue.config.productionTip = false
import mixin from '@/mixins/mixin.js'
Vue.mixin(mixin)
import { resetForm,isNotEmpty,addDateRange,parseTime } from "@/common/utillib.js";
Vue.prototype.resetForm = resetForm
Vue.prototype.$isNotEmpty = isNotEmpty
Vue.prototype.addDateRange = addDateRange
Vue.prototype.parseTime = parseTime
App.mpType = 'app'
const app = new Vue({
	store,
    ...App
})
Vue.prototype.$bus = new Vue();
app.$mount()
// #endif

// #ifdef VUE3
import { createSSRApp } from 'vue'
import App from './App.vue'
export function createApp() {
  const app = createSSRApp(App)
  return {
    app
  }
}
// #endif