<template>
	<view>
		<uni-nav-bar :status-bar="true" :fixed="true" :border="false" height="0rpx" :zIndex="1001">
			<template v-slot:allslot>
			</template>
		</uni-nav-bar>
		<view class="container" v-if="activeStep!=-3">
			<view class="page-title" v-if="activeStep !== -1 && activeStep !== -2&& activeStep !== -3">
				{{title}}
			</view>
			<guide @onGuideComplete="onGuideComplete" v-if="activeStep==0" :isAgain="isAgain"
				:isNewDevice="isNewDevice"></guide>

			<target-wifi-info v-if="activeStep==1" :title="'请选择设备要连接的WiFi'"
				@onTargetWifiInputComplete="onTargetWifiInputComplete"></target-wifi-info>

			<input-wifi-info ref="deviceInputwifi" v-if="activeStep==2" :ismode="setmode" :dtu="deviceId"
				title="连接设备WiFi" @onTargetDeviceInputComplete="onTargetDeviceInputComplete"></input-wifi-info>

			<do-config ref="doConnect" v-if="activeStep==3" @doConfigFun="doConfigFun" @doAfresh="doAfresh"
				:ismode="setmode" :isNewDevice="isNewDevice"></do-config>
			<success-view v-if="activeStep==-1" @connectSuccess="connectSuccess" @successAddDevice="successAddDevice"
				:dtu="deviceId"></success-view>
		</view>
		<error2-view v-if="activeStep==-3" @tryAgain="tryAgainMode"></error2-view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>

</template>

