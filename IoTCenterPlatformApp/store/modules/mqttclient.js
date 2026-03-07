import {
	getClientId,
	getClientId2,
	getWebIp,
	asyncClientIdOperation
} from "@/api/mqtt";
import request from '@/common/request.js'
import {
	getToken
} from '@/common/auth.js'
import mqtt from '@/common/mqtt.js'
import store from '@/store'
import serverUrl from '@/common/constVar.js'
const mqttclient = {
	namespaced: true,
	state: {
		mqclient: null,
		msgHandlers: {},
		reconnectTimes:0
	},
	mutations: {
		set_reconnectTimes: (state, num) => {
			state.reconnectTimes = num;
		},
		CHANGE_CLIENT: (state, client) => {
			state.mqclient = client;
		},
		Add_Handler: (state, {
			key,
			func
		}) => {
			state.msgHandlers[key] = func;
		},
		Del_Handler: (state, key) => {
			delete state.msgHandlers[key];
		}
	},
	getters: {},
	actions: {
		getClient({
			commit,
			rootState
		}) {
			return new Promise(async (resolve, reject) => {

				try {
					if (mqttclient.state.mqclient != null) {
						// console.log("mqtt属性", mqttclient.state.mqclient);
						resolve(mqttclient.state.mqclient);
						return;
					}
					try {
						// console.log("退出登录后的查看");
						let ssl=false
						// #ifdef H5
						if(window.location.protocol === 'https:'){
							ssl=true
						}
						if (serverUrl.getServerUrl()!='/'&&serverUrl.getServerUrl().indexOf('https://')>-1) {
							ssl=true
						}
						// #endif
						
						let rsp = await getWebIp({
							ssl: ssl
						})
						let tmprsp = await getClientId();
						let options = {
							username: rootState.user.uid,
							password: "123",
							clean: true,
							connectTimeout: 3000,
							clientId: tmprsp.data,
							keepalive: 10,
							cbReconnect: async (c, next) => {
								// console.log('重连uuuuuuu',c,mqttclient.state.mqclient);
								try {
									let clirsp = await getClientId();
									c.options.clientId = clirsp.data;
									next()
								} catch (e) {
									console.log(e,'报错信息');
									//TODO handle the exception
									if (mqttclient.state.mqclient) {
										// mqttclient.state.mqclient._events.disconnect()
										if(e.code&&e.code==400||e.code&&e.code==401||e.code&&e.code==50012){
											// console.log("进到这里",e.code);
											mqttclient.state.mqclient._events.end()
										}else{
											
											if(store.state.isSetNotimeoutTips){
												mqttclient.state.mqclient._events.end()
											}
										}
										// mqttclient.state.mqclient._events.disconnect()
									}

								}

							}
						};
						// #ifdef H5
						let relUrl = "ws://" + rsp.data + "/mqtt";
						if(window.location.protocol === 'https:'){
							relUrl= "wss://" + rsp.data + "/mqtt";
						}
						if (serverUrl.getServerUrl()!='/'&&serverUrl.getServerUrl().indexOf('https://')>-1) {
							relUrl= "wss://" + rsp.data + "/mqtt";
						}
						
						// #endif
						// #ifdef MP-WEIXIN||APP-PLUS
						let relUrl = "wx://" + rsp.data + "/mqtt";
						if (serverUrl.getServerUrl()!='/'&&serverUrl.getServerUrl().indexOf('https://')>-1) {
							relUrl= "wxs://" + rsp.data + "/mqtt";
						}
						// #endif
						let that = this
						let xclient = mqtt.connect(relUrl, options)
						xclient.__proto__._reconnect = function(as) {
							console.info("dfsd",as,mqttclient.state.mqclient)
							if(mqttclient.state.mqclient==null) {
								xclient.__proto__._reconnect=()=>{}
								return
							}
							const next = () => {
								xclient.emit('reconnect')
								xclient._setupStream();
							}
							if (xclient.options.cbReconnect) {
								xclient.options.cbReconnect(xclient, next)
							} else {
								next()
							}
						}

						xclient.on('connect', () => {
							// let tkey = "newprop/" + this.deviceId;
							console.info("dddd")
							// xclient.subscribe(tkey);
							resolve(xclient);
						}).on('disconnect', function() {
							// console.info("ccccmqtt断开链接")
							commit('CHANGE_CLIENT', null);
						}).on('error', function(error) {
							// console.log("error", error);
						}).on('end', function() {
							// console.log('on end')
							// console.info("cccc11111")
							commit('CHANGE_CLIENT', null);
						}).on('message', function(topic, message) {
							// console.log(message, 'ttttt', topic)
							if (mqttclient.state.msgHandlers.hasOwnProperty(topic)) {
								mqttclient.state.msgHandlers[topic](message);
							}
						})
						commit('CHANGE_CLIENT', xclient);
						// console.log("请求连接", rsp);
					} catch (e) {
						console.log('eeeee', e);
						//TODO handle the exception
						if (mqttclient.state.mqclient&&e.code&&e.code==400||e.code&&e.code==401||e.code&&e.code==50012) {
							mqttclient.state.mqclient._events.end()
							// mqttclient.state.mqclient._events.disconnect()
							reject(e)
						}else{
							if(store.state.isSetNotimeoutTips){
								mqttclient.state.mqclient._events.end()
								reject(e)
							}
						}
					}


				} catch (error) {
					//TODO handle the exception
					reject(error)
				}

			})
		}
	}
}

export default mqttclient