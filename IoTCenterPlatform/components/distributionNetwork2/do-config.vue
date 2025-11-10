<template>
	<view>
		<view class="page-subtitle">Start network distribution</view>
		<view class="page-main">
			<view class="progress-list">
				<view v-for="(item,index) in connectDeviceSteps" :key="index" class="progress-item"
					:class="{'done' : curStep > index}">
					<image class="item-icon icon-check" src="/static/check-blue.svg" v-if="curStep > index" />
					<image class="item-icon icon-loading" src="/static/connecting-loading.svg" v-else />
					{{item}}
				</view>
			</view>
			<view class="tip-txt" hover-class="checkActive" @click="jmptoErr" v-if="!isNewDevice">Network distribution
				failure? Click here
				to view the solution!</view>
		</view>
		<msg-prompt ref="promptMsg" @confirm="afresh" @closenotice="closenotice"></msg-prompt>
	</view>
</template>

<script>
	import {
		deviceIsOnline,
		devicesIsOnline
	} from '@/api/wifi.js'

	//#ifndef MP-WEIXIN
	const TCPSocket = uni.requireNativePlugin('Aimer-TCPPlugin');
	//#endif
	export default {
		name: "do-config",
		props: {
			ismode: {
				type: Boolean
			},
			isNewDevice: {
				type: Boolean,
				default: false,
			}
		},
		data() {
			return {
				curStep: 0,
				connectDeviceSteps: [
					'Establishing a connection between the device and the phone',
					'Device obtains SSID and PWD',
					'The device successfully connected to the router',
				],
				verifycount: 0,
				dtuId: null,
				connCount: 0,
				messageStr: "",
				messageStr1: '',
				isOnline: false, //设备此时是否是在线的
				dtuNum: 0, //物联网通讯id的号码
				// isNewDevice: false, //是否是新设备，即编号是否是iot2300065以上
			};
		},
		destroyed() {
			this.disConn();
		},
		methods: {
			closenotice() {
				//关闭消息提示回到设备列表页
				this.$store.commit('SET_DEVICE_LIST', true)
				uni.switchTab({
					url: '/pages/devices/devices'
				})
			},
			afresh() {
				//重新配网
				console.log("执行了");
				this.$emit('doAfresh');
			},
			disConn() {
				//#ifndef MP-WEIXIN
				if (this.curStep > 0) {
					TCPSocket.disconnect({
						channel: '1' //可选 1~20
					});
				}
				//#endif
			},
			jmptoErr() {
				this.disConn();
				this.$emit('doConfigFun', 'fail');
			},
			async VerifySuccess() {
				try {
					let rs = await devicesIsOnline(this.dtuId);
					console.log(rs, '是否在线');
					if (rs.data == true) {
						this.curStep = 3;
						setTimeout(() => {
							this.$emit('doConfigFun', 'ok');
						}, 300);
					} else {
						if (this.verifycount > 40) {
							if (this.isNewDevice) {
								this.$refs.promptMsg.noticeOpen(
									'Network configuration failed, possibly due to incorrect password in the WiFi environment where the device is connected. Do you want to reconfigure the network?',
									'system prompt', true)
							} else {
								this.$emit('doConfigFun', 'fail');
							}

						} else {
							setTimeout(() => {
								this.verifycount += 1;
								this.VerifySuccess();
							}, 2000);
						}
					}
				} catch (err) {
					console.log(err, '是否在线');
					setTimeout(() => {
						this.VerifySuccess();
					}, 2000);
				}

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
					this.curStep = 1;
					if (this.ismode) {
						tcp.write(this.messageStr)
					} else {
						if (this.isNewDevice && this.isOnline) { //如果设备是新设备并且现在处于在线状态需要先将设备设置离线
							let msg = "AT*OFFLINE#\r\nAT*GPRSMODE=0#"
							// let msg1 = "AT*GPRSMODE=0#"
							tcp.write(msg)
							// tcp.write(msg1)
						} else {
							tcp.write("AT*GPRSMODE=1#" + "\r\n" + this.messageStr)
						}
					}
					let times = 4000
					// console.log(this.isOnline, 'isOnlineisOnlineisOnline');
					if (this.isNewDevice) {
						if (this.isOnline) {
							//如果设备在进行tcp连接之前就是在线的，需延长获取设备在线状态的时间
							this.getDeviceStatus(true)
						} else {
							setTimeout(() => {
								this.VerifySuccess();
							}, times);
						}
					} else {
						setTimeout(() => {
							this.VerifySuccess();
						}, times);
					}
				})
				tcp.onError((err) => {
					++this.connCount;
					//TCP断开连接
					console.log('TCP断开连接', this.curStep);

					if (this.connCount < 10 && this.curStep == 0) {
						//再次尝试连接
						setTimeout(() => {
							this.appConnFunc();
						}, 1000);

					} else {
						if (this.curStep == 1) {
							this.curStep = 2;
						}
					}
				})
				tcp.onMessage((msg) => {
					let buffer = msg.message
					//ArrayBuffer字符串转换，很重要
					let unit8Arr = new Uint8Array(msg.message);
					let encodedString = String.fromCharCode.apply(null, unit8Arr);
					let hexstr = decodeURIComponent(escape((encodedString)));
					if (!this.isOnline && this.curStep < 2) {
						if (hexstr.substr(0, 2) == 'OK') {
							this.isSendSuccess = true
							this.curStep = 2
						} else {
							if (this.isNewDevice) {
								this.$refs.promptMsg.noticeOpen(
									'Network configuration failed, possibly due to incorrect password in the WiFi environment where the device is connected. Do you want to reconfigure the network?',
									'system prompt', true)
							} else {
								this.$emit('doConfigFun', 'fail');
							}
						}
					}
				})
				//#endif
				//#ifndef MP-WEIXIN
				console.info("连接成功", result)
				if (result.status == '0') {
					console.info("连接成功")
					//TCP连接成功
					this.curStep = 1;
					if (this.ismode) {
						TCPSocket.send({
							//charsetname:'GBK',//可不选,默认UTF-8
							channel: '1', //可选 1~20
							message: "AT*GPRSMODE=1#" + "\r\n" + this.messageStr
						});
					} else {
						if (this.isNewDevice && this.isOnline) { //如果设备是新设备并且现在处于在线状态需要先将设备设置离线
							let msg = "AT*OFFLINE#\r\nAT*GPRSMODE=0#"
							// let msg1 = "AT*GPRSMODE=0#"
							TCPSocket.send({
								//charsetname:'GBK',//可不选,默认UTF-8
								channel: '1', //可选 1~20
								message: msg
							});
						} else {
							TCPSocket.send({
								//charsetname:'GBK',//可不选,默认UTF-8
								channel: '1', //可选 1~20
								message: "AT*GPRSMODE=1#" + "\r\n" + this.messageStr
							});
						}

					}
					let times = 4000
					if (this.isNewDevice) {
						if (this.isOnline) {
							//如果设备在进行tcp连接之前就是在线的，先获取设备是否设置离线成功
							this.getDeviceStatus(true)
						} else {
							setTimeout(() => {
								this.VerifySuccess();
							}, times);
						}
					} else {
						setTimeout(() => {
							this.VerifySuccess();
						}, times);
					}
					return;
				} else if (result.status == '1') {
					++this.connCount;
					//TCP断开连接
					console.log('TCP断开连接', this.curStep);
					if (this.connCount < 10 && this.curStep == 0) {
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

					} else {
						if (this.curStep == 1) {
							this.curStep = 2;
						}
					}
					return;
				}
				if (result.receivedMsg) {
					//服务器返回字符串
					console.log(result.receivedMsg, 'xxxxxxxx');
					if (!this.isOnline && this.curStep < 2) {
						if (result.receivedMsg.substr(0, 2) == 'OK') {
							this.curStep = 2;
						} else {
							if (this.isNewDevice) {
								this.$refs.promptMsg.noticeOpen(
									'Network configuration failed, possibly due to incorrect password in the WiFi environment where the device is connected. Do you want to reconfigure the network?',
									'system prompt', true)
							} else {
								this.$emit('doConfigFun', 'fail');
							}
						}
					}

				}
				//#endif
			},
			async getDeviceStatus() {
				try {
					let rs = await devicesIsOnline(this.dtuId);
					console.log(rs, '是否在线');
					if (rs.data == false) {
						//如果设备是新设备并且现在处于在线状态，设置设备离线后查看设备在线状态，只有当设备为离线时才发送模拟在线并发送配网信息
						this.isOnline = false
						let msg = "AT*GPRSMODE=1#" + "\r\n" + this.messageStr
						TCPSocket.send({
							//charsetname:'GBK',//可不选,默认UTF-8
							channel: '1', //可选 1~20
							message: msg
						});
						this.VerifySuccess();
					} else {
						setTimeout(() => {
							this.getDeviceStatus();
						}, 2000);
					}
				} catch (err) {
					console.log(err, '是否在线');
					setTimeout(() => {
						this.getDeviceStatus();
					}, 2000);
				}
			},
			setSocketConnect(lianJietargetWifi, id, isOnline) {
				this.isOnline = isOnline //设备之前是否在线
				this.verifycount = 0;
				this.connCount = 0;
				this.curStep = 0;
				this.dtuId = id;
				let num = Number(this.dtuId.substring(3))
				this.dtuNum = num
				console.log("截取的字符串", num);
				if (this.isNewDevice) {
					this.messageStr = "AT*WSET=" + lianJietargetWifi.selectedWifiSSID + "," + lianJietargetWifi
						.selectedWifiPassword + ",2#"
				} else {
					if (num <= 2300065 && num != 2300003) {
						this.isOnline = false
					}
					this.messageStr = "AT*WSET=" + lianJietargetWifi.selectedWifiSSID + "," + lianJietargetWifi
						.selectedWifiPassword + ",0#"
				}
				if (this.ismode) {
					this.messageStr += "\r\nAT*RESTART#";
				}
				//#ifdef MP-WEIXIN
				this.appConnFunc();
				//#endif
				//#ifndef MP-WEIXIN
				TCPSocket.connect({
						//charsetname:'GBK',//可不选,默认UTF-8,针对服务端数据的字符集格式转化
						channel: '1', //可选 1~20
						ip: '192.168.77.1',
						port: '6888'
					},
					this.appConnFunc
				);
				//#endif
			}
		}
	}
