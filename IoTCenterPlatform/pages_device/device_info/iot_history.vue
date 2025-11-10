<template>
	<view>
		<top title="History Data" leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" rightWidth="157rpx"
			:isleftBack="true" backgroundColor="#161A26"></top>

		<view class="history_con">
			<uni-nav-bar :status-bar="false" :fixed="true" :border="false" height="110rpx" backgroundColor="inherit"
				:zIndex="998">
				<template v-slot:allslot>
					<view class="tab_ul_con">
						<view class="tab_ul">
							<view class="tab_li" :class="{'active_li':showTime=='Today'}" @click="setShowTime('Today')">
								Today</view>
							<view class="tab_li" :class="{'active_li':showTime=='Last week'}"
								@click="setShowTime('Last week')">Last week</view>
							<view class="tab_li" :class="{'active_li':showTime=='Last month'}"
								@click="setShowTime('Last month')">Last month</view>
						</view>
					</view>
				</template>
			</uni-nav-bar>
			<uni-nav-bar :status-bar="false" :fixed="true" :border="false" height="192rpx" backgroundColor="inherit"
				:zIndex="998">
				<!-- 140rpx+20+36-4 -->
				<template v-slot:allslot>
					<view class="date_con_con">
						<view class="li_label">Time interval</view>
						<view class="int_date">
							<!-- <view class="date_con" style="color: #fff;">
									<uni-datetime-picker ref="dateChoice1" class="date" type="datetime"
										placeholder-style="font-size:28rpx;color:#999999" :clear-icon="false"
										v-model="choiceTime[0]" placeholder='开始日期' :isCustom="true" @change='dateChange1' :isDark="isDark">
										<view class="date_slot" :class="{'has_val':choiceTime[0]}">
											<view class="text">
												{{choiceTime[0]?choiceTime[0]:'Start date'}}
											</view>
											<custom-icons iconsName="icon-xuanzeshijian" iconsSize="36rpx"
												iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
										</view>
									</uni-datetime-picker>
								</view>
								<view class="row_line"></view>
								<view class="date_con">
									<uni-datetime-picker ref="dateChoice2" class="date" type="datetime" :clear-icon="false"
										v-model="choiceTime[1]" placeholder='结束日期' :isCustom="true" @change='dateChange2' :isDark="isDark">
										<view class="date_slot" :class="{'has_val':choiceTime[1]}">
											<view class="text">
												{{choiceTime[1]?choiceTime[1]:'End date'}}
											</view>
											<custom-icons iconsName="icon-xuanzeshijian" iconsSize="36rpx"
												iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
										</view>
									</uni-datetime-picker>
								</view> -->
							<view class="date_con long_date" style="color: #fff;">
								<uni-datetime-picker v-model="choiceTime" type="datetimerange" @change="dateChange"
									:isDark="true" :isCustom="true" />
							</view>
						</view>
					</view>
				</template>
			</uni-nav-bar>
			<v-tabs v-model="current" :scroll="false" :tabs="tabArr" color="rgba(255, 255, 255, 0.5)"
				activeColor="rgba(255, 255, 255, 1)" :fixed="true" @change="changeTab" :fixedLineWid="true"
				fontSize="28rpx" activeFontSize="28rpx" paddingItem="0" lineHeight="4rpx" :hasBorder="false"
				height="72rpx" padding="0 50% 0 0 " bgColor="rgba(22, 26, 38, 1)" lineColor="rgba(255, 255, 255, 1)"
				:zIndex="996" scrollBgColor="rgba(22, 26, 38, 1)" scrollConWid="100%"></v-tabs>
			<uni-nav-bar :status-bar="false" :fixed="true" :border="false" height="112rpx" backgroundColor="inherit"
				v-if="current==0" :zIndex="996">
				<template v-slot:allslot>
					<view class="history_title">
						<view class="name">{{activeAttr.Name}}</view>
						<view class="unit">
							{{'（'+activeAttr.Unit+'）'}}
						</view>
					</view>
				</template>
			</uni-nav-bar>
			<view class="history_con">
				<view class="history_ul" v-if="current==0">
					<view class="history_li" v-for="item in oldInfoList">
						<view class="li_val">
							{{item.Value.toFixed(2)+' '+item.Unit}}
						</view>
						<view class="li_time">
							{{item.UpdatedOn}}
						</view>
					</view>
					<uni-load-more iconType="circle" :status="status" v-if="status" />
				</view>
				<view class="history_view" v-if="current==1">
					<qiun-data-charts type="line" :opts="{extra:{line:{type:'curve'}},legend:{show: false}}"
						:eopts="echartOpts" :chartData="chartsDataLine1" :echartsH5="true" :echartsApp="true"
						:loadingType="0" />
				</view>
			</view>
		</view>

		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		DeviceOldInfo
	} from '@/api/device.js'
	import echarts from '@/common/echarts.min.js'
	var dayjs = require('@/common/day.js')
	import {
		parseTime
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				topTitle: '历史数据',
				isDark: false,
				deviceOldQuery: {
					pageNum: 1,
					pageSize: 30
				},
				tabArr: ['Tabulation', 'Chart'],
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
				chartsDataLine1: {},
				echartOpts: {
					title: [{
						// show: false,
						text: '排气压力',
						top: '14',
						left: 0,
						textStyle: { //标题样式
							fontSize: '16',
							color: 'rgba(255, 255, 255, 1)',
						}
					}, {
						// show: false,
						text: '(Kpa)',
						top: '14',
						left: 0,
						textStyle: { //标题样式
							fontSize: '16',
							color: 'rgba(255, 255, 255, 1)',
						}
					}],
					tooltip: {
						trigger: 'axis',
						// axisPointer: {
						// 	type: 'cross',
						// 	animation: true,
						// 	label: {
						// 		backgroundColor: '#505765',
						// 		fontSize: 12
						// 	}
						// },
					},
					legend: {
						"show": false,
					},
					grid: [{
						left: '2%',
						right: '8%',
						bottom: '5%',
						containLabel: true, //这个可以实现将数据包裹在内部，不会有竖轴的值被遮盖
					}],
					xAxis: {
						type: 'category',
						// boundaryGap: true,
						axisLine: {
							show: false
						},
						axisTick: {
							show: false, //是否显示网状线 默认为true
							alignWithLabel: false
						},
						axisLabel: {
							color: "rgba(255, 255, 255, 0.5)", //坐标轴的文本颜色
							fontSize: '12',
						},
						textStyle: {
							color: '#A2A2A2',
							fontSize: 12,
							padding: [30, 100, 10, 0]
						},
					},
					yAxis: {
						type: 'value',
						// interval: 0.0200,
						boundaryGap: false,
						axisTick: {
							show: false, //是否显示网状线 默认为true
							alignWithLabel: true
						},
						axisLine: {
							show: false
						},
						splitNumber: 6,
						//网格样式
						splitLine: {
							show: true,
							lineStyle: {
								color: ['rgba(255, 255, 255, 0.1)'],
								width: 1,
								type: 'dashed'
							}
						},
						axisLabel: {
							color: "rgba(255, 255, 255, 0.5)", //坐标轴的文本颜色
							fontSize: '12'
						}
					},
					seriesTemplate: {
						color: ['rgba(255, 53, 53, 1)'], //折线条的颜色
						showSymbol: false, //是否默认展示圆点
						// symbol: 'emptyCircle', //设定为实心点
						// symbolSize: 8, //设定实心点的大小
						// emphasis: {
						// 	focus: 'series'
						// },
						itemStyle: {
							normal: {
								color: "rgba(255, 53, 53, 1)", //改变折线点的颜色
								borderColor: 'rgba(22, 26, 38, 1)',
								borderWidth: 1,
								lineStyle: {
									color: "rgba(255, 53, 53, 1)", //改变折线颜色
								},
							},
						},
						areaStyle: {
							color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{
								offset: 0,
								color: 'rgba(255, 53, 53, 0.20)'
							}, {
								offset: 1,
								color: 'rgba(255, 53, 53, 0)'
							}])
						}
					}
				}
			}
		},
		onLoad(options) {
			console.log("options", options);
			if (options.deviceId) {
				this.deviceId = options.deviceId
			}
			if (options.activeAttr) {
				this.activeAttr = JSON.parse(options.activeAttr)
			}
			if (options.deviceNu) {
				this.deviceNu = options.deviceNu
				this.topTitle = '历史数据（' + options.deviceNu + '）'
			}
			// console.log("this.activeAttr", this.activeAttr);
			// if (options.code) {
			// 	this.code = options.code
			// }
			// if (options.unit) {
			// 	this.unit = options.unit
			// }
			if (this.deviceId && this.activeAttr.Code) {
				this.setChoiceTime()
				// this.choiceTime = []
				// this.showTime = ''
				// this.deviceOldQuery.pageNum = 1
				// this.deviceOldQuery.pageSize = 100
				// this.getDeviceOldInfo()
			}
			this.echartOpts.title[0].text = this.activeAttr.Name
			if (this.activeAttr.Unit) {
				this.echartOpts.title[1].text = '（' + this.activeAttr.Unit + '）'
				this.echartOpts.title[1].left = this.activeAttr.Name.length * 10
			} else {
				this.echartOpts.title[1].text = ''
				this.echartOpts.title[1].left = this.activeAttr.Name.length * 10
			}

			this.chartsDataLine1 = {
				"series": [{
					"name": "时间轴1",
					"data": [
						[10000, 55],
						[30000, 25],
						[50000, 55],
						[70000, 25],
						[90000, 55]
					]
				}]
			}

		},
		onReachBottom() { //上拉触底
			// console.log("现在的状态是什么", this.status);
			if (this.current == 0) {
				if (this.status != 'noMore') {
					this.deviceOldQuery.pageNum++;
					this.status = "loading";
					this.getDeviceOldInfo();
					// console.log("现在是第几页", this.page);
				}
			}
		},
		methods: {
			dateChange() {
				this.$nextTick(() => {
					this.showTime = ''
					this.choiceTime2 = JSON.parse(JSON.stringify(this.choiceTime))
					// console.log("this.choiceTime", this.choiceTime);
					this.deviceOldQuery.pageNum = 1
					this.oldInfoList = []
					this.getDeviceOldInfo()
				})
			},
			changeTab() {
				//v-tabs切换
				this.deviceOldQuery.pageNum = 1
				this.getDeviceOldInfo()
				if (this.current == 1) {
					this.oldInfoList = []
				}
			},
			setShowTime(val) {
				//
				this.showTime = val
				this.setChoiceTime()
			},
			getDeviceOldInfo() {
				// 获取设备相关历史数据
				// this.oldInfoList = [];
				// let obj = {
				// 	Id: this.deviceId,
				// 	Code: this.activeAttr.Code
				// };
				if (this.choiceTime && this.choiceTime.length > 0) {
					if (dayjs(this.choiceTime[1]).isBefore(this.choiceTime[0]) || dayjs(this.choiceTime[1]).isSame(this
							.choiceTime[0])) {
						this.$refs.promptMsg.noticeOpen('The end time cannot be less than or equal to the start time!',
							'System prompt information')
						this.status = 'noMore'
						return
					}
				} else if (!this.choiceTime || this.choiceTime && this.choiceTime.length == 0) {
					this.$refs.promptMsg.noticeOpen('The time range cannot be empty!',
						'System prompt information')
					this.status = 'noMore'
					return
				}
				let obj = this.addDateRange({
						Id: this.deviceId,
						Code: this.activeAttr.Code
					},
					this.choiceTime
				)
				// obj.beginTime = this.parseTime(obj.beginTime);
				// obj.endTime = this.parseTime(obj.endTime);
				// obj.beginTime = dayjs(new Date(this.choiceTime[0])).format('YYYY-MM-DD HH:mm:ss');
				// obj.endTime = dayjs(new Date(this.choiceTime[1])).format('YYYY-MM-DD HH:mm:ss');
				if (this.current == 0) {
					obj.pageNum = this.deviceOldQuery.pageNum
					obj.pageSize = this.deviceOldQuery.pageSize
				} else {
					delete obj.pageNum
					delete obj.pageSize
				}
				if (obj.pageNum == 1) {
					this.oldInfoList = []
				}
				// obj.beginTime = '2023-11-21 00:00:00';
				// obj.endTime = '2023-11-21 23:59:59';
				// console.log("时间转换后", obj);
				this.status = 'loading'
				DeviceOldInfo(obj).then((res) => {
					// console.log("历史数据111", res);
					if (this.current == 0) {
						if (obj.pageNum == 1) {
							this.oldInfoList = []
						}
						this.oldInfoList = [...this.oldInfoList, ...res.data.List]
						if (res.data.List.length < obj.pageSize) {
							this.status = 'noMore'
						} else {
							this.status = 'more'
						}
					} else {

						this.oldInfoList = JSON.parse(JSON.stringify(res.data.List));
						this.status = 'noMore'
					}

					// let arrt = [23, 45, 57, 567, 56, 5, 99, 65, 56,99, 65, 56];
					// this.oldInfoList = arrt;
					if (this.current == 1) {
						if (this.activeAttr.OptionType == "geo") {

						} else {
							let mydata = [];
							// console.log("历史数据", this.oldInfoList);
							let xData = [];
							for (let i = 0; i < this.oldInfoList.length; i++) {
								// mydata = [
								//   ...mydata,
								//   ...[[this.oldInfoList[i].UpdatedOn, this.oldInfoList[i].Value]]
								// ];
								this.oldInfoList[i].value = this.oldInfoList[i].Value;
								this.oldInfoList[i].name = this.oldInfoList[i].UpdatedOn;
								// console.log("刚加载完数据", mydata);
								let rowUpDate = dayjs(this.oldInfoList[i].UpdatedOn).format('YYYY-MM-DD') + '\n' +
									dayjs(this.oldInfoList[i].UpdatedOn).format('HH:mm:ss')
								xData = [...xData, ...[
									[rowUpDate, this.oldInfoList[i].Value.toFixed(2)]
								]];
								// console.log("xData", xData);
							}
							this.chartsDataLine1.series[0].name = this.activeAttr.Name
							this.chartsDataLine1.series[0].data = JSON.parse(JSON.stringify(xData))
							// if (this.oldInfoList.length > 0) {
							// 	this.echartsHei = 338.5;
							// }
							mydata = JSON.parse(JSON.stringify(this.oldInfoList));
						}
					}
				}).catch(err => {
					// console.log("err数据错误", err);
					this.setMsgTop(err)
				});
			},
			setChoiceTime() {
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
					this.getDeviceOldInfo();
				}
			},
		}
	}
