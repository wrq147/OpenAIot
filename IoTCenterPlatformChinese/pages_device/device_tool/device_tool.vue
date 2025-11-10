<template>
	<view>
		<top title="设备dtuid修改小工具" leftWidth="157rpx" leftIcon="icon-fanhui" rightWidth="157rpx" :isleftBack="true"
			backgroundColor="rgba(255, 255, 255, 1)">
		</top>
		<view class="wifi-form">
			<view class="weui-cells">
				<view class="weui-cell weui-cell_select">
					<view class="weui-cell__hd weui-cell__hd_in-select-after">
						<view class="weui-label">WiFi</view>
					</view>
					<view class="weui-cell__bd">
						<view v-if="isIOS">
							<input v-model="selectedWifiSSID" class="weui-input" placeholder="请输入连接的wifi的SSID"
								placeholder-class="form-placeholder" placeholder-style="color:#D8D8D8" />
						</view>
						<view class="can_select_wifi" v-else @click="openWifiSelectList"
							:class="{'selected_wifi':selectedWifiSSID}">
							<view class="text">{{selectedWifiSSID?selectedWifiSSID:'请选择wifi'}}</view>
							<uni-icons type="right" size="20" color=" #D8D8D8"></uni-icons>
						</view>
					</view>
				</view>
				<view class="weui-cell weui-cell_input">
					<view class="weui-cell__hd">
						<view class="weui-label">改后dtuid</view>
					</view>
					<view class="weui-cell__bd">
						<input class="weui-input" placeholder="请输入修改后的dtuid" placeholder-class="form-placeholder"
							placeholder-style="color:#D8D8D8" v-model="editDtuid" />
					</view>
				</view>
			</view>
			<button class="submit_button" @click="onClickComplete" :disabled="loading"
				:style="{'opacity':loading?0.6:1,'margin-top': '60rpx'}" :loading="loading">
				确认修改
			</button>
		</view>
		
		<jp-select ref="wifipick" :checkAll="false" :list="wifiListList" :item="selectedWifiSSID" select="radio"
			@checked="onWifiPickerSelect" tite="请选择要链接的wifi"></jp-select>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		isLocationPermissionGranted,
		requestLocationPermission
	} from '@/pages_device/components/distributionNetwork/perm.js'
	//#ifndef MP-WEIXIN
	const TCPSocket = uni.requireNativePlugin('Aimer-TCPPlugin');
	//#endif
	export default {
		data() {
			return {
				loading:false,
				wifiListList: [],
				selectedWifiSSID: '',
				editDtuid: '', //输入的wifi密码
				hasGetWifi: false
			};
		},
		computed: {
			isIOS() {
				return uni.getSystemInfoSync().platform == 'ios';
			},
		},
		mounted() {
			this.wifiListList = [];
			//#ifdef MP-WEIXIN
			wx.stopWifi()
			wx.startWifi()
			wx.onGetWifiList(this.onLoadWifiList);
			//#endif
			//#ifndef MP-WEIXIN
			// #ifdef APP-PLUS
			uni.stopWifi({
				success: (res) => {
				}
			})
			uni.startWifi({
				success: (res) => {
				}
			})
			uni.onGetWifiList(this.onLoadWifiList);
			//#endif
			//#endif
		},
		unmounted() {
			//#ifdef MP-WEIXIN
			wx.stopWifi()
			wx.startWifi()
			wx.offGetWifiList(this.onLoadWifiList);
			//#endif
			//#ifndef MP-WEIXIN
			// #ifdef APP-PLUS
			uni.stopWifi({
				success: (res) => {
				}
			})
			uni.startWifi({
				success: (res) => {
				}
			})
			uni.offGetWifiList(this.onLoadWifiList);
			//#endif
			//#endif
		},
		methods: {
			onLoadWifiList(res) {
				let wifilist = res.wifiList;
				let wifiiids = wifilist.map((x) => {
					return x.SSID;
				});
				this.wifiListList.push(...wifiiids);
				this.wifiListList = Array.from(new Set(this.wifiListList));
				this.hasGetWifi = true;
				uni.hideLoading();
			},

			async openWifiSelectList() {

				if (!await requestLocationPermission()) {
					return;
				}
				uni.showLoading({
					title: '加载中'
				});

				setTimeout(() => {
					if (!this.hasGetWifi) {
						uni.hideLoading();
						// uni.showToast({
						// 	type:'none',
						// 	title: '请确认是否开启位置定位功能',
						// 	duration: 2000
						// });
						uni.showModal({
							title: '系统提示',
							content: '请确认是否开启位置定位功能以及wifi功能',
							showCancel: false,
							success: (res) => {
								this.$refs.wifipick.toCancel();
								if (res.confirm) {
									//#ifdef MP-WEIXIN
									wx.stopWifi()
									//#endif
									// #ifdef APP-PLUS
									uni.stopWifi({
										success: (res) => {
										}
									})
									//#endif
									//#ifdef MP-WEIXIN
									wx.startWifi()
									//#endif
									// #ifdef APP-PLUS
									uni.startWifi({
										success: (res) => {
										}
									})
									//#endif
								}
							}
						});
					}

				}, 4000);
				//#ifdef MP-WEIXIN
				wx.stopWifi()
				wx.startWifi()
				wx.offGetWifiList(this.onLoadWifiList)
				wx.onGetWifiList(this.onLoadWifiList);
				wx.getWifiList();

				//#endif
				// #ifdef APP-PLUS
				uni.stopWifi({
					success: (res) => {
					}
				})
				uni.startWifi({
					success: (res) => {
					}
				})
				uni.offGetWifiList(this.onLoadWifiList);
				uni.onGetWifiList(this.onLoadWifiList);
				uni.getWifiList({
					complete: res => {}
				});
				//#endif
				this.wifiListList = [];
				this.$refs.wifipick.toOpen();

			},
			onWifiPickerSelect(val) {
				this.selectedWifiSSID = val;
			},
			onClickComplete() {
				
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
				let this_ = this
				wx.getConnectedWifi({
					success: async (rsp) => {
						// console.log("当前连接的wifi",res,res.wifi.SSID,this.deviceId);
						if (rsp.wifi && rsp.wifi.SSID && rsp.wifi.SSID == this_.selectedWifiSSID) {
							if(this.editDtuid){
								this.appConnFunc();
							}else{
								this.loading = false;
								this.$refs.promptMsg.noticeOpen('请填写修改后的dtuid',
									'系统消息', true)
							}
						} else {
							this.loading = true;
							this.tryConnect()
						}
					},
					fail: (fail) => {
						this.loading = true;
						this.tryConnect()
					},
					complete: (any) => {
						console.log('any', any);
						// this.loading = false;
					}
				})
				//#endif
				// #ifdef APP-PLUS 
				let this_ = this
				uni.getConnectedWifi({
					success: async (rsp) => {
						if (rsp.wifi && rsp.wifi.SSID && rsp.wifi.SSID == this_.selectedWifiSSID) {
							if(this.editDtuid){
								TCPSocket.connect({
										//charsetname:'GBK',//可不选,默认UTF-8,针对服务端数据的字符集格式转化
										channel: '1', //可选 1~20
										ip: '192.168.77.1',
										port: '6888'
									},
									this.appConnFunc
								);
							}else{
								this.loading = false;
								this.$refs.promptMsg.noticeOpen('请填写修改后的dtuid',
									'系统消息', true)
							}
						} else {
							this.loading = true;
							this.tryConnect()
						}
					},
					fail: (fail) => {
						// console.log("当前连接的wififail", fail);
						this.loading = true;
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
					SSID: this.selectedWifiSSID,
					password: "12345678",
					forceNewApi: true,
					success: (res) => {
						uni.getConnectedWifi({
							success: async (rsp) => {
								if (rsp.wifi && rsp.wifi.SSID && rsp.wifi.SSID == this_.selectedWifiSSID) {
									if(this.editDtuid){
										this.appConnFunc();
									}else{
										this.loading = false;
										this.$refs.promptMsg.noticeOpen('请填写修改后的dtuid',
											'系统消息', true)
									}
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
					SSID: this.selectedWifiSSID,
					password: "12345678",
					forceNewApi: true,
					success: (res) => {
						uni.getConnectedWifi({
							success: async (rsp) => {
								if (rsp.wifi && rsp.wifi.SSID && rsp.wifi.SSID == this_.selectedWifiSSID) {
									if(this.editDtuid){
										TCPSocket.connect({
												//charsetname:'GBK',//可不选,默认UTF-8,针对服务端数据的字符集格式转化
												channel: '1', //可选 1~20
												ip: '192.168.77.1',
												port: '6888'
											},
											this.appConnFunc
										);
									}else{
										this.loading = false;
										this.$refs.promptMsg.noticeOpen('请填写修改后的dtuid',
											'系统消息', true)
									}
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
			},
			appConnFunc(result) {
				//#ifdef MP-WEIXIN
				const tcp = wx.createTCPSocket();
				tcp.connect({
					address: '192.168.77.1',
					port: 6888
				})
				//连接成功建立的时候触发该事件
				tcp.onConnect((e) => {
					tcp.write("AT*DTUID="+this.editDtuid+"#"+ "\r\n"+"AT*RESTART#")
					let times = 4000
					// console.log(this.isOnline, 'isOnlineisOnlineisOnline');
			
				})
				tcp.onError((err) => {
					++this.connCount;
					//TCP断开连接
			
					if (this.connCount < 10) {
						//再次尝试连接
						setTimeout(() => {
							this.appConnFunc();
						}, 1000);
					} 
			
			
				})
				tcp.onMessage((msg) => {
					let buffer = msg.message
					//ArrayBuffer字符串转换，很重要
					let unit8Arr = new Uint8Array(msg.message);
					let encodedString = String.fromCharCode.apply(null, unit8Arr);
					let hexstr = decodeURIComponent(escape((encodedString)));
					if (hexstr.substr(0, 2) == 'OK') {
						uni.showToast({
							title: '修改成功',
							icon:'success',
							duration:2000
						})
						setTimeout(()=>{
							uni.navigateBack()
						},2000)
					} else {
						this.loading = false;
						this.$refs.promptMsg.noticeOpen('修改失败，请重新尝试？',
							'系统消息', true)
					}
				})
				//#endif
				//#ifndef MP-WEIXIN
				console.info("连接成功", result)
				if (result.status == '0') {
					console.info("tcp连接成功")
					//TCP连接成功
					TCPSocket.send({
						//charsetname:'GBK',//可不选,默认UTF-8
						channel: '1', //可选 1~20
						message: "AT*DTUID="+this.editDtuid+"#"+"\r\n"+"AT*RESTART#"
					});
			
					let times = 4000
				} else if (result.status == '1') {
					++this.connCount;
					//TCP断开连接
			
					if (this.connCount < 10) {
						//再次尝试连接
						setTimeout(() => {
							TCPSocket.connect({
									channel: '1', //可选 1~20
									ip: '192.168.77.1',
									port: '6888'
								},
								this.appConnFunc
							);
						}, 1000);
			
					}
			
					return;
				}
				if (result.receivedMsg) {
					//服务器返回字符串
					if (result.receivedMsg.substr(0, 2) == 'OK') {
						uni.showToast({
							title: '修改成功',
							icon:'success',
							duration:3000
						})
						setTimeout(()=>{
							uni.navigateBack()
						},2000)
					} else {
						this.loading = false;
						this.$refs.promptMsg.noticeOpen('修改失败，请重新尝试？','系统消息', true)
					}
				}
				//#endif
			},

		}
	}
</script>

<style lang="less" scoped>
	.wifi-form {
		margin-top: 60rpx;
		padding: 0 30rpx;
		display: flex;
		flex-direction: column;
		justify-content: center;
	}

	.wifi-form .weui-cells {
		padding-top: 30rpx;
	}

	.wifi-form .weui-cell {
		display: flex;
		justify-content: flex-start;
		width: 100%;
		height: 63rpx;
		// line-height: 63rpx;
		margin-top: 50rpx;
		border-bottom: 1rpx solid #EAEAEA;
		font-size: 32rpx;

		.weui-cell__hd {
			margin-right: 60rpx;
			width: 150rpx;
		}

		.weui-cell__bd {
			display: flex;
			justify-content: flex-start;
			align-items: flex-start;
			width: calc(100% - 150rpx - 60rpx);

			.weui-select {
				height: 63rpx;
			}

			.can_select_wifi {
				width: 100%;
				display: flex;
				justify-content: space-between;
				align-items: flex-start;
				color: #D8D8D8;

				&.selected_wifi {
					color: #333333;
				}
			}
		}
	}

	.wifi-form .weui-select {
		padding-left: 0;
	}

	.form-placeholder {
		color: #D8D8D8;
	}
</style>