</script>

<style lang="less" scoped>
	.tip-txt {
		margin-top: 180rpx;
		font-size: 32rpx;
		cursor: pointer;
		text-decoration: underline;
		color: rgba(255, 53, 53, 1);
	}

	.checkActive {
		background: #91ab45;
		box-shadow: 0upx 0upx 0upx #91ab45;
		transition: background, box-shadow 0.3s ease;
	}

	:host {
		width: 100%;
	}

	.page-subtitle {
		color: #333;
		font-size: 40rpx;
	}

	.progress-list {
		margin-top: 100rpx;
		display: inline-flex;
		flex-direction: column;
	}

	.progress-list .progress-item {
		position: relative;
		font-size: 30rpx;
		color: #333333;
		margin-bottom: 20rpx;
		width: 100%;
		box-sizing: border-box;
		padding-left: 44rpx;
	}

	.progress-list .progress-item.done {
		color: #888888;
	}

	.progress-list .progress-item .item-icon {
		position: absolute;
		left: 0;
		top: 50%;
		-webkit-transform: translateY(-50%);
		-ms-transform: translateY(-50%);
		transform: translateY(-50%);
	}

	.progress-list .progress-item .icon-check {
		left: 0;
		width: 28rpx;
		height: 28rpx;
	}

	.progress-list .progress-item .icon-loading {
		left: -3rpx;
		width: 38rpx;
		height: 38rpx;
		-webkit-animation: rotate 1s linear infinite;
		animation: rotate 1s linear infinite;
	}

	@-webkit-keyframes rotate {
		from {
			-webkit-transform: translateY(-50%) rotate(0);
			transform: translateY(-50%) rotate(0);
		}

		to {
			-webkit-transform: translateY(-50%) rotate(360deg);
			transform: translateY(-50%) rotate(360deg);
		}
	}

	@keyframes rotate {
		from {
			-webkit-transform: translateY(-50%) rotate(0);
			transform: translateY(-50%) rotate(0);
		}

		to {
			-webkit-transform: translateY(-50%) rotate(360deg);
			transform: translateY(-50%) rotate(360deg);
		}
	}
</style>