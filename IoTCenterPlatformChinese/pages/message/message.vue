<template>
	<view class="pages_bgcon">
		<!-- <top :title="topTitle"  :titleIsLeft="true" :isNoLeftPadding="true" backgroundColor="#ffffff"
			:rightWidth="120" rightText="All read" rightWidth="157rpx" @clickRight="messageAllRead">
		</top> -->
		<top :title="topTitle" leftWidth="157rpx" leftIcon="icon-fanhui" rightText="全部已读" rightWidth="157rpx"
			:isleftBack="true" backgroundColor="#FFFFFF" @clickRight="messageAllRead"></top>
		<uni-nav-bar :status-bar="false" :fixed="true" :border="false" height="156rpx" backgroundColor="#ffffff"
			v-if="isCheckPermi(['/DiscussService/Comment/List'])">
			<template v-slot:allslot>
				<view class="comment_con">
					<view class="con_li" @click="jumpToComment(0)">
						<view class="li_con">
							<view class="title">评论我的</view>
						</view>
						<image class="image" :src="getSerVerUrl()+'/appimg/images/comment_on_my.png'" mode=""></image>
					</view>
					<view class="con_li" @click="jumpToComment(1)">
						<view class="li_con">
							<view class="title">我的评论</view>
						</view>
						<image class="image" :src="getSerVerUrl()+'/appimg/images/my_comment.png'" mode=""></image>
					</view>
				</view>
			</template>
		</uni-nav-bar>
		<v-tabs v-model="current" :scroll="false" :tabs="tabArr" color="#999999" activeColor="#333333" :fixed="true"
			@change="changeTab" :lineScale="0.14" fontSize="28rpx" activeFontSize="28rpx" paddingItem="0"
			lineHeight="4rpx" :hasBorder="false" height="72rpx" padding="0" bgColor="#ffffff"
			lineColor="rgba(35, 113, 255, 1)" :zIndex="996" scrollPadding="0" scrollBgColor="#ffffff"></v-tabs>
		<view class="message_list">
			<view class="message_li" v-for="(item,index) in messageLists" :class="{'already':item.status!=0}"
				@click.stop="toEveryMessage(item.click_url,item.id,item)">
				<view class="li_top">
					<view class="top_left">
						<view class="dot" v-if="item.status==0"></view>
						<view class="title">{{item.label}}</view>
					</view>
					<view class="top_right">
						{{returnTimeText(item.create_time)}}
					</view>
				</view>
				<view class="li_cot">
					{{item.content}}
				</view>
			</view>
			<uni-load-more iconType="circle" :status="status" v-if="status" />
		</view>
		<msg-prompt ref="promptMsg" @confirm="confirmAllRead"></msg-prompt>
	</view>
</template>

