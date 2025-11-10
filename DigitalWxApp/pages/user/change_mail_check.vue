<template>
	<view>
		<view class="wrap">
			<view class="tips">
				绑定邮件已发送到{{email}}
			</view>
			<view class="row">
				<view class="right">
					<!-- <input type="text" placeholder="请输入验证码" /> -->
					<uni-easyinput class="uni-mt-5" trim="all" maxlength="10" v-model="codeValue"
						:inputBorder='showBorder' placeholder="验证码"></uni-easyinput>

					<wh-captcha ref="captcha" :secord="30" title="获取验证码" waitTitle="SECORD秒"
						normalClass="captcha-normal" disabledClass="captcha-disabled" @click="getCaptcha"></wh-captcha>
				</view>
			</view>
			<view class="common_btn" @click="bindEmail">
				完成绑定
			</view>
		</view>
	</view>
</template>

<script>
	import whCaptcha from '../../components/wh-captcha/wh-captcha.vue';
	import test from '@/common/ys-validate.js'
	import {
		reloadPrePage
	} from '@/common/util.js'
	import {
		getEmailCode,
		bindMailbox
	} from '@/api/email.js'
	export default {
		data() {
			return {
				check: null,
				isShowClear: false, //是否显示输入框删除标签
				showBorder: false,
				email: null,
				codeValue: '' //验证码
			}
		},
		onLoad(options) {
			if (options.email) {
				this.email = options.email
			}
			if (this.$refs.captcha.canSend()) {
				this.$refs.captcha.begin()
			}
		},
		methods: {
			inputFocus() {
				//获取焦点
				this.isShowClear = true
			},
			inputBlur() {
				//失去焦点
				this.isShowClear = false
			},
			async getCaptcha() { //点击验证码后再次发送验证码
				let rsp = await getEmailCode({
					email: this.email
				})
				if (rsp.code == 0) {
					if (this.$refs.captcha.canSend()) {
						this.$refs.captcha.begin()
					}
				}
			},
			async bindEmail() {
				//绑定邮箱
				let res = await bindMailbox({
					note: this.codeValue
				})
				if(res.code==0){
					setTimeout(() => {
						uni.showToast({
							icon: 'success',
							title: '绑定成功'
						});
					}, 200)
					reloadPrePage(2,'email')
					setTimeout(()=>{
						uni.navigateBack({delta:2})
					},200)
				}
			}
		}
	}
</script>

<style lang="scss">
	.wrap {
		width: 100%;

		.tips {
			width: 100%;
			box-sizing: border-box;
			height: 60rpx;
			line-height: 60rpx;
			text-align: left;
			padding-left: 10rpx;
			background-color: #E3F4FE;
			color: #41A9E8;
		}

		.row {
			width: 100%;
			height: 100rpx;
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
				height: 100rpx;
			}

			input::-ms-input-placeholder {
				color: #999999;
			}

			.yzm {
				width: 155rpx;
				height: 60rpx;
				line-height: 60rpx;
				color: #50A6FA;
				border: 1rpx solid #50A6FA;
				font-size: 28rpx;
				text-align: center;
				border-radius: 6rpx;
			}
		}

		.common_btn {
			margin-top: 80rpx;
		}

		// 验证码样式
		.captcha-normal {
			width: 155rpx;
			height: 60rpx;
			line-height: 60rpx;
			color: #50A6FA;
			border: 1rpx solid #50A6FA;
			font-size: 28rpx;
			text-align: center;
			border-radius: 6rpx;
		}

		.captcha-disabled {
			width: 155rpx;
			height: 60rpx;
			line-height: 60rpx;
			color: #50A6FA;
			border: 1rpx solid #50A6FA;
			font-size: 28rpx;
			text-align: center;
			border-radius: 6rpx;
		}

	}
</style>
