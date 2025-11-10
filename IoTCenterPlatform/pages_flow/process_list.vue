<template>
	<view>
		<top :title="topTitle" leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" backgroundColor="#161A26"
			rightWidth="157rpx" :rightIcon="current==3?'icon-tianjia':''" :isleftBack="true" @clickRight="addProcess">
		</top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="Please enter the name"></search-compt>
		<select-compt ref="selectCompt" :haidate="true" :timeQuery="timeQuery" :querydata="querydata"
			@selectFinsh="selectFinsh" fixedHeight="216rpx"></select-compt>
		<v-tabs v-model="current" :scroll="false" :tabs="tabArr" color="rgba(255, 255, 255, 0.5)"
			activeColor="rgba(255, 255, 255, 1)" :fixed="true" @change="changeTab" :lineScale="0.16" fontSize="28rpx"
			activeFontSize="28rpx" paddingItem="0" lineHeight="4rpx" :hasBorder="false" height="72rpx" padding="0"
			bgColor="rgba(22, 26, 38, 1)" lineColor="rgba(255, 255, 255, 1)" :zIndex="1" scrollPadding="0 0"
			scrollBgColor="rgba(22, 26, 38, 1)"></v-tabs>
		<view class="process_con">
			<view class="process_li" v-for="item in list" @click.stop="toProccessInfo(item)">
				<view class="li_con">
					<view class="li_left" :style="{'background':item.Background}">
						<custom-icons :iconsName="item.Icon?iconSubStr(item.Icon):''" iconsSize="40rpx"></custom-icons>
					</view>
					<view class="li_right">
						<view class="name">
							{{item.FlowName}}
						</view>
						<view class="info_li" v-if="current==0">
							Reception time: {{item.StartTime}}
						</view>
						<view class="info_li" v-if="current==1">
							Processing time: {{item.StartTime}}
						</view>
						<view class="info_li" v-if="current==2">
							Cc Time: {{item.StartTime}}
						</view>
						<view class="info_li" v-if="current==3">
							Submission time: {{item.createTime}}
						</view>
						<view class="info_li" v-if="current!=3">
							Promoter: {{item.startRealName}}
						</view>
					</view>
				</view>
				<view class="view_line" v-if="current==3"></view>
				<view class="hadle_con" v-if="current==3">
					<view class="tips in" v-if="item.Status==0">
						In progress
					</view>
					<view class="tips Draft" v-if="item.Status==1">
						Draft
					</view>
					<view class="tips Rejected" v-if="item.Status==2">
						Rejected
					</view>
					<view class="tips" v-if="item.Status==3">
						Completed
					</view>
					<view class="hadle_right">
						<view class="hadle_li" v-if="item.Status==1" @click.stop="editProccess(item)">
							<custom-icons iconsName="icon-bianji" iconsSize="24rpx"
								iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
							<text class="text">Edit</text>
						</view>
						<view class="hadle_li" v-if="item.Status==0||item.Status==1" @click.stop="handleStop(item)">
							<custom-icons iconsName="icon-quxiaoliucheng" iconsSize="24rpx"
								iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
							<text class="text">Revoke</text>
						</view>
						<view class="hadle_li" v-if="item.Status==2||item.Status==3" @click.stop="handleDelete(item)">
							<custom-icons iconsName="icon-shanchu" iconsSize="24rpx"
								iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
							<text class="text">Delete</text>
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
	export default {
		data() {
			return {
				timeQuery: {
					name: 'date',
					params: ['beginTime', 'endTime'],
					value: [],
				},
				status: 'loading',
				tabArr: ['To-do', 'Completed', 'Cc to me', 'Created'],
				querydata: {
					pageNum: 1,
					pageSize: 10
				},
				current: 0,
				selectListParam: [],
				topTitle: 'Processes',
				list: [],
				activeDelId:'',//当前执行取消或删除的流程的id
				isStop:false,//为true表示执行取消流程，false表示删除流程
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
			     this.activeDelId=row.Id
					this.$refs.promptMsg.noticeOpen(
						'Are you sure to delete the data item with the process definition number ' + row.Id +'?', 'warn', true
					)
			    },
				async confirmDelete(){
					if(this.isStop){
						if(this.activeDelId){
							this.isStop=false
							try{
								await canProcess(this.activeDelId)
								this.$store.commit('SET_UPDATE_REPORT', true)
								this.$refs.promptMsg.open('Cancel successful', 2000) //提示信息组件
								this.loadData();
								this.activeDelId=''
								
							}catch(e){
								//TODO handle the exception
								this.setMsgTop(e)
							}
						}
					}else{
						if(this.activeDelId){
							try{
								await delProcess(this.activeDelId)
								this.$store.commit('SET_UPDATE_REPORT', true)
								this.$refs.promptMsg.open('Delete successful', 2000) //提示信息组件
								this.loadData()
								this.activeDelId=''
							}catch(e){
								//TODO handle the exception
								this.setMsgTop(e)
							}
						}
					}
				},
			/**  取消流程申请 */
			    handleStop(row) {
					this.activeDelId=row.Id
					this.isStop=true
			      this.$refs.promptMsg.noticeOpen(
			      	'Are you sure to cancel the process application with the number ' + row.Id +'?', 'warn', true
			      )
			    },
			/**编辑流程 */
			editProccess(row) {
				uni.navigateTo({
					url:'/pages_flow/process_form?procDefId='+row.TemplateId+'&id='+row.Id
				})
			},
			toProccessInfo(row){
				if(this.current==3){
					uni.navigateTo({
						url:'/pages_flow/process_detail?id='+row.Id+'&isRoot=true'
					})
				}else if(this.current==0){
					uni.navigateTo({
						url:'/pages_flow/process_detail?id='+row.ExecutionNodeId+'&toDo=true'
					})
				}else{
					uni.navigateTo({
						url:'/pages_flow/process_detail?id='+row.ExecutionNodeId
					})
				}
				
			},
			loadData(query){
				this.querydata.pageNum=1
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
					console.log(rsp);
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
					console.log(rsp);
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
					console.log(rsp);
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
					console.log(rsp);
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
				console.log(index, 'index');
				this.querydata.pageNum = 1
				if (index == 0) {
					if(this.querydata.name){
						this.querydata.key=this.querydata.name
						delete this.querydata.name
					}
					this.gettodoList()
				}
				if (index == 1) {
					if(this.querydata.name){
						this.querydata.key=this.querydata.name
						delete this.querydata.name
					}
					this.getfinishedList()
				}
				if (index == 2) {
					if(this.querydata.name){
						this.querydata.key=this.querydata.name
						delete this.querydata.name
					}
					this.getcsList()
				}
				if (index == 3) {
					if(this.querydata.key){
						this.querydata.name=this.querydata.key
						delete this.querydata.key
					}
					this.getmyProcessList()
				}
			},
			searching(val) {
				//搜索
				if(this.current==3){
					if (val) {
						this.querydata.name = val
					
					} else {
						delete this.querydata.name
						if(this.querydata.key){
							delete this.querydata.key
						}
					}
				}else{
					if (val) {
						this.querydata.key = val
					
					} else {
						delete this.querydata.key
						if(this.querydata.name){
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
				this.querydata = JSON.parse(JSON.stringify(query))
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
			background-color: rgba(28, 34, 50, 1);
			color: #fff;
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
						font-size: 32rpx;
						line-height: 44rpx;
					}

					.info_li {
						font-size: 28rpx;
						line-height: 40rpx;
						color: rgba(255, 255, 255, 0.5);
						margin-top: 10rpx;
					}
				}
			}

			.view_line {
				width: 100%;
				height: 1rpx;
				background-color: rgba(255, 255, 255, 0.2);
				margin-top: 20rpx;
			}

			.hadle_con {
				margin-top: 24rpx;
				display: flex;
				justify-content: space-between;
				align-items: center;

				.tips {
					padding: 6rpx 8rpx;
					border: 1rpx solid rgba(255, 255, 255, 0.5);
					color: rgba(255, 255, 255, 0.5);
					font-size: 20rpx;
					line-height: 20rpx;
					width: inherit;
					border-radius: 4rpx;
					&.in{
						border-color: #EFA902;
						color: #EFA902;
					}
					&.Draft{
						border-color: #FF853D;
						color: #FF853D;
					}
					&.Rejected{
						border-color: #FF3535;
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
							color: rgba(255, 255, 255, 0.5);
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