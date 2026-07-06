<template>
	<view class="pages_bgcon" style="background-color: rgba(255, 255, 255, 1);min-height: 100vh;height: auto;">
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
				:zIndex="997" v-if="oldType&&oldType=='online'">
				<!-- 140rpx+20+36-4 -->
				<template v-slot:allslot>
					<view class="date_con_con">
						<view class="li_label">属性</view>
						<view class="int_date">
							<view class="date_con long_date" style="color: #333;">
								<uni-data-select v-model="activeAttr.Code" :localdata="properties"
									@change="changeProperties" width="100%" placeholder="请选择值"
									borderColor="rgba(255, 255, 255, 0.20)" palColor="rgba(193, 193, 193, 1)"
									:isCustom="true" :isDark="false"></uni-data-select>
							</view>
						</view>
					</view>
				</template>
			</uni-nav-bar>
			<v-tabs v-model="current" :scroll="false" :tabs="tabArr" color="#999999"
				activeColor="#333333" :fixed="true" @change="changeTab" :fixedLineWid="true"
				fontSize="28rpx" activeFontSize="32rpx" paddingItem="0" lineHeight="4rpx" :hasBorder="false"
				height="72rpx" :padding="activeAttr.OptionType&&activeAttr.OptionType=='onLine'?'0 calc(100% - 104rpx) 0 0 ':'0 calc(100% - 200rpx) 0 0'" bgColor="#ffffff" lineColor="#333333"
				:zIndex="996" scrollBgColor="#ffffff" scrollConWid="100%" :bold="true"></v-tabs>
			<uni-nav-bar :status-bar="false" :fixed="true" :border="false" height="112rpx" backgroundColor="inherit"
				v-if="current==0" :zIndex="996">
				<template v-slot:allslot>
					<view class="history_title" v-if="activeAttr.OptionType&&activeAttr.OptionType=='onLine'">
						<view class="name">设备离在线数据</view>
					</view>
					<view class="history_title" v-if="activeAttr.OptionType&&activeAttr.OptionType!='onLine'">
						<view class="name">{{activeAttr.Name}}</view>
						<view class="unit" v-if="activeAttr.Unit">
							{{'（'+activeAttr.Unit+'）'}}
						</view>
					</view>
					
				</template>
			</uni-nav-bar>
			<view class="history_con">
				<view class="history_ul" v-if="current==0">
					<view class="history_li_con">
						<view class="history_li" v-for="(item,inx) in oldInfoList" v-if="activeAttr.OptionType&&activeAttr.OptionType=='onLine'" :key="'k'+inx">
							<view class="li_val">
								{{item.IsOnline?'在线':'离线'}}
							</view>
							<view class="li_time">
								{{item.CreatedOn}}
							</view>
						</view>
						<view class="history_li" v-for="(item,inx) in oldInfoList" v-if="activeAttr.OptionType&&activeAttr.OptionType!='onLine'" :key="'t'+inx+item.Code">
							<view class="li_val" v-if="activeAttr.OptionType!='enum'">
								{{item.Value.toFixed(2)+' '+item.Unit}}
							</view>
							<view class="li_val" v-if="activeAttr.OptionType==='enum'">
								{{item.Value?item.Value:''}}
							</view>
							<view class="li_time">
								{{item.UpdatedOn}}
							</view>
						</view>
					</view>
					<uni-load-more iconType="circle" :status="status" v-if="status" />
				</view>
				<view class="history_view" v-show="current==1">
					<!-- <qiun-data-charts type="line" :opts="{extra:{line:{type:'curve'}},legend:{show: false}}"
						:eopts="echartOpts" :chartData="chartsDataLine1" :echartsH5="true" :echartsApp="true"
						:loadingType="0"></qiun-data-charts> -->
						<view v-show="activeAttr.OptionType!='enum'" class="" :style="{width: '100%', height: 'calc(100vh - 566rpx - '+statusBarHeight+')'}">
							<l-echart class="line-chart" uid="history1" ref="lineChart"></l-echart>
						</view>
						<gantt_chart :activeOldAttr="activeAttr" :oldInfoList="oldInfoList" v-if="activeAttr.OptionType=='enum'" ref="gantt_chart"></gantt_chart>
				</view>
			</view>
		</view>

		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		DeviceOldInfo,
		DeviceOnLineOldInfo
	} from '@/api/device.js'
	import echarts from '@/pages_device/echarts.min.js'
	var dayjs = require('@/common/day.js')
	import {
		parseTime
	} from '@/common/utillib.js'
	import gantt_chart from './gantt_chart.vue'
	export default {
		components:{
			gantt_chart
		},
		data() {
			return {
				statusBarHeight: Number(uni.getSystemInfoSync().statusBarHeight) * 2 + 'rpx',
				properties:[],//设备属性参数
				oldType:'',
				topTitle: '历史数据',
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
					},{
						// show: false,
						text: '(Kpa)',
						top: '14',
						left: 0,
						textStyle: { //标题样式
							fontSize: '14',
							color: '#999999',
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
						left: '3%',
						right: '5%',
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
					dataZoom: [{
						type: 'inside',
						start: 0,
						end: 100,
					}, {
						bottom: 10, //下滑块距离x轴底部的距离
						height: 20, //下滑块手柄的高度调节
						type: 'slider', //类型,滑动块插件
						show: true, //是否显示下滑块
						xAxisIndex: [0], //选择的x轴
						start: 120, //初始数据显示多少
						end: 135, //初始数据最多显示多少
						zoomLock: true
					}],
					series: {
						name: '',
						type: 'line',
						data: [],
						color: ["#1c84c6"], //折线条的颜色
						showSymbol: true, //是否默认展示圆点
						// symbol: "circle", //设定为实心点
						// symbolSize: 8, //设定实心点的大小
						itemStyle: {
							normal: {
								color: "#2371FF", //改变折线点的颜色
								borderColor: "#ffffff",
								borderWidth: 2,
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
					}
				}
			}
		},
		onLoad(options) {
			
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
			if(options.oldType){
				this.oldType=options.oldType
			}else{
				this.oldType=''
			}
			this.echartOpts.title[0].text = this.activeAttr.Name
			if(this.activeAttr.Unit){
				this.echartOpts.title[1].text = '（' + this.activeAttr.Unit + '）'
				this.echartOpts.title[1].left = 0
			}else{
				this.echartOpts.title[1].text = ''
				this.echartOpts.title[1].left = 0
			}
			if(this.oldType){
				const eventChannel = this.getOpenerEventChannel();
				// 监听acceptDataFromOpenerPage事件，获取上一页面通过eventChannel传送到当前页面的数据
				eventChannel.on('acceptDataFromOpenerPage', (data) => {
					// console.log(data.properties,data.properties.length,'data.properties');
					this.properties = data.properties.filter(
						(item) =>
						item.option.type == "date" ||
						item.option.type == "float" ||
						item.option.type == "int" ||
						item.option.type == "enum"
					);
					
					// item.option.type == "geo" ||
					this.properties=this.properties.map(row => {
						row.text = row.name
						row.value = row.code
						return row
					})
					this.properties.push({
						code: 'leavingOnline',
						description: '',
						name: '离在线',
						option:{
							type:'onLine',
							unit:''
						},
						updatedOn: "",
						text:'离在线',
						value: 'leavingOnline'
					})
				})
				
				this.choiceTime=[]
				this.showTime=''
				this.deviceOldQuery.pageNum=1
				this.deviceOldQuery.pageSize=100
				this.activeAttr={
					Code: 'leavingOnline',
					Description: '',
					Name: '离在线',
					OptionType: 'onLine',
					Unit: '',
					UpdatedOn: "",
					Value: ''
				}
				if(this.activeAttr.OptionType&&this.activeAttr.OptionType=='onLine'){
					this.tabArr=['表格']
					this.getOnlineOld()
				}else{
					this.tabArr=['表格', '图表']
				}
				
			}else{
				this.tabArr=['表格', '图表']
				if (this.deviceId && this.activeAttr.Code) {
					// this.setChoiceTime()
					this.choiceTime=[]
					this.showTime=''
					this.deviceOldQuery.pageNum=1
					this.deviceOldQuery.pageSize=100
					this.getDeviceOldInfo()
				}
				
			}
			
			
			// this.chartsDataLine1 = {
			// 	"series": [{
			// 		"name": "时间轴1",
			// 		"data": [
			// 			[10000, 55],
			// 			[30000, 25],
			// 			[50000, 55],
			// 			[70000, 25],
			// 			[90000, 55]
			// 		]
			// 	}]
			// }
			

		},
		onReachBottom() { //上拉触底
			// console.log("现在的状态是什么", this.status);
			if (this.current == 0) {
				if (this.status != 'noMore') {
					this.deviceOldQuery.pageNum++;
					this.status = "loading";
					if(this.activeAttr.OptionType&&this.activeAttr.OptionType=='onLine'){
						this.getOnlineOld()
					}else{
						this.getDeviceOldInfo()
					}
					// console.log("现在是第几页", this.page);
				}
			}
		},
		methods: {
			returnEchartKey(){
				let time=(new Date()).getTime()
				return time
			},
			changeProperties(val){
				//历史数据属性修改
				let selObj=this.properties.find(row=>row.code==val)
				// console.log("选中",val,selObj);
				this.activeAttr={
					Code: selObj.code,
					Description: selObj.description,
					Name: selObj.name,
					OptionType: selObj.option.type,
					Unit: selObj.option.unit,
					UpdatedOn: ""
				}
				this.deviceOldQuery.pageNum = 1
				if(this.activeAttr.OptionType&&this.activeAttr.OptionType=='onLine'){
					this.current = 0
					this.tabArr=['表格']
					this.getOnlineOld()
				}else{
					this.tabArr=['表格', '图表']
					this.getDeviceOldInfo()
				}
			},
			getOnlineOld(){
				//获取离在线历史数据
				if(this.choiceTime&&this.choiceTime.length>0){
					if(dayjs(this.choiceTime[1]).isBefore(this.choiceTime[0])||dayjs(this.choiceTime[1]).isSame(this.choiceTime[0])){
						this.$refs.promptMsg.noticeOpen('结束时间不能小于或等于开始时间','提示信息')
						this.status = 'noMore'
						return 
					}
				}
				let obj = this.addDateRange({
						Id: this.deviceId
					},
					this.choiceTime
				)
				if (this.current == 0) {
					obj.pageNum = this.deviceOldQuery.pageNum
					obj.pageSize = this.deviceOldQuery.pageSize
				}
				if (obj.pageNum == 1) {
					this.oldInfoList = []
				}
				this.status = 'loading'
				DeviceOnLineOldInfo(obj).then((res) => {
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
					}
				}).catch(err => {
					// console.log("err数据错误", err);
					this.setMsgTop(err)
				});
			},
			dateChange() {
				this.$nextTick(() => {
					this.choiceTime2 = this.choiceTime
					this.showTime=''
					this.deviceOldQuery.pageNum = 1
					this.oldInfoList = []
					if(this.activeAttr.OptionType&&this.activeAttr.OptionType=='onLine'){
						this.getOnlineOld()
					}else{
						this.getDeviceOldInfo()
					}
					
				})
			},
			changeTab() {
				//v-tabs切换
				this.deviceOldQuery.pageNum = 1
				if(this.activeAttr.OptionType&&this.activeAttr.OptionType=='onLine'){
					this.getOnlineOld()
				}else{
					this.getDeviceOldInfo()
					
				}
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
				if(this.choiceTime&&this.choiceTime.length>0){
					if(dayjs(this.choiceTime[1]).isBefore(this.choiceTime[0])||dayjs(this.choiceTime[1]).isSame(this.choiceTime[0])){
						this.$refs.promptMsg.noticeOpen('结束时间不能小于或等于开始时间','提示信息')
						this.status = 'noMore'
						if(this.current==1){
							this.chartsDataLine1.series[0].name = this.activeAttr.Name
							this.chartsDataLine1.series[0].data = []
						}
						return 
					}
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
				// obj.beginTime = '2023-11-21 00:00:00';
				// obj.endTime = '2023-11-21 23:59:59';
				// console.log("时间转换后", obj);
				this.status = 'loading'
				DeviceOldInfo(obj).then((res) => {
					// console.log("历史数据111", res,obj.pageNum);
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
							if(this.activeAttr.OptionType == "enum"){
								for (let i = 0; i < this.oldInfoList.length; i++) {
									// mydata = [
									//   ...mydata,
									//   ...[[this.oldInfoList[i].UpdatedOn, this.oldInfoList[i].Value]]
									// ];
									this.oldInfoList[i].value = this.oldInfoList[i].Value;
									this.oldInfoList[i].name = this.oldInfoList[i].UpdatedOn;
									// console.log("刚加载完数据", mydata);
									// let rowUpDate = dayjs(this.oldInfoList[i].UpdatedOn).format('YYYY-MM-DD') + '\n' +
									// 	dayjs(this.oldInfoList[i].UpdatedOn).format('HH:mm:ss')
									// xData = [...[
									// 	[rowUpDate, Number(this.oldInfoList[i].Value.toFixed(2))]
									// ],...xData];
									// console.log("xData", xData);
								}
							}else{
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
									xData = [...[
										[rowUpDate, Number(this.oldInfoList[i].Value.toFixed(2))]
									],...xData];
									// console.log("xData", xData);
								}
								this.echartOpts.title[0].text = this.activeAttr.Name
								if(this.activeAttr.Unit){
									this.echartOpts.title[1].text = '（' + this.activeAttr.Unit + '）'
									this.echartOpts.title[1].left = this.activeAttr.Name.length*14
								}else{
									this.echartOpts.title[1].text = ''
									this.echartOpts.title[1].left = this.activeAttr.Name.length*14
								}
								// this.chartsDataLine1.series[0].name = this.activeAttr.Name
								// this.chartsDataLine1.series[0].data = JSON.parse(JSON.stringify(xData))
								this.echartOpts.series.name = this.activeAttr.Name
								this.echartOpts.series.data = JSON.parse(JSON.stringify(xData))
								// console.log(this.echartOpts,'this.echartOptsthis.echartOpts');
								this.$refs.lineChart.init(echarts, chart => {
									// console.log("chart",chart);
									chart.setOption(this.echartOpts);
								});
							}
							
							// if (this.oldInfoList.length > 0) {
							// 	this.echartsHei = 338.5;
							// }
							// mydata = JSON.parse(JSON.stringify(this.oldInfoList));
						}
					}
				}).catch(err => {
					console.log("err数据错误", err);
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
					end=dayjs(new Date(end)).format('YYYY-MM-DD HH:mm:ss')
					start=dayjs(new Date(start)).format('YYYY-MM-DD HH:mm:ss')
					this.choiceTime.push(start);
					this.choiceTime.push(end);
					this.choiceTime2.push(start);
					this.choiceTime2.push(end);
				}
				if (this.showTime == "Last week") {
					const { startOfWeek, endOfWeek } = this.getLastWeekRange()
					let end = endOfWeek;
					let start = startOfWeek;
					// let end = new Date();
					// let start = new Date();
					// start.setTime(start.getTime() - 3600 * 1000 * 24 * 7);
					// end=dayjs(new Date(end)).format('YYYY-MM-DD HH:mm:ss')
					// start=dayjs(new Date(start)).format('YYYY-MM-DD HH:mm:ss')
					this.choiceTime.push(start);
					this.choiceTime.push(end);
					this.choiceTime2.push(start);
					this.choiceTime2.push(end);
				}
				if (this.showTime == "Last month") {
					let dateval=this.getLastMonthBoundaries()
					let end = dateval.lastDay;
					let start = dateval.firstDay;
					// let end = new Date();
					// let start = new Date();
					// start.setTime(start.getTime() - 3600 * 1000 * 24 * 30);
					// end=dayjs(new Date(end)).format('YYYY-MM-DD HH:mm:ss')
					// start=dayjs(new Date(start)).format('YYYY-MM-DD HH:mm:ss')
					this.choiceTime.push(start);
					this.choiceTime.push(end);
					this.choiceTime2.push(start);
					this.choiceTime2.push(end);
				}
				if (this.showTime) {
					this.deviceOldQuery.pageNum = 1
					if(this.activeAttr.OptionType&&this.activeAttr.OptionType=='onLine'){
						this.getOnlineOld()
					}else{
						this.getDeviceOldInfo()
					}
				}
			},
			getLastWeekRange(){
				//获取上一周时间的函数
			    // 获取当前日期
			    const now = dayjs();
			    // 获取上周周一的日期（周一作为一周的开始）
			    const startOfWeek = now.startOf('week').subtract(1, 'week').format('YYYY-MM-DD HH:mm:ss'); // 减去一周得到上周的周一
			    // 获取上周周日的日期（周日作为一周的结束）
			    const endOfWeek = now.endOf('week').subtract(1, 'week').format('YYYY-MM-DD HH:mm:ss'); // 减去一周得到上周的周日
			    return { startOfWeek, endOfWeek };
			},
			getLastMonthBoundaries() {
			    // 获取上个月的时间点
			    const lastMonth = dayjs().subtract(1, 'month');
			    
			    // 获取上个月的第一天
			    const firstDayOfLastMonth = lastMonth.startOf('month').format('YYYY-MM-DD HH:mm:ss');
			    
			    // 获取上个月的最后一天
			    const lastDayOfLastMonth = lastMonth.endOf('month').format('YYYY-MM-DD HH:mm:ss');
			    
			    // 返回上个月第一天和最后一天的日期
			    return {
			        firstDay: firstDayOfLastMonth,
			        lastDay: lastDayOfLastMonth
			    };
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
				padding-top: 8rpx;
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
			font-size: 28rpx;
			line-height: 28rpx;
			color: #333333;
			display: flex;
			align-items: center;
			justify-content: flex-start;
			width: 100%;
			box-sizing: border-box;

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
			.history_li_con{
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
			width: 750rpx;
			margin: 0 0 0 -20rpx;
			height: 846rpx;
			display: flex;
			justify-content: center;
			align-items: flex-start;
		}
	}
</style>