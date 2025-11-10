<template>
	<view class="pages_bgcon">
		<top :isLeftSlot="true" :isRightSlot="true" :title="topTitle" leftWidth="120rpx" rightWidth="120rpx" :isleftBack="true"
			backgroundColor="#ffffff">
			<template v-slot:top_left>
				<view class="title_left">
					<view class="icon_con" style="margin-right: 36rpx;">
						<custom-icons iconsName="icon-fanhui" iconsSize="36rpx" iconsColor="#333"></custom-icons>
					</view>
					<view class="icon_con" :style="{'background':flowInfo.Background}" v-if="flowInfo" style="width: 40rpx;border-radius: 4rpx;">
						<custom-icons :iconsName="flowInfo.Icon?iconSubStr(flowInfo.Icon):''" iconsSize="36rpx"
							iconsColor="#ffffff"></custom-icons>
					</view>
				</view>
			</template>
			<template v-slot:top_right v-if="!isManage">
				<view class="title_right">
					<view class="icon_con" @click.stop="handleTask" v-if="startWay==0||flowTemplateId&&planeHandleList&&planeHandleList.length>0">
						<custom-icons iconsName="icon-tianjia" iconsSize="36rpx" iconsColor="#333"></custom-icons>
					</view>
					<view class="icon_con" style="margin-left: 36rpx;" @click.stop="openSelect" v-if="!flowTemplateId">
						<custom-icons iconsName="icon-shaixuan" iconsSize="34rpx" iconsColor="#333"></custom-icons>
					</view>
					
				</view>
			</template>
		</top>
		<search-compt ref="userSearch" @openSelect="openSelect" @searchFocus="jumpSelectEmploy" pal="请选择发起人" backgroundColor="#fff"
			inputBg="#F8F8F8" @clearSearch="clearUser" v-if="isManage||flowTemplateId"></search-compt>
		<select-compt ref="selectCompt" :selectListParam="selectListParam" :querydata="taskQuery"
			@selectFinsh="finishSearch" :haidate="false" :fixedHeight="isManage||flowTemplateId?'216rpx':'128rpx'" :notOnlySearch="isManage"></select-compt>
		<view class="task_list">
			<view class="task_li" v-for="item in planeTaskList" :key="item.Id" @click="viewTaskInfo(item)">
				<view class="li_top">
					<view class="number_handle">
						<view class="number">{{item.PlaneNumber}}</view>
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
					<view class="time_li">开始时间：{{item.StartOn?item.StartOn:'-'}}</view>
					<view class="time_li">截止时间：{{item.EndOn?item.EndOn:'-'}}</view>
				</view>
				<view class="li_bottom">
					<view class="time_li">
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
			</view>
			<uni-load-more iconType="circle" :status="status" v-if="status" />
		</view>
		<uni-popup ref="bottomPop" type="bottom" :mask-click="true" :zIndex="999"
			maskBackgroundColor="rgba(0, 0, 0, 0.5)">
			<scroll-view :scroll-top="scrollTop" scroll-y="true" class="scroll-Y"
				@scrolltolower="planeReachBottom">
				<view class="popup_list_con">
					<view class="popup_list">
						<view class="popup_li borradio" v-for="item in planeHandleList" @click.stop="startTask(item)">
							{{item.Name}}
						</view>
						<view class="popup_li cancel" @click="closePopup">
							取消
						</view>
					</view>
				</view>
			</scroll-view>
		</uni-popup>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		devPlaneTaskList,
		cancelTask,
		devPlaneList
	} from '@/api/devplane.js'
	export default {
		data() {
			return {
				isManage:false,//是否是计划任务管理
				flowInfo:undefined,
				scrollTop:0,
				planeList: [],
				planeHandleList: [],
				status: 'loading',
				current: 0,
				tabArr: ['进行中', '待执行', '执行中', '已完成', '已过期', '已验收', '验收失败', '已作废'],
				topTitle: '计划任务',
				flowTemplateId: '', //计划流程模板id
				planeId: '', //计划类型id
				deviceId: '', //设备id
				deviceName: '', //设备名称
				taskQuery: {
					pageNum: 1,
					pageSize: 30,
					UserId: undefined,
					PlanTypeId: undefined,
					TaskStatus: null
				},
				planeTaskList: [],
				startWay: undefined,
				selectListParam: [{
					name: '计划状态',
					params: 'TaskStatus',
					pal: '请选择计划状态',
					value: null,
					localdata: [{
						text: '进行中',
						value: 0
					}, {
						text: '待执行',
						value: 1
					}, {
						text: '执行中',
						value: 2
					}, {
						text: '已完成',
						value: 3
					}, {
						text: '已过期',
						value: 4
					}, {
						text: '已验收',
						value: 5
					}, {
						text: '验收失败',
						value: 6
					}, {
						text: '已作废',
						value: 7
					},]
				}, {
					name: '计划类型',
					params: 'PlanTypeId',
					pal: '请选择计划类型',
					value: null,
					isshowTag:true,
					localdata: []
				}],
			}
		},
		onLoad(options) {
			const eventChannel = this.getOpenerEventChannel();
			console.log(eventChannel,'eventChannel');
			// 监听acceptDataFromOpenerPage事件，获取上一页面通过eventChannel传送到当前页面的数据
			if(options.isManage){
				this.isManage=true
				this.loadTaskList()
				this.getdevPlaneList2()
			}else{
				this.isManage=false
				this.taskQuery.UserId=this.$store.state.user.uid//过滤自己的计划任务
			}
			eventChannel.on('planeTaskForm', (data) => {
				// console.log(data,'uuuuu')
				if(data){
					if (data.id || data.templateId) {
						if (data.templateId) {
							this.flowTemplateId = data.templateId
							this.taskQuery.FlowTemplateId = this.flowTemplateId
							delete this.taskQuery.UserId
						} else if (data.id) {
							this.planeId = data.id
							this.taskQuery.PlanTypeId = this.planeId
							this.selectListParam.splice(1,1)
						}
					
					} else {
						uni.navigateBack()
					}
					
					if (data.deviceId) {
						this.deviceId = data.deviceId
						this.taskQuery.DeviceId = this.deviceId
					}
					if (data.deviceName) {
						this.deviceName = data.deviceName
					}
					if (data.flowTemplateName) {
						this.topTitle = data.flowTemplateName
					}
					if(data.flowInfo){
						this.flowInfo=data.flowInfo
					}
					if (data.planeName) {
						this.topTitle = '计划类型:'+data.planeName
					}
					if (data.startWay!==undefined) {
						this.startWay = data.startWay
					}
					this.loadTaskList()
					if(this.flowTemplateId){
						this.getdevPlaneList2()
					}
				}
				
				
			})

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
			openSelect() {
				this.$refs.selectCompt.openSelect()
			},
			clearUser() {
				this.taskQuery.UserId = undefined
				this.$refs.userSearch.setsearchval('')
				this.finishSearch()
			},
			getdevPlaneList2() {
				//获取设备计划类型
				devPlaneList({
					DeviceId: this.deviceId?this.deviceId:undefined,
					FlowTemplateId:this.flowTemplateId?this.flowTemplateId:undefined,
					pageSize:0
				}).then(res => {
					this.planeList = res.data.List.map(row => {
						let obj = {
							text: row.Name,
							value: row.Id
						}//筛选增加标记
						if(row.StartWay==0){
							obj.tagVal='手动'
						}else if(row.StartWay==1){
							obj.tagVal='定时'
						}else if(row.StartWay==2){
							obj.tagVal='设备事件'
						}
						return obj
					})
					
					this.selectListParam[1].localdata=this.planeList
					this.planeHandleList=res.data.List.filter(row=>row.StartWay==0)
				}).catch(err => {
					this.setMsgTop(err)
				})
			},
			finishSearch(querydata) {
				//进行搜索过滤
				if(querydata){
					this.taskQuery=JSON.parse(JSON.stringify(querydata))
				}
				this.taskQuery.pageNum = 1
				this.planeTaskList = []
				this.loadTaskList()
			},
			jumpSelectEmploy() {
				let selected = JSON.stringify([{
					id: this.taskQuery.UserId?this.taskQuery.UserId:this.$store.state.user.uid,
					type: "user"
				}])
				uni.navigateTo({
					url: '/pages_flow/inventory/employee_select?selected=' + selected
				})
			},
			selectEmplee(val) {
				//选中负责人后
				if (val) {
					let obj = val[0]
					this.taskQuery.UserId = obj.id
					this.$refs.userSearch.setsearchval(obj.name)
					this.$forceUpdate()
					this.finishSearch()
				}

			},
			loadData() {
				//流程任务执行完
				this.loadTaskList('load')
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
			viewTaskInfo(item) {
				uni.navigateTo({
					url: '/pages_flow/device/task_detail?id=' + item.Id,
					success: (res) => {}
				})
			},
			handleTask(){
				if(this.flowTemplateId){
					if(this.planeHandleList&&this.planeHandleList.length>0){
						if(this.planeHandleList.length==1){
							this.startTask(this.planeHandleList[0])
						}else{
							this.$refs.bottomPop.open()
						}
					}
				}else{
					this.startTask()
				}
				
			},
			closePopup(){
				this.$refs.bottomPop.close()
			},
			startTask(item) {
				//发起任务
				console.log("发起操作",item);
				if (item&&item.StartWay==0 || this.startWay == 0) {
					this.$refs.bottomPop.close()
					uni.navigateTo({
						url: '/pages_flow/device/task_add',
						success: (res) => {
							// 通过eventChannel向被打开页面传送数据
							let jumpform = {
								id: this.planeId,
							}
							if(item&&item.Id){
								jumpform.id=item.Id
							}
							if (this.deviceId) {
								jumpform.deviceId = this.deviceId
							}
							if (this.deviceName) {
								jumpform.deviceName = this.deviceName
							}
							res.eventChannel.emit('planeTaskForm', jumpform)
						}
					})
				}
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
					if (this.taskQuery.pageNum == 1) {
						this.planeTaskList = []
					}
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
			changeTab() {
				// 切换状态
			}
		}
	}
</script>

<style lang="less" scoped>
	// .pages_select {
	// 	position: fixed;
	// 	height: 88rpx;
	// 	// padding-top: 2rpx;
	// 	background-color: #ffffff;
	// 	display: flex;
	// 	justify-content: space-between;
	// 	align-items: center;
	// 	width: 100%;

	// 	.select_li {
	// 		width: 33.33%;
	// 	}
	// }

	// .pages_select_zhanwei {
	// 	width: 100%;
	// 	height: 88rpx;
	// }

	.title_right {
		display: flex;
		justify-content: flex-end;
		align-items: center;

		.icon_con {
			display: flex;
			justify-content: center;
			align-items: center;
		}
	}
	.title_left {
		display: flex;
		justify-content: flex-start;
		align-items: center;

		.icon_con {
			display: flex;
			justify-content: center;
			align-items: center;
		}
	}
	.task_list {
		width: 100%;
		padding: 0 20rpx;
		box-sizing: border-box;

		.task_li {
			width: 100%;
			background: #ffffff;
			margin-top: 20rpx;
			border-radius: 10rpx;
			padding: 0 30rpx;
			box-sizing: border-box;

			.li_top {
				padding: 30rpx 0;
				border-bottom: 1rpx solid rgba(234, 234, 234, 1);

				.number_handle {
					display: flex;
					justify-content: space-between;
					align-items: center;
					height: 30rpx;
					font-size: 30rpx;
					line-height: 30rpx;
					color: #333333;
					font-weight: bold;

					.status {
						font-size: 24rpx;
						font-weight: normal;
						color: rgba(153, 153, 153, 1);

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

				.time_li {
					margin-top: 20rpx;
				}
			}

			.time_li {
				display: flex;
				justify-content: space-between;
				align-items: center;
				font-size: 24rpx;
				color: rgba(153, 153, 153, 1);
				line-height: 24rpx;

			}

			.li_bottom {

				// padding: 28rpx 0;
				.time_li {
					.user {
						padding: 28rpx 0;
					}
				}

				.handle_con {
					display: flex;
					justify-content: flex-end;
					align-items: center;
				}

				.handle {
					margin-left: 50rpx;
					height: 28rpx;
					line-height: 28rpx;
					font-weight: normal;
					font-size: 28rpx;
					color: #999999;
					display: flex;
					justify-content: flex-end;
					align-items: center;
					padding: 28rpx 0;

					.text {
						margin-left: 10rpx;
					}
				}

			}
		}
	}
</style>