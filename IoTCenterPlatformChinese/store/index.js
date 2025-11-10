import Vue from 'vue'
import Vuex from 'vuex'
import user from './modules/user.js'
import data from './modules/data.js'
import orgLis from './modules/orgLis.js'
import mqttclient from './modules/mqttclient.js'
import getters from './getters'
Vue.use(Vuex)
	import checkUpdate from '@/common/check-update.js'
import {
    orgStyle
} from '@/api/code.js'
const store=new Vuex.Store({
	modules: {
	        user,
			data,
			mqttclient,
			orgLis,
	},
	getters,
	state:{
		appversion:'',
		noReadNum:0,
		isLoadDevice:false,
		globalData:{},
		wifiList:[],
		messageComt:{
			messageCot:'提示信息',
			durationTime:0,
		},
		isReloadInventory:false,
		isReloadMessage:true,
		isReloadMessageList:false,
		rulesAddForm:{},
		otherRulesForm:{},
		updateDeviceList:[],
		appLogoUrl:'',//app图标路径/static/images/jiayun_logo.png//'/static/images/logo.png'///static/images/logo.png,/static/images/weichenlogo.png
		isOnlyLogo:true,//logo展开，只显示logo
		appNameText:'',//app名称嘉瑜智能//悟空云//物信科技//悟空云//天琪智能
		logType:'',//登录类型//发布别忘记修改manifest.json文件
		loginOrg:'265479371714629',//悟空云：161821026938949，物信科技：238314281250885//系统：0//新的测试厂家265099977453637
		//测试厂家：166813412991045,力达集团：262592462065733,玮晟机械:265479371714629,//天琪智能225913046712389//泉州振大电气有限公司:322100070375493//云能：324652669624389
		loginThemeId:'',//当前企业主题
		checkUpdatesThemeId:'266149997854789',//检验app更新版本的主题//悟空云：338360098664517，物信科技：238314281250885//系统：''//新的测试厂家243970991366213
		//力达集团：262706057551941,玮晟机械:266149997854789,//天琪智能270796910747717//363470537969733
		//泉州振大电气有限公司322218277240901//云能：324669821296709//橙子物联：332080844304453
		isNotAutoCreate:true,
		qywxAppId:'',
		isSetNotimeoutTips:false,//是否显示超时提示
		isRelogin:false,//是否允许重新加载接口
		mobileNavType:'default',//移动端导航类型default默认//workOrder工单
		homejumpurl:'',//移动端导航类型default默认//workOrder工单
		hasLoadVersion:false,//是否有检测过app版本
		// promptComptParams:{
		// 	type:'message',//提示类型
		// 	operate:'open',//执行操作
		// 	message:'提示信息',//A提示信息内容
		// 	title:'System prompt information',
		// 	duration:2000,//提示信息关闭延迟时间
		// 	key:0
		// },
		// promptOperateNum:0,
	},
	mutations:{
		// SET_PROMPT_PARAMS(state,info){//全局设置消息提示
		// console.log("info1",info);
		// 	if(info.type){
		// 		state.promptComptParams.type=info.type
		// 	}
		// 	if(info.operate){
		// 		state.promptComptParams.operate=info.operate
		// 	}
		// 	if(info.message){
		// 		state.promptComptParams.message=info.message
		// 	}
		// 	if(info.duration){
		// 		state.promptComptParams.duration=info.duration
		// 	}
		// 	state.promptComptParams.key=state.promptOperateNum+1
		// 	state.promptOperateNum=state.promptOperateNum+1
		// 	console.log(state.promptComptParams,'state.promptComptParams');
		// },
		SET_isRelogin(state,info){
			state.isRelogin=info
		},
		SET_isSetNotimeoutTips(state,info){
			state.isSetNotimeoutTips=info
		},
		SET_VERSION_NUMBER(state,info){
			state.appversion=info
		},
		SET_MESSAGE_COMPT(state,info){
			state.messageComt=info
		},
		SET_GLOBAL_DATA(state,info){
			state.globalData=info
		},
		SET_WIFI_LIST(state,info){
			state.wifiList=info
		},
		SET_DEVICE_LIST(state,info){//设置设备列表重新加载
			state.isLoadDevice=info
		},
		SET_INVENTORY_INFO(state,info){//设置库存信息重新加载
			state.isReloadInventory=info
		},
		SET_MESSAGE_INFO(state,info){//设置未读消息方法重新加载
			state.isReloadMessage=info
		},
		SET_MESSAGELIST_INFO(state,info){//消息列表重新加载
			state.isReloadMessageList=info
		},
		SET_NOREAD_INFO(state,info){//消息未读数量重新加载
			state.noReadNum=info
		},
		SET_RULESADD_INFO(state,info){//规则信息参数
			state.rulesAddForm=info
		},
		SET_OTHERRULES_INFO(state,info){//规则中转参数
			state.otherRulesForm=info
		},
		SET_UPDATE_DEVICE(state,info){//消息未读数量重新加载
			state.updateDeviceList=info
		},
		SET_loginOrg(state,info){//h5登录组织
			state.loginOrg=info
		},
		SET_appLogoUrl(state,info){//app logo地址
			state.appLogoUrl=info
		},
		SET_appNameText(state,info){//app名称
			state.appNameText=info
		},
		SET_isNotAutoCreate(state,info){//是否没有自定义创建账号
			state.isNotAutoCreate=info
		},
		SET_qywxAppId(state,info){//企业微信appid
			state.qywxAppId=info
		},
		SET_mobileNavType(state,info){//是否展开logo
			state.mobileNavType=info
		},
		SET_homejumpurl(state,info){//是否展开logo
			state.homejumpurl=info
		},
		SET_loginThemeId(state,info){//设置主题id
			state.loginThemeId=info
		},
		SET_hasLoadVersion(state,info){//设置主题id
			state.hasLoadVersion=info
		},
	},
	actions:{
		// 设置app主题信息
		setThemeInfo({
			commit,
			state
		},orgId) {
			return new Promise((resolve, reject) => {
				orgStyle({orgId:orgId}).then(res => {
					// console.log("主题设置",res);
					if(res.data){
						let styleJson=''
						if(res.data.StyleJson){
							styleJson=JSON.parse(res.data.StyleJson)
							if(styleJson.mobileLogo.indexOf('http')===-1){
								styleJson.mobileLogo=location.origin+styleJson.mobileLogo
							}
							// console.log('styleJson.mobileLogo',styleJson.mobileLogo);
							commit('SET_appLogoUrl', styleJson.mobileLogo);
							commit('SET_isNotAutoCreate', styleJson.isNotAutoCreate);
							
							commit('SET_appNameText', res.data.Name)
							commit('SET_loginThemeId', res.data.Id)
							commit('SET_loginOrg', orgId)
							
							if(styleJson.mobileNavType){
								commit('SET_mobileNavType', styleJson.mobileNavType)
								if(styleJson.mobileNavType=='workOrder'){
									commit('SET_homejumpurl', '/pages/index/index2')
								}else{
									commit('SET_homejumpurl', '')
								}
							}else{
								commit('SET_mobileNavType', 'default')
								commit('SET_homejumpurl', '')
							}
						}
					}else{
						commit('SET_appLogoUrl', '');
						commit('SET_isNotAutoCreate', true);
						
						commit('SET_appNameText', '')
						commit('SET_loginThemeId', '')
						commit('SET_loginOrg', orgId)
						
						commit('SET_mobileNavType', 'default')
						commit('SET_homejumpurl', '')
					}
					// #ifdef APP-PLUS
					// if(!state.hasLoadVersion){//进入app后有检测过版本则不在检测更新
					// 	checkUpdate();
					// 	commit('SET_hasLoadVersion',true)
					// }
					
					// #endif
					resolve(res)
				}).catch(error => {
					reject(error)
				})
			})
		},
	},
})
export default store