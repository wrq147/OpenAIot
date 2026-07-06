<template>
	<view class="" style="width: 100%;">
		<view class="state_con" v-if="activeOldAttr&&activeOldAttr.OptionType=='enum'">
			<view class="state_li" v-for="ke in Object.keys(enumValColor)" :key="'state'+ke">
				<view class="li_box" :style="{background:enumValColor[ke]}"></view>
				<view class="li_text">{{ke}}</view>
			</view>
		</view>
		<view class="" :style="{width: '100%', height: 'calc(100vh - 706rpx - '+statusBarHeight+')'}">
			<l-echart :style="{width: '100%', height: 'calc(100vh - 706rpx - '+statusBarHeight+')'}" class="line-chart2" uid="history2" ref="lineChart2"></l-echart>
		</view>
	</view>
</template>

<script>
	import {
		echartBarOpts
	} from './echartOpts.js'
	import echarts from '@/pages_device/echarts.min.js'
	var dayjs = require('@/common/day.js')
	export default {
		props: {
			activeOldAttr: {
				type: Object,
				default: () => {}
			},
			oldInfoList:{
				type:Array,
				default:()=>{
					return []
				}
			}
		},
		data() {
			return {
				statusBarHeight: Number(uni.getSystemInfoSync().statusBarHeight) * 2 + 'rpx',
				echartOpts: {},
				enumValColor: {},
				colorList: ['rgb(65,105,225)', 'rgb(135,206,250)', 'rgb(0,191,255)', 'rgb(176,224,230)', 'rgb(95,158,160)',
					'rgb(25,25,112)', 'rgb(176,196,222)', 'rgb(30,144,255)',
					'rgb(255,182,193)', 'rgb(220,20,60)', 'rgb(255,20,147)', 'rgb(218,112,214)', 'rgb(139,0,139)',
					'rgb(86,85,21)', 'rgb(175,238,238)', 'rgb(0,206,209)', 'rgb(47,79,79)', 'rgb(0,128,128)',
					'rgb(32,178,170)', 'rgb(127,255,170)', 'rgb(245,255,250)', 'rgb(173,255,47)', 'rgb(107,142,35)',
					'rgb(255,2550)', 'rgb(238,232,170)', 'rgb(128,128,0)', 'rgb(222.184,135)', 'rgb(255,140,0)',
					'rgb(205,133,63)', 'rgb(210,105,30)', 'rgb(139,69,19)', 'rgb(255,160,122)', 'rgb(233,150,122)',
					'rgb(250,128,114)', 'rgb(240,128,128)', 'rgb(188,143,143)', 'rgb(205,92,92)', 'rgb(220,220,220)',
					'rgb(169,169,169)', 'rgb(128,128,128)', 'rgb(0,0,0)'
				]
			}
		},
		watch:{
			oldInfoList:{
				handler(){
					// console.log(this.oldInfoList,'oldInfoList');
					this.echartOpts = echartBarOpts
					this.$nextTick(()=>{
						setTimeout(()=>{
							this.processDataFunc()
						},1500)
					})
				},
				deep:true,
				immediate:true
			}
		},
		mounted() {
			
		},
		methods: {
			processDataFunc() {
				if (this.oldInfoList.length > 0) {
					let afterData = this.processData(this.oldInfoList)
					// console.log("得到的最后数据",afterData);
					this.echartOpts.yAxis.data = ['']
					this.echartOpts.xAxis.min = new Date(this.oldInfoList[this.oldInfoList.length - 1].UpdatedOn)
					this.echartOpts.xAxis.max = new Date()
					this.echartOpts.series[0].data = afterData
					// let funstr=`(value,index)=>{ // 第一个参数没有用 所以用_代替
					//   if (index === 0 || index === ${afterData.length - 1}) {
					//         return value; // 返回任务的名称或其他标识信息
					//     } else {
					//         return ''; // 其他位置不显示标签
					//     }
					// },`
					// this.echartOpts.xAxis.axisLabel.formatter=eval(funstr)
					// console.log("图表数据结果",this.echartOpts);
					this.$refs.lineChart2.init(echarts, chart => {
						// console.log("chart",chart);
						chart.setOption(this.echartOpts);
					});
				}
			},
			processData(sortedStates) { //处理枚举历史数据
				const processedData = [];
				// 处理所有状态时间段
				for (let i = sortedStates.length - 1; i >= 0; i--) {
					let index = sortedStates.length - 1 - i
					const state = sortedStates[i];
					const startTime = new Date(state.UpdatedOn);
					let endTime;

					// 如果不是最后一个状态，结束时间为下一个状态的开始时间
					if (i > 0) {
						endTime = new Date(sortedStates[i - 1].UpdatedOn);
					}
					// 如果是最后一个状态，结束时间为班次结束时间
					else {
						endTime = new Date()
					}
					state.Value=state.Value?state.Value:''
					let activeColor = ''
					if (this.enumValColor[state.Value]) {
						activeColor = this.enumValColor[state.Value]
					} else {
						let keyArr = Object.keys(this.enumValColor)
						if (this.colorList[keyArr.length]) {
							activeColor = this.colorList[keyArr.length]
						} else {
							activeColor = this.getRandomColor()
						}
						this.enumValColor[state.Value] = activeColor
					}
					this.$forceUpdate()
					// 确保时间段有效
					if (endTime > startTime) {
						const duration = endTime - startTime;
						processedData.push({
							name: this.activeOldAttr.Name,
							value: [index, startTime, endTime, duration],
							itemStyle: {
								normal: {
									nameLight: state.Value,
									name: state.Value,
									color: activeColor
								}
							},
						})
						if (state.Value == '自动加载运行') {
							console.log('数据', {
								name: this.activeOldAttr.Name,
								value: [index, startTime, endTime, duration],
								itemStyle: {
									normal: {
										nameLight: state.Value,
										name: state.Value,
										color: activeColor
									}
								},
							});
						}
					}
				}
				// this.processDataList = processedData;
				return processedData;
			},
			getRandomColor() {
				var r = Math.floor(Math.random() * 256);
				var g = Math.floor(Math.random() * 256);
				var b = Math.floor(Math.random() * 256);
				let colorStr = 'rgb(' + r + ',' + g + ',' + b + ')';
				if (this.colorList.includes(colorStr)) {
					return this.getRandomColor()
				}
				return colorStr;
			},
		}
	}
</script>

<style lang="less">
	.state_con {
		display: flex;
		// margin-right: -20rpx;
		flex-wrap: wrap;
		align-items: center;
		margin-top: 40rpx;
		justify-content: center;
		width: calc(100% - 20rpx);
		.state_li {
			display: flex;
			justify-content: flex-start;
			align-items: center;
			margin-right: 20rpx;

			.li_box {
				width: 30rpx;
				height: 30rpx;
				border-radius: 6rpx;
			}

			.li_text {
				font-size: 24rpx;
				color: #333333;
				margin-left: 10rpx;
			}
		}
	}
</style>