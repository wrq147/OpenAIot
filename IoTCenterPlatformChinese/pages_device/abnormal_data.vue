<template>
	<view class="pages_bgcon" style="background-color: rgba(255, 255, 255, 1);">
		<top :title="topTitle" leftWidth="60rpx" leftIcon="icon-fanhui" rightWidth="60rpx" :isleftBack="true"
			backgroundColor="#ffffff"></top>

		<view class="history_con">
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
			<uni-nav-bar :status-bar="false" :fixed="true" :border="false" height="192rpx" backgroundColor="inherit"
				:zIndex="998">
				<!-- 140rpx+20+36-4 -->
				<template v-slot:allslot>
					<view class="date_con_con">
						<view class="li_label">属性</view>
						<view class="int_date">
							<view class="date_con long_date" style="color: #333;">
								<uni-data-select v-model="deviceOldQuery.Code" :localdata="properties"
									@change="changeProperties" width="100%" placeholder="请选择值"
									borderColor="rgba(255, 255, 255, 0.20)" palColor="rgba(193, 193, 193, 1)"
									:isCustom="true" :isDark="false"></uni-data-select>
							</view>
						</view>
					</view>
				</template>
			</uni-nav-bar>
			<v-tabs v-model="current" :scroll="false" :tabs="tabArr" color="#999999" activeColor="#333333" :fixed="true"
				@change="changeTab" :fixedLineWid="true" fontSize="28rpx" activeFontSize="28rpx" paddingItem="0"
				lineHeight="4rpx" :hasBorder="false" height="72rpx" padding="0 50% 0 0 " bgColor="#ffffff"
				lineColor="#333333" :zIndex="996" scrollBgColor="#ffffff" scrollConWid="100%"></v-tabs>
			<!-- <uni-nav-bar :status-bar="false" :fixed="true" :border="false" height="112rpx" backgroundColor="inherit"
				v-if="current==0" :zIndex="996">
				<template v-slot:allslot>
					<view class="history_title">
						<view class="name">{{activeAttr.Name}}</view>
						<view class="unit" v-if="activeAttr.Unit">
							{{'（'+activeAttr.Unit+'）'}}
						</view>
					</view>
				</template>
			</uni-nav-bar> -->
			<view class="history_con">
				<view class="history_ul" v-if="current==0">
					<view class="history_li_con">
						<view class="history_li" v-for="item in oldInfoList">
							<view class="li_val">
								{{item.Value.toFixed(2)+' '+item.Unit}}
							</view>
							<view class="li_time">
								{{item.UpdatedOn}}
							</view>
						</view>
					</view>
					<uni-load-more iconType="circle" :status="status" v-if="status" />
				</view>
				<view class="history_view" v-if="current==1" style="height: auto;">
					<!-- <qiun-data-charts v-for="(item,inx) in chartsDataLineList"
						v-if="chartsDataLineList&&chartsDataLineList.length>0&&status!='loading'" type="mix"
						:opts="{extra:{line:{type:'curve'},scatter:{type:'curve'}},legend:{show: false},enableScroll: true,enableScrollBounce: true,}"
						:eopts="echartOptsList[inx]" :chartData="item" :echartsH5="true" :echartsApp="true"
						:loadingType="0" :ontouch="true" /> -->
						<view v-for="(item,inx) in echartOptsList" class="" :style="{width: '100%', height: '400rpx'}" v-if="echartOptsList&&echartOptsList.length>0&&status!='loading'">
							<l-echart class="line-chart" :uid="'abnormal_data'+inx" :ref="'lineChart'+inx"></l-echart>
						</view>
					<uni-load-more iconType="circle" :status="status" v-if="chartsDataLineList.length==0||status=='loading'" />
				</view>
			</view>
		</view>

		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		DeviceOldInfo,
		abnormalDataInfo
	} from '@/api/device.js'
	import echarts from '@/pages_device/echarts.min.js'
	var dayjs = require('@/common/day.js')
	import {
		parseTime
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				properties: [],
				topTitle: '异常数据',
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
				chartsDataLine1: {},
				echartOpts: {
					title: [{
						// show: false,
						text: '排气压力',
						top: '14',
						left: 0,
						textStyle: { //标题样式
							fontSize: '14',
							color: '#333333',
						}
					}, {
						// show: false,
						text: '(Kpa)',
						top: '14',
						left: 0,
						textStyle: { //标题样式
							fontSize: '14',
							color: '#999999',
						}
					}],
					dataZoom: [{
						type: "slider",
						start: 0,
						end: 100,
						left: 20, // 组件离左侧的距离，默认为 10，表示10%的宽度
						bottom: '10',

					}],
					tooltip: {
						trigger: 'axis',
					},
					legend: {
						"show": false,
					},
					grid: [{
						left: '2%',
						right: '8%',
						// bottom: '5%',
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
							color: "#999999", //坐标轴的文本颜色
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
								color: ['#EAEAEA'],
								width: 1,
								type: 'dashed'
							}
						},
						axisLabel: {
							color: "#999999", //坐标轴的文本颜色
							fontSize: '12'
						}
					},
					series: [{
						name: '',
						type: 'line',
						data: [],
						color: ["#1c84c6"], //折线条的颜色
						showSymbol: true, //是否默认展示圆点
						z:0,
						// symbol: "circle", //设定为实心点
						// symbolSize: 8, //设定实心点的大小
						itemStyle: {
							normal: {
								color: "#2371FF", //改变折线点的颜色
								borderColor: "#ffffff",
								borderWidth: 0,
								
								lineStyle: {
									color: "#2371FF" //改变折线颜色
								}
							}
						},
						areaStyle: {
							color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{
									offset: 0,
									color: "rgba(35, 113, 255, 0.20)"
								},
								{
									offset: 1,
									color: "rgba(41, 83, 255, 0)"
								}
							])
						},
					},{
						name: '',
						type: 'scatter',
						data: [],
						color: ["#1c84c6"], //折线条的颜色
						showSymbol: true, //是否默认展示圆点
						z:1,
						// symbol: "circle", //设定为实心点
						// symbolSize: 8, //设定实心点的大小
						itemStyle: {
							normal: {
								color: "#E74032", //改变折线点的颜色
								borderColor: "#ffffff",
								borderWidth: 0,
								lineStyle: {
									color: "#E74032" //改变折线颜色
								}
							}
						},
					}]
				},
				echartOptsList: [],
				chartsDataLineList: [],
				echartsCodeGroup: []
			}
		},
		async onLoad(options) {
			const eventChannel = this.getOpenerEventChannel();
			// 监听acceptDataFromOpenerPage事件，获取上一页面通过eventChannel传送到当前页面的数据
			eventChannel.on('acceptDataFromOpenerPage', (data) => {
				this.properties = data.properties.filter(
					(item) =>
					item.option.type == "date" ||
					item.option.type == "float" ||
					item.option.type == "int" ||
					item.option.type == "geo"
				);
				this.properties.map(row => {
					row.text = row.name
					row.value = row.code
				})
			})
			if (options.id) {
				this.deviceId = options.id
			}
			if (options.activeAttr) {
				this.activeAttr = JSON.parse(options.activeAttr)
			}
			if (options.deviceNu) {
				this.deviceNu = options.deviceNu
				this.topTitle = '历史数据（' + options.deviceNu + '）'
			}
			if (this.deviceId) {
				// this.setChoiceTime()
				this.choiceTime = []
				this.showTime = ''
				this.deviceOldQuery.pageNum = 1
				this.deviceOldQuery.pageSize = 100
				await this.getDeviceOldInfo()
			}
			// this.echartOpts.title[0].text = this.activeAttr.Name
			// if (this.activeAttr.Unit) {
			// 	this.echartOpts.title[1].text = '（' + this.activeAttr.Unit + '）'
			// 	this.echartOpts.title[1].left = this.activeAttr.Name.length * 14
			// } else {
			// 	this.echartOpts.title[1].text = ''
			// 	this.echartOpts.title[1].left = this.activeAttr.Name.length * 14
			// }

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
			changeProperties() {
				//产品属性修改了

			},
			dateChange() {
				this.$nextTick(() => {
					this.choiceTime2 = this.choiceTime
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
			async getDeviceOldInfo() {
				// 获取设备相关历史数据
				this.echartOptsList = [];
				this.chartsDataLineList = [];
				this.status = 'loading'
				try {
					let obj = this.addDateRange({
							Id: this.deviceId,
							Code: this.activeAttr.Code
						},
						this.choiceTime
					)
					if (this.current == 0) {
						obj.pageNum = this.deviceOldQuery.pageNum
						obj.pageSize = this.deviceOldQuery.pageSize
					} else {
						if(this.choiceTime&&this.choiceTime.length>0){
							delete obj.pageNum
							delete obj.pageSize
						}else{
							obj.pageNum = this.deviceOldQuery.pageNum
							obj.pageSize = this.deviceOldQuery.pageSize
						}
					}

					if (obj.pageNum == 1) {
						this.oldInfoList = []
					}
					
					let errInfo = await abnormalDataInfo(obj)
					let oldInfo = {}
					if (this.current == 0) {
						if (obj.pageNum == 1) {
							this.oldInfoList = []
						}
						this.oldInfoList = [...this.oldInfoList, ...errInfo.data.List]
						if (errInfo.data.List.length < obj.pageSize) {
							this.status = 'noMore'
						} else {
							this.status = 'more'
						}
					} else {
						if (
							this.current == 1 &&
							errInfo.data &&
							errInfo.data.List &&
							errInfo.data.List.length > 0
						) {
							let times = errInfo.data.List.map((rw) => {
								return new Date(rw.UpdatedOn);
							});
							const minTime = new Date(
								Math.min(...times.map((time) => time.getTime()))
							);
							const maxTime = new Date(
								Math.max(...times.map((time) => time.getTime()))
							);
							let startDate = dayjs(minTime).format("YYYY-MM-DD HH:mm:ss");
							let endDate = dayjs(maxTime).format("YYYY-MM-DD HH:mm:ss");
							obj.beginTime = this.parseTime(startDate);
							obj.endTime = this.parseTime(endDate);
							delete obj.pageNum;
							delete obj.pageSize;
							oldInfo = await DeviceOldInfo(obj);
						}
						this.oldInfoList = [];
						this.echartsCodeGroup = [];
						let errorList = JSON.parse(JSON.stringify(errInfo.data.List));
						let afterGroupErrorList = this.listGrouping(errorList, "Code");
						let afterGroupHistoryList = {}
						if (oldInfo.data && oldInfo.data.List.length > 0) {
							afterGroupHistoryList = this.listGrouping(oldInfo.data.List, "Code");
						}
						if (errorList && errorList.length > 0) {
							for (let codeKey in afterGroupErrorList) {
								let groupObj = {
									oldList: afterGroupHistoryList[codeKey],
									errorList: afterGroupErrorList[codeKey],
									code: codeKey,
								};
								this.echartsCodeGroup.push(groupObj);
							}
						} else {
							// this.errorList = [];
						}
						this.status = 'noMore'
					}
					if (this.current == 1) {
						if (this.activeAttr.OptionType == "geo") {

						} else {
							if (this.echartsCodeGroup && this.echartsCodeGroup.length > 0) {

								for (let ix = 0; ix < this.echartsCodeGroup.length; ix++) {
									let attrCode = this.echartsCodeGroup[ix].code;
									let attrCodeObj = this.properties.find(rw => rw.code == attrCode)
									console.log(attrCodeObj,'attrCodeObj');
									let oldInfoList = this.echartsCodeGroup[ix].oldList;
									let errorList = this.echartsCodeGroup[ix].errorList;
									if (oldInfoList.length > 0 && errorList.length > 0) {
										this.echartOptsList[ix] = JSON.parse(JSON.stringify(this.echartOpts))
										if(attrCodeObj){
											this.echartOptsList[ix].title[0].text = attrCodeObj.name
											
											if (attrCodeObj.option && attrCodeObj.option.unit) {
												this.echartOptsList[ix].title[1].text = '（' + attrCodeObj.option.unit + '）'
												this.echartOptsList[ix].title[1].left = attrCodeObj.name.length * 14
											} else {
												this.echartOptsList[ix].title[1].text = ''
												this.echartOptsList[ix].title[1].left = attrCodeObj.name.length * 14
											}
										}else{
											this.echartOptsList[ix].title[0].text=''
											if (attrCodeObj&&attrCodeObj.option && attrCodeObj.option.unit) {
												this.echartOptsList[ix].title[1].text = '（' + attrCodeObj.option.unit + '）'
												this.echartOptsList[ix].title[1].left = attrCodeObj.name.length * 14
											} else {
												this.echartOptsList[ix].title[1].text = ''
												this.echartOptsList[ix].title[1].left = 0
											}
										}
										

										let xData = [];
										// let xData2 = [];
										let oldSourceData = []
										for (let i = 0; i < oldInfoList.length; i++) {
											oldInfoList[i].value = oldInfoList[i].Value;
											oldInfoList[i].name = oldInfoList[i].UpdatedOn;
											// console.log("刚加载完数据", mydata);
											let rowUpDate = dayjs(oldInfoList[i].UpdatedOn).format('YYYY-MM-DD') +
												'\n' + dayjs(oldInfoList[i].UpdatedOn).format('HH:mm:ss')
											oldSourceData = [...oldSourceData, ...[
												[rowUpDate, Number(oldInfoList[i].Value.toFixed(2))]
											]];
											xData.push(rowUpDate)
											// oldSourceData.push(oldInfoList[i].value)
										}
										let errorSourceData = []
										for (let i = 0; i < errorList.length; i++) {
											let row = errorList[i]
											let rowUpDate = dayjs(row.UpdatedOn).format('YYYY-MM-DD') +
												'\n' + dayjs(row.UpdatedOn).format('HH:mm:ss')
											errorSourceData = [...errorSourceData, ...[
												[rowUpDate, Number(row.Value.toFixed(2))]
											]];
											// xData2.push(rowUpDate)
											// errorSourceData.push(errorList[i].Value)
										}
										// this.echartOptsList[ix].xAxis.data=JSON.parse(JSON.stringify(xData))
										// this.echartOptsList[ix].xAxis.data = JSON.parse(JSON.stringify(xData))
										this.echartOptsList[ix].series[0].name = '历史数据'
										this.echartOptsList[ix].series[0].type = 'line'
										this.echartOptsList[ix].series[0].data = JSON.parse(JSON.stringify(oldSourceData))
										this.echartOptsList[ix].series[1].name = '异常数据'
										this.echartOptsList[ix].series[1].type = 'scatter'
										this.echartOptsList[ix].series[1].data = JSON.parse(JSON.stringify(errorSourceData))
										
										
										
									}

								}
								this.$nextTick(()=>{
									this.echartOptsList.map((item,inx)=>{
										this.$refs['lineChart'+inx][0].init(echarts, chart => {
											chart.setOption(item);
										});
									})
								})
							}
							// let mydata = [];
							// // console.log("历史数据", this.oldInfoList);
							// let xData = [];
							// for (let i = 0; i < this.oldInfoList.length; i++) {
							// 	this.oldInfoList[i].value = this.oldInfoList[i].Value;
							// 	this.oldInfoList[i].name = this.oldInfoList[i].UpdatedOn;
							// 	// console.log("刚加载完数据", mydata);
							// 	let rowUpDate = dayjs(this.oldInfoList[i].UpdatedOn).format('YYYY-MM-DD') + '\n' +
							// 		dayjs(this.oldInfoList[i].UpdatedOn).format('HH:mm:ss')
							// 	xData = [...xData, ...[
							// 		[rowUpDate, this.oldInfoList[i].Value.toFixed(2)]
							// 	]];
							// }
							// this.chartsDataLine1.series[0].name = this.activeAttr.Name
							// this.chartsDataLine1.series[0].data = JSON.parse(JSON.stringify(xData))
							// mydata = JSON.parse(JSON.stringify(this.oldInfoList));
						}
					}
				} catch (e) {
					//TODO handle the exception
					console.log("错误", e);
				}
			},
			listGrouping(list, groupParams) {
				return list.reduce((result, currentItem) => {
					// 使用 key 函数提取分组键，如果未定义则直接使用属性名
					const groupKey =
						typeof groupParams === "function" ?
						key(currentItem) :
						currentItem[groupParams];

					// 确保 result 对象中有对应分组的数组
					if (!result[groupKey]) {
						result[groupKey] = [];
					}

					// 将当前项添加到对应分组的数组中
					result[groupKey].push(currentItem);

					return result;
				}, {});
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

		.history_title {
			background-color: #ffffff;
			padding: 40rpx 0 40rpx 20rpx;
			font-size: 32rpx;
			line-height: 32rpx;
			color: #333333;
			display: flex;
			align-items: center;
			justify-content: flex-start;
			width: 100%;

			.unit {
				color: #999999;
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
			background-color: #F8F8F8;
			border-radius: 10rpx;

			.history_li_con {
				width: 100%;
				padding: 0 30rpx;
				box-sizing: border-box;
			}

			.history_li {
				width: 100%;
				height: 100rpx;
				display: flex;
				justify-content: space-between;
				align-items: center;
				box-sizing: border-box;
				color: #333333;
				font-size: 32rpx;
				border-bottom: 1rpx solid #EAEAEA;

				.li_time {
					color: #999999;
					font-size: 24rpx;
				}
			}
		}

		.history_view {
			width: 100%;
			height: 846rpx;
			display: flex;
			justify-content: center;
			align-items: center;
			flex-direction: column;
			
		}
	}
</style>