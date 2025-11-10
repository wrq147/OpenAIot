<template>
	<view>
		<!-- <cu-custom class="navBox" bgColor="bg-gradual-red" :isBack="true">
			<block slot="content">地图轨迹</block>
		</cu-custom> -->
		<!-- #ifdef APP-PLUS -->
		<top :title="topTitle" leftWidth="60rpx" leftIcon="icon-fanhui" rightWidth="60rpx" :isleftBack="true"
			backgroundColor="#ffffff"></top>
		<!-- #endif -->
		<!-- #ifndef APP-PLUS -->
		<top :title="topTitle" leftWidth="60rpx" leftIcon="icon-fanhui" rightWidth="64rpx" :isleftBack="true"
			backgroundColor="#ffffff" :rightText="isDisabled?'停止':'播放'" @clickRight="setStart"></top>
		<!-- #endif -->
		<view class="navBox"></view>
		<uni-nav-bar :status-bar="false" :fixed="true" :border="false" height="110rpx" backgroundColor="#fff"
			:zIndex="998">
			<template v-slot:allslot>
				<view class="tab_ul_con">
					<view class="tab_ul">
						<view class="tab_li" :class="{'active_li':showTime=='Today'}" @click="setShowTime('Today')">
							今天</view>
						<view class="tab_li" :class="{'active_li':showTime=='Last week'}"
							@click="setShowTime('Last week')">上周</view>
						<view class="tab_li" :class="{'active_li':showTime=='Last month'}"
							@click="setShowTime('Last month')">上月</view>
					</view>
				</view>
			</template>
		</uni-nav-bar>
		<uni-nav-bar :status-bar="false" :fixed="true" :border="false" height="192rpx" backgroundColor="inherit"
			:zIndex="998">
			<!-- 140rpx+20+36-4 -->
			<template v-slot:allslot>
				<view class="date_con_con">
					<view class="li_label">时间区间</view>
					<view class="int_date">
						<view class="date_con long_date" style="color: #333;">
							<uni-datetime-picker v-model="choiceTime" type="datetimerange" @change="dateChange"
								:border="false" :isCustom="true" />
						</view>
					</view>
				</view>
			</template>
		</uni-nav-bar>
		<view class="container1">
			<map id='map' :latitude="latitude" :longitude="longitude" :markers="covers"
				:style="{ width: '100%', height: mapHeight + 'px' }" :scale="16" :polyline="polyline">
			</map>
			<!-- <view class="btnBox">
				<button :disabled="isDisabled" @click="start" class="cu-btn bg-red round shadow lg">轨迹回放</button>
			</view> -->
		</view>
	</view>
</template>

<!-- 注：vue只支持小程序及H5，如打包App，请修改代码为nvue，或加群要文件 -->

