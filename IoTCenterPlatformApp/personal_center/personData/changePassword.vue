<template>
	<view class="changePassword">
		<top leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#ffffff" title="修改密码" class="CRM-header"></top>
		<uni-forms ref="loginForm" :modelValue="loginForm" :rules="rules" labelWidth="80" label-position="top"
			style="width: 92%; margin: 50rpx 4%">
			<uni-forms-item required label="旧密码" name="oldPassword" class="addPool-form-item">
				<input v-model="loginForm.oldPassword" type="password" class="addPool-easyinput" placeholder="请输入旧密码"
					placeholder-style="color:#C1C1C1;font-size:32rpx;" />
			</uni-forms-item>
			<uni-forms-item required label="新密码" name="newPassword" class="addPool-form-item">
				<input v-model="loginForm.newPassword" type="password" class="addPool-easyinput" placeholder="请输入新密码"
					placeholder-style="color:#C1C1C1;font-size:32rpx;" />
			</uni-forms-item>

			<uni-forms-item required label="确认新密码" name="confirmPassword" class="addPool-form-item">
				<input v-model="loginForm.confirmPassword" type="password" class="addPool-easyinput"
					placeholder="请再次输入新密码" placeholder-style="color:#C1C1C1;font-size:32rpx;" />
			</uni-forms-item>
			<button class="Confirm" :disabled="isSubmit" @click="handSubmit()" :class="[
                    loginForm.oldPassword && loginForm.newPassword && loginForm.confirmPassword
                        ? 'ConfirmActive'
                        : 'ConfirmOn',
                ]">
				确定修改
			</button>
		</uni-forms>

		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		updateUserPwd
	} from '@/api/personalCenter';
	export default {
		data() {
			return {
				isSubmit: false,
				hideCode: false,
				getCodeText: 'Get code',
				getCodeBtnColor: '#ffffff',
				getCodeisWaiting: false,
				disabled: false,
				loginForm: {
					oldPassword: '',
					newPassword: '',
					confirmPassword: '',
				},
				rules: {
					oldPassword: {
						rules: [{
							required: true,
							errorMessage: '请输入旧密码',
						}, ],
					},
					newPassword: {
						rules: [{
								required: true,
								errorMessage: '请输入新密码',
							},
							{
								minLength: 6,
								maxLength: 20,
								errorMessage: '密码长度在 {minLength} 到 {maxLength} 个字符',
							},
						],
					},
					confirmPassword: {
						rules: [{
								required: true,
								errorMessage: '请输入确认新密码',
							},
							{
								minLength: 6,
								maxLength: 20,
								errorMessage: '密码长度在 {minLength} 到 {maxLength} 个字符',
							},
						],
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
						if (this.loginForm.confirmPassword !== this.loginForm.newPassword) {
							uni.showToast({
								icon: 'none',
								title: '两个密码输入不一致',
							});
							return;
						}
						updateUserPwd(this.loginForm.oldPassword, this.loginForm.confirmPassword)
							.then((response) => {
								if (response.code == 0) {
									uni.showToast({
										title: '修改成功！',
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
						title: '请输入您的电话号码！',
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
							this.getCodeText = 'Sending...'; //发送验证码
							this.getCodeisWaiting = true;
							this.getCodeBtnColor = 'rgba(255,255,255,0.5)'; //追加样式，修改颜色
							//示例用定时器模拟请求效果
							//setTimeout(()用于在指定的毫秒数后调用函数或计算表达式
							setTimeout(() => {
								//this.$common.msg('验证码已发送')
								uni.showToast({
									title: 'Verification code has been sent',
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
				let holdTime = 60; //定义变量并赋值
				this.getCodeText = '60s';
				//setInterval（）是一个实现定时调用的函数，可按照指定的周期（以毫秒计）来调用函数或计算表达式。
				//setInterval方法会不停地调用函数，直到 clearInterval被调用或窗口被关闭。
				this.Timer = setInterval(() => {
					if (holdTime <= 0) {
						this.disabled = false;
						this.getCodeisWaiting = false;
						this.getCodeBtnColor = '#ffffff';
						this.getCodeText = 'Get code';
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
	.changePassword {
		.BindPhoneNumber-text {
			display: flex;
			background: #161a26 !important;
			border: 1rpx solid rgba(255, 255, 255, 0.2);
			border-radius: 12rpx;
			font-size: 30rpx;
			height: 88rpx;
			color: #fff;
			// padding:0rpx 20rpx;
			align-items: center;

			.GetCode {
				color: #ff3535;
				padding-left: 20rpx;
				margin-left: auto;
				font-size: 32rpx;
				line-height: 38rpx;
				outline: none;
				width: 200rpx;
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
			margin-top: 40rpx;
			height: 100rpx;
			line-height: 100rpx;
			text-align: center;
			border-radius: 8rpx;
			font-size: 36rpx;
		}

		uni-button:after {
			border: none;
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