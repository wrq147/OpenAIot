<template>
	<view>
		<view class="plane_task">
			<view class="plane_addbtn_con">
				<view class="plane_addbtn" @click="toStartTask">
					<view class="icon_con">
						<custom-icons iconsName="icon-tianjia" iconsSize="18rpx"
							iconsColor="rgba(35, 113, 255, 1)"></custom-icons>
					</view>
					<view class="text">新建工单</view>
				</view>
			</view>
			<view class="task_list">
				<view class="task_li" v-for="item in planeTaskList" @click="viewTaskInfo(item)">
					<view class="title_handle">
						<view class="title">{{item.PlanName}}</view>
						<view class="status" v-if="item.TaskStatus==4||item.TaskStatus==7">
							{{getTaskStatus(item.TaskStatus)}}
						</view>
						<view class="status blue" v-if="item.TaskStatus==0||item.TaskStatus==1||item.TaskStatus==2">
							{{getTaskStatus(item.TaskStatus)}}
						</view>
						<view class="status green" v-if="item.TaskStatus==3||item.TaskStatus==5">
							{{getTaskStatus(item.TaskStatus)}}
						</view>
						<view class="status red" v-if="item.TaskStatus==6">{{getTaskStatus(item.TaskStatus)}}</view>
					</view>
					<view class="number">{{item.PlaneNumber}}</view>
					<view class="time_con">
						<view class="time_li">
							<view class="li_title">开始时间：</view>
							<view class="li_value">{{item.StartOn?item.StartOn:'-'}}</view>
						</view>
						<view class="span_line"></view>
						<view class="time_li">
							<view class="li_title">结束时间：</view>
							<view class="li_value">{{item.EndOn?item.EndOn:'-'}}</view>
						</view>
					</view>
					<view class="li_bottom">
						<view class="user" v-if="item.UserInfo">{{item.UserInfo.RealName}}</view>
						<view class="handle_con">
							<view class="handle" @click.stop="jumpTodo(item)"
								v-if="item.TodoTasks&&item.TodoTasks.length>0">
								<custom-icons iconsName="icon-zhihang" iconsSize="28rpx"
									iconsColor="#999999"></custom-icons>
								<text class="text">执行</text>
							</view>
							<view class="handle" @click.stop="revokeTask(item)" v-if="item.TaskStatus!=7">
								<custom-icons iconsName="icon-quxiaoliucheng" iconsSize="28rpx"
									iconsColor="#999999"></custom-icons>
								<text class="text">废弃</text>
							</view>
						</view>
					</view>
				</view>
				<uni-load-more iconType="circle" :status="status" v-if="status" />
			</view>
		</view>
		<uni-popup ref="bottomPop" type="bottom" :mask-click="true" :zIndex="999"
			maskBackgroundColor="rgba(0, 0, 0, 0.5)">
			<view class="popup_list_con">
				<view class="popup_list">
					<view class="popup_li borradio" v-for="item in planeList" @click.stop="startTask(item)">
						{{item.Name}}
					</view>
					<view class="popup_li cancel" @click="closePopup">
						取消
					</view>
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
		devPlaneTaskList,
		cancelTask
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
				// serveList:[],
				deviceBasicInfo: {},
				status: 'loading',
				planeList: [], //计划列表
				planeQuery: {
					pageNum: 1,
					pageSize: 30,
					CanStart: true,
					StartWay: 0 //过滤手动
				},
				taskQuery: {
					pageNum: 1,
					pageSize: 30
				},
				planeTaskList: [] //计划任务列表
			};
		},
		watch: {
			deviceBasic: {
				async handler(newvalue, oldvalue) {
					this.deviceBasicInfo = JSON.parse(JSON.stringify(this.deviceBasic))
					this.planeQuery.DeviceId = this.deviceBasicInfo.Id
					this.taskQuery.DeviceId = this.deviceBasicInfo.Id
					await this.getdevPlaneList(1)
					this.loadTaskList()
				},
				// 代表在wacth里声明了firstName这个方法之后立即先去执行handler方法
				immediate: true,
				deep: true,
			},
		},
		mounted() {

		},
		onReachBottom() { //上拉触底
			// console.log("现在的状态是什么", this.status);
			if (this.status != 'noMore') {
				this.taskQuery.pageNum++;
				this.status = "loading";
				this.loadTaskList();
				// console.log("现在是第几页", this.page);
			}
		},
		methods: {
			loadData(){
				//流程任务执行完
				this.loadTaskList('load')
			},
			viewTaskInfo(item) {
				uni.navigateTo({
					url: '/pages_flow/device/task_detail?id=' + item.Id,
					success: (res) => {}
				})
			},
			jumpTodo(item) {
				if (item.TodoTasks && item.TodoTasks.length == 1) {
					let row = item.TodoTasks[0]
					uni.navigateTo({
						url: '/pages_flow/process_detail?id=' + row.ExecutionNodeId + '&toDo=true'
					})
				} else {
					uni.navigateTo({
						url: '/pages_device/device_info/dev_todo?id=' + item.Id,
						success: (res) => {}
					})
				}

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
			toStartTask() {
				this.$refs.bottomPop.open()
			},
			closePopup() {
				this.$refs.bottomPop.close()
			},
			loadTaskList(query) {
				//任务列表
				if (query && query == 'load') {
					this.taskQuery.pageNum = 1
					this.status = 'loading'
				}
				if (this.taskQuery.pageNum == 1) {
					this.planeTaskList = []
				}
				devPlaneTaskList(this.taskQuery).then(res => {
					// console.log(res,'任务列表');
					this.planeTaskList = [...this.planeTaskList, ...res.data.List]
					if (res.data.List.length < this.taskQuery.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				}).catch(err => {
					this.setMsgTop(err)
				})
			},
			async getdevPlaneList(pageNum, status) {
				//获取设备计划类型
				this.planeQuery.pageNum = pageNum
				this.status = status
				if (this.planeQuery.pageNum == 1) {
					this.planeList = []
				}
				try {
					let res = await devPlaneList(this.planeQuery)
					if (res.data && res.data.List && res.data.List.length > 0) {
						for (let i = 0; i < res.data.List.length; i++) {
							if (res.data.List[i].TimerCron) {
								res.data.List[i].TimerCronName = await this.getCronName(res.data.List[i].TimerCron)
							} else {
								res.data.List[i].TimerCronName = ''
							}
						}
					}
					this.planeList = [...this.planeList, ...res.data.List]
					if (res.data.List.length < this.planeQuery.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}
			},
			jumpPlaneTask(item) {
				//跳转至计划任务
				uni.navigateTo({
					url: '/pages_device/device_info/plane_task',
					success: (res) => {
						// 通过eventChannel向被打开页面传送数据
						res.eventChannel.emit('planeTaskForm', {
							id: item.Id,
							planeName: item.Name,
							deviceId: this.deviceBasicInfo.Id,
							deviceName: this.deviceBasicInfo.Name,
							startWay: item.StartWay
						})
					}
				})
			},
			getTaskStatus(val) {
				switch (val) {
					case 0:
						return '进行中'
						break;
					case 1:
						return '待执行'
						break;
					case 2:
						return '执行中'
						break;
					case 3:
						return '已完成'
						break;
					case 4:
						return '已过期'
						break;
					case 5:
						return '已验收'
						break;
					case 6:
						return '验收失败'
						break;
					case 7:
						return '已作废'
						break;
				}
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
			},
			revokeTask(item) {
				//废弃任务
				uni.showModal({
					title: '系统提示',
					content: '是否确定要废除变化为' + item.PlaneNumber + '的任务',
					showCancel: true,
					success: (res) => {
						if (res.confirm) {
							cancelTask({
								id: item.Id
							}).then(rsp => {
								this.$refs.promptMsg.open('废除成功', 2000)
								this.taskQuery.pageNum = 1
								this.loadTaskList()
							}).catch(err => {
								this.setMsgTop(err)
							})
						}
					}
				});
			},
		}
	}
</script>

<style lang="scss" scoped>
	

	.plane_task {
		// padding: 20rpx 0 0;
		width: 100%;
		box-sizing: border-box;
		position: relative;
		
		.task_list {
			width: 100%;
			padding-top: 140rpx;
			.task_li:first-child{
				margin-top: 0;
			}
			.task_li {
				margin-top: 20rpx;
				width: 100%;
				padding: 0 30rpx;
				box-sizing: border-box;
				background-color: #ffffff;
				border-radius: 10rpx;

				.title_handle {
					display: flex;
					justify-content: space-between;
					align-items: center;
					line-height: 40rpx;
					padding-top: 25rpx;

					.title {
						font-size: 30rpx;
						color: #333333;
					}

					.status {
						font-size: 24rpx;
						font-weight: normal;
						color: rgba(153, 153, 153, 1);
						min-width: 72rpx;

						&.blue {
							color: rgba(35, 113, 255, 1);
						}

						&.green {
							color: rgba(80, 201, 122, 1);
						}

						&.red {
							color: rgba(255, 53, 53, 1);
						}
					}
				}

				.number {
					font-size: 24rpx;
					color: rgba(102, 102, 102, 1);
					line-height: 24rpx;
					margin-top: 15rpx;
				}

				.time_con {
					display: flex;
					justify-content: space-between;
					align-items: center;
					height: 112rpx;
					padding: 24rpx 30rpx;
					width: 100%;
					margin-top: 30rpx;
					box-sizing: border-box;
					background-color: rgba(248, 248, 248, 1);

					.time_li {
						font-size: 24rpx;

						.li_title {
							color: rgba(153, 153, 153, 1);
						}

						.li_value {
							color: rgba(102, 102, 102, 1);
						}
					}

					.span_line {
						width: 1rpx;
						border-radius: 4rpx;
						height: 64rpx;
						background-color: rgba(225, 225, 225, 1);
					}
				}

				.li_bottom {
					height: 24rpx;
					font-size: 24rpx;
					display: flex;
					justify-content: space-between;
					align-items: center;
					padding: 30rpx 0;
					color: rgba(153, 153, 153, 1);

					.handle_con {
						display: flex;
						justify-content: flex-end;
						align-items: center;

						.handle {
							display: flex;
							justify-content: flex-start;
							align-items: center;
							margin-left: 50rpx;

							.text {
								margin-left: 10rpx;
							}
						}
					}
				}
			}

		}
		.plane_addbtn_con{
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
		
	}
</style>