<script>
	import {
		messageList,
		setRead,
		setReadAll
	} from "@/api/message";
	var dayjs = require('@/common/day.js')
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				topTitle: '消息',
				current: 0,
				tabArr: ['全部消息', '未读消息', '已读消息'],
				status: 'loading',
				queryParams: {
					pageNum: 1,
					pageSize: 10,
					status: -1
				},
				messageLists: [] ,//消息列表
				isFinishLoad:false
			}
		},
		onLoad() {
			this.getMessageList()
		},
		onShow() {
			if (this.isFinishLoad&&this.$store.state.isReloadMessageList) {
				this.loadMessageLis()
				this.$store.commit('SET_MESSAGELIST_INFO', false)
			}
		},
		onReachBottom() { //上拉触底
			// console.log("现在的状态是什么", this.status);
			if (this.status != 'noMore') {
				this.queryParams.pageNum++;
				this.status = "loading";
				this.getMessageList();
				// console.log("现在是第几页", this.page);
			}
		},
		methods: {
			jumpToComment(inx) {
				uni.navigateTo({
					url: '/pages/comments/comments?active=' + inx
				})
			},
			toEveryMessage(routs, id, item) {

				if (item.status == 0) {
					setRead({
						id
					}).then(res => {
						if (res.code == 0) {
							// this.$refs.mytab.getNoReadCount()
							setPagesParam('readMessage', 'load', 1,false)
							this.$store.commit('SET_MESSAGE_INFO', true)
							if (routs) {
								if (item.click_type == "Approval") {
									let srt = item.click_url.substring(item.click_url.lastIndexOf('=') + 1)
									uni.navigateTo({
										url: '/pages_flow/process_detail?id=' + srt + '&isMessage=true'
									})
								}
								if (item.click_type == "设备告警") {
									uni.navigateTo({
										url: '/pages_device/alarm/list'
									})
								}
								if(item.click_type=='PlaneGroupNotice'){
									uni.navigateTo({
										url: '/pages_device/device_info/plane_task?isManage=true'
									})
								}
								if(item.click_type=='PlaneTaskNotice'){
									let srt = item.click_url.substring(item.click_url.lastIndexOf('=') + 1)
									uni.navigateTo({
										url: '/pages_flow/device/task_detail?id='+srt
									})
								}
							}
							this.loadMessageLis()
						}
					});
				} else {
					if (routs) {
						// console.log(routs, '路径信息111');
						if (item.click_type == "Approval") {
							let srt = item.click_url.substring(item.click_url.lastIndexOf('=') + 1)
							uni.navigateTo({
								url: '/pages_flow/process_detail?id=' + srt + '&isMessage=true'
							})
						}
						if (item.click_type == "设备告警") {
							uni.navigateTo({
								url: '/pages_device/alarm/list'
							})
						}
						if(item.click_type=='PlaneGroupNotice'){
							uni.navigateTo({
								url: '/pages_device/device_info/plane_task?isManage=true'
							})
						}
						if(item.click_type=='PlaneTaskNotice'){
							let srt = item.click_url.substring(item.click_url.lastIndexOf('=') + 1)
							uni.navigateTo({
								url: '/pages_flow/device/task_detail?id='+srt
							})
						}
					}
				}
			},
			returnTimeText(val) {
				let nowTime = new Date()
				let tomorrowDate = dayjs(val).add(1, 'day')
				tomorrowDate = dayjs(tomorrowDate).format('YYYY-MM-DD') + ' 00:00:00'
				let afterTomorrowDate = dayjs(val).add(2, 'day')
				afterTomorrowDate = dayjs(afterTomorrowDate).format('YYYY-MM-DD') + ' 00:00:00'
				let sixSecond = dayjs(val).add(60, 'second')
				let sixMinute = dayjs(val).add(60, 'minute')
				if (dayjs().isBefore(sixSecond)) {
					return '刚刚'
				} else if (dayjs().isBefore(sixMinute)) {
					const date1 = dayjs('2019-01-25')
					let diffNum = dayjs(nowTime).diff(dayjs(val), 'minute', false)
					return diffNum + '分钟之前'
				} else if (dayjs().isBefore(dayjs(tomorrowDate))) {
					return dayjs(val).format('HH:mm')
				} else if (dayjs().isBefore(dayjs(afterTomorrowDate))) {
					return '昨天'
				} else {
					return dayjs(val).format('YYYY-MM-DD')
				}
			},
			loadMessageLis() {
				this.queryParams.pageNum = 1
				this.getMessageList()
			},
			getMessageList() {
				this.status = "loading";
				if (this.queryParams.pageNum == 1) {
					this.messageLists = []
				}
				messageList(this.queryParams).then(res => {
					if (res.code == 0) {
						this.messageLists = [...this.messageLists, ...res.data.List];
						this.topTitle = '消息（' + res.data.Total + '）';
						if (res.data.List.length < this.queryParams.pageSize) {
							this.status = 'noMore';
						} else {
							this.status = 'more';
						}
					}
					// console.log("新闻列表",this.messageLists);
					this.isFinishLoad=true
				}).catch(err => {
					console.log("消息报错",err);
					this.setMsgTop(err)
				});
			},
			confirmAllRead() {
				//全部已读
				this.$refs.promptMsg.loadingOpen('加载中...')
				setReadAll().then(res => {
					if (res.code == 0) {
						this.getMessageList();
						this.$store.commit('SET_MESSAGE_INFO', true)
						console.log("this.$store.state.isReloadMessage",this.$store.state.isReloadMessage);
					}
					this.$refs.promptMsg.loadingColse()
				}).catch(err => {
					this.$refs.promptMsg.loadingColse()
					this.setMsgTop(err)
				});
			},
			messageAllRead() {
				//消息已读
				this.$refs.promptMsg.noticeOpen('是否将所有消息设置为已读?', '系统提示', true)
			},
			changeTab(index) {
				//切换
				if (index == 0) {
					this.queryParams.status = -1 //全部
				} else if (index == 1) { //未读
					this.queryParams.status = 0
				} else { //已读
					this.queryParams.status = 1
				}
				this.queryParams.pageNum = 1
				this.getMessageList()
			}
		}
	}
