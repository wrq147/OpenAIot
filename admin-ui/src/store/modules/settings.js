import defaultSettings from '@/settings'

const { sideTheme, showSettings, topNav, tagsView, fixedHeader, sidebarLogo, dynamicTitle,loginUrl,isNotAutoCreate,qywxAppId } = defaultSettings
let storageSetting;
const layoutSettingStr = localStorage.getItem('layout-setting');
if (layoutSettingStr) {
    try {
        storageSetting = JSON.parse(layoutSettingStr);
    } catch (error) {
        console.error('解析layout-setting从localStorage获取的值失败：', error);
        storageSetting = {};
    }
} else {
    storageSetting = {};
}
const state = {
  title: '',
  theme: storageSetting.theme || '#409EFF',
  sideTheme: storageSetting.sideTheme || sideTheme,
  showSettings: showSettings,
  topNav:  storageSetting.topNav === undefined ? topNav : storageSetting.topNav,
  tagsView: storageSetting.tagsView === undefined ? tagsView : storageSetting.tagsView,
  fixedHeader: storageSetting.fixedHeader === undefined ? fixedHeader : storageSetting.fixedHeader,
  sidebarLogo: storageSetting.sidebarLogo === undefined ? sidebarLogo : storageSetting.sidebarLogo,
  dynamicTitle: storageSetting.dynamicTitle === undefined ? dynamicTitle : storageSetting.dynamicTitle,
  loginUrl: storageSetting.loginUrl === undefined ? loginUrl : storageSetting.loginUrl,
  isNotAutoCreate: storageSetting.isNotAutoCreate === undefined ? isNotAutoCreate : storageSetting.isNotAutoCreate,
  qywxAppId: storageSetting.qywxAppId === undefined ? qywxAppId : storageSetting.qywxAppId,
}
const mutations = {
  CHANGE_SETTING: (state, { key, value }) => {
    if (state.hasOwnProperty(key)) {
      state[key] = value
    }
  }
}

const actions = {
  // 修改布局设置
  changeSetting({ commit }, data) {
    commit('CHANGE_SETTING', data)
  },
  // 设置网页标题
  setTitle({ commit }, title) {
    state.title = title
  }
}

export default {
  namespaced: true,
  state,
  mutations,
  actions
}

