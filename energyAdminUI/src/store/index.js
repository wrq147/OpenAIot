import Vue from 'vue'
import Vuex from 'vuex'
import app from './modules/app'
import datas from './modules/datas'
import orgLis from './modules/orgLis'
import user from './modules/user'
import tagsView from './modules/tagsView'
import permission from './modules/permission'
import settings from './modules/settings'
import flowable from './modules/flowable'
import rulesFlowable from './modules/rulesFlowable'
import mqttclient from './modules/mqttclient'
import getters from './getters'
import printTemplateModule from './modules/print/index.js'
import snapshot from './modules/snapshot'
Vue.use(Vuex)

const store = new Vuex.Store({
    modules: {
        app,
        datas,
        user,
        tagsView,
        permission,
        settings,
        flowable,
        rulesFlowable,
        mqttclient,
        orgLis,
        printTemplateModule,
        snapshot
    },
    getters
})

export default store