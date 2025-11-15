import Vue from 'vue'
import store from '@/store'
import {
    orgStyle,
} from "@/api/system/StyleMan";
import defaultSettings from '@/settings'
import { getConfigKey } from "@/api/system/config.js";
const { sideTheme, topNav, tagsView, sidebarLogo, dynamicTitle, loginUrl, isNotAutoCreate, qywxAppId } = defaultSettings

function getUrlRelativePath(url) {
    if (url.indexOf("//") != -1) {
        var arrurl = url.split("//");
        var start = arrurl[1].indexOf("/");
        var relUrl = arrurl[1].substring(start);
        if (relUrl.indexOf("?") != -1) {
            relUrl = relUrl.split("?")[0];
        }
        if (relUrl.indexOf("/#") == 0) {
            relUrl = relUrl.substr(2);
        }
        return relUrl;
    } else {
        var relUrl = url.substring(start);
        if (relUrl.indexOf("?") != -1) {
            relUrl = relUrl.split("?")[0];
        }
        if (relUrl.indexOf("/#") == 0) {
            relUrl = relUrl.substr(2);
        }
        return relUrl;
    }
}
export function judgeWhite(whiteList, str) {
    for (let i = 0; i < whiteList.length; i++) {
        let item = whiteList[i]
        let relapath = getUrlRelativePath(item);
        if (i && str.indexOf(relapath) !== -1) {
            return true
        }
    }
    return false
}
export async function loginThemeInfo(orgId) {
    let res = await orgStyle({ orgId: orgId })
    if (res.data) {
        let StyleJson = {}
        let pcLoginUrl = ''
        if (res.data.StyleJson) {
            StyleJson = JSON.parse(res.data.StyleJson)
            if (StyleJson.pcLoginUrl) {
                pcLoginUrl = StyleJson.pcLoginUrl
            }
        }
        return pcLoginUrl
    } else {
        return ''
    }
}
export async function loadOrgTheme(orgId) {
    new Vue().$cache.local.remove("layout-setting")
    let res = await orgStyle({ orgId: orgId })
    let info = null
        // console.log("主题信息", res.data);
    if (res.data) {
        // console.log("主题信息", res.data);
        info = {
            themeName: res.data.Name,
            PhotoUrl: res.data.PhotoUrl,
            Remark: res.data.Remark,
            themeId: res.data.Id,
            isTopNav: (JSON.parse(res.data.StyleJson)).isTopNav,
            isTagsViews: (JSON.parse(res.data.StyleJson)).isTagsViews,
            isShowLogo: (JSON.parse(res.data.StyleJson)).isShowLogo,
            isActiveTiltle: (JSON.parse(res.data.StyleJson)).isActiveTiltle,
            themeType: (JSON.parse(res.data.StyleJson)).themeType,
            loginUrl: (JSON.parse(res.data.StyleJson)).pcLoginUrl,
            isNotAutoCreate: (JSON.parse(res.data.StyleJson)).isNotAutoCreate ? (JSON.parse(res.data.StyleJson)).isNotAutoCreate : true,
            qywxAppId: (JSON.parse(res.data.StyleJson)).qywxAppId ? (JSON.parse(res.data.StyleJson)).qywxAppId : '',
        }
        saveSetting(info, true)
    } else {
        info = null
        let configNav = ''
        let res = await getConfigKey("system.nav")
        if (res.data && res.data == "top") {
            configNav = true
        } else {
            configNav = false
        }
        saveSetting(info, true, configNav)
    }
}
export function saveSetting(themeInfo, isNoeLoading, configNav) {
    if (!isNoeLoading) {
        new Vue().$modal.loading("正在保存到本地，请稍后...");
    }
    if (themeInfo) {} else {
        themeInfo = {
            themeType: sideTheme,
            isActiveTiltle: dynamicTitle,
            isShowLogo: sidebarLogo,
            isTagsViews: tagsView,
            isTopNav: topNav,
            loginUrl: loginUrl ? loginUrl : '',
            isNotAutoCreate: isNotAutoCreate,
            qywxAppId: qywxAppId
        }
        if (configNav !== undefined) {
            themeInfo.isTopNav = configNav
        }
    }
    // console.log(themeInfo,'themeInfo');
    store.dispatch('settings/changeSetting', {
        key: 'sideTheme',
        value: themeInfo.themeType
    })
    store.dispatch('settings/changeSetting', {
        key: 'dynamicTitle',
        value: themeInfo.isActiveTiltle
    })
    store.dispatch('settings/changeSetting', {
        key: 'sidebarLogo',
        value: themeInfo.isShowLogo
    })
    store.dispatch('settings/changeSetting', {
        key: 'tagsView',
        value: false
    })
    store.dispatch('settings/changeSetting', {
        key: 'topNav',
        value: false
    })
    store.dispatch('settings/changeSetting', {
        key: 'isNotAutoCreate',
        value: themeInfo.isNotAutoCreate
    })
    store.dispatch('settings/changeSetting', {
        key: 'qywxAppId',
        value: themeInfo.qywxAppId
    })
    if (themeInfo.loginUrl) {
        store.dispatch('settings/changeSetting', {
            key: 'loginUrl',
            value: themeInfo.loginUrl
        })
    } else {
        themeInfo.loginUrl = ''
    }

    store.commit("SET_SIDEBAR_ROUTERS", store.state.permission.defaultRoutes);
    new Vue().$cache.local.set(
        "layout-setting",
        `{
          "topNav":${false},
          "tagsView":${false},
          "fixedHeader":true,
          "sidebarLogo":${themeInfo.isShowLogo},
          "dynamicTitle":${themeInfo.isActiveTiltle},
          "sideTheme":"${themeInfo.themeType}",
          "theme":"#1890FF",
          "loginUrl":"${themeInfo.loginUrl}",
          "qywxAppId":"${themeInfo.qywxAppId}",
          "isNotAutoCreate":${themeInfo.isNotAutoCreate}
        }`
    );
    if (!isNoeLoading) {
        setTimeout(new Vue().$modal.closeLoading(), 1000)
    }

}