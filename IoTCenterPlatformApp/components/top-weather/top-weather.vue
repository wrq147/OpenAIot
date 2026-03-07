<template>
	<view>
		<view class="bg_con" :style="{'height':isfixed?Number(statusBarHeight*2)+662+'rpx':662+'rpx','position':isfixed?'fixed':'absolute'}">
			<view class="bg_con_con">
				<view class="top_bg" :style="{'height':isfixed?Number(statusBarHeight*2)+662+'rpx':662+'rpx'}">
				</view>
			</view>
		</view>
		<view class="top_info_con" :style="{'padding-top':isfixed?statusBarHeight*2+50+'rpx':50+'rpx'}" :class="{'hide_top_info':isShowFixedTitle}">
			<view class="top_info_title" :style="{'opacity': titleOpac}">
				<view class="title_left" @click="openLeftPopup">
					<view class="user_name">Hi, {{userName}}</view>
					<view class="icon_con">
						<custom-icons iconsName="icon-a-youjiantouhong" iconsSize="36rpx"
							iconsColor="#333"></custom-icons>
					</view>
				</view>
				<!--#ifndef MP-WEIXIN -->
				<slot name="title_right"></slot>
				<!--#endif -->
			</view>
			<view class="device_num_tips" :style="{'opacity': numOpac}">
				<slot></slot>
				<!--#ifdef MP-WEIXIN -->
				<slot name="title_right" v-if="hasHandleIcon"></slot>
				<!--#endif -->
			</view>
			<!--#ifndef MP-WEIXIN -->
			<view class="bg_info" :style="{'opacity': weatherOpac}">
			<!--#endif -->
			<!--#ifdef MP-WEIXIN -->
			<view class="bg_info" :style="{'opacity': weatherOpac,'margin-top':hasHandleIcon?'50rpx':'4rpx'}">
			<!--#endif -->
				<view class="weather_info" v-if="weatherInfo&&weatherInfo.weather">
					<image v-if="getSerVerUrl()" class="weather_img" :src="getWeatherImg(weatherInfo.weather)" mode=""></image>
					<view class="info_con">
						<view class="wea_text">
							<text class="text1">{{weatherInfo.city}}</text>
							<text>{{weatherInfo.weather}}</text>
						</view>
						<view class="tem_hum">
							<view class="text">温度{{weatherInfo.temperature}}℃</view>
							<view class="line"></view>
							<view class="text">湿度{{weatherInfo.humidity}}%</view>
						</view>
					</view>
				</view>
				<view class="weather_info" v-else>
					<image v-if="getSerVerUrl()" class="weather_img" :src="getSerVerUrl()+'/appimg/weather_img/duoyun.png'" mode=""></image>
					<view class="info_con">
						<view class="wea_text">
							<text class="text1">本地天气</text>
							<text>--</text>
						</view>
						<view class="tem_hum">
							<view class="text">请开启定位服务</view>
						</view>
					</view>
				</view>
				<view class="right_bg">
					<image v-if="getSerVerUrl()" class="bg_image" :src="getSerVerUrl()+'/appimg/chahua.png'" mode=""></image>
				</view>
			</view>
			<slot name="footercontent"></slot>
		</view>
	</view>
</template>

