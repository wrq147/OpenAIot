<template>
	<view>
		<top :title="topTitle" leftWidth="0rpx" :titleIsLeft="true" :isNoLeftPadding="true" backgroundColor="#161A26"
			:rightWidth="120" rightText="All read" rightWidth="157rpx" @clickRight="messageAllRead">
		</top>
		<uni-nav-bar :status-bar="false" :fixed="true" :border="false" height="156rpx" backgroundColor="#161A26"
			v-if="isCheckPermi(['/DiscussService/Comment/List'])">
			<template v-slot:allslot>
				<view class="comment_con">
					<view class="con_li" @click="jumpToComment(0)">
						<view class="li_con">
							<view class="title">Comment on me</view>
							<!-- <view class="cot">
								<custom-icons iconsName="icon-xingbiao" iconsSize="20rpx"></custom-icons>
								<view class="text">8 news</view>
							</view> -->
						</view>
						<image class="image" src="/static/images/comment_on_my.png" mode=""></image>
					</view>
					<view class="con_li" @click="jumpToComment(1)">
						<view class="li_con">
							<view class="title">My comments</view>
						</view>
						<image class="image" src="/static/images/my_comment.png" mode=""></image>
					</view>
				</view>
			</template>
		</uni-nav-bar>
		<v-tabs v-model="current" :scroll="false" :tabs="tabArr" color="rgba(255, 255, 255, 0.5)"
			activeColor="rgba(255, 255, 255, 1)" :fixed="true" @change="changeTab" :lineScale="0.14" fontSize="28rpx"
			activeFontSize="28rpx" paddingItem="0" lineHeight="4rpx" :hasBorder="false" height="72rpx" padding="0"
			bgColor="rgba(22, 26, 38, 1)" lineColor="rgba(255, 255, 255, 1)" :zIndex="996" scrollPadding="20rpx 0"
			scrollBgColor="rgba(22, 26, 38, 1)"></v-tabs>
		<view class="message_list">
			<view class="message_li" v-for="(item,index) in messageLists" :class="{'already':item.status!=0}"
				@click.stop="toEveryMessage(item.click_url,item.id,item)">
				<view class="li_top">
					<view class="top_left">
						<view class="dot" v-if="item.status==0"></view>
						<view class="title">{{item.label}}</view>
						<!-- <view class="tips" v-if="item.type!=0">Notice</view> -->
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
		<my-tab-bar ref="mytab" active="Messages"></my-tab-bar>
	</view>
</template>

