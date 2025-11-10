<template>
	<view>
		<top :title="topTitle" leftText="Back" leftWidth="120rpx" rightWidth="120rpx" :isleftBack="true"
			leftIcon="icon-fanhui" backgroundColor="#161A26">
		</top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="Please enter the name"></search-compt>
		<select-compt ref="selectCompt" :haidate="true" :timeQuery="timeQuery" :querydata="querydata"
			@selectFinsh="selectFinsh"></select-compt>

		<view class="task_con">
			<view class="task_list">
				<view class="task_li" v-for="(item,inx) in taskList">
					<view class="li_title">
						<view class="name">
							{{item.Name}}
						</view>
						<view class="house" v-if="item.HouseId">
							Warehouse: {{item.House.StoreName}}
						</view>
					</view>
					<view class="li_content">
						<view class="cot_li">
							<view class="left"></view>
							<view class="right">
								<view class="right_li">
									<view class="label">Created</view>
									<view class="text">{{item.createTime}}</view>
								</view>
							</view>
							<view class="dot" :class="{'active':item.createTime}"></view>
						</view>
						<view class="cot_li">
							<view class="left"></view>
							<view class="right">
								<view class="right_li">
									<view class="label">Initial check</view>
									<view class="text">{{item.StartOn}}</view>
								</view>
								<view class="right_li" v-for="user in item.UserList" v-if="user.TimeIn==0">
									<view class="user_info">
										<image class="image" :src="user.UserInfo.Avatar" mode=""
											v-if="user.UserInfo&&user.UserInfo.Avatar"></image>
										<view class="user_name" v-if="user.UserInfo&&user.UserInfo.RealName">
											{{user.UserInfo.RealName}}</view>
									</view>
									<view class="btn" v-if="user.UserId==loginUserId" @click.stop="checking(item)">Start</view>
								</view>
							</view>
							<view class="dot" :class="{'active':item.StartOn}"></view>
						</view>
						<view class="cot_li noborder">
							<view class="left"></view>
							<view class="right">
								<view class="right_li">
									<view class="label">Recheck</view>
									<view class="text">{{item.CheckOn}}</view>
								</view>
								<view class="right_li" v-for="user in item.UserList" v-if="user.TimeIn==1">
									<view class="user_info">
										<image class="image" :src="user.UserInfo.Avatar" mode=""
											v-if="user.UserInfo&&user.UserInfo.Avatar"></image>
										<view class="user_name" v-if="user.UserInfo&&user.UserInfo.RealName">
											{{user.UserInfo.RealName}}</view>
									</view>
									<view class="btn" v-if="user.UserId==loginUserId" @click.stop="checking(item)">Start</view>
								</view>
							</view>
							<view class="dot" :class="{'active':item.CheckOn}"></view>
						</view>
					</view>
				</view>
			</view>
		</view>
		<uni-load-more iconType="circle" :status="status" v-if="status" />
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		invTask
	} from "@/api/inventory";
	export default {
		data() {
			return {
				topTitle: 'Checking tasks',
				querydata: {
					pageNum: 1,
					pageSize: 10,
					Status: 9,
				},
				timeQuery: {
					name: 'Initial trading time',
					params: ['beginTime', 'endTime'],
					value: [],
				},
				status: 'loading',
				taskList: [] //盘点任务
			};
		},
		computed:{
			loginUserId(){
				console.log(this.$store.state.user,'this.$store.user');
				return this.$store.state.user.uid
			}
		},
		async onLoad() {
			await this.loadData()
		},
		onReachBottom() { //上拉触底
			// console.log("现在的状态是什么", this.status);
			if (this.status != 'noMore') {
				this.querydata.pageNum++;
				this.status = "loading";
				this.getList();
				// console.log("现在是第几页", this.page);
			}
		},
		methods: {
			checking(row){
				uni.navigateTo({
					url:'/pages_Inventory/checking?id='+row.Id
				})
			},
			async getList() {
				try {
					let response = await invTask(this.querydata)
					console.log(response, 'response盘点任务');
					if (this.querydata.pageNum == 1) {
						this.taskList = []
					}
					this.taskList = response.data.List;
					this.total = response.data.Total;
					if (response.data.List.length < this.querydata.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				} catch (e) {
					//TODO handle the exception
					this.status = 'noMore';
					this.setMsgTop(e)
				}

			},
			async loadData(query) {
				this.querydata.pageNum = 1
				this.status = 'loading'
				await this.getList()
			},
			async searching(val) {
				//搜索
				if (val) {
					this.querydata.Name = val

				} else {
					delete this.querydata.Name
				}
				await this.loadData()
			},
			openSelect() {
				this.$refs.selectCompt.openSelect()
			},
			selectFinsh(query) {
				console.log("query", query);
				this.querydata = JSON.parse(JSON.stringify(query))
				this.querydata.pageNum = 1
				this.taskList = []
				this.status = 'loading'
				this.getList()

			},
		}
	}
</script>

<style lang="less" scoped>
	.task_con {
		padding: 0 20rpx;
		width: 100%;
		box-sizing: border-box;

		.task_list {
			.task_li {
				padding: 24rpx 30rpx;
				border-radius: 10rpx;
				margin-top: 20rpx;
				background-color: rgba(28, 34, 50, 1);
				color: rgba(255, 255, 255, 1);

				.li_title {
					padding-bottom: 23rpx;
					border-bottom: 1rpx solid rgba(255, 255, 255, 0.2);

					.name {
						font-size: 32rpx;
						line-height: 44rpx;
					}

					.house {
						font-size: 28rpx;
						line-height: 40rpx;
						color: rgba(255, 255, 255, 0.5);
						margin-top: 4rpx;
					}
				}

				.li_content {
					width: 100%;
					margin-top: 38rpx;

					.cot_li {
						position: relative;
						margin-left: 5rpx;
						width: calc(100% - 5rpx);
						display: flex;
						justify-content: flex-start;
						align-items: flex-start;
						padding-bottom: 50rpx;
						border-left: 2rpx solid rgba(255, 255, 255, 0.2);

						&.noborder {
							border-left: none;
							padding-bottom: 0;
						}

						.left {
							width: 25rpx;
							height: 100%;
						}

						.right {

							margin-top: -16rpx;
							width: calc(100% - 25rpx);

							.right_li {
								width: 100%;
								display: flex;
								justify-content: space-between;
								align-items: center;
								font-size: 28rpx;
								color: rgba(255, 255, 255, 0.5);

								.label {
									line-height: 40rpx;
								}

								.text {
									line-height: 40rpx;
									font-size: 24rpx;
								}

								.user_info {
									display: flex;
									justify-content: flex-start;
									align-items: center;
									margin-top: 20rpx;

									.image {
										width: 60rpx;
										height: 60rpx;
										border-radius: 50%;
										margin-right: 20rpx;
									}

									.user_name {
										color: rgba(255, 255, 255, 1);
										font-size: 32rpx;
									}
								}

								.btn {
									margin-top: 20rpx;
									background: linear-gradient(180deg, #FF3535 0%, #FF613D 100%);
									width: 128rpx;
									height: 48rpx;
									border-radius: 6rpx;
									color: rgba(255, 255, 255, 1);
									font-size: 24rpx;
									display: flex;
									align-items: center;
									justify-content: center;
								}
							}
						}

						.dot {
							position: absolute;
							left: -7rpx;
							top: -6rpx;
							width: 12rpx;
							height: 12rpx;
							background-color: rgba(73, 78, 91, 1);
							border-radius: 50%;

							&.active {
								background-color: rgba(255, 255, 255, 1);
							}
						}
					}
				}
			}
		}
	}
</style>