<script>
	import {
		getWeatherInfo,
		getGeocoder
	} from '@/api/weather.js'
	import {
		retunWeatherImg
	} from '@/common/weatherInfo.js'
	import serverUrl from '@/common/constVar.js'
	export default {
		name: "top_weather",
		props: {
			statusBarHeight: {
				type: Number,
				default: 0
			},
			defaultTop: {
				type: Number,
				default: 0
			},
			isShowFixedTitle: {
				type: Boolean,
				default: false
			},
			userName: {
				type: String,
				default: ''
			},
			hasHandleIcon: {
				type: Boolean,
				default: true
			},
			isfixed: {
				type: Boolean,
				default: true
			},
		},
		data() {
			return {
				weatherInfo: {},
				titleOpac: 1,
				numOpac: 1,
				weatherOpac: 1,
			};
		},
		mounted() {
			if (serverUrl.returnSetWeather()) {
				this.getLocationInfo()
			}
		},
		methods: {
			getSerVerUrl(){
				return serverUrl.getServerUrl()
			},
			getWeatherImg(val) {
				//获取天气形象
				return retunWeatherImg(val)
			},
			async getWeather(val) {
				try {
					let res = await getWeatherInfo(val)
					// console.log("天气", res);
					if (res.data.city) {
						res.data.city = res.data.city.substring(0, res.data.city.length - 1)
					}
					this.weatherInfo = res.data
				} catch (err) {
					//TODO handle the exception
					if (this.timer) {
						clearInterval(this.timer)
					}
				}
			},
			getLocationInfo() {
				//获取本地地址信息
				uni.getLocation({
					type: 'gcj02',
					success: (res) => {
						getGeocoder({
							location: res.latitude + ',' + res.longitude,
							maptype: 'lc'
						}).then(async coderes => {
							await this.getWeather(coderes.data.AddressCode)
							this.timer = setInterval(async () => {
								await this.getWeather(coderes.data.AddressCode)
							}, 50000)

						}).catch(err => {
							console.log('err', err);
						})
					},
					fail: (res) => {
						if (res.errMsg == "getLocation:fail auth deny") {
							uni.showModal({
								content: '检测到您没打开获取信息功能权限，是否去设置打开？',
								confirmText: "确认",
								cancelText: '取消',
								success: (res) => {
									if (res.confirm) {
										uni.openSetting({
											success: (res) => {}
										})
									} else {
										return false;
									}
								}
							})
						}
					}
				});
			},
			openLeftPopup() {
				this.$emit('openLeftPopup')
			},
			setDomOpac(data) {
				//设置元素的透明度
				switch (true) {
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 160):
						this.numOpac = 0
						this.titleOpac = 0
						this.weatherOpac = 0
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 136):
						this.numOpac = 0
						this.titleOpac = 0
						this.weatherOpac = 0.1
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 110):
						this.numOpac = 0
						this.titleOpac = 0
						this.weatherOpac = 0.2
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 84):
						this.numOpac = 0
						this.titleOpac = 0
						this.weatherOpac = 0.3
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 58):
						this.numOpac = 0
						this.titleOpac = 0
						this.weatherOpac = 0.4
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 32):
						this.weatherOpac = 0.5
						this.numOpac = 0
						this.titleOpac = 0
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 26):
						this.weatherOpac = 1
						this.numOpac = 0.25
						this.titleOpac = 0
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 20):
						this.weatherOpac = 1
						this.numOpac = 0.5
						this.titleOpac = 0
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 15):
						this.weatherOpac = 1
						this.numOpac = 1
						this.titleOpac = 0.14
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 10):
						this.weatherOpac = 1
						this.numOpac = 1
						this.titleOpac = 0.26
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 5):
						this.weatherOpac = 1
						this.numOpac = 1
						this.titleOpac = 0.35
						break;
					case data.top < Number(this.defaultTop - this.statusBarHeight):
						this.weatherOpac = 1
						this.numOpac = 1
						this.titleOpac = 0.5
						break;
					default:
						this.weatherOpac = 1
						this.numOpac = 1
						this.titleOpac = 1
				}
			},
		},
	}
</script>

<style lang="less">
	.bg_con {
		position: fixed;
		top: 0;
		left: 0;
		width: 100%;
		// height: 100vh;
		z-index: 0;

		.bg_con_con {
			position: relative;
			width: 100%;
			height: 100%;

			.top_bg {
				position: absolute;
				top: 0;
				left: 0;
				width: 100%;
				height: 400rpx;
				background: linear-gradient(180deg, #DEEBFA 0%, #F5F8F9 100%);
			}

			.list_bg {
				position: absolute;
				top: 0;
				left: 0;
				width: 100%;
				background-color: #F5F8F9;
			}
		}
	}
</style>