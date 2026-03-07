<template>
	<view>
		<top :title="topTitle" leftWidth="157rpx" leftIcon="icon-fanhui" backgroundColor="#ffffff" rightWidth="157rpx"
			:rightIcon="current==3?'icon-tianjia':''" :isleftBack="true" @clickRight="addProcess">
		</top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="请输入名称" inputBg="#F8F8F8"></search-compt>
		<select-compt ref="selectCompt" :haidate="true" :timeQuery="timeQuery" :querydata="querydata"
			@selectFinsh="selectFinsh" fixedHeight="216rpx"></select-compt>
		<v-tabs v-model="current" :scroll="false" :tabs="tabArr" color="#999999" activeColor="#333333" :fixed="true"
			@change="changeTab" :lineScale="0.16" fontSize="28rpx" activeFontSize="28rpx" paddingItem="0"
			lineHeight="4rpx" :hasBorder="false" height="72rpx" padding="0" bgColor="#ffffff" lineColor="#2371FF"
			:zIndex="1" scrollPadding="0 0" scrollBgColor="#ffffff"></v-tabs>
		<view class="process_con">
			<view class="process_li" v-for="item in list" @click.stop="toProccessInfo(item)">
				<view class="li_con">
					<view class="li_left" :style="{'background':item.Background}">
						<custom-icons :iconsName="item.Icon?iconSubStr(item.Icon):''" iconsSize="40rpx"
							iconsColor="#ffffff"></custom-icons>
					</view>
					<view class="li_right">
						<view class="name">
							{{item.FlowName}}
						</view>
						<view class="info_li" v-if="current==0">
							接收时间: {{item.StartTime}}
						</view>
						<view class="info_li" v-if="current==1">
							处理时间: {{item.StartTime}}
						</view>
						<view class="info_li" v-if="current==2">
							抄送时间: {{item.StartTime}}
						</view>
						<view class="info_li" v-if="current==3">
							提交时间: {{item.createTime}}
						</view>
						<view class="info_li" v-if="current!=3">
							处理人: {{item.startRealName}}
						</view>
					</view>
				</view>
				<view class="view_line" v-if="current==3"></view>
				<view class="hadle_con" v-if="current==3">
					<view class="tips in" v-if="item.Status==0">
						进行中
					</view>
					<view class="tips Draft" v-if="item.Status==1">
						保存中
					</view>
					<view class="tips Rejected" v-if="item.Status==2">
						已取消
					</view>
					<view class="tips" v-if="item.Status==3">
						已完成
					</view>
					<view class="hadle_right">
						<view class="hadle_li" v-if="item.Status==1" @click.stop="editProccess(item)">
							<custom-icons iconsName="icon-bianji" iconsSize="24rpx" iconsColor="#999999"></custom-icons>
							<text class="text">编辑</text>
						</view>
						<view class="hadle_li" v-if="item.Status==0||item.Status==1" @click.stop="handleStop(item)">
							<custom-icons iconsName="icon-quxiaoliucheng" iconsSize="24rpx"
								iconsColor="#999999"></custom-icons>
							<text class="text">撤销</text>
						</view>
						<view class="hadle_li" v-if="item.Status==2||item.Status==3" @click.stop="handleDelete(item)">
							<custom-icons iconsName="icon-shanchu" iconsSize="24rpx"
								iconsColor="#999999"></custom-icons>
							<text class="text">删除</text>
						</view>
					</view>
				</view>
			</view>
			<uni-load-more iconType="circle" :status="status" v-if="status" />
		</view>

		<msg-prompt ref="promptMsg" @confirm="confirmDelete"></msg-prompt>
	</view>
</template>