</script>

<style lang="less" scoped>
	.history_con {
		// width: 100%;
		// padding: 0 20rpx 50rpx;
		box-sizing: border-box;
		width: 100%;

		.tab_ul_con {
			width: 100%;
			padding-top: 20rpx;
			background-color: rgba(22, 26, 38, 1);
			display: flex;
			justify-content: center;
		}

		.tab_ul {
			width: calc(100% - 40rpx);
			height: 88rpx;
			display: flex;
			justify-content: center;
			align-items: center;
			border: 1rpx solid rgba(255, 255, 255, 0.20);
			border-radius: 10rpx;
			background-color: rgba(22, 26, 38, 1);

			.tab_li {
				width: calc(100% / 3);
				height: 80rpx;
				border-radius: 10rpx;
				display: flex;
				justify-content: center;
				align-items: center;
				color: rgba(255, 255, 255, 0.5);

				&.active_li {
					background: linear-gradient(180deg, #FF3535 0%, #FF613D 100%);
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
			background-color: rgba(22, 26, 38, 1);

			.li_label {
				color: rgba(255, 255, 255, 0.5);
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

		.history_title {
			background-color: rgba(22, 26, 38, 1);
			padding: 40rpx 0 40rpx 20rpx;
			font-size: 32rpx;
			line-height: 32rpx;
			color: rgba(255, 255, 255, 1);
			display: flex;
			align-items: center;
			justify-content: flex-start;
			width: 100%;
			box-sizing: border-box;

			.unit {
				color: rgba(255, 255, 255, 0.5);
				margin-left: 10rpx;
			}
		}

		.history_con {
			width: 100%;
			padding: 0 20rpx 50rpx;
			box-sizing: border-box;
		}

		.history_ul {
			width: 100%;
			background-color: rgba(28, 34, 50, 1);
			border-radius: 10rpx;

			.history_li {
				width: 100%;
				height: 100rpx;
				display: flex;
				justify-content: space-between;
				align-items: center;
				padding: 0 30rpx;
				box-sizing: border-box;
				color: rgba(255, 255, 255, 1);
				font-size: 32rpx;

				.li_time {
					color: rgba(255, 255, 255, 0.5);
					font-size: 24rpx;
				}
			}
		}

		.history_view {
			width: 100%;
			height: 846rpx;
			display: flex;
			justify-content: center;
			align-items: flex-start;
		}
	}
</style>