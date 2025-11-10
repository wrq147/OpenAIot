<template>
	<view class="ContactDetails">
		<top leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#F5F8F9" title="详情" rightText="编辑"
			class="CRM-header">
			<template v-slot:top_right>
				<view class="right_top" @click="handEdit()">
					<!-- <custom-icons iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons> -->
					<view class="iconfont icon-bianji" style="font-size:31rpx;"></view>
				</view>
			</template>
		</top>
		<view class="ContactDetails-section">
			<view class="ContactDetails-section-autve" :style="{'background-image':`url(${getSerVerUrl()}/appimg/headerLogoNew.png)`}">
				<view class="image t-icon-morentouxiang1">

				</view>
			</view>
			<view class="ContactDetails-section-title">
				{{arr.RealName}}
			</view>
			<view class="ContactDetails-section-txt">
				{{arr.PostName}}
			</view>
			<view class="ContactDetails-section-border"></view>
			<view class="ContactDetails-section-item" style="border-top: 0.5px solid #f8f8f8;padding-top:12rpx;">
				<view class="ContactDetails-section-item-left">
					<view class="ContactDetails-section-txt">手机号</view>
					<view class="phone">{{arr.Mobile}}</view>
				</view>
				<view class="iconfont t-icon-dianhua1" @click="dialogConfirm()"></view>
			</view>

			<view class="ContactDetails-section-item" v-if="arr.Email">
				<view class="ContactDetails-section-item-left">
					<view class="ContactDetails-section-txt">邮箱</view>
					<view class="phone">{{arr.Email}}</view>
				</view>

			</view>
			<view class="ContactDetails-section-item" v-if="arr.WxNumber">
				<view class="ContactDetails-section-item-left">
					<view class="ContactDetails-section-txt">微信号</view>
					<view class="phone">{{arr.WxNumber}}</view>
				</view>

			</view>
			<view class="ContactDetails-section-item" v-if="arr.DeptName">
				<view class="ContactDetails-section-item-left">
					<view class="ContactDetails-section-txt">部门</view>
					<view class="phone">{{arr.DeptName}}</view>
				</view>

			</view>

			<view class="ContactDetails-section-item" v-if="arr.PostName">
				<view class="ContactDetails-section-item-left">
					<view class="ContactDetails-section-txt">职务</view>
					<view class="phone">{{arr.PostName}}</view>
				</view>

			</view>

			<view class="ContactDetails-section-item" v-if="arr.LeaderName">
				<view class="ContactDetails-section-item-left">
					<view class="ContactDetails-section-txt">负责人</view>
					<view class="phone">{{arr.LeaderName}}</view>
				</view>

			</view>
			<view class="ContactDetails-section-item" v-if="arr.HelperName">
				<view class="ContactDetails-section-item-left">
					<view class="ContactDetails-section-txt">协作人</view>
					<view class="phone">{{arr.HelperName}}</view>
				</view>

			</view>
			<view class="ContactDetails-section-item borderNone">
				<view class="ContactDetails-section-item-left">
					<view class="ContactDetails-section-txt">创建时间</view>
					<view class="phone">{{arr.createTime}}</view>
				</view>

			</view>
		</view>
		<view class="Delete-button" @click="handDelete()">
			删除
		</view>
		<view style="width: 100%;height:1rpx;"></view>
		<msg-prompt ref="promptMsg" @confirm="confirmUnbind"></msg-prompt>
	</view>
</template>

<script>
	import {
		CustomerInfo, //联系人详情
		ContactRemove //联系人删除
	} from "@/api/crmApi";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				arr: []
			}
		},
		onLoad(option) {
			if (option.id) {
				this.list(option.id);

			}
		},
		methods: {
			dialogConfirm() {
				uni.makePhoneCall({
					phoneNumber: this.arr.Mobile, // 这里就是自己要拨打的电话号码
					success: (res) => {
						console.log('调用成功!')
					},
					fail: (res) => {
						console.log('调用失败!')
					}
				})
			},
			confirmUnbind() {
				//删除
				ContactRemove({
					id: this.arr.Id
				}).then((res) => {
					if (res.code == 0) {
						uni.showToast({
							title: '删除成功！',
							icon: 'none'
						})
						setTimeout(() => {
							setPagesParam('list')
						}, 700)
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			handDelete() {
				//删除
				this.$refs.promptMsg.noticeOpen(
					"你确定要删除名为 " + this.arr.RealName + "的联系人吗?"
				)
			},
			handEdit() {
				uni.navigateTo({
					url: './ContactAddition?id=' + this.arr.Id
				})
			},
			list(id) {
				CustomerInfo({
					id: id
				}).then((res) => {
					if (res.code == 0) {
						//console.log(res,'联系人详情')
						if (res.data.Helper) {
							res.data.HelperName = (res.data.HelperUsers.map(row => row.RealName)).join(',')
						}
						if (res.data.LeaderId) {
							res.data.LeaderName = res.data.LeaderUser.RealName
						}

						this.arr = res.data
					}
				})
			},
		}
	}
</script>

<style lang="less" scoped>
	.ContactDetails {
		.t-icon-dianhua1 {
			width: 50rpx;
			height: 50rpx;
			color: #E9F1FF;
			text-align: right;
			margin-left: auto;
			margin-right: 20rpx;
			font-size: 50rpx;
		}

		.Delete-button {
			width: 94%;
			margin: 50rpx 3%;
			height: 100rpx;
			background: #fff;
			color: #999999;
			text-align: center;
			line-height: 100rpx;
			border-radius: 8rpx;
			font-size: 34rpx;
		}

		.ContactDetails-section {
			background: #ffffff;
			display: flex;
			flex-direction: column;
			margin: 0rpx 3%;
			margin-top: 100rpx;
			color: #333;
			text-align: center;

			.ContactDetails-section-item {
				display: flex;
				margin: 0rpx 4%;
				margin-top: 17rpx;
				align-items: center;
				padding-bottom: 25rpx;
				border-bottom: 1rpx solid #f8f8f8;

				.customIcons-dianhua {
					margin-left: auto;
					margin-top: 10rpx;
				}

				.ContactDetails-section-item-left {
					text-align: left;

					.phone {
						font-size: 32rpx;
						margin-top: 10rpx;
					}
				}

				.t-icon-dianhua {
					width: 50rpx;
					height: 50rpx;
					margin-left: auto;
					margin-top: 10rpx;
				}
			}

			.borderNone {
				border: none;
			}

			.ContactDetails-section-title {
				font-size: 32rpx;
				color: #333;
				margin-top: 20rpx;
				font-weight: bold;
			}

			.ContactDetails-section-border {
				height: 1rpx;
				background: rgba(255, 255, 255, .1);
				margin-top: 25rpx;
				width: 94%;
				margin: 30rpx 3%;
				margin-bottom: 0rpx;
			}

			.ContactDetails-section-txt {
				font-size: 28rpx;
				margin-top: 10rpx;
				color: #999999;
			}

			.ContactDetails-section-autve {
				width: 160rpx;
				height: 160rpx;
				border-radius: 50%;
				margin: 0 auto;
				margin-top: -80rpx;
				// background: url('../../static/headerLogoNew.png') no-repeat;
				background-repeat: no-repeat;
				background-size: 100% 100%;
				display: flex;
				justify-content: center;
				align-items: center;

				.image {
					width: 130rpx;
					height: 130rpx;
					border-radius: 50%;
				}
			}
		}
	}
</style>