<script>
	import {
		DeviceOldInfo
	} from '@/api/device.js'
	var dayjs = require('@/common/day.js')
	export default {
		data() {
			return {
				map: null,

				windowHeight: 0,
				mapHeight: 0,
				timer: null,

				isDisabled: false,
				isStart: false,
				playIndex: 1,

				id: 0, // 使用 marker点击事件 需要填写id
				title: 'map',
				latitude: 34.263734,
				longitude: 108.934843,
				// 标记点
				covers: [{
					id: 1,
					width: 42,
					height: 47,
					rotate: 270,
					latitude: 34.259428,
					longitude: 108.947040,
					iconPath: 'http://cdn.zhoukaiwen.com/car.png',
					callout: {
						content: "陕A·88888", // 车牌信息
						display: "ALWAYS",
						fontWeight: "bold",
						color: "#5A7BEE", //文本颜色
						fontSize: "12px",
						bgColor: "#ffffff", //背景色
						padding: 5, //文本边缘留白
						textAlign: "center",
					},
					anchor: {
						x: 0.5,
						y: 0.5,
					},
				}],

				// 线
				polyline: [],

				// 坐标数据
				coordinate: [],
				topTitle: '轨迹',
				isDark: false,
				deviceOldQuery: {
					pageNum: 1,
					pageSize: 30
				},
				tabArr: ['表格', '图表'],
				deviceId: 0,
				deviceNu: '',
				activeAttr: {},
				code: null,
				unit: '',
				showTime: 'Today',
				choiceTime: null,
				choiceTime2: null,
				current: 0,
				oldInfoList: [],
				status: 'loading',
			}
		},
		watch: {

		},
		// 分享小程序
		onShareAppMessage(res) {
			return {
				title: '看看这个小程序多好玩～',
			};
		},
		onReady() {
			// 创建map对象
			this.map = uni.createMapContext('map');
			// 获取屏幕高度
			uni.getSystemInfo({
				success: res => {
					this.windowHeight = res.windowHeight;
				}
			});
		},
		mounted() {
			this.setNavTop('.navBox')


		},
		async onLoad(options) {
			if (options.deviceId) {
				this.deviceId = options.deviceId
			}
			if (options.activeAttr) {
				this.activeAttr = JSON.parse(options.activeAttr)
			}
			if (options.deviceNu) {
				this.deviceNu = options.deviceNu
				this.topTitle = '轨迹（' + options.deviceNu + '）'
			}
			// console.log("this.activeAttr", this.activeAttr);
			// if (options.code) {
			// 	this.code = options.code
			// }
			// if (options.unit) {
			// 	this.unit = options.unit
			// }
			if (this.deviceId && this.activeAttr.Code) {
				await this.setChoiceTime()
				// this.getDeviceOldInfo()
			}
		},
		methods: {
			dateChange() {
				this.$nextTick(() => {
					this.choiceTime2 = this.choiceTime
					this.deviceOldQuery.pageNum = 1
					this.oldInfoList = []
					this.getDeviceOldInfo()
				})
			},
			async setChoiceTime() {
				//设置展示时间
				this.choiceTime = [];
				this.choiceTime2 = []
				if (this.showTime == "Today") {
					let end = new Date(
						new Date(new Date().toLocaleDateString()).getTime() +
						24 * 60 * 60 * 1000 -
						1
					);
					let start = new Date(
						new Date(new Date().toLocaleDateString()).getTime()
					);
					end = dayjs(new Date(end)).format('YYYY-MM-DD HH:mm:ss')
					start = dayjs(new Date(start)).format('YYYY-MM-DD HH:mm:ss')
					this.choiceTime.push(start);
					this.choiceTime.push(end);
					this.choiceTime2.push(start);
					this.choiceTime2.push(end);
				}
				if (this.showTime == "Last week") {
					let end = new Date();
					let start = new Date();
					start.setTime(start.getTime() - 3600 * 1000 * 24 * 7);
					end = dayjs(new Date(end)).format('YYYY-MM-DD HH:mm:ss')
					start = dayjs(new Date(start)).format('YYYY-MM-DD HH:mm:ss')
					this.choiceTime.push(start);
					this.choiceTime.push(end);
					this.choiceTime2.push(start);
					this.choiceTime2.push(end);
				}
				if (this.showTime == "Last month") {
					let end = new Date();
					let start = new Date();
					start.setTime(start.getTime() - 3600 * 1000 * 24 * 30);
					end = dayjs(new Date(end)).format('YYYY-MM-DD HH:mm:ss')
					start = dayjs(new Date(start)).format('YYYY-MM-DD HH:mm:ss')
					this.choiceTime.push(start);
					this.choiceTime.push(end);
					this.choiceTime2.push(start);
					this.choiceTime2.push(end);
				}
				if (this.showTime) {
					this.deviceOldQuery.pageNum = 1
					// console.log("到这里");
					await this.getDeviceOldInfo();
				}
			},
			async getDeviceOldInfo() {
				// 获取设备相关历史数据
				// this.oldInfoList = [];
				// let obj = {
				// 	Id: this.deviceId,
				// 	Code: this.activeAttr.Code
				// };
				let obj = this.addDateRange({
						Id: this.deviceId,
						Code: this.activeAttr.Code
					},
					this.choiceTime
				)
				this.status = 'loading'
				try {
					let res = await DeviceOldInfo(obj)
					this.oldInfoList = JSON.parse(JSON.stringify(res.data.List));
					this.status = 'noMore'
					this.coordinate = []
					let arr = []
					// this.oldInfoList.map(row => {
					// 	console.log(row, '值');
					// 	let obj = {
					// 		latitude: row.Value.lat,
					// 		longitude: row.Value.lng,
					// 		problem: false,
					// 	}
					// 	arr.push(obj)
					// })

					if (this.oldInfoList && this.oldInfoList.length > 0) {
						let current = {}
						for (let i = 0; i < this.oldInfoList.length; i++) {
							let row = this.oldInfoList[i]
							if (current.latitude === row.Value.lat && current.longitude === row.Value.lng) {

							} else {
								let obj = {
									latitude: row.Value.lat,
									longitude: row.Value.lng,
									problem: false,
								}
								current = obj
								arr.push(obj)
							}

						}
						this.coordinate = JSON.parse(JSON.stringify(arr))
						this.latitude = this.oldInfoList[0].Value.lat;
						this.longitude = this.oldInfoList[0].Value.lng;
						this.covers = [{
								id: 1,
								width: 16,
								height: 16,
								rotate: 0,
								latitude: this.oldInfoList[0].Value.lat,
								longitude: this.oldInfoList[0].Value.lng,
								iconPath: getSerVerUrl()+'/appimg/marker.png',
								callout: {
									content: "", // 车牌信息
									display: "ALWAYS",
									fontWeight: "bold",
									color: "#5A7BEE", //文本颜色
									fontSize: "12px",
									bgColor: "#ffffff", //背景色
									padding: 5, //文本边缘留白
									textAlign: "center",
								},
								anchor: {
									x: 0.5,
									y: 0.5,
								},
							}],
							this.$nextTick(() => {
								this.polyline = [{
									points: this.coordinate,
									color: '#025ADD',
									width: 4,
									dottedLine: false,
								}];
							})
					}

				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}

			},
			setShowTime(val) {
				//
				this.showTime = val
				this.setChoiceTime()
			},
			setNavTop(style) {
				let view = uni.createSelectorQuery().select(style);
				view
					.boundingClientRect(data => {
						this.mapHeight = this.windowHeight - data.height - 195;
					})
					.exec();
			},
			setStart() {
				if (!this.isDisabled) {
					this.playIndex = 1
					if (this.coordinate.length > 1) {
						this.start()
					}
				}

			},
			start() {
				this.isStart = true;
				this.isDisabled = true;
				let data = this.coordinate;
				let len = data.length;
				let datai = data[this.playIndex];
				let _this = this;
				_this.map.translateMarker({
					markerId: 1,
					autoRotate: false,
					destination: {
						longitude: datai.longitude, // 车辆即将移动到的下一个点的经度
						latitude: datai.latitude, // 车辆即将移动到的下一个点的纬度
					},
					duration: 700,
					complete: function() {
						_this.playIndex++;
						if (_this.playIndex < len) {
							_this.start(_this.playIndex, data);
						} else {
							// uni.showToast({
							// 	title: '播放完成',
							// 	duration: 1400,
							// 	icon: 'none'
							// });
							_this.playIndex = 0;
							_this.isStart = false;
							_this.isDisabled = false;
						}
					},
					animationEnd: function() {
						// 轨迹回放完成 处理H5端
						_this.playIndex++;
						if (_this.playIndex < len) {
							_this.start(_this.playIndex, data);
						} else {
							// uni.showToast({
							// 	title: '播放完成',
							// 	duration: 1400,
							// 	icon: 'none'
							// });
							_this.playIndex = 0;
							_this.isStart = false;
							_this.isDisabled = false;
						}
					},
					fail(e) {
						// 轨迹回放失败
					},
				});
			},
		}
	}
