<template>
	<view>
		<view class="page-subtitle">Please select the WiFi device you want to connect to</view>
		<view class="wifi-form">
			<view class="tip-txt" v-if="!isIOS">
				<!-- 如果点击下一步，无法连接Wi-Fi，请您手动连接{{connssid}}的Wi-Fi(密码<text class="red">12345678</text>)，然后再次点击下一步 -->
				If you click Next and cannot connect to Wi Fi, please manually connect to WiFi on {{connssid}} (password <text class="red">12345678</text>), and then click Next again
			</view>
			<view class="tip-txt" v-if="isIOS">
				<!-- 如果点击下一步，没有反应，请先手动连接{{connssid}}的Wi-Fi(密码<text class="red">12345678</text>)，然后再点击下一步 -->
				If there is no response when clicking Next, please manually connect to WiFi on {{connssid}} (password <text class="red">12345678</text>) first, and then click Next
			</view>
		</view>
		<btn-group :buttons="[{ btnText: 'Next', type: 'primary', id: 'complete'}]"
			@onBottomButtonClick="onBottomButtonClick" :fixed-bottom="true" :loading="loading" />
	</view>
</template>

<script>
	import btnGroup from '@/components/distributionNetwork/btn-group.vue'
	import {
		deviceIsOnline,
		devicesIsOnline
	} from '@/api/wifi.js'
	export default {
		name: "target-wifi-info",
		components: {
			btnGroup
		},
		props: {
			title: {
				type: String,
			},
			dtu: {
				type: String
			},
			ismode: {
				type: Boolean
			}
		},
		data() {
			return {
				loading: false,
				dtuId: null,
				setmode: false,
				jmpConn: false,
				connssid: ""
			};
		},
		computed: {
			isIOS() {
				return uni.getSystemInfoSync().platform == 'ios';
			},
		},
		watch: {
			dtu: {
				handler(newValue, oldValue) {
					this.dtuId = newValue;
				},
				immediate: true
			},
			ismode: {
				handler(newValue, oldValue) {
					this.setmode = newValue;
				},
				immediate: true
			}
		},
		mounted() {
			if (this.dtuId == null) {
				this.connssid = this.setmode ? "enter_set" : "iot前缀";
			} else {
				this.connssid = this.setmode ? "enter_set" : this.dtuId;
			}
		},
		methods: {
			onBottomButtonClick(e) {
				//按钮事件
				// console.log("打印按妞信息", e, e.btn.id);
				switch (e.btn.id) {
					case 'complete':
						this.onClickComplete(e);
						break;
				}
			},
			onClickComplete(val) {
				// if (this.isIOS) {
				// 	this.$emit('onTargetDeviceInputComplete')
				// 	return;
				// }
				this.loading = true
				//#ifdef MP-WEIXIN
				wx.stopWifi()
				//#endif
				// #ifdef APP-PLUS
				uni.stopWifi({
					success: (res) => {
						console.log("res", res);
					}
				})
				//#endif
				//#ifdef MP-WEIXIN
				wx.startWifi()
				//#endif
				// #ifdef APP-PLUS
				uni.startWifi({
					success: (res) => {
						console.log("res", res);
					}
				})
				//#endif
				//#ifdef MP-WEIXIN
				if (this.dtuId == null) {
					// this.jmpConn = true;
					wx.connectWifi({
						maunal: true,
						complete: () => {
							this.loading = false;
						}
					})
					return;
				}
				let this_ = this
				wx.getConnectedWifi({
					success: async (rsp) => {
						// console.log("当前连接的wifi",res,res.wifi.SSID,this.deviceId);
						if (rsp.wifi && rsp.wifi.SSID && rsp.wifi.SSID == this_.connssid) {
							// try{
							// 	let rs = await devicesIsOnline(this.dtuId);
							// 	console.log(rs, '是否在线');
							// 	this_.$emit('onTargetDeviceInputComplete',rs.data)
							// }catch(e){
							// 	//TODO handle the exception
							// 	this_.$emit('onTargetDeviceInputComplete',false)
							// }
							this_.$emit('onTargetDeviceInputComplete')
						} else {
							this.loading=true
							this.tryConnect()
						}
					},
					fail: (fail) => {
						this.loading=true
						this.tryConnect()
					},
					complete: (any) => {
						console.log('any', any);
						// this.loading = false;
					}
				})
				//#endif
				// #ifdef APP-PLUS 
				if (this.dtuId == null) {
					// this.jmpConn = true;
					uni.connectWifi({
						maunal: true,
						complete: () => {
							this.loading = false;
						}
					})
					return;
				}
				let this_ = this
				uni.getConnectedWifi({
					success: (rsp) => {
						console.log("当前连接的wifi",rsp.wifi, this_.connssid, this.setmode, this.ismode);
						if (rsp.wifi && rsp.wifi.SSID && rsp.wifi.SSID == this_.connssid) {
							setTimeout(async ()=>{
								// this_.$emit('onTargetDeviceInputComplete')
								this_.$emit('onTargetDeviceInputComplete')
							},100)
						} else {
							this.loading=true
							this.tryConnect()
						}
					},
					fail: (fail) => {
						console.log("当前连接的wififail", fail);
						this.loading=true
						this.tryConnect()
					},
					complete: (any) => {
						console.log('any', any);
						// this.loading = false;
					}
				})
				//#endif
			},
			tryConnect() {
				let this_=this
				//#ifdef MP-WEIXIN
				wx.connectWifi({
					SSID: this.connssid,
					password: "12345678",
					forceNewApi: true,
					success: (res) => {
						uni.getConnectedWifi({
							success: async (rsp) => {
								if (rsp.wifi && rsp.wifi.SSID && rsp.wifi.SSID == this_.connssid) {
									this_.$emit('onTargetDeviceInputComplete')
									// try{
									// 	let rs = await devicesIsOnline(this.dtuId);
									// 	console.log(rs, '是否在线');
									// 	this_.$emit('onTargetDeviceInputComplete',rs.data)
									// }catch(e){
									// 	//TODO handle the exception
									// 	this_.$emit('onTargetDeviceInputComplete',false)
									// }
								}else{
									this.loading = false;
									wx.connectWifi({
										maunal: true,
										success(res) {
											console.info(res);
										},
										fail(err) {}
									})
								}
							},
							fail: (fail) => {
								this.loading = false;
								wx.connectWifi({
									maunal: true,
									success(res) {
										console.info(res);
									},
									fail(err) {}
								})
							},
						})
				
					},
					fail: (err) => {
						console.log("111111111");
						// this.jmpConn = true;
						wx.connectWifi({
							maunal: true,
							success(res) {
								console.info(res);
							},
							fail(err) {}
						})
						this.loading = false;
					},
					complete: () => {
						// this.loading = false;
					}
				})
				//#endif
				// #ifdef APP-PLUS 
				uni.connectWifi({
					SSID: this.connssid,
					password: "12345678",
					forceNewApi: true,
					success: (res) => {
						uni.getConnectedWifi({
							success: async (rsp) => {
								if (rsp.wifi && rsp.wifi.SSID && rsp.wifi.SSID == this_.connssid) {
									this_.$emit('onTargetDeviceInputComplete')
									// try{
									// 	let rs = await devicesIsOnline(this.dtuId);
									// 	console.log(rs, '是否在线');
									// 	this_.$emit('onTargetDeviceInputComplete',rs.data)
									// }catch(e){
									// 	//TODO handle the exception
									// 	this_.$emit('onTargetDeviceInputComplete',false)
									// }
								}else{
									this.loading = false;
									uni.connectWifi({
										maunal: true,
										success(res) {
											console.info(res);
										},
										fail(err) {}
									})
								}
							},
							fail: (fail) => {
								this.loading = false;
								uni.connectWifi({
									maunal: true,
									success(res) {
										console.info(res);
									},
									fail(err) {}
								})
							},
						})
					},
					fail: (err) => {
						console.info(err)
						// this.jmpConn = true;
						uni.connectWifi({
							maunal: true,
							success(res) {
								console.info(res);
							},
							fail(err) {}
						})
						this.loading = false;
					},
					complete: () => {
						// this.loading = false;
					}
				})
				//#endif
			}

		}

	}
</script>

<style lang="less">
	.wifi-form {
		// width: 750rpx;
		margin-top: 80rpx;
	}

	.tip-txt {
		font-size: 32rpx;
		line-height: 48rpx;
		color: #333;

		.red {
			color: rgba(255, 53, 53, 1);
		}
	}
</style>