<template>
	<view>
		<uni-popup ref="popup" type="center" :mask-click="false" :zIndex="999" maskBackgroundColor="rgba(0, 0, 0, 0)">
			<uni-popup-message type="info" :message="messages" :duration="duration" :isCustomMsg="true">
				<view class="message_con">
					{{messages}}
				</view>
			</uni-popup-message>
		</uni-popup>
		<uni-popup ref="loadingPop" type="center" :mask-click="false" :zIndex="999" maskBackgroundColor="rgba(0, 0, 0, 0)">
			<view class="loading_con">
				<image class="image" src="/static/images/apng/1.png" mode=""></image>
				<view class="text1">
					{{loadingText}}
				</view>
			</view>
		</uni-popup>
		<uni-popup ref="successPop" type="center" :mask-click="false" :zIndex="999" maskBackgroundColor="rgba(0, 0, 0, 0)">
			<view class="loading_con">
				<custom-icons iconsName="icon-chenggong" iconsSize="68rpx"></custom-icons>
				<view class="text">
					Success
				</view>
			</view>
		</uni-popup>
		<uni-popup ref="errorPop" type="center" :mask-click="false" :zIndex="999" maskBackgroundColor="rgba(0, 0, 0, 0)">
			<view class="loading_con">
				<custom-icons iconsName="icon-shibai" iconsSize="68rpx"></custom-icons>
				<view class="text">
					Error
				</view>
			</view>
		</uni-popup>
		<uni-popup ref="cliPopup" type="center" :mask-click="false" :zIndex="999">
			<view class="notice_con">
				<view class="notice_cont">
					<view class="title">{{noticeTitle}}</view>
					<view class="content">
						{{noticeContent}}
					</view>
					<button class="submit_button" @click.stop="confirm">
						Confirm
					</button>
					<view class="close_icon" @click.stop="noticeColse(true)">
						<custom-icons iconsName="icon-guanbidanchuang" iconsSize="36rpx"
							iconsColor="rgba(255, 255, 255, 0.2)"></custom-icons>
					</view>
				</view>
			</view>
		</uni-popup>
		<uni-popup ref="cliPopup2" type="center" :mask-click="false" :zIndex="999">
			<view class="notice_con">
				<view class="notice_cont">
					<view class="title">{{noticeTitle}}</view>
					<view class="content">
						{{noticeContent}}
					</view>
					<view class="btn_con">
						<button class="jump_button" @click.stop="noticeColse2">
							cancel
						</button>
						<button class="submit_button" @click.stop="confirm2">
							Confirm
						</button>
					</view>
					
					<view class="close_icon" @click="noticeColse2">
						<custom-icons iconsName="icon-guanbidanchuang" iconsSize="36rpx"
							iconsColor="rgba(255, 255, 255, 0.2)"></custom-icons>
					</view>
				</view>
			</view>
		</uni-popup>
		<uni-nav-bar :status-bar="true" :fixed="true" v-if="cusMask" :border="false" :height="fixedHeight" :zIndex="998"
			backgroundColor="#161A26">
			<!-- 为自定义的弹出层 -->
			<template v-slot:allslot>
				<view class="cliPopupmask" :style="{'height':fixedHeight}">
				</view>
			</template>
		</uni-nav-bar>
	</view>
</template>

<script>
	export default {
		name: "msg-prompt",
		props: {
			fixedHeight: {
				type: String,
				default: '100vh'
			}
		},
		data() {
			return {
				messages: '提示信息提示信息提示信息提示信息提示信息提示信息',
				duration: 2000,
				cusMask: false,
				noticeTitle: 'System prompt information',
				noticeContent: 'The operation is too frequent. Please verify and try again',
				loadingText:'loading...',
			};
		},
		mounted() {
		},
		methods: {
			loadingOpen(text) {
				if(text){
					this.loadingText=text
				}
				this.$refs.loadingPop.open()
			},
			loadingColse() {
				this.$refs.loadingPop.close()
			},
			confirm() {
				console.log("confirm");
				this.noticeColse()
				this.$emit('confirm')
			},
			confirm2() {
				console.log("confirm");
				this.noticeColse2()
				this.$emit('confirm')
			},
			noticeOpen(msg, title,hideMask,callback) {
				if (msg) {
					this.noticeContent = msg
				}
				if (title) {
					this.noticeTitle = title
				}
				this.cusMask = true
				if(hideMask){
					this.cusMask = false
				}
				this.$refs.cliPopup.open()
				if(callback){
					callback()
				}
				
			},
			noticeColse(val) {
				this.cusMask = false
				this.$refs.cliPopup.close()
				if(val){
					this.$emit('closenotice')
				}
			},
			noticeOpen2(msg, title,hideMask,callback) {
				if (msg) {
					this.noticeContent = msg
				}
				if (title) {
					this.noticeTitle = title
				}
				this.cusMask = true
				if(hideMask){
					this.cusMask = false
				}
				this.$refs.cliPopup2.open()
				if(callback){
					callback()
				}
				
			},
			noticeColse2() {
				this.cusMask = false
				this.$refs.cliPopup2.close()
			},
			open(msg, dur) { //消息提示方法
				if (msg != null && msg != undefined) {
					this.messages = msg
				}
				if (dur != null && dur != undefined) {
					this.duration = dur
				}
				this.$forceUpdate()
				this.$refs.popup.open()
				setTimeout(() => {
					this.$refs.popup.close()
				}, this.duration)
			},
			succossOpen(msg, dur) { //消息提示方法
				this.$refs.successPop.open()
				setTimeout(() => {
					this.$refs.successPop.close()
				}, 2000)
			},
			errorOpen(msg, dur) { //消息提示方法
				this.$refs.errorPop.open()
				setTimeout(() => {
					this.$refs.errorPop.close()
				}, 2000)
			}
		}
	}
</script>

<style lang="less" scoped>
	.message_con {
		background-color: #6A7289;
		color: #FFFFFF;
		padding: 16rpx 26rpx;
		line-height: 48rpx;
		font-size: 32rpx;
		border-radius: 10rpx;
		// box-shadow: 0rpx 0rpx 5rpx 2rpx #12192C;
	}

	.cliPopupmask {
		position: fixed;
		top: 0;
		width: calc(100% + 40rpx);
		left: -20rpx;
		z-index: 996;
		background-color: rgba(0, 0, 0, 0.7);
	}

	.loading_con {
		width: 200rpx;
		height: 200rpx;
		border-radius: 10rpx;
		display: flex;
		justify-content: center;
		align-items: center;
		flex-direction: column;
		background-color: rgba(0, 0, 0, 1);

		.image {
			width: 160rpx;
			height: 160rpx;
		}
		.text{
			color: rgba(255, 255, 255, 1);
			font-size: 28rpx;
			margin-top: 24rpx;
		}
		.text1{
			color: rgba(255, 255, 255, 1);
			font-size: 28rpx;
			margin-bottom: 20rpx;
			line-height: 40rpx;
		}
	}
</style>