<script>
	import {
		messageList,
		setRead,
		setReadAll
	} from "@/api/message";
	import {
		log
	} from "mqtt/dist/mqtt";
	var dayjs = require('@/common/day.js')
	// var relativeTime = require('@/common/dayExtend/relativeTime')
	// dayjs.extend(relativeTime)
	export default {
		data() {
			return {
				topTitle: 'Messages',
				current: 0,
				tabArr: ['All', 'Unread', 'Read'],
				status: 'loading',
				queryParams: {
					pageNum: 1,
					pageSize: 10,
					status: -1
				},
				messageLists: [] //消息列表
			}
		},
		onLoad() {
			this.$nextTick(()=>{
				this.$refs.mytab.loadCheck()
			})
			this.getMessageList()
		},
		onShow() {
			if(this.$store.state.isReloadMessageList){
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
				console.log("单条信息", routs, id, item);

				if (item.status == 0) {
					setRead({
						id
					}).then(res => {
						if (res.code == 0) {
							this.$refs.mytab.getNoReadCount()
							if (routs) {
								// this.$router.push(routs);
								// console.log(routs, '路径信息');
								if (item.click_type == "Approval") {
									let srt = item.click_url.substring(item.click_url.lastIndexOf('=') + 1)
									console.log(srt, 'srtsrtsrt');
									uni.navigateTo({
										url: '/pages_flow/process_detail?id=' + srt
									})
								}
								if (item.click_type == "设备告警") {
									let srt = item.click_url.substring(item.click_url.lastIndexOf('=') + 1)
									console.log(srt, 'srtsrtsrt');
									uni.navigateTo({
										url: '/pages_device/alarm/list'
									})
								}
							}
							this.loadMessageLis()
						}
					});
				} else {
					if (routs) {
						// console.log(routs, '路径信息111');
						if(item.click_type=="Approval"){
							let srt=item.click_url.substring(item.click_url.lastIndexOf('=')+1)
							console.log(srt,'srtsrtsrt');
							uni.navigateTo({
								url:'/pages_flow/process_detail?id='+srt
							})
						}
						if (item.click_type == "设备告警") {
							let srt = item.click_url.substring(item.click_url.lastIndexOf('=') + 1)
							console.log(srt, 'srtsrtsrt');
							uni.navigateTo({
								url: '/pages_device/alarm/list'
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
					return 'just now'
				} else if (dayjs().isBefore(sixMinute)) {
					const date1 = dayjs('2019-01-25')
					let diffNum = dayjs(nowTime).diff(dayjs(val), 'minute', false)
					return diffNum + 'mins ago'
				} else if (dayjs().isBefore(dayjs(tomorrowDate))) {
					return dayjs(val).format('HH:mm')
				} else if (dayjs().isBefore(dayjs(afterTomorrowDate))) {
					return 'Yesterday'
				} else {
					return dayjs(val).format('YYYY-MM-DD')
				}
			},
			loadMessageLis(){
				console.log("执行加载消息列表");
				this.queryParams.pageNum=1
				this.getMessageList()
			},
			getMessageList() {
				this.status = "loading";
				if (this.queryParams.pageNum == 1) {
					this.messageLists = []
				}
				messageList(this.queryParams).then(res => {
					console.log("消息列表", res);
					if (res.code == 0) {
						this.messageLists = [...this.messageLists, ...res.data.List];
						this.topTitle = 'Messages（' + res.data.Total + '）';
						if (res.data.List.length < this.queryParams.pageSize) {
							this.status = 'noMore';
						} else {
							this.status = 'more';
						}
					}
				}).catch(err => {
					this.setMsgTop(err)
				});
			},
			confirmAllRead() {
				//全部已读
				this.$refs.promptMsg.loadingOpen('Loading...')
				setReadAll().then(res => {
					console.log("设置消息全部已读", res);
					if (res.code == 0) {
						this.getMessageList();
						this.$refs.mytab.getNoReadCount()
					}
					this.$refs.promptMsg.loadingColse()
				}).catch(err => {
					this.$refs.promptMsg.loadingColse()
					this.setMsgTop(err)
				});
			},
			messageAllRead() {
				//消息已读
				this.$refs.promptMsg.noticeOpen('Do you want to set all messages as read?', 'system prompt', true)
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
		padding: 20rpx;
		box-sizing: border-box;

		.message_li {
			width: 100%;
			padding: 26rpx 30rpx;
			box-sizing: border-box;
			background-color: rgba(28, 34, 50, 1);
			border-radius: 10rpx;
			color: rgba(255, 255, 255, 1);
			margin-top: 20rpx;

			&.already {
				color: rgba(255, 255, 255, 0.5);

				.li_top {
					.top_left {
						.title {
							margin-left: 0;
						}
					}
					.top_right {
						color: rgba(255, 255, 255, 0.2);
					}
				}

				.li_cot {
					color: rgba(255, 255, 255, 0.2);
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
						border: 1rpx solid rgba(255, 53, 53, 1);
						color: rgba(255, 53, 53, 1);
						margin-left: 20rpx;
					}
				}

				.top_right {
					font-size: 24rpx;
					color: rgba(255, 255, 255, 0.5);
					margin-top: 4rpx;
				}
			}

			.li_cot {
				line-height: 40rpx;
				font-size: 28rpx;
				color: rgba(255, 255, 255, 0.5);
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
					color: rgba(255, 255, 255, 0.6);
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