<template>
	<view>
		<map ref="myMap" :key="'map'+mapKey" :latitude="info.Lat" :longitude="info.Lng" scale="13" :show-location="true"
			style="width: 100%; height: 100vh;border-radius: 20rpx;margin-top: 20rpx;" :markers="covers"
			:polyline="polylines">
		</map>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	// #ifdef H5
	var jWeixin = require('jweixin-module')
	// #endif
	export default {
		data() {
			return {
				mapKey: 1,
				info: {
					Lat: '39.909',
					Lng: '116.39742'
				},
				covers: [{
					latitude: 39.909,
					longitude: 116.39742,
					iconPath: require(getSerVerUrl()+'/appimg/marker.png'),
					label: {
						content: '11'
					}
				}, {
					latitude: 39.90,
					longitude: 116.39,
					iconPath: require(getSerVerUrl()+'/appimg/marker.png'),
					label: {
						content: '222'
					}
				}],
				polylines: [{
					points: [{
						latitude: 39.909,
						longitude: 116.39742
					}, {
						latitude: 39.90,
						longitude: 116.39
					}],
					color: '#E53E30',
					width: '5',
					arrowLine: true
				}],
			}
		},
		onLoad() {
			// this.covers=[]
			this.setLocation()


		},
		destroyed() {
			var ua = window.navigator.userAgent.toLowerCase();
			// console.log(this.$store.state.qywxAppId,'this.$store.state.qywxAppId');
			// ||this.$store.state.qywxAppId
			if (/wxwork/i.test(ua)) { //企业微信
				jWeixin.invoke('stopAutoLBS', {}, function(res) {
					if (res.err_msg == "stopAutoLBS:ok") {
						//调用成功
					} else {
						//错误处理
					}
				});
			} else {
				uni.offLocationChange((res) => {
			
				})
				uni.offLocationChangeError((res) => {
			
				})
			}
		},
		beforeDestroy() {
			var ua = window.navigator.userAgent.toLowerCase();
			// console.log(this.$store.state.qywxAppId,'this.$store.state.qywxAppId');
			// ||this.$store.state.qywxAppId
			if (/wxwork/i.test(ua)) { //企业微信
				jWeixin.invoke('stopAutoLBS', {}, function(res) {
					if (res.err_msg == "stopAutoLBS:ok") {
						//调用成功
					} else {
						//错误处理
					}
				});
			} else {
				uni.offLocationChange((res) => {

				})
				uni.offLocationChangeError((res) => {

				})
			}
		},
		methods: {
			setLocation() {
				var ua = window.navigator.userAgent.toLowerCase();
				// console.log(this.$store.state.qywxAppId,'this.$store.state.qywxAppId');
				// ||this.$store.state.qywxAppId
				if (/wxwork/i.test(ua)) { //企业微信
					jWeixin.invoke('startAutoLBS', {
							type: 'gcj02', // wgs84是gps坐标，gcj02是火星坐标
						},
						(res1) => {
							console.log("定位出错提示", res1);
							this.$refs.promptMsg.open(JSON.stringify(res1), 3500)
							if (res1.err_msg == "startAutoLBS:ok") {
								//调用成功
								jWeixin.onLocationChange((res) => {
									this.$refs.promptMsg.open('位置' + JSON.stringify(res), 3500)
									console.log('纬度：' + res.latitude);
									console.log('经度：' + res.longitude);
									uni.showToast({
										title: '经度：' + res.longitude +',纬度：' + res.latitude,
										icon: 'none'
									})
									let markObj = {
										latitude: res.latitude,
										longitude: res.longitude,
										iconPath: require(getSerVerUrl()+'/appimg/marker.png'),
										label: {
											content: res.street
										}
									}
									let pointObj = {
										latitude: res.latitude,
										longitude: res.longitude
									}
									this.covers.push(markObj)
									this.polylines[0].points.push(pointObj)
									this.polylines=[...this.polylines]
									// this.mapKey++

								});
							} else {
								//错误处理
								if (res1.err_msg == "getLocation:fail auth deny") {
									uni.showModal({
										content: '检测到您没打开获取信息功能权限，是否去设置打开？',
										confirmText: "确认",
										cancelText: '取消',
										success: (res) => {
											if (res.confirm) {
												window.open(
													'intent://settings/'
												); // 安卓系统可能支持此协议
											} else {
												return false;
											}
										}
									})
								} else if (res1.err_msg.indexOf('permission') !== -1) {
									uni.showModal({
										title: '提示',
										content: '请在浏览器设置中开启定位权限',
										showCancel: false,
										confirmText: '去设置',
										success: function(res) {
											if (res.confirm) {
												// 尝试打开系统设置页面（仅部分浏览器支持）
												window.open(
													'intent://settings/'
												); // 安卓系统可能支持此协议
											}
										}
									});
								} else {
									console.error('获取位置失败', err);
								}
							}
						});


				} else {
					uni.startLocationUpdate({
						type: 'gcj02',
						success: (res) => {
							uni.onLocationChange((res) => {
								console.log('纬度：' + res.latitude);
								console.log('经度：' + res.longitude);
								uni.showToast({
									title: '经度：' + res.longitude + ',纬度：' + res.latitude,
									icon: 'none'
								})
								let markObj = {
									latitude: res.latitude,
									longitude: res.longitude,
									iconPath: require(getSerVerUrl()+'/appimg/marker.png'),
									label: {
										content: res.street
									}
								}
								let pointObj = {
									latitude: res.latitude,
									longitude: res.longitude
								}
								this.covers.push(markObj)
								this.polylines[0].points.push(pointObj)
								this.polylines=[...this.polylines]
								// this.mapKey++

							});
							uni.onLocationChangeError(re => {
								console.log("定位出错: ", re);
							})
						},
						fail: (err) => {
							if (err.errMsg == "getLocation:fail auth deny") {
								uni.showModal({
									content: '检测到您没打开获取信息功能权限，是否去设置打开？',
									confirmText: "确认",
									cancelText: '取消',
									success: (res) => {
										if (res.confirm) {
											window.open('intent://settings/'); // 安卓系统可能支持此协议
										} else {
											return false;
										}
									}
								})
							} else if (err.errMsg.indexOf('permission') !== -1) {
								uni.showModal({
									title: '提示',
									content: '请在浏览器设置中开启定位权限',
									showCancel: false,
									confirmText: '去设置',
									success: function(res) {
										if (res.confirm) {
											// 尝试打开系统设置页面（仅部分浏览器支持）
											window.open('intent://settings/'); // 安卓系统可能支持此协议
										}
									}
								});
							} else {
								console.error('获取位置失败', err);
							}
						}
					})

				}

			},
		}
	}
</script>

<style>

</style>