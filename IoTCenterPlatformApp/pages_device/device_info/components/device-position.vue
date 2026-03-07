<template>
	<view class="map_con">

		<map :latitude="deviceBasicInfo.Lat" :longitude="deviceBasicInfo.Lng" scale="16" :show-location="true"
			style="width: 100%; height: 560rpx;border-radius: 20rpx;margin-top: 20rpx;" :markers="covers">
		</map>
		<view class="address_title" v-if="deviceAddressInfo">
			<image class="image" :src="getSerVerUrl()+'/appimg/marker.png'" mode=""></image>
			<view class="text">{{deviceAddressInfo}}</view>
			<view class="address_" @click="viewAddressList" v-if="positionInfo&&positionInfo.mapcode">历史轨迹</view>
		</view>
		<view class="weather_info" v-if="weatherInfo&&weatherInfo.weather">
			<image class="weather_img" :src="getWeatherImg(weatherInfo.weather)" mode=""></image>
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
			<image class="weather_img" :src="getSerVerUrl()+'/appimg/weather_img/duoyun.png'" mode=""></image>
			<view class="info_con">
				<view class="wea_text">
					<text class="text1">设备天气</text>
					<text>--</text>
				</view>
				<!-- <view class="tem_hum">
					<view class="text">请开启定位服务</view>
				</view> -->
			</view>
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
		name: "device-position",
		props: {
			deviceBasic: {
				type: Object,
				default: () => {
					return {}
				}
			},
			positionInfo: {
				type: Object,
				default: () => {
					return {}
				}
			},
		},
		computed:{
			iconpath(){
				return serverUrl.getServerUrl()+'/appimg/marker.png'
			}
		},
		data() {
			return {
				deviceAddressInfo: '',
				weatherInfo: {},
				covers: [{
					id: 0,
					latitude: 39.909,
					longitude: 116.39742,
					iconPath: '',
					height:30,
					width:30
				}],
				deviceBasicInfo:{},
				mapKey:1,
				mapKeyStr:'map1'
			};
		},
		watch: {
			deviceBasic: {
				async handler(newvalue, oldvalue) {
					this.deviceBasicInfo=JSON.parse(JSON.stringify(this.deviceBasic))
					if (this.deviceBasicInfo.Lat && this.deviceBasicInfo.Lng) {
						// let abs = this.wgs84_to_bd09(this.deviceBasicInfo.Lat, this.deviceBasicInfo.Lng)
						// this.deviceBasicInfo.Lat = abs.bdLat
						// this.deviceBasicInfo.Lng = abs.bdLng
						this.covers[0].latitude = this.deviceBasicInfo.Lat
						this.covers[0].longitude = this.deviceBasicInfo.Lng
						// console.log("yyyyyyyyyy",this.deviceBasicInfo);
						this.tabArr = ['基本信息', '运行状态', '设备功能', '报警信息', '设备位置']
						this.current = 0;
						let coderes =null
						// #ifndef H5
						try{
							coderes = await getGeocoder({
								location: this.deviceBasicInfo.Lat + ',' + this.deviceBasicInfo.Lng,
								maptype: 'gd'
							})
							this.deviceAddressInfo = coderes.data.AddressName + coderes.data.AddressDetail
						}catch(e){
							//TODO handle the exception
						}
						// #endif
						// #ifdef H5
						try{
							coderes = await getGeocoder({
								location: this.deviceBasicInfo.Lat + ',' + this.deviceBasicInfo.Lng,
								maptype: 'gd'
							})
							this.deviceAddressInfo = coderes.data.AddressName + coderes.data.AddressDetail
						}catch(e){
							//TODO handle the exception
						}
						// #endif
						if (serverUrl.returnSetWeather()) {
							if (this.deviceBasicInfo.AreaCode) {
								await this.getWeather(this.deviceBasicInfo.AreaCode)
								this.timer = setInterval(async () => {
									await this.getWeather(this.deviceBasicInfo.AreaCode)
								}, 50000)
							} else {
								if(coderes&&coderes.data&&coderes.data.AddressCode){
									await this.getWeather(coderes.data.AddressCode)
									this.timer = setInterval(async () => {
										await this.getWeather(coderes.data.AddressCode)
									}, 50000)
								}
								
							}
						}

					}
				},
				// 代表在wacth里声明了firstName这个方法之后立即先去执行handler方法
				immediate: true,
				deep: true,
			},
		},
		mounted() {
			// console.log("位置图标",this.covers[0].iconPath=this.iconpath);
			this.covers[0].iconPath=this.iconpath
		},
		methods: {
			getSerVerUrl(){
				return serverUrl.getServerUrl()
			},
			viewAddressList() {
				//查看历史记录
				let item=this.positionInfo
				let itemObj = {
					Code: item.mapcode,
					Name: item.name,
					OptionType: item.option.type,
					Unit: item.option.unit
				}
				if (itemObj.OptionType == 'date' || itemObj.OptionType == 'float' || itemObj.OptionType == 'int') {
					uni.navigateTo({
						url: '/pages_device/device_info/iot_history?deviceId=' + this.deviceBasicInfo.Id +
							'&deviceNu=' + this.deviceBasicInfo.DeviceId + '&activeAttr=' + JSON.stringify(itemObj)
					})
				} else if (itemObj.OptionType == 'geo') {
					uni.navigateTo({
						url: '/pages_device/map_track?deviceId=' + this.deviceBasicInfo.Id + '&deviceNu=' + this
							.deviceBasicInfo.DeviceId + '&activeAttr=' + JSON.stringify(itemObj),
							fail:(err)=>{
								console.log("跳转失败",err);
							}
					})
				}
			
			},
			wgs84_to_bd09(lat, lng) {
				var x_pi = 3.14159265358979324 * 3000.0 / 180.0;
				var x = lng - 0.0065;
				var y = lat - 0.006;
				var z = Math.sqrt(x * x + y * y) - 0.00002 * Math.sin(y * x_pi);
				var theta = Math.atan2(y, x) - 0.000003 * Math.cos(x * x_pi);
				var bd_lng = z * Math.cos(theta) + 0.0065;
				var bd_lat = z * Math.sin(theta) + 0.006;
				return {
					bdLat: bd_lat,
					bdLng: bd_lng
				};
			},
			getWeatherImg(val) {
				//获取天气形象
				return retunWeatherImg(val)
			},
			async getWeather(val) {
				try {
					let res = await getWeatherInfo(val)
					if (res.data.city) {
						res.data.city = res.data.city.substring(0, res.data.city.length - 1)
					}
					// console.log(res.data,'res.data');
					this.weatherInfo = res.data
				} catch (err) {
					//TODO handle the exception
					console.log("err天气", err);
				}
			},
		}
	}
