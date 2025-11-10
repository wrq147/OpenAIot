import Vue from 'vue'
import Vuex from 'vuex'
import user from './modules/user.js'
import data from './modules/data.js'
import orgLis from './modules/orgLis.js'
import mqttclient from './modules/mqttclient.js'
import getters from './getters'
Vue.use(Vuex)

const store=new Vuex.Store({
	modules: {
	        user,
			data,
			mqttclient,
			orgLis,
	},
	getters,
	state:{
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
		updateDeviceList:[],
		isReloadReport:false,
		isSetNotimeoutTips:false,//是否显示超时提示
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
		SET_isSetNotimeoutTips(state,info){
			state.isSetNotimeoutTips=info
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
		SET_UPDATE_DEVICE(state,info){//消息未读数量重新加载
			state.updateDeviceList=info
		},
		SET_UPDATE_REPORT(state,info){//消息未读数量重新加载
			state.isReloadReport=info
		}
		
	},
	actions:{
		
	},
})
export default store