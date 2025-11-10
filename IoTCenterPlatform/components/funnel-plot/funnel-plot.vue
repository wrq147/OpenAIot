<template>
	<view>
		<view class="echart_con">
			<l-echart class="line-chart" ref="lineChart"></l-echart>
		</view>
	</view>
</template>

<script>
	import {
		FunnelPlot,//漏斗图
	} from "@/api/crmApi";
	import * as echarts from '@/common/echarts.min.js';
	export default {
		name: "FunnelPlot",//漏斗图组件
		props: {
			timeDate: {
				type: String,
				default: 0
			},
			// noRead: {
			// 	type: Number,
			// 	default: 0
			// }
		},
		watch: {
		    timeDate(newValue) {
		      // 在这里处理新值的更新逻辑
			 // console.log(newValue,'切换时候实时到接口更新');
			  this.timeDate=newValue
			  this.setCharts();
		    }
		  },
		data() {
			return {
				opt: {
					// tooltip: {
					//   trigger: 'item',
					//   formatter: '{b} : {c}%'
					// },
					labelLine: {
						show: true
					},
					series: [{
							name: '漏斗',
							type: 'funnel',
							// top: 0,
							bottom: 0,
							left: -60,
							width: '100%',
							height: '350',
							min: 0,
							max: 100,
							minSize: '30%',
							maxSize: '60%',
							sort: 'descending',
							gap: 0,
							color: ['rgba(255, 255, 255, 1)'],
							labelLine: {
								//视觉引导线样式
								length: 60,
								lineStyle: {
									width: 1,
									type: 'solid'
								}
							},
							label: {
								//漏斗外部显示的
								position: 'right', //位置
								formatter: '{b}', //显示的内容
								fontStyle: 'normal',
								fontSize: 12
								// textBorderColor: '#fff'
							},
							emphasis: {
								//鼠标移入数据项的tooltip设置
								show: false
							},
							itemStyle: {
								opacity: 1, //图形透明度
								borderColor: '#fff', //图形边框颜色
								borderWidth: 0 //图形边框宽度
							},
							data: [
								//我的数据是根据需求自己设置的name
								{
									value: 80,
									name: '24'
								},
								{
									value: 60,
									name: '23'
								},
								{
									value: 40,
									name: '22'
								},
								{
									value: 20,
									name: '21'
								},
								{
									value: 10,
									name: '20'
								},
								{
									value: 5,
									name: '19'
								},
								{
									value: 0,
									name: '18'
								}
							],
							z: 99
						},
						{
							name: '漏斗',
							type: 'funnel',
							// top: 0,
							bottom: 0,
							left: -60,
							width: '100%',
							height: '350',
							min: 0,
							max: 100,
							minSize: '30%',
							maxSize: '60%',
							// sort: 'ascending',
							gap: 0,
							color: [
								'rgba(63, 67, 86, 1)',
								'rgba(99, 101, 122, 1)',
								'rgba(239, 169, 2, 1)',
								'rgba(255, 133, 61, 1)',
								'rgba(237, 87, 87, 1)',
								'rgba(255, 87, 136, 1)',
								'rgba(255, 53, 53, 1)',

							],
							label: {
								//设置字体放在漏斗内部
								position: 'inside',
								formatter: '{b}',
								color: '#fff'
							},
							emphasis: {
								label: {
									fontSize: 14 //鼠标移入字体变大 显示toolList
								}
							},
							itemStyle: {
								opacity: 1, //图形透明度
								borderColor: '#fff', //图形边框颜色
								borderWidth: 0 //图形边框宽度
							},
							data: [
								//我的数据是根据需求自己设置的name
								{
									value: 0,
									name: 'Invalid'
								},
								{
									value: 5,
									name: 'Lost order'
								},
								{
									value: 10,
									name: 'Win order'
								},
								{
									value: 20,
									name: 'Business negotiation'
								},
								{
									value: 40,
									name: 'Proposal quotation'
								},
								{
									value: 60,
									name: 'Requirements determination'
								},
								{
									value: 80,
									name: 'Requirement Discovery'
								}
							],
							z: 100
						}
					]
				},
			}
		},
		created() {
		
		},
		mounted() {
			this.setCharts()
			console.log(this.timeDate,'arr1')
		},
		methods: {
			setCharts() {
				this.$refs.lineChart.init(echarts, chart => {
					FunnelPlot({
						day:this.timeDate
					}).then((res)=>{
						if(res.code==0){
							//console.log(res.data)
							var result=res.data;
							result.reverse()
							result.forEach((item,index)=>{
								this.opt.series[1].data[index].name=item.Name
							})
							var data=res.data;
							data.reverse()
							data.forEach((item,index)=>{
								this.opt.series[0].data[index].name=item.Count
							})
							chart.setOption(this.opt);
						}
					})
				});
			}
		}
	}
</script>

<style lang="less" scoped>
// .echart_con {
// 		width: 100%;
// 		height: 596rpx;
// 	}
</style>