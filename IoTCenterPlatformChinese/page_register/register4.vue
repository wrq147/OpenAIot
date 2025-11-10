<template>
	<view style="min-height: 100vh;background-color: #ffffff;">
		<top title=" " leftWidth="157rpx" leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#fff"></top>
		<view class="register_con">
			<!-- <view class="title_text">欢迎来到{{appName}}!</view> -->
			<view class="text_tips" style="margin-top: 154rpx;">请设置密码</view>
			<view class="password_input">
				<view class="pass_li">
					<uni-easyinput placeholderStyle="color:#999999;font-size:32rpx" inputHeight="88rpx" :styles="styles"
						type="password" v-model="password" placeholder="请输入密码" contentFontSize="32rpx"
						primaryColor="#2371FF" />
				</view>
				<view class="pass_li">
					<uni-easyinput placeholderStyle="color:#999999;font-size:32rpx" inputHeight="88rpx" :styles="styles"
						type="password" v-model="lastpassword" placeholder="请再次输入密码" contentFontSize="32rpx"
						primaryColor="#2371FF" />
				</view>
			</view>
			<button class="submit_button" @click="nextClick" :disabled="isLoading" :style="{'opacity':isLoading?0.6:1}"
				:loading="isLoading">
				确认注册
			</button>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		getCodeImg,
		emailReg,
		phoneReg
	} from "@/api/login";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	import {
		setToken,
		setRefreshToken,
	} from '@/common/auth.js'
	export default {
		data() {
			return {
				isLoading: false,
				password: '',
				lastpassword: '',
				styles: {
					color: '#333',
					backgroundColor: '#F8F8F8',
					disableColor: 'rgba(28, 34, 50, 0.5)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				basicInfo: {},
				yaoqingId: null, //代理商邀请码
				str: '', //链接上加上代理商邀请码
				isAppReg: false
			};
		},
		computed: {
			appName() {
				return this.$store.state.appNameText
			},
		},
		onLoad(options) {
			this.str = ''
			if (options.register) {
				this.basicInfo = JSON.parse(options.register)
				const eventChannel = this.getOpenerEventChannel();
				// 监听acceptDataFromOpenerPage事件，获取上一页面通过eventChannel传送到当前页面的数据
				eventChannel.on('acceptDataFromOpenerPage', (data) => {
					// console.log(data)
					this.basicInfo.avatar = data.avatar
					this.basicInfo.code = data.code
					if (data.isAppReg) {
						this.isAppReg = true
						this.str += '&isAppReg=' + this.isAppReg
					}
				})

			}
			if (options.yaoqingId) {
				this.yaoqingId = options.yaoqingId
				this.str += "&yaoqingId=" + this.yaoqingId
			}
		},
		methods: {
			setPhoneReg() {
				// 手机号注册
				if (this.password && this.lastpassword && (this.password == this.lastpassword)) {
					this.isLoading = true
					phoneReg({
						code: undefined,
						avatar: this.basicInfo.avatar,
						realName: this.basicInfo.realName,
						mobile: this.basicInfo.mobile,
						email: '',
						password: this.password,
						deptId: this.basicInfo.deptId,
						postName: this.basicInfo.postName,
						mobileCode: this.basicInfo.mobileCode
					}).then(async res => {
						// console.log("手机号注册完成", res);
						setToken(res.data.token);
						setRefreshToken(res.data.refresh_token);
						this.$refs.promptMsg.succossOpen()
						if (this.yaoqingId) {
							uni.reLaunch({
								url: '/page_register/daili_register3?yaoqingId=' + this.yaoqingId
							})
						}
						if (this.basicInfo.code) {
							// setTimeout(() => {
							// 	uni.reLaunch({
							// 		url: '/pages/index/index'
							// 	})
							// }, 1500)
							uni.navigateTo({
								url: '/page_register/register1?code=' + this.basicInfo.code
							})
						}
						if (this.isAppReg) {
							this.$store.commit('orgLis/SET_ORG_LIST', null)
							let list = await this.$store.dispatch("orgLis/setOrgList");
							// console.log("登入用户企业信息",list);
							if (list && list.length > 0) {
								await this.$store.dispatch('GetInfo')
								if (this.t) {
									if (this.t.indexOf('crm/crm') > -1 || this.t.indexOf('devices/devices') > -1 ||
										this.t
										.indexOf('index/index') > -1 || this.t.indexOf('rules/rules') > -1 ||
										this.t
										.indexOf('profile/profile') > -1) {
										uni.switchTab({
											url: '/' + this.t
										})
									} else {
										let pages = getCurrentPages();
										let route = pages[pages.length - 1].route;
										let options = pages[pages.length - 1].options;
										let urlStr = ""
										for (var key in options) {
											if (key != 't') {
												urlStr += '&' + key + '=' + options[key]
											}
										}
										urlStr = '?' + urlStr.substring(1)
										// console.log("urlStr", urlStr);
										uni.redirectTo({
											url: '/' + this.t + urlStr
										})
									}
								} else {
									// uni.switchTab({
									// 	url: '/pages/home/home'
									// })
									uni.switchTab({
										url: this.$store.state.homejumpurl?this.$store.state.homejumpurl:'/pages/index/index'
									})
								}
							} else {
								uni.navigateTo({
									url: '/page_register/choose_addorg?isAppReg=true'
								})
							}
						}

					}).catch(err => {
						this.isLoading = false
						this.setMsgTop(err)
						if (err.code == 104 || err.code == 102) {
							uni.navigateBack()
						}
					})
				} else {
					if (!(this.password == this.lastpassword)) {
						this.$refs.promptMsg.open('两次填写的密码需要一致', 2000)
					}
					if (!this.lastpassword) {
						this.$refs.promptMsg.open('请再次输入密码', 2000)
					}
					if (!this.password) {
						this.$refs.promptMsg.open('请输入密码', 2000)
					}

				}
			},
			setEmailReg() {
				// 邮箱注册
				if (this.password && this.lastpassword && (this.password == this.lastpassword)) {
					this.isLoading = true
					emailReg({
						code: undefined,
						avatar:this.basicInfo.avatar,
						realName: this.basicInfo.realName,
						mobile: '',
						email: this.basicInfo.email,
						password: this.password,
						deptId: this.basicInfo.deptId,
						postName: this.basicInfo.postName
					}).then(res => {
						// console.log("邮箱注册完成", res);
						setToken(res.data.token);
						setRefreshToken(res.data.refresh_token);
						this.$refs.promptMsg.succossOpen()
						setTimeout(() => {
							if (this.yaoqingId) {
								uni.reLaunch({
									url: '/page_register/email_active?yaoqingId=' + this.yaoqingId
								})
								// uni.reLaunch({
								// 	url: '/page_register/daili_register3?yaoqingId=' + this
								// 		.yaoqingId
								// })
							}
							if (this.isAppReg) {
								uni.reLaunch({
									url: '/page_register/email_active?isAppReg=' + this.isAppReg
								})
							}
							if (this.basicInfo.code) {
								uni.reLaunch({
									url: '/page_register/email_active?code=' + this.basicInfo.code
								})
							}
						}, 2000)
					}).catch(err => {
						this.isLoading = false
						this.setMsgTop(err)
					})
				} else {
					if (!(this.password == this.lastpassword)) {
						this.$refs.promptMsg.open('两次填写的密码需要一致', 2000)
					}
					if (!this.lastpassword) {
						this.$refs.promptMsg.open('请再次输入密码', 2000)
					}
					if (!this.password) {
						this.$refs.promptMsg.open('请输入密码', 2000)
					}

				}
			},
			nextClick() {
				// console.log("The passwords filled in twice need to be consistent");
				if (this.basicInfo.isEmailReg) {
					this.setEmailReg()
				} else {
					this.setPhoneReg()
				}

			}
		}
	}
</script>

<style lang="less" scoped>
	.password_input {
		margin-top: 8rpx;
		width: 100%;

		.pass_li {
			margin-top: 20rpx;
		}
	}
</style>