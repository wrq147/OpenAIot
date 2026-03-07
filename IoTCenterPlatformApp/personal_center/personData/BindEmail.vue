<template>
	<view class="BindEmail">
		<top leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#ffffff" title="绑定邮箱" class="CRM-header"></top>
		<uni-forms ref="loginForm" :modelValue="loginForm" :rules="rules" labelWidth="80" label-position="top"
			style="width: 92%; margin: 50rpx 4%">
			<uni-forms-item required label="邮箱" name="email" class="addPool-form-item">
				<input v-model="loginForm.email" type="text" class="addPool-easyinput" placeholder="请输入邮箱"
					placeholder-style="color:#C1C1C1;" />
			</uni-forms-item>

			<uni-forms-item required label="验证码" name="code" class="addPool-form-item">
				<view class="BindPhoneNumber-text">
					<input class="BindPhoneNumber-text-input" v-model="loginForm.code" type="text" placeholder="请输入验证码"
						placeholder-style="color:#C1C1C1;" />
					<button :disabled="disabled" class="GetCode" @click="handGetCode">
						{{ getCodeText }}
					</button>
				</view>
			</uni-forms-item>
			<button class="Confirm" :disabled="isSubmit" @click="handSubmit"
				:class="[loginForm.email && loginForm.code ? 'ConfirmActive' : 'ConfirmOn']">
				保存
			</button>
		</uni-forms>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		sendMobileCode,
		sendImgCode,
		bindEmail,
		sendEmailCode
	} from '@/api/personalCenter';
	export default {
		data() {
			return {
				isSubmit: false,
				hideCode: false,
				getCodeText: '获取验证码',
				getCodeBtnColor: '#ffffff',
				getCodeisWaiting: false,
				disabled: false,
				loginForm: {
					email: '',
					code: '',
				},
				rules: {
					email: {
						rules: [{
							required: true,
							errorMessage: '请输入邮箱',
						}, ],
					},
					code: {
						rules: [{
							required: true,
							errorMessage: '请输入验证码',
						}, ],
					},
				},
				uuid: '',
				imgImg: '',
			};
		},
		onLoad(Option) {
			if (Option.email) {
				this.loginForm.email = Option.email;
			}
		},
		methods: {
			handSubmit() {
				this.$refs.loginForm
					.validate()
					.then((res) => {
						bindEmail({
								note: this.loginForm.code,
							})
							.then((response) => {
								if (response.code == 0) {
									uni.showToast({
										title: '绑定成功！',
										icon: 'none',
									});
									setTimeout(() => {
										uni.navigateTo({
											url: './personData',
										});
									}, 500);
								}
							})
							.catch((err) => {
								this.setMsgTop(err);
							});
					})
					.catch((err) => {});
			},

			handGetCode() {
				// console.log(this.uuid);
				// return
				if (this.loginForm.email == '') {
					uni.showToast({
						title: '请输入您的邮箱！',
						icon: 'none',
					});
					return;
				}
				//if(this.uuid){
				sendEmailCode({
						email: this.loginForm.email,
					})
					.then((res) => {
						if (res.code == 0) {
							this.disabled = true;
							this.getCodeText = '发送...'; //发送验证码
							this.getCodeisWaiting = true;
							this.getCodeBtnColor = 'rgba(255,255,255,0.5)'; //追加样式，修改颜色
							//示例用定时器模拟请求效果
							//setTimeout(()用于在指定的毫秒数后调用函数或计算表达式
							setTimeout(() => {
								//this.$common.msg('验证码已发送')
								uni.showToast({
									title: '验证码已发送',
									icon: 'none',
								}); //弹出提示框
								this.setTimer(); //调用定时器方法
							}, 1000);
						}
					})
					.catch((err) => {
						this.setMsgTop(err);
					});
				//}
			},
			setTimer() {
				let holdTime = 120; //定义变量并赋值
				this.getCodeText = '120s';
				//setInterval（）是一个实现定时调用的函数，可按照指定的周期（以毫秒计）来调用函数或计算表达式。
				//setInterval方法会不停地调用函数，直到 clearInterval被调用或窗口被关闭。
				this.Timer = setInterval(() => {
					if (holdTime <= 0) {
						this.disabled = false;
						this.getCodeisWaiting = false;
						this.getCodeBtnColor = '#ffffff';
						this.getCodeText = '发送验证码';
						clearInterval(this.Timer); //清除该函数
						return; //返回前面
					}
					this.getCodeText = holdTime + 's';
					holdTime--;
				}, 1000);
			},
		},
	};
</script>
<style>
	page {
		background: #ffffff;
	}
</style>
<style lang="less" scoped>
	uni-button:after {
		border: none !important;
	}

	.BindEmail {
		.BindPhoneNumber-text {
			display: flex;
			background: #f8f8f8 !important;
			border: 1rpx solid rgba(255, 255, 255, 0.2);
			border-radius: 12rpx;
			font-size: 30rpx;
			height: 88rpx;
			color: #333;
			// padding:0rpx 20rpx;
			align-items: center;

			.GetCode {
				color: #2371ff;
				padding-left: 20rpx;
				margin-left: auto;
				font-size: 28rpx;
				line-height: 38rpx;
				outline: none;
				width: 300rpx;
				background: none;
				text-align: center;
			}

			.BindPhoneNumber-text-input {
				width: 500rpx;
				padding-right: 30rpx;
				border-right: 1rpx solid rgba(255, 255, 255, 0.2);
				padding-left: 20rpx;
			}
		}

		.Confirm {
			width: 92%;
			margin-top: 46rpx;
			height: 100rpx;
			line-height: 100rpx;
			text-align: center;
			border-radius: 8rpx;
			font-size: 32rpx;
		}

		.ConfirmActive {
			background: #2371ff;
			color: #fff;
		}

		.ConfirmOn {
			background: #f5f5f5;
			color: #999;
		}

		.addPool-form-item {
			margin-top: 46rpx;
			margin-bottom: 35rpx;

			.addPool-textarea {
				font-size: 28rpx;
				border: 1rpx solid rgba(255, 255, 255, 0.2);
				border-radius: 12rpx;
				padding: 20rpx;
				height: 120rpx;
				width: 94%;
				color: #fff;
				margin-top: 20rpx;
			}

			.addPool-selected {
				::v-deep.uni-select__input-text {
					color: #fff !important;
				}

				::v-deep.uni-select {
					height: 88rpx !important;
					border: 1rpx solid rgba(255, 255, 255, 0.2);
				}

				::v-deep.uni-select__input-placeholder {
					font-size: 30rpx !important;
					color: rgba(255, 255, 255, 0.2) !important;
				}
			}

			::v-deep.uni-forms-item__label {
				height: 50rpx;
				color: rgba(255, 255, 255, 0.7);
			}

			::v-deep.uni-forms-item {}

			.addPool-easyinput {
				background: #f8f8f8 !important;
				border: 1rpx solid rgba(255, 255, 255, 0.2);
				border-radius: 12rpx;
				font-size: 30rpx;
				height: 88rpx;
				color: #333;
				padding: 0rpx 20rpx;
			}
		}

		.imgImg {
			width: 224rpx;
			height: 88rpx;
			margin-left: auto;
		}
	}
</style>