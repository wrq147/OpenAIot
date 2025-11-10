<template>
	<view>
		<view class="plane_addbtn_con">
			<view class="plane_addbtn" @click="toStartTask">
				<view class="icon_con">
					<custom-icons iconsName="icon-tianjia" iconsSize="18rpx"
						iconsColor="rgba(35, 113, 255, 1)"></custom-icons>
				</view>
				<view class="text">新建工单</view>
			</view>
		</view>
		<view class="plane_con">
			<view class="plane_li" v-for="(item,inx) in planeList" :key="item.Id" @click="jumpPlaneTask(planeList[inx])">
				<view class="li_left_con">
					<view class="icon_con" :style="{'background':item.Background}">
						<custom-icons :iconsName="item.Icon?iconSubStr(item.Icon):''" iconsSize="40rpx"
							iconsColor="#ffffff"></custom-icons>
					</view>
					<view class="li_left">
						<view class="name">{{item.FlowTemplateName}}</view>
					</view>
				</view>
				<view class="li_right">
					<view class="number">{{item.WaitDeal}}</view>
					<custom-icons iconsName="icon-a-youjiantouhong" iconsSize="20rpx"
						iconsColor="#999999"></custom-icons>
				</view>
			</view>
		</view>
		<uni-popup ref="bottomPop" type="bottom" :mask-click="true" :zIndex="999"
			maskBackgroundColor="rgba(0, 0, 0, 0.5)">
			<view class="popup_list_con">
				<view class="title_con">
					<view class="kong">取消</view>
					<view class="title">发起任务</view>
					<view class="cancel" @click.stop="closePopup">取消</view>
				</view>
				<view class="popup_list">
					<scroll-view :scroll-top="scrollTop" scroll-y="true" class="scroll-Y">
						<view class="popup_li borradio" v-for="item in planeHandleList" @click.stop="startTask(item)">
							<view class="icon_con" :style="{'background':item.Background}">
								<custom-icons :iconsName="item.Icon?iconSubStr(item.Icon):''" iconsSize="40rpx"
									iconsColor="#ffffff"></custom-icons>
							</view>
							<view class="temple_name">
								<view class="name">{{item.FlowTemplateName}}</view>
								<view class="type">{{item.Name}}</view>
							</view>
						</view>
					</scroll-view>
				</view>
			</view>

		</uni-popup>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		toCronDes
	} from "@/api/ruselSevic.js";
	import {
		devPlaneList,
		devFlowList
	} from '@/api/devplane.js'
	export default {
		name: "device-plane",
		props: {
			deviceBasic: {
				type: Object,
				default: () => {
					return {}
				}
			},
		},
		data() {
			return {
				scrollTop: 0,
				deviceBasicInfo: {},
				status: 'loading',
				status2: 'loading',
				planeList: [], //计划列表
				planeHandleList: [], //手动发起的计划列表
				planeQuery: {},
				planeQuery2: {
					pageNum: 1,
					pageSize: 0,
					CanStart: true,
					StartWay: 0
				},
				isHandleStart: false //是否正则发起手动
			};
		},
		watch: {
			deviceBasic: {
				async handler(newvalue, oldvalue) {
					this.deviceBasicInfo = JSON.parse(JSON.stringify(this.deviceBasic))
					this.planeQuery.id = this.deviceBasicInfo.Id
					this.planeQuery2.DeviceId = this.deviceBasicInfo.Id
					await this.getdevPlaneList()

				},
				// 代表在wacth里声明了firstName这个方法之后立即先去执行handler方法
				immediate: true,
				deep: true,
			},
		},
		mounted() {},
		methods: {
			async toStartTask() {
				this.$refs.bottomPop.open()
				this.isHandleStart = true
				await this.getdevPlaneList2()
			},
			closePopup() {
				this.$refs.bottomPop.close()
			},
			startTask(row) {
				//开启任务
				this.$refs.bottomPop.close()
				if (row.StartWay == 0) {
					uni.navigateTo({
						url: '/pages_flow/device/task_add',
						success: (res) => {
							// 通过eventChannel向被打开页面传送数据
							let jumpform = {
								id: row.Id,
							}
							if (this.deviceBasicInfo.Id) {
								jumpform.deviceId = this.deviceBasicInfo.Id
							}
							if (this.deviceBasicInfo.Name) {
								jumpform.deviceName = this.deviceBasicInfo.Name
							}
							res.eventChannel.emit('planeTaskForm', jumpform)
						}
					})
				}
			},
			async getdevPlaneList2() {
				//获取设备计划类型
				try {
					let res = await devPlaneList(this.planeQuery2)
					this.planeHandleList = res.data.List
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}
			},
			async getdevPlaneList() {
				//获取设备计划类型
				try {
					let res = await devFlowList(this.planeQuery)
					// console.log(res, 'resres');
					this.planeList = res.data
					this.$forceUpdate()
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}
			},
			jumpPlaneTask(item) {
				//跳转至计划任务
				console.log(item,'item');
				uni.navigateTo({
					url: '/pages_device/device_info/plane_task',
					success: (res) => {
						// 通过eventChannel向被打开页面传送数据
						res.eventChannel.emit('planeTaskForm', {
							templateId: item.FlowTemplateId,
							flowTemplateName: item.FlowTemplateName,
							deviceId: this.deviceBasicInfo.Id,
							deviceName: this.deviceBasicInfo.Name,
							startWay: item.StartWay,
							flowInfo:{
								Background:item.Background,
								Icon:item.Icon
							}
						})
					}
				})
			},
			async getCronName(str) {
				try {
					let x = await toCronDes(str)
					return x.data

				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
					return ''
				}
			}
		}
	}