</script>

<style lang="less" scoped>
	.message_list {
		width: 100%;
		padding: 0 20rpx;
		box-sizing: border-box;
		// padding-top: 40rpx;

		.message_li {
			width: 100%;
			padding: 26rpx 30rpx;
			box-sizing: border-box;
			background-color: rgba(255, 255, 255, 1);
			border-radius: 10rpx;
			color: #333;
			margin-top: 20rpx;

			&.already {
				color: rgba(153, 153, 153, 1);

				.li_top {
					.top_left {
						.title {
							margin-left: 0;
						}
					}
					.top_right {
						color: rgba(153, 153, 153, 1);
					}
				}

				.li_cot {
					color: rgba(193, 193, 193, 1);
				}
			}

			.li_top {
				display: flex;
				justify-content: space-between;
				align-items: center;
				line-height: 44rpx;

				.top_left {
					display: flex;
					justify-content: flex-start;
					align-items: center;

					.dot {
						background-color: rgba(255, 53, 53, 1);
						width: 12rpx;
						height: 12rpx;
						border-radius: 50%;
					}

					.title {
						font-size: 32rpx;
						line-height: 44rpx;
						margin-left: 16rpx;
					}

					.tips {
						font-size: 22rpx;
						line-height: 22rpx;
						padding: 5rpx 8rpx;
						border-radius: 4rpx;
						// border: 1rpx solid rgba(255, 53, 53, 1);
						color: rgba(35, 113, 255, 1);
						background-color: rgba(233, 241, 255, 1);
						margin-left: 20rpx;
						&.private{
							background-color: rgba(255, 245, 245, 1);
							color: rgba(255, 53, 53, 1);
						}
					}
				}

				.top_right {
					font-size: 24rpx;
					color: rgba(153, 153, 153, 1);
					margin-top: 4rpx;
				}
			}

			.li_cot {
				line-height: 40rpx;
				font-size: 28rpx;
				color: rgba(153, 153, 153, 1);
			}
		}
	}

	.comment_con {
		width: 100%;
		padding: 20rpx;
		box-sizing: border-box;
		display: flex;
		justify-content: space-between;
		align-items: center;

		.con_li {
			width: calc(50% - 10rpx);
			height: 116rpx;
			position: relative;

			.li_con {
				width: 100%;
				padding: 14rpx 30rpx;
				box-sizing: border-box;
				height: 116rpx;
				position: absolute;
				top: 0;
				left: 0;
				z-index: 1;

				.title {
					font-size: 32rpx;
					color: rgba(255, 255, 255, 1);
					line-height: 44rpx;
				}

				.cot {
					line-height: 40rpx;
					font-size: 28rpx;
					display: flex;
					justify-content: flex-start;
					align-items: center;

					.text {
						color: rgba(255, 255, 255, 1);
						margin-left: 10rpx;
					}
				}
			}

			.image {
				position: absolute;
				top: 0;
				left: 0;
				width: 100%;
				height: 116rpx;
				z-index: 0;
			}
		}
	}
</style>