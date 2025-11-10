<template>
	<view class="machines_statistic_con">
		<view class="machines_statistic">
			<view class="icon_title">
				<view class="icon_title_left">
					<custom-icons iconsName="icon-shebei-xianxing" iconsSize="28rpx"></custom-icons>
					<view class="left_text">Machines</view>
				</view>
			</view>
			<view class="data_content">
				<view class="data_dis">
					<view class="dis_li" v-for="item in machinesList" v-if="totalStatus!='loading'">
						<view class="dot" :style="{'backgroundColor':item.color}"></view>
						<view class="num">{{item.num}}</view>
						<view class="label">{{item.name}}</view>
					</view>
					<view class="dis_li skeleton_con" v-for="item in [1,2,3]" v-if="totalStatus=='loading'">
						
					</view>
				</view>
				<view class="data_view ">
					<view class="view_con" v-if="totalStatus!='loading'">
						<qiun-data-charts type="ring" :opts="ringopts" :chartData="chartsDataRing" :loadingType="0"/>
					</view>
					<view class="view_con skeleton_con" v-else>
						
					</view>
				</view>
			</view>
			<view class="alarm_cont" v-if="totalStatus!='loading'&&alarmNum>0" @click.stop="toAlarmList">
				<view class="alarm_left">
					<custom-icons iconsName="icon-baojing" iconsSize="28rpx" iconsColor="#FF3535"></custom-icons>
					<view class="text">Alarm machines</view>
				</view>
				<view class="alarm_right">
					<view class="text">{{alarmNum}}</view>
					<custom-icons iconsName="icon-a-youjiantouhong" iconsSize="20rpx"
						iconsColor="#FF9A9A"></custom-icons>
				</view>
			</view>
			<view class="alarm_cont skeleton_con" v-else-if="totalStatus=='loading'">
				
			</view>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {deviceStatistics} from '@/api/device.js'
	export default {
		name: "machines", //搜索主键
		data() {
			return {
				//环形图相关数据
				chartsDataRing: {},
				ringopts: {
					legend: {
						show: false
					},
					color: ['#EFA902', '#63657A', '#3F4356'],
					extra: {
						ring: {
							ringWidth: 15,
							border: false,
							centerColor: '#1C2232',
							activeRadius: 0
						}
					},
					title: {
						name: '0',
						fontSize: 22,
						color: "rgba(255, 255, 255, 1)"
					},
					subtitle: {
						name: 'Machines',
						fontSize: 14,
						color: "rgba(255, 255, 255, 0.5)"
					}
				},
				PieA: { //环形图
					"series": [{
						"data": [{
							"name": "Online",
							"value": 0,
							"labelShow": false
						}, {
							"name": "Offline",
							"value": 0,
							"labelShow": false
						}]
					}]
				},
				//环形图相关数据
				machinesList: [{
					name: 'Total',
					num: 0,
					color: '#FFFFFF'
				}, {
					name: 'Online',
					num: 0,
					color: '#EFA902'
				}, {
					name: 'Offline',
					num: 0,
					color: '#63657A'
				}, 
				// {
				// 	name: 'Unknow',
				// 	num: 56,
				// 	color: '#3F4356'
				// },
				],
				alarmNum:0,
				totalStatus:'loading'
			};
		},
		mounted() {
			
			this.getDeviceStatistics()
		},
		methods: {
			toAlarmList(){
				//跳转去报警列表
				uni.navigateTo({
					url:'/pages_device/alarm/list'
				})
			},
			getDeviceStatistics() {
				//获取设备统计信息
				this.totalStatus='loading'
				deviceStatistics().then((res) => {
					// console.log("设备统计信息res", res);
					let data = res.data;
					this.machinesList=[{
						name: 'Total',
						num: data.TotalCount,
						color: '#FFFFFF'
					}, {
						name: 'Online',
						num: data.OnlineCount,
						color: '#EFA902'
					}, {
						name: 'Offline',
						num: data.OfflineCount,
						color: '#63657A'
					}];
					this.alarmNum=data.EventCount
					this.ringopts.title.name=data.TotalCount
					this.PieA.series[0].data[0].value=data.OnlineCount
					this.PieA.series[0].data[1].value=data.OfflineCount
					this.chartsDataRing = JSON.parse(JSON.stringify(this.PieA))
					this.totalStatus='nomore'
				}).catch(err=>{
					this.setMsgTop(err)
				});
			},
			
		}
	}
</script>

<style lang="less" scoped>
	.machines_statistic_con {
		padding: 0 20rpx;
		width: 100%;
		height: 100%;
		box-sizing: border-box;
		margin-top: 20rpx;

		.machines_statistic {
			box-sizing: border-box;
			padding: 30rpx 30rpx 10rpx;
			width: 100%;
			// height: 364rpx;
			background-color: rgba(28, 34, 50, 1);
			border-radius: 10rpx;

			.icon_title {
				height: 36rpx;
				line-height: 36rpx;
			}

			.data_content {
				display: flex;
				justify-content: space-between;
				align-items: flex-start;

				.data_dis {
					width: calc(100% - 262rpx);
					display: flex;
					justify-content: flex-start;
					flex-wrap: wrap;
					padding-top: 32rpx;

					.dis_li {
						position: relative;
						padding: 30rpx 0 30rpx 28rpx;
						width: 50%;
						box-sizing: border-box;
						&.skeleton_con{
							width: 144rpx;
							height: 108rpx;
							border-radius: 10rpx;
							background-color: rgba(63, 67, 86, 1);
							margin-right: 20rpx;
							margin-bottom: 20rpx;
						}

						.dot {
							position: absolute;
							left: 0;
							top: 40rpx;
							width: 12rpx;
							height: 12rpx;
							border-radius: 50%;
						}

						.num {
							color: #fff;
							font-size: 32rpx;
							line-height: 32rpx;
							margin-bottom: 12rpx;
						}

						.label {
							color: rgba(255, 255, 255, 0.5);
							font-size: 24rpx;
							line-height: 24rpx;
						}
					}
				}

				.data_view {
					// position: fixed;
					width: 262rpx;
					height: 262rpx;
					position: relative;

					// background-color: #fff;
					.view_con {
						width: 504rpx;
						height: 400rpx;
						position: absolute;
						top: -100rpx;
						left: -131rpx;
						&.skeleton_con{
							width: 262rpx;
							height: 262rpx;
							background-color: rgba(63, 67, 86, 1);
							top: 0rpx;
							left: 0rpx;
							border-radius: 10rpx;
						}
					}
				}
			}

		}
	}
</style>