<script>
	import {
		todoList,
		myProcessList,
		finishedList,
		csList,
		canProcess,
		delProcess
	} from '@/api/process.js'
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				timeQuery: {
					name: '日期',
					params: ['beginTime', 'endTime'],
					value: [],
				},
				status: 'loading',
				tabArr: ['待办任务', '已办任务', '抄送我的', '我的流程'],
				querydata: {
					pageNum: 1,
					pageSize: 10
				},
				current: 0,
				selectListParam: [],
				topTitle: '流程',
				list: [],
				activeDelId: '', //当前执行取消或删除的流程的id
				isStop: false, //为true表示执行取消流程，false表示删除流程
				isChangeInfo:false
			};
		},
		onLoad(options) {
			if (options.item) {
				this.current = Number(options.item)
				if (this.current == 0) {
					this.gettodoList()
				}
				if (this.current == 1) {
					this.getfinishedList()
				}
				if (this.current == 2) {
					this.getcsList()
				}
				if (this.current == 3) {
					this.getmyProcessList()
				}
			}
		},
		onShow() {
			if(this.isChangeInfo){
				setPagesParam('loadReportInfo', 'load', 1,false)
				this.isChangeInfo=false
			}
		},
		onReachBottom() { //上拉触底
			// console.log("现在的状态是什么", this.status);
			if (this.status != 'noMore') {
				this.querydata.pageNum++;
				this.status = "loading";
				if (this.current == 0) {
					this.gettodoList()
				}
				if (this.current == 1) {
					this.getfinishedList()
				}
				if (this.current == 2) {
					this.getcsList()
				}
				if (this.current == 3) {
					this.getmyProcessList()
				}
				// console.log("现在是第几页", this.page);
			}
		},
		methods: {
			/** 删除按钮操作 */
			handleDelete(row) {
				this.activeDelId = row.Id
				this.$refs.promptMsg.noticeOpen(
					'是否确定要删除流程定义编号为 ' + row.Id + '的数据项?', '提示', true
				)
			},
			async confirmDelete() {
				if (this.isStop) {
					if (this.activeDelId) {
						this.isStop = false
						try {
							await canProcess(this.activeDelId)
							this.$refs.promptMsg.open('取消成功', 2000) //提示信息组件
							setPagesParam('loadReportInfo', 'load', 1,false)
							this.loadData();
							this.activeDelId = ''

						} catch (e) {
							//TODO handle the exception
							this.setMsgTop(e)
						}
					}
				} else {
					if (this.activeDelId) {
						try {
							await delProcess(this.activeDelId)
							this.$refs.promptMsg.open('删除成功', 2000) //提示信息组件
							setPagesParam('loadReportInfo', 'load', 1,false)
							this.loadData()
							this.activeDelId = ''
						} catch (e) {
							//TODO handle the exception
							this.setMsgTop(e)
						}
					}
				}
			},
			/**  取消流程申请 */
			handleStop(row) {
				this.activeDelId = row.Id
				this.isStop = true
				this.$refs.promptMsg.noticeOpen(
					'是否确定要取消流程定义编号为 ' + row.Id + '的数据项?', '提示', true
				)
			},
			/**编辑流程 */
			editProccess(row) {
				uni.navigateTo({
					url: '/pages_flow/process_form?procDefId=' + row.TemplateId + '&id=' + row.Id
				})
			},
			toProccessInfo(row) {
				if (this.current == 3) {
					uni.navigateTo({
						url: '/pages_flow/process_detail?id=' + row.Id + '&isRoot=true'
					})
				} else if (this.current == 0) {
					uni.navigateTo({
						url: '/pages_flow/process_detail?id=' + row.ExecutionNodeId + '&toDo=true'
					})
				} else {
					uni.navigateTo({
						url: '/pages_flow/process_detail?id=' + row.ExecutionNodeId
					})
				}

			},
			loadData(query) {
				if(query){
					this.isChangeInfo=true
				}
				this.querydata.pageNum = 1
				if (this.current == 0) {
					this.gettodoList()
				}
				if (this.current == 1) {
					this.getfinishedList()
				}
				if (this.current == 2) {
					this.getcsList()
				}
				if (this.current == 3) {
					this.getmyProcessList()
				}
			},
			async gettodoList() {
				//待办任务
				try {
					if (this.querydata.pageNum == 1) {
						this.list = []
						this.status = 'loading'
					}
					let rsp = await todoList(this.querydata)
					if (this.querydata.pageNum == 1) {
						this.list = []
					}
					this.list = [...this.list, ...rsp.data.List]
					if (rsp.data.List.length < this.querydata.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}
			},
			async getfinishedList() {
				//已办任务
				try {
					if (this.querydata.pageNum == 1) {
						this.list = []
						this.status = 'loading'
					}
					let rsp = await finishedList(this.querydata)
					if (this.querydata.pageNum == 1) {
						this.list = []
					}
					this.list = [...this.list, ...rsp.data.List]
					if (rsp.data.List.length < this.querydata.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}
			},
			async getcsList() {
				//抄送我的
				try {
					if (this.querydata.pageNum == 1) {
						this.list = []
						this.status = 'loading'
					}
					let rsp = await csList(this.querydata)
					if (this.querydata.pageNum == 1) {
						this.list = []
					}
					this.list = [...this.list, ...rsp.data.List]
					if (rsp.data.List.length < this.querydata.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}
			},
			async getmyProcessList() {
				//我创建的
				try {
					if (this.querydata.pageNum == 1) {
						this.list = []
						this.status = 'loading'
					}
					let rsp = await myProcessList(this.querydata)
					if (this.querydata.pageNum == 1) {
						this.list = []
					}
					this.list = [...this.list, ...rsp.data.List]
					if (rsp.data.List.length < this.querydata.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}
			},
			changeTab(index) {
				//切换tab
				this.querydata.pageNum = 1
				if (index == 0) {
					if (this.querydata.name) {
						this.querydata.key = this.querydata.name
						delete this.querydata.name
					}
					this.gettodoList()
				}
				if (index == 1) {
					if (this.querydata.name) {
						this.querydata.key = this.querydata.name
						delete this.querydata.name
					}
					this.getfinishedList()
				}
				if (index == 2) {
					if (this.querydata.name) {
						this.querydata.key = this.querydata.name
						delete this.querydata.name
					}
					this.getcsList()
				}
				if (index == 3) {
					if (this.querydata.key) {
						this.querydata.name = this.querydata.key
						delete this.querydata.key
					}
					this.getmyProcessList()
				}
			},
			searching(val) {
				//搜索
				if (this.current == 3) {
					if (val) {
						this.querydata.name = val

					} else {
						delete this.querydata.name
						if (this.querydata.key) {
							delete this.querydata.key
						}
					}
				} else {
					if (val) {
						this.querydata.key = val

					} else {
						delete this.querydata.key
						if (this.querydata.name) {
							delete this.querydata.name
						}
					}
				}

				this.loadData()
			},
			openSelect() {
				this.$refs.selectCompt.openSelect()
			},
			addProcess() {
				//添加流程
				if (this.current == 3) {
					uni.navigateTo({
						url: '/pages_flow/add_process'
					})
				}
			},
			selectFinsh(query) {
				if(query){
					this.querydata = JSON.parse(JSON.stringify(query))
				}
				
				this.querydata.pageNum = 1
				this.status = 'loading'
				if (this.current == 0) {
					this.gettodoList()
				}
				if (this.current == 1) {
					this.getfinishedList()
				}
				if (this.current == 2) {
					this.getcsList()
				}
				if (this.current == 3) {
					this.getmyProcessList()
				}
			}
		}
	}
</script>

<style lang="less">
	.process_con {
		width: 100%;
		padding: 0 20rpx;
		box-sizing: border-box;

		.process_li {
			margin-top: 20rpx;
			// display: flex;
			// justify-content: flex-start;
			// align-items: flex-start;
			padding: 24rpx 30rpx;
			width: 100%;
			box-sizing: border-box;
			background-color: #FFFFFF;
			color: #333;
			border-radius: 10rpx;

			.li_con {
				display: flex;
				justify-content: flex-start;
				align-items: flex-start;

				.li_left {
					width: 80rpx;
					height: 80rpx;
					background-color: rgba(255, 133, 61, 1);
					border-radius: 10rpx;
					display: flex;
					justify-content: center;
					align-items: center;
					margin-right: 20rpx;
					margin-top: 6rpx;
				}

				.li_right {
					.name {
						font-size: 30rpx;
						line-height: 44rpx;
					}

					.info_li {
						font-size: 24rpx;
						line-height: 40rpx;
						color: #999999;
						margin-top: 10rpx;
					}
				}
			}

			.view_line {
				width: 100%;
				height: 1rpx;
				background-color: #EAEAEA;
				margin-top: 20rpx;
			}

			.hadle_con {
				margin-top: 24rpx;
				display: flex;
				justify-content: space-between;
				align-items: center;

				.tips {
					padding: 6rpx 8rpx;
					font-size: 20rpx;
					height: 20rpx;
					line-height: 20rpx;
					width: inherit;
					border-radius: 4rpx;
					background-color: #EEFAF2;
					color: #50C97A;

					&.in {
						background-color: #E9F1FF;
						color: #2371FF;
					}

					&.Draft {
						background-color: #F8F8F8;
						color: #999999;
					}

					&.Rejected {
						background-color: #FFF5F5;
						color: #FF3535;
					}
				}

				.hadle_right {
					display: flex;
					justify-content: flex-end;
					align-items: center;

					.hadle_li {
						display: flex;
						justify-content: flex-end;
						align-items: center;
						margin-right: 52rpx;

						.text {
							font-size: 28rpx;
							margin-left: 10rpx;
							color: #999999;
						}
					}

					.hadle_li:last-child {
						margin-right: 0;
					}
				}

			}

		}
	}
</style>