</script>

<style lang="scss" scoped>
	.map_con {
		.address_title {
			display: flex;
			align-items: center;
			line-height: 42rpx;
			font-size: 30rpx;
			margin-top: 24rpx;
			margin-bottom: 24rpx;

			.image {
				width: 30rpx;
				height: 30rpx;
			}

			.text {
				margin-left: 16rpx;
				font-weight: bold;
				max-width: calc(100% - 46px);
			}

			.address_ {
				height: 42rpx;
				display: flex;
				align-items: flex-end;
				margin-left: 20rpx;
				color: #2371FF;
				font-size: 24rpx;
				font-weight: bold;
				text-decoration: underline;
			}
		}

		.weather_info {
			display: flex;
			justify-content: flex-start;

			.weather_img {
				width: 100rpx;
				height: 100rpx;
				margin-right: 20rpx;
			}

			.info_con {
				padding: 18rpx 0 20rpx 0;

				.wea_text {
					font-size: 24rpx;
					color: #999999;
					line-height: 24rpx;

					.text1 {
						margin-right: 10rpx;
					}
				}

				.tem_hum {
					display: flex;
					justify-content: flex-start;
					align-items: center;
					font-size: 22rpx;
					color: #999999;
					line-height: 22rpx;
					margin-top: 16rpx;

					.line {
						background-color: #999999;
						height: 22rpx;
						width: 2rpx;
						margin: 0 10rpx;
					}
				}
			}
		}
	}
</style>