<template>
	<view>
		<top :title="topTitle" leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" rightWidth="157rpx"
			:isleftBack="true" backgroundColor="#161A26"></top>
		<v-tabs v-model="current" :scroll="false" :tabs="tabArr" color="rgba(255, 255, 255, 0.5)"
			activeColor="rgba(255, 255, 255, 1)" :fixed="true" @change="changeTab" :lineScale="0.1" fontSize="28rpx"
			activeFontSize="28rpx" paddingItem="0" lineHeight="4rpx" :hasBorder="false" height="72rpx" padding="0"
			bgColor="rgba(22, 26, 38, 1)" lineColor="rgba(255, 255, 255, 1)" :zIndex="996" scrollPadding="0 0 20rpx 0"
			scrollBgColor="rgba(22, 26, 38, 1)"></v-tabs>

		<view class="comments_list">
			<view class="comments_li" v-for="item in pinglunList">
				<view class="user_info" v-if="item.UserId">
					<image class="image" :src="item.UserInfo.Avatar" mode="" v-if="item.UserInfo"></image>
					<view class="info">
						<view class="name">{{item.UserInfo.RealName}}</view>
						<view class="time">{{item.CreateOn}}</view>
					</view>
				</view>
				<view class="content" v-html="item.Content"></view>
				<view class="type" v-if="item.Subject.TargetType=='客户'">{{item.Subject.Title}}<text
						class="type_name">（Customer）</text></view>
				<view class="type" v-else-if="item.Subject.TargetType=='线索'">{{item.Subject.Title}}<text
						class="type_name">（Clue）</text></view>
				<view class="type" v-else-if="item.Subject.TargetType=='商机'">{{item.Subject.Title}}<text
						class="type_name">（Opportunity）</text></view>
			</view>
			<uni-load-more iconType="circle" :status="status" v-if="status" />
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		commentList
	} from '@/api/message'
	export default {
		data() {
			return {
				topTitle: 'comments',
				current: 0,
				tabArr: ['Comment on me', 'My comments'],
				status: 'loading',
				queryParams: {
					pageNum: 1,
					pageSize: 10,
					IsMy: true
				},
				pinglunList: [] //评论列表
			}
		},
		onLoad(options) {
			if (options.active) {
				this.current = Number(options.active)
				this.loadpinglunList()
			}
		},
		onReachBottom() { //上拉触底
			// console.log("现在的状态是什么", this.status);
			if (this.status != 'noMore') {
				this.queryParams.pageNum++;
				this.status = "loading";
				this.loadpinglunList();
				// console.log("现在是第几页", this.page);
			}
		},
		methods: {
			changeTab(inx) {
				this.queryParams.pageNum=1
				this.loadpinglunList()
			},
			loadpinglunList() {
				//
				this.status = 'loading'
				if (this.current == 1) { //是否是我的评论
					if (this.queryParams.IsMyReply) {
						delete this.queryParams.IsMyReply
						this.queryParams.IsMy = true
					} else {
						this.queryParams.IsMy = true
					}
				} else if (this.current == 0) { //是否是回复我的评论
					if (this.queryParams.IsMy) {
						delete this.queryParams.IsMy
						this.queryParams.IsMyReply = true
					} else {
						this.queryParams.IsMyReply = true
					}
				}
				if(this.queryParams.pageNum==1){
					this.pinglunList=[]
				}
				commentList(this.queryParams).then(res => {
					console.log("评论列表", res);
					if (res.data && res.data.List) {
						this.topTitle = 'comments（' + res.data.Total + '）'
						this.pinglunList = [...this.pinglunList, ...res.data.List]
					}
					if (res.data.List.length < this.queryParams.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}

				}).catch(err => {
					this.status = 'noMore';
					this.setMsgTop(err)
				})
			},
		}
	}
</script>

<style lang="less" scoped>
	.comments_list {
		width: 100%;
		padding: 0 20rpx;
		box-sizing: border-box;

		.comments_li {
			width: 100%;
			background-color: rgba(28, 34, 50, 1);
			border-radius: 10rpx;
			padding: 30rpx;
			box-sizing: border-box;
			color: rgba(255, 255, 255, 1);
			margin-top: 20rpx;
			font-size: 32rpx;

			.user_info {
				display: flex;
				justify-content: flex-start;
				align-items: center;

				.image {
					width: 60rpx;
					height: 60rpx;
					border-radius: 50%;
				}

				.info {
					margin-left: 20rpx;

					.name {
						font-size: 28rpx;
						line-height: 40rpx;
					}

					.time {
						font-size: 24rpx;
						line-height: 34rpx;
						color: rgba(255, 255, 255, 0.5);
					}
				}
			}

			.content {
				margin-top: 20rpx;
			}

			.type {
				margin-top: 20rpx;
				background-color: rgba(22, 26, 38, 1);
				width: 100%;
				padding: 20rpx 24rpx;
				border-radius: 10rpx;
				font-size: 28rpx;
				box-sizing: border-box;

				.type_name {
					color: rgba(255, 255, 255, 0.5);
				}
			}
		}
	}
</style>