</script>

<style lang="scss" scoped>
	.container1 {
		position: relative;
		padding: 0;
		z-index: 1;
		height: inherit;
	}

	.btnBox {
		width: 750rpx;
		position: absolute;
		bottom: 60rpx;
		z-index: 99;
		display: flex;
		justify-content: space-around;
	}

	.tab_ul_con {
		width: 100%;
		padding-top: 20rpx;
		background-color: #ffffff;
		display: flex;
		justify-content: center;
	}

	.tab_ul {
		width: calc(100% - 40rpx);
		height: 88rpx;
		display: flex;
		justify-content: center;
		align-items: center;
		// border: 1rpx solid rgba(255, 255, 255, 0.20);
		border-radius: 10rpx;
		background-color: #F8F8F8;

		.tab_li {
			width: calc(100% / 3);
			height: 80rpx;
			border-radius: 10rpx;
			display: flex;
			justify-content: center;
			align-items: center;
			color: #999999;

			&.active_li {
				background: linear-gradient(180deg, #2371FF 0%, #2371FF 100%);
				color: rgba(255, 255, 255, 1);
			}
		}
	}

	.date_con_con {
		width: 100%;
		display: flex;
		justify-content: center;
		align-items: center;
		flex-direction: column;
		padding-top: 20rpx;
		padding-bottom: 36rpx;
		background-color: #fff;

		.li_label {
			color: #999999;
			font-size: 32rpx;
			line-height: 32rpx;
			margin-bottom: 20rpx;
			width: 100%;
			text-align: left;
			padding-left: 20rpx;
			box-sizing: border-box;
		}

		.int_date {
			width: 100%;
			padding: 0 20rpx;
			box-sizing: border-box;
		}
	}
</style>