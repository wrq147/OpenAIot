<template>
	<view style="min-height: 100vh;background-color: #ffffff;">
		<!-- <view class="logo_con login_logo onlylogo">
			<image class="logo" :src="appLogo" mode="heightFix"></image>
			<view class="logo_text">
				{{appName}}
			</view>
		</view> -->
		<!-- <view class="logo_bg" :style="{'height':Number(statusBarHeight*2)+512+'rpx'}"></view> -->
		
		<uni-forms ref="loginForm" :modelValue="loginForm" :rules="rules" labelWidth='80' label-position="top">
			<view class="form_con login_form" :style="{'top':Number(statusBarHeight*2)+128+'rpx'}">
				<view class="login_tab">
					<view class="tab_li active">
						重置密码
					</view>
				</view>
				<uni-forms-item label="密码" required name="password" id="username_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="username" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="password" v-model="loginForm.password"
							placeholder="请输入密码" contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="确认密码" required name="newPassword" id="password_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="password" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="password" v-model="loginForm.newPassword"
							placeholder="请输入确认密码" contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)" />
					</view>
				</uni-forms-item>
				<button class="submit_button" style="margin-top: 120rpx;" @click="submit" :disabled="isLoading" :style="{'opacity':isLoading?0.6:1}"
					:loading="isLoading">
					确定
				</button>
			</view>
		</uni-forms>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		getToken,
		setToken,
	} from '@/common/auth.js'
	// const app = getApp(); // 获取 App 实例
	import {
		forgetNewPass
	} from "@/api/login";

	import serverUrl from '@/common/constVar.js'
	export default {
		data() {
			return {
				statusBarHeight: uni.getSystemInfoSync().statusBarHeight,
				loginForm: {
					username: "",
					password: "",
					rememberMe: false,
					code: "",
					uuid: ""
				},
				rules: {
					newPassword: {
						rules: [{
							required: true,
							errorMessage: '请再次确认密码',
						}]
					},
					password: {
						rules: [{
							required: true,
							errorMessage: '请输入密码',
						}]
					},

				},
				styles: {
					color: '#333',
					backgroundColor: 'rgba(248, 248, 248, 1)',
					disableColor: 'rgba(248, 248, 248, 0.5)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				isLoading: false,
			}
		},
		computed: {
			appLogo() {
				return this.$store.state.appLogoUrl
			},
			appName() {
				return this.$store.state.appNameText
			},
		},
		async onLoad(options) {

		},

		methods: {
			submit() {
				this.$refs.loginForm.validate().then(res => {
					if (this.loginForm.password != this.loginForm.newPassword) {
						uni.showToast({
							title: '两次密码输入不一致！',
							icon: 'none'
						})
						return
					}

					forgetNewPass({
						newPassword: this.loginForm.newPassword,
						code: this.$route.query.UpdatePasswordCode
					}).then(async (res) => {
						if (res.code == 0) {
							uni.showToast({
								title: '修改成功',
								icon: 'none',
								duration:2000
							})
							await this.$store.dispatch('GetInfo')
							this.$store.commit('orgLis/SET_ORG_LIST', null)
							setTimeout(()=>{
								uni.switchTab({
									url: this.$store.state.homejumpurl?this.$store.state.homejumpurl:'/pages/index/index'
								})
							},2000)
						}
					}).catch((err) => {
						uni.showToast({
							title: err,
							icon: 'none'
						})

					})

				})
			}
		}
	}
</script>

<style lang="less" scoped>
	.ForgotPassword {
		margin-left: auto;
		color: rgba(153, 153, 153, 1);
		font-size: 28rpx;
	}

	.message_con {
		width: 100%;
		background-color: rgba(248, 248, 248, 1);
		display: flex;
		justify-content: space-between;
		align-items: center;
		border-radius: 10rpx;

		.message_line {
			width: 1rpx;
			height: 30rpx;
			background-color: rgba(193, 193, 193, 1);
		}

		.message_input {
			width: 469rpx;
		}

		.code_btn_con {
			width: 200rpx;
			text-align: center;
			font-size: 28rpx;
			color: rgba(35, 113, 255, 1);

			.daoji_text {
				color: rgba(193, 193, 193, 1);
			}
		}
	}
</style>