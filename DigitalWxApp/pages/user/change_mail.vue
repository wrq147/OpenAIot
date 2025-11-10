<template>
	<view>
		<view class="wrap">
			<view class="row">
				<view class="right">
					<uni-easyinput class="iptTxt" :focus="true" :clearable="isShowClear" trim="both" @focus="inputFocus"
						maxlength="20" v-model="email" :inputBorder="false" placeholder="请输入邮箱地址" @clear="cancel"
						@blur="verifyEmail">
					</uni-easyinput>
				</view>
			</view>
			<view class="common_btn" @click="sentEmailCode">
				绑定
			</view>
		</view>
	</view>
</template>

<script>
	import {
		getEmailCode
	} from '@/api/email.js'
	export default {
		data() {
			return {
				email: null,
				isShowClear: false, //是否显示输入框删除标签
				showBorder: false,
			}
		},
		onLoad(options) {
			if (options.email) {
				this.email = options.email
			}
			if (options.email == 'null') {
				this.email = ''

			}
			// console.log("options外部值",this.email);
		},
		methods: {
			verifyEmail() {
				//验证手机号
				if (!(/^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.[a-zA-Z0-9]{2,6}$/.test(this.email))) {
					uni.showToast({
						title: '邮箱格式错误',
						icon: "none"
					});
					return;
				}
			},
			cancel() {
				// console.log("清除");
				this.email = '';
			},
			inputFocus() {
				//获取焦点
				this.isShowClear = true
			},
			// inputBlur(){
			// 	//失去焦点
			// 	this.isShowClear=false
			// },
			async sentEmailCode() {
				if (this.email == '' || this.email == null) {
					uni.showToast({
						title: "邮箱不能为空",
						icon: "none",
						duration: 1000
					});
					return
				}
				if (!(/^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.[a-zA-Z0-9]{2,6}$/.test(this.email))) {
					uni.showToast({
						title: '邮箱格式错误',
						icon: "none"
					});
					return;
				}
				//发送邮箱验证码
				uni.showLoading({
					title: '正在发送邮件',
					mask: true
				})
				try {

					let rsp = await getEmailCode({
						email: this.email
					})
					// console.log("发送短信至邮箱",rsp);
					if (rsp.code == 0) {
						uni.hideLoading()
						uni.navigateTo({
							url: '/pages/user/change_mail_check?email=' + this.email
						})

					}
				} catch (e) {
					console.log(e);
					uni.hideLoading()
					//TODO handle the exception
				}

			}
		}
	}
</script>

<style lang="scss">
	.wrap {
		width: 100%;

		.row {
			width: 100%;
			height: 120rpx;
			display: flex;
			align-items: center;
			padding: 0 30rpx;
			box-sizing: border-box;
			background-color: #FFFFFF;
			font-size: 30rpx;
			color: #666666;
			border-top: 1rpx solid #F6F6F6
		}

		.row .right {
			display: flex;
			align-items: center;
			width: 100%;

			input {
				display: block;
				outline: none;
				width: 100%;
				height: 120rpx;
			}

			input::-ms-input-placeholder {
				color: #999999;
			}


		}

		.common_btn {
			margin-top: 80rpx;
		}


	}
</style>
