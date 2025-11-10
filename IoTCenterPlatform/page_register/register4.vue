<template>
	<view>
		<top title=" " leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" :isleftBack="true"
			backgroundColor="#161A26"></top>
		<view class="register_con">
			<view class="title_text">Welcome!</view>
			<view class="text_tips">Enter your password</view>
			<view class="password_input">
				<view class="pass_li">
					<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx" inputHeight="88rpx"
						:styles="styles" type="password" v-model="password" placeholder="Please enter your password"
						contentFontSize="32rpx" primaryColor="rgba(255, 255, 255, 0.5)" />
				</view>
				<view class="pass_li">
					<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx" inputHeight="88rpx"
						:styles="styles" type="password" v-model="lastpassword"
						placeholder="Please enter your password again" contentFontSize="32rpx"
						primaryColor="rgba(255, 255, 255, 0.5)" />
				</view>
			</view>
			<button class="submit_button" @click="nextClick" :disabled="isLoading" :style="{'opacity':isLoading?0.6:1}"
				:loading="isLoading">
				Complete
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
					color: '#fff',
					backgroundColor: '#161A26',
					disableColor: 'rgba(28, 34, 50, 1)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				basicInfo: {},
				yaoqingId: null, //代理商邀请码
				str: '', //链接上加上代理商邀请码
				isAppReg: false
			};
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
						await this.$store.dispatch('GetInfo')
						if (this.yaoqingId) {
							uni.reLaunch({
								url: '/page_register/daili_register3?yaoqingId=' + this.yaoqingId
							})
						}
						if (this.basicInfo.code) {
							// setTimeout(() => {
							// 	uni.reLaunch({
							// 		url: '/pages/devices/devices'
							// 	})
							// }, 1500)
							uni.navigateTo({
								url:'/page_register/register1?code='+this.basicInfo.code
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
										.indexOf('home/home') > -1 || this.t.indexOf('message/message') > -1 ||
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
									uni.switchTab({
										url: '/pages/home/home'
									})
									// uni.switchTab({
									// 	url: '/pages/index/index'
									// })
								}
							} else {
								uni.reLaunch({
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
						this.$refs.promptMsg.open('The passwords filled in twice need to be consistent', 2000)
					}
					if (!this.lastpassword) {
						this.$refs.promptMsg.open('Please enter your password again', 2000)
					}
					if (!this.password) {
						this.$refs.promptMsg.open('Please enter your password', 2000)
					}

				}
			},
			setEmailReg() {
				// 邮箱注册
				if (this.password && this.lastpassword && (this.password == this.lastpassword)) {
					this.isLoading = true
					emailReg({
						code: undefined,
						avatar: decodeURIComponent(this.basicInfo.avatar),
						realName: this.basicInfo.realName,
						mobile: '',
						email: this.basicInfo.email,
						password: this.password,
						deptId: this.basicInfo.deptId,
						postName: this.basicInfo.postName
					}).then(async res => {
						// console.log("邮箱注册完成", res);
						setToken(res.data.token);
						setRefreshToken(res.data.refresh_token);
						this.$refs.promptMsg.succossOpen()
						await this.$store.dispatch('GetInfo')
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
									url: '/page_register/email_active?isAppReg=' + this
										.isAppReg
								})
							}
							if (this.basicInfo.code) {
								uni.reLaunch({
									url: '/page_register/email_active?code='+this.basicInfo.code
								})
							}
						}, 2000)
					}).catch(err => {
						this.isLoading = false
						this.setMsgTop(err)
					})
				} else {
					if (!(this.password == this.lastpassword)) {
						this.$refs.promptMsg.open('The passwords filled in twice need to be consistent', 2000)
					}
					if (!this.lastpassword) {
						this.$refs.promptMsg.open('Please enter your password again', 2000)
					}
					if (!this.password) {
						this.$refs.promptMsg.open('Please enter your password', 2000)
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