<script>
	import {
		addUseDevice
	} from '@/api/device.js'
	import {
		deviceIsOnline,
		devicesIsOnline
	} from '@/api/wifi.js'
	import doConfig from '@/components/distributionNetwork2/do-config.vue'
	import guide from '@/components/distributionNetwork2/guide.vue'
	import error2View from '@/components/distributionNetwork2/error2-view.vue'
	import successView from '@/components/distributionNetwork2/success-view.vue'
	import inputWifiInfo from '@/components/distributionNetwork2/input-wifi-info.vue'
	import targetWifiInfo from '@/components/distributionNetwork2/target-wifi-info'
	// import doConfig from '@/components/distributionNetwork/do-config.vue'
	// import guide from '@/components/distributionNetwork/guide.vue'
	// import error2View from '@/components/distributionNetwork/error2-view.vue'
	// import successView from '@/components/distributionNetwork/success-view.vue'
	// import inputWifiInfo from '@/components/distributionNetwork/input-wifi-info.vue'
	// import targetWifiInfo from '@/components/distributionNetwork/target-wifi-info'
	export default {
		components: {
			guide,
			inputWifiInfo,
			targetWifiInfo,
			doConfig,
			successView,
			error2View
		},
		data() {
			return {
				activeStep: 0,
				needDeviceAp: false,
				title: 'SoftAP distribution network',
				// title: 'SoftAP 配网',
				step: {
					Guide: 0,
					InputTargetWiFi: 1,
					InputDeviceWiFi: 2,
					DoConfig: 3,
					Success: -1,
					Fail: -3,
				},
				WifiStatus: {
					UDP_CONNECTED: 0,
					UDP_GET_SSID_AND_PWD: 1,
					DEVICE_CONNECTED_SUCCESS: 2,
					DEVICE_CONNECTED_FAIL: 3
				},
				// tcp: wx.createTCPSocket()
				lianJietargetWifi: null,
				scanParams: '',
				deviceId: null,
				setmode: false,
				isAgain: false,
				isEnglish: true,
				isNewDevice: false, //是否是新设备
				isOnline: false
			}
		},
		onLoad: function(option) {
			if (this.$store.state.user && this.$store.state.user.uid) {} else {
				this.$store.dispatch('GetInfo').then(() => {})
			}
			this.deviceId = option.dtuId;
			let num = Number(this.deviceId.substring(3))
			console.log("截取的字符串", num);
			let updateList=this.$store.state.updateDeviceList
			// console.log("包含的升级设备",updateList,updateList&&updateList.length>0&&updateList.includes(this.deviceId));
			if(updateList&&updateList.length>0&&updateList.includes(this.deviceId)){
				this.isNewDevice = true
			}else{
				if (num <= 2300065) {
					this.isNewDevice = false
				} else if (num > 2300065) {
					this.isNewDevice = true
				}
			}
			devicesIsOnline(this.deviceId).then(rs => {
				this.isOnline = rs.data
				console.log(rs, '是否在线');
			}).catch(err => {
				this.setMsgTop(err)
			})
			// this.deviceId = 'iot2300003';
			if (this.isEnglish) {
				this.title = 'SoftAP distribution network'
			} else {
				this.title = "SoftAP 配网"
			}
			this.isAgain = false
			if (option.mode == "setmode") {
				this.setmode = true;
			}
			//#ifdef MP-WEIXIN
			wx.startWifi()
			//#endif
			// #ifdef APP-PLUS
			uni.startWifi({
				success: (res) => {
					// console.log("res", res);
				}
			})
			//#endif
		},
		onUnload() {
			//#ifdef MP-WEIXIN
			wx.stopWifi()
			//#endif
			// #ifdef APP-PLUS
			uni.stopWifi({
				success: (res) => {
					// console.log("res", res);
				}
			})
			//#endif
		},
		methods: {
			onGuideComplete(val) {
				// guide的下一步点击事件
				if (val && val == 'fail') {
					this.activeStep = this.step.Fail
				} else {
					this.activeStep = this.step.InputTargetWiFi
				}

			},
			onTargetWifiInputComplete(targetWifi) {
				let that = this
				this.lianJietargetWifi = targetWifi
				this.activeStep = this.step.InputDeviceWiFi;
			},
			onTargetDeviceInputComplete() {
				this.activeStep = this.step.DoConfig
				this.$nextTick(() => {
					this.$refs.doConnect.setSocketConnect(this.lianJietargetWifi, this.deviceId, this.isOnline)
				})

			},
			tryAgain() {
				//重试
				this.lianJietargetWifi = null;
				this.isAgain = true
				if (this.isEnglish) {
					this.title = 'Setmode distribution network'
				} else {
					this.title = "Setmode 配网"
				}

				this.activeStep = this.step.Guide;
			},
			tryAgainMode() {
				//重试
				this.lianJietargetWifi = null;
				this.setmode = true;
				this.isAgain = true
				if (this.isEnglish) {
					this.title = 'Setmode distribution network'
				} else {
					this.title = "Setmode 配网"
				}
				this.activeStep = this.step.Guide;
			},
			doConfigFun(val) {
				if (val == 'ok') {
					this.activeStep = this.step.Success
				} else {
					this.activeStep = this.step.Fail
				}
			},
			doAfresh() {
				//新款设备重新配网
				this.activeStep = this.step.Guide;
			},
			connectSuccess() {
				//配网成功
				// console.log("配网成功跳转");
				// uni.switchTab({
				// 	url: '/pages/home/home'
				// })
				// this.$store.commit('SET_DEVICE_LIST', true)
				uni.switchTab({
					url: '/pages/devices/devices'
				})
			},
			successAddDevice() {
				//配网成功，添加设备
				let that = this
				that.$refs.promptMsg.loadingOpen('Adding...')
				// console.log("that.deviceId",that.deviceId);
				// let id='iot2300005'

				addUseDevice(that.deviceId).then(res2 => {
					// console.log("接口访问res",res2);
					if (res2.code == 0) {
						that.$refs.promptMsg.loadingColse()
						that.$refs.promptMsg.open('Added successfully', 2000)
						setTimeout(() => {
							this.$store.commit('SET_DEVICE_LIST', true)
							uni.switchTab({
								url: '/pages/devices/devices'
							})
						}, 2050)
					}
				}).catch(err => {
					that.$refs.promptMsg.loadingColse()
					// console.log("添加设备报错1111",err);
					that.setMsgTop(err)
				})
			}
		}
	}
</script>

<style lang="less">
</style>