</script>

<style lang="scss" scoped>
	.popup_list_con {
		background-color: #ffffff;
		padding-top: 30rpx;
		padding: 30rpx 30rpx 20rpx;

		.title_con {
			display: flex;
			justify-content: space-between;
			align-items: center;
			height: 54rpx;

			.title {
				font-size: 36rpx;
				color: #333333;
				font-weight: bold;
			}

			.kong {
				height: 100%;
				font-size: 28rpx;
				color: rgba(153, 153, 153, 0);
			}

			.cancel {
				height: 100%;
				font-size: 28rpx;
				color: rgba(153, 153, 153, 1);
			}
		}

		.popup_list {
			padding-top: 10rpx;
			max-height: calc(70vh - 114rpx);
			overflow-y: scroll;
			.popup_li {
				padding: 30rpx;
				background-color: rgba(248, 248, 248, 1);
				border-radius: 6rpx;
				display: flex;
				justify-content: flex-start;
				align-items: center;
				line-height: auto;
				margin-top: 20rpx;

				.icon_con {
					width: 80rpx;
					height: 80rpx;
					border-radius: 10rpx;
					display: flex;
					justify-content: center;
					align-items: center;
				}

				.temple_name {
					margin-left: 30rpx;

					.name {
						line-height: 36rpx;
						box-sizing: border-box;
						text-align: left;
						font-size: 28rpx;
					}

					.type {
						margin-top: 12rpx;
						color: rgba(153, 153, 153, 1);
						font-size: 24rpx;
						line-height: 32rpx;
						text-align: left;
					}
				}
			}
		}
	}

	.plane_addbtn_con {
		position: fixed;
		// top: 20rpx;
		left: 0;
		width: 100%;
		padding: 20rpx 20rpx 0;
		background-color: rgba(245, 248, 249, 1);
		box-sizing: border-box;

		.plane_addbtn {
			display: flex;
			justify-content: center;
			align-items: center;
			width: 100%;
			height: 100rpx;
			font-size: 30rpx;
			color: rgba(35, 113, 255, 1);
			background-color: rgba(255, 255, 255, 1);
			border-radius: 10rpx;

			.icon_con {
				width: 40rpx;
				height: 40rpx;
				border-radius: 50%;
				background-color: rgba(233, 241, 255, 1);
				display: flex;
				justify-content: center;
				align-items: center;
				margin-right: 16rpx;
			}
		}
	}

	.plane_con {
		padding-top: 140rpx;

		.plane_li:first-child {
			margin-top: 0;
		}

		.plane_li {
			width: 100%;
			height: 140rpx;
			background: #ffffff;
			border-radius: 10rpx;
			margin-top: 20rpx;
			display: flex;
			justify-content: space-between;
			align-items: center;
			padding: 0 30rpx;
			box-sizing: border-box;

			.li_left_con {
				display: flex;
				justify-content: flex-start;
				align-items: center;

				.icon_con {
					width: 80rpx;
					height: 80rpx;
					background-color: rgba(255, 133, 61, 1);
					border-radius: 10rpx;
					display: flex;
					justify-content: center;
					align-items: center;
					margin-right: 20rpx;
				}

				.li_left {
					display: flex;
					justify-content: center;
					flex-direction: column;

					.name {
						font-size: 30rpx;
						color: #333333;
						line-height: 30rpx;
						font-weight: bold;
					}
				}
			}

			.li_right {
				display: flex;
				justify-content: flex-end;
				align-items: center;

				.number {
					color: rgba(153, 153, 153, 1);
					font-size: 24rpx;
					margin-right: 20rpx;
				}
			}

		}
	}
</style>