<template>
	<view>
		<view class="logo_con">
			<image class="logo" src="/static/images/logo.png" mode=""></image>
		</view>
		<uni-forms ref="loginForm" :modelValue="loginForm" :rules="rules" labelWidth='80' label-position="top"
			v-if="loginWay==1">
			<view class="form_con">
				<view class="login_tab">
					<view class="tab_li" style="margin-right:50rpx;" :class="{'active':loginWay==1}"
						@click="setLoginWay(1)">
						Account login
					</view>
					<view class="tab_li" :class="{'active':loginWay==2}" @click="setLoginWay(2)">
						Verification code login
					</view>
				</view>
				<uni-forms-item label="Account" required name="username" id="username_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="username" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="loginForm.username"
							placeholder="Please enter your account" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="Password" required name="password" id="password_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="password" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="password" v-model="loginForm.password"
							placeholder="Please enter the password" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="Verification code" required name="code" id="code_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="code" class="form_li">
						<view class="code_con_left">
							<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
								inputHeight="88rpx" :styles="styles" type="text" v-model="loginForm.code"
								placeholder="Please enter the code" contentFontSize="32rpx"
								primaryColor="rgba(255, 255, 255, 0.5)" />
						</view>
						<view class="code_con_right">
							<image class="code_img" :src="codeUrl" mode="" @click.stop="getCode"></image>
						</view>
					</view>
				</uni-forms-item>
				<view class="remenber">
					<view class="left" @click="changeRemember">
						<view v-if="loginForm.rememberMe" class="icon t-icon-gouxuan t-icon">
						</view>
						<view v-else class="icon t-icon-weixuanzhong t-icon">
						</view>
						<view class="txt">
							Remember password
						</view>
					</view>
					<view class="ForgotPassword" @click="handForget(2,100)">
						Forgot password？
					</view>
				</view>
				<button class="submit_button" @click="submit" :disabled="isLoading" :style="{'opacity':isLoading?0.6:1}"
					:loading="isLoading">
					Log on
				</button>
				<!-- <button class="submit_button" @click="toRegister">
				 Register
				</button> -->
				<view class="zhece_tips" @click.stop="toRegister" v-if="register">
					You don't have an account yet?<view class="link_re">
						Register Now
					</view>
				</view>
			</view>

		</uni-forms>


		<uni-forms ref="loginForm" :modelValue="loginForm" :rules="rules" labelWidth='80' label-position="top"
			v-if="loginWay==2">
			<view class="form_con">
				<view class="login_tab">
					<view class="tab_li" style="margin-right:50rpx;" :class="{'active':loginWay==1}"
						@click="setLoginWay(1)">
						Account login
					</view>
					<view class="tab_li" :class="{'active':loginWay==2}" @click="setLoginWay(2)">
						Verification code login
					</view>
				</view>
				<uni-forms-item label=" E-mail" required name="email" id="username_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="username" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="loginForm.email"
							placeholder="Please enter your e-mail" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" />
					</view>
				</uni-forms-item>
				<uni-forms-item label=" Verification code" required name="VerificationCode" id="password_form"
					labelFont="32rpx" contentFont="32rpx" :requireOpacity="0.5">
					<view id="password" class="form_li">
						<view class="message_con">
							<view class="message_input">
								<uni-easyinput v-model="loginForm.VerificationCode"
									placeholderStyle="color:rgba(255, 255, 255, .20);font-size:32rpx"
									inputHeight="88rpx" :styles="styles" type="text"
									placeholder="Please enter verification code" contentFontSize="32rpx"
									primaryColor="rgba(255, 53, 53, 1)" />
							</view>
							<view class="message_line"></view>
							<view class="code_btn_con">
								<view class="btn_text" v-if="messageDaoji<=0" @click="getMessageCode">
									Get code
								</view>
								<view class="daoji_text" v-if="messageDaoji>0">
									{{messageDaoji}}s
								</view>
							</view>
						</view>

					</view>
				</uni-forms-item>
				<!-- 	<uni-forms-item label=" Graphic verification code" required name="code" id="code_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="code" class="form_li">
						<view class="code_con_left">
							<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
								inputHeight="88rpx" :styles="styles" type="text" v-model="loginForm.code"
								placeholder="Please enter the code" contentFontSize="32rpx"
								primaryColor="rgba(255, 255, 255, 0.5)" />
						</view>
						<view class="code_con_right">
							<image class="code_img" :src="codeUrl" mode="" @click.stop="getCode"></image>
						</view>
					</view>
				</uni-forms-item> -->

				<button style="margin-top:100rpx;" class="submit_button" @click="submitEmils" :disabled="isLoading"
					:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
					Log on
				</button>
				<!-- <button class="submit_button" @click="toRegister">
				 Register
				</button> -->
				<view class="zhece_tips" @click.stop="toRegister" v-if="register">
					You don't have an account yet?<view class="link_re">
						Register Now
					</view>
				</view>
			</view>

		</uni-forms>
		<uni-popup ref="activePopup" type="center" :mask-click="false" :zIndex="999">
			<view class="notice_con">
				<view class="notice_cont">
					<view class="title">Verify</view>
					<view class="content">
						Please log in to the email
						<text class="red_text"> {{this.loginForm.username}} </text>to activate
					</view>
					<button class="submit_button yellow_btn" @click.stop="reSendEmail" v-if="!isDaoji">
						Resend activation email
					</button>
					<button class="jump_button botton_mar" v-if="isDaoji">
						Resend activation email {{daojiNum}}s
					</button>
					<button class="submit_button" @click.stop="confirm">
						Confirm
					</button>
					<view class="close_icon" @click="closeActivePop()">
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
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		getToken,
		setToken,
		removeToken,
		getRefreshToken,
		setRefreshToken,
		removeRefreshToken,
		setStorageKey,
		getStorageKey,
		removeStorageKey
	} from '@/common/auth.js'
	import {
		encrypt,
		decrypt
	} from "@/common/jsencrypt.js";
	// const app = getApp(); // 获取 App 实例
	import {
		getCodeImg,
		getUserInfo,
		sendEmailCode,
		visitorEmailCode,
		loginEmails,
		logout
	} from "@/api/login.js";
	import {
		getConfigKey
	} from '@/api/config.js'
	export default {
		data() {
			return {
				messageDaoji: 0,
				loginForm: {
					email: "",
					password: "",
					rememberMe: false,
					code: "",
					uuid: "",
					VerificationCode: ""
				},
				rules: {
					username: {
						rules: [{
							required: true,
							errorMessage: 'Please enter an account',
						}]
					},
					password: {
						rules: [{
							required: true,
							errorMessage: 'Please enter the password',
						}]
					},
					code: {
						rules: [{
							required: true,
							errorMessage: 'Please enter the code',
						}]
					},
					email: {
						rules: [{
							required: true,
							errorMessage: 'Please enter your e-mail',
						}]
					},
					VerificationCode: {
						rules: [{
							required: true,
							errorMessage: 'Please enter verification code',
						}]
					},

				},
				styles: {
					color: '#fff',
					backgroundColor: '#161A26',
					disableColor: 'rgba(28, 34, 50, 1)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				isLoading: false,
				// 验证码开关
				captchaOnOff: true,
				// 注册开关
				register: false,
				redirect: undefined,
				codeUrl: "",
				// active相关变量
				fixedHeight: '100vh',
				cusMask: false,
				isDaoji: false,
				// active相关变量
				t: '', //保存的页面
				daojiNum: 60,
				options: {},
				loginWay: 1,
				paramType: -1,
			}
		},
		async onLoad(options) {
			// console.log("login登录页面");
			uni.showLoading({
				title: 'Loading'
			})
			//进入登录页就表示离线了
			// try{
			// 	await logout()
			// }catch(e){
			// 	//TODO handle the exception
			// 	uni.hideLoading()
			// }
			this.$store.commit('SET_MESSAGE_INFO', true)
			// #ifdef APP-PLUS
			plus.navigator.closeSplashscreen();
			// #endif
			if (getToken() && getRefreshToken() && !options.noFirt) { //&&options.noFirt有值表示并非第一次进到登录
				try {
					
					if (options.code) {
						if (options.t) {
							this.t = decodeURIComponent(options.t)
						}
						let urlStr = ''
						for (var key in options) {
							if (key != 't') {
								urlStr += '&' + key + '=' + options[key]
							}
						}
						urlStr = '?' + urlStr.substring(1)
						uni.redirectTo({
							url: '/' + this.t + urlStr
						})
					} else {
						if (this.$store.state.user.roles && this.$store.state.user.roles.length > 0) {} else {
							let rsp = await getUserInfo({
								id: 0
							})
							await this.$store.dispatch('GetInfo')
							this.$store.commit('SET_ROLES', rsp.data.user.roleIds)
						}
						this.$store.commit('orgLis/SET_ORG_LIST', null)
						let list = await this.$store.dispatch("orgLis/setOrgList");
						if (list && list.length > 0) {
							setTimeout(() => {
								uni.switchTab({
									url: '/pages/devices/devices'
								})
								uni.hideLoading()
								return
							}, 500)
						} else {
							uni.navigateTo({
								url: '/page_register/choose_addorg?isAppReg=true'
							})
						}
					}

				} catch (e) {
					//TODO handle the exception
					uni.hideLoading()
				}

			} else {
				uni.hideLoading()
			}
			this.setRegShow()
			// this.$nextTick(()=>{
			// 	this.$refs.popup.open()
			// })
			// getApp().open('err', 10)
			this.getCode();
			this.getCookie();
			if (options.t) {
				this.t = decodeURIComponent(options.t)
			}
			this.options = JSON.parse(JSON.stringify(options))
		},

		methods: {
			getMessageCode() {
				if (!this.loginForm.email || this.loginForm.email == '') {
					uni.showToast({
						title: '请输入邮箱',
						icon: 'none'
					})
					return
				}
				let mobForm = {
					email: this.loginForm.email
				}
				// if(this.mesImgid&&this.mesImgid!=''){
				// 	if(!this.mesImgCode&&this.mesImgCode==''){
				// 		uni.showToast({
				// 			title:'请输入图形验证码',
				// 			icon:'none'
				// 		})
				// 	}
				// 	mobForm.imgid=this.mesImgid
				// 	mobForm.code=this.mesImgCode
				// }
				uni.showLoading({
					title: '发送中'
				})
				visitorEmailCode(mobForm).then(res => {
					uni.showToast({
						title: '发送成功'
					})
					this.showMesImgCode = false
					this.mesImgid = ''
					this.messageDaoji = 120
					let mestimer = setInterval(() => {
						if (this.messageDaoji > 0) {
							this.messageDaoji--
						} else {
							clearInterval(mestimer)
						}
					}, 1000)
					uni.hideLoading()
				}).catch(err => {
					console.log("发送错误", err);
					uni.hideLoading()
					this.setMsgTop(err)
					// if(err.code==2){
					// 	this.showMesImgCode=true
					// 	 this.getMesImgCode()
					// }
				})


			},
			handForget(type, param) {
				this.loginWay = type;
				this.paramType = param;
				//console.log(this.paramType,'this.paramType')
			},
			setLoginWay(type) {
				this.loginWay = type
				this.paramType = -1
				//console.log(this.paramType,'this.paramType')
			},
			setRegShow() {
				getConfigKey("sys.account.registerUser").then(res => {
					this.register = res.data;
					if (res.data == 'false') {
						this.register = false
					}
				}).catch(err => {
					console.log(err, '验证码');
					this.setMsgTop(err)
				});
			},
			toRegister() {
				//跳转去注册
				uni.navigateTo({
					url: '/page_register/daili_register4?isAppReg=true'
				})
			},
			reSendEmail() {
				//重新发送激活邮件
				this.$refs.promptMsg.loadingOpen('Sending...')
				sendEmailCode({
					email: this.loginForm.username
				}).then(res => {
					this.$refs.promptMsg.loadingColse()
					this.isDaoji = true
					this.$refs.promptMsg.open('Successfully sent!', 2000) //提示信息组件
					let timer = setInterval(() => {
						if (this.daojiNum > 0) {
							this.daojiNum--
						} else {
							clearInterval(timer)
							this.isDaoji = false
						}
					}, 1000)
				}).catch(err => {
					this.$refs.promptMsg.loadingColse()
					this.setMsgTop(err)
				})
			},
			openActivePop() {
				//打开邮箱激活弹窗
				this.cusMask = true
				this.$refs.activePopup.open()
			},
			closeActivePop() {
				//关闭邮箱激活弹窗
				this.cusMask = false
				this.$refs.activePopup.close()
			},
			async confirm() {
				let rsp = await getUserInfo({
					id: 0
				})
				let infoRsp = rsp.data.user
				if (!infoRsp.EmailActive) {
					this.$refs.promptMsg.open(
						'The email has not been activated yet. Please go to activate it before proceeding！', 2000)
					this.openActivePop()
				} else {
					this.$store.commit('orgLis/SET_ORG_LIST', null)
					let list = await this.$store.dispatch("orgLis/setOrgList");
					await this.$store.dispatch('GetInfo')
					this.$store.commit('SET_ROLES', rsp.data.user.roleIds)
					if (list && list.length > 0) {
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
								let urlStr = ""
								for (var key in this.options) {
									if (key != 't') {
										urlStr += '&' + key + '=' + this.options[key]
									}
								}
								urlStr = '?' + urlStr.substring(1)
								if (this.t.indexOf('index/login') > -1 || this.t.indexOf('page_register/msg_tips') > -
									1) {
									uni.switchTab({
										url: '/pages/devices/devices'
									})
								} else {
									uni.redirectTo({
										url: '/' + this.t + urlStr
									})
								}

							}
						} else {
							// uni.switchTab({
							// 	url: '/pages/home/home'
							// })
							uni.switchTab({
								url: '/pages/devices/devices'
							})
						}
					} else {
						if (this.options.code) {
							let urlStr = ''
							for (var key in this.options) {
								if (key != 't') {
									urlStr += '&' + key + '=' + this.options[key]
								}
							}
							urlStr = '?' + urlStr.substring(1)
							uni.redirectTo({
								url: '/' + this.t + urlStr
							})
						} else {
							uni.navigateTo({
								url: '/page_register/choose_addorg?isAppReg=true'
							})
						}
					}

				}
			},
			changeRemember(val) {
				this.loginForm.rememberMe = !this.loginForm.rememberMe
				if (val) {
					setStorageKey("username", this.loginForm.username);
					setStorageKey("password", encrypt(this.loginForm.password));
					setStorageKey("rememberMe", this.loginForm.rememberMe);
				} else {
					removeStorageKey("username");
					removeStorageKey("password");
					removeStorageKey("rememberMe");
				}
				this.$forceUpdate()
			},
			getCode() {
				getCodeImg().then(res => {
					this.captchaOnOff =
						res.data.captchaOnOff === undefined ? true : res.data.captchaOnOff;
					if (this.captchaOnOff) {
						this.codeUrl = "data:image/gif;base64," + res.data.img;
						this.loginForm.uuid = res.data.uuid;
					}
				}).catch(err => {
					// console.log(err,'验证码');
					this.setMsgTop(err)
				});
			},
			getCookie() {
				const username = getStorageKey("username");
				const password = getStorageKey("password");
				const rememberMe = getStorageKey("rememberMe");
				this.loginForm = {
					username: username === undefined || !username ? this.loginForm.username : username,
					password: password === undefined || !password ? this.loginForm.password : decrypt(password),
					rememberMe: rememberMe === undefined ? false : Boolean(rememberMe)
				};
			},
			submitEmils() {
				this.$refs.loginForm.validate().then(res => {
					loginEmails({
						email: this.loginForm.email,
						code: this.loginForm.VerificationCode
					}).then(async (res) => {
						if (res.code == 0) {
							setToken(res.data.token)
							setRefreshToken(res.data.refresh_token)
							uni.showToast({
								title: '登录成功！',
								icon: 'none'
							})
							if (this.paramType == 100) {
								uni.navigateTo({
									url: './forgetPass?UpdatePasswordCode=' + res.data.ext_info
										.extObj.UpdatePasswordCode
								})
							} else {
								// console.log(this.$store.state.user.roles,
								// 	'this.$store.state.user.roles');
								// await this.$store.dispatch('getUserInfo')
								this.$store.commit('orgLis/SET_ORG_LIST', null)
								let list = await this.$store.dispatch("orgLis/setOrgList");
								if (list && list.length > 0) {
									setTimeout(() => {
										if (this.t) {
											if (this.t.indexOf('crm/crm') > -1 || this.t
												.indexOf(
													'devices/devices') > -
												1 ||
												this.t
												.indexOf('home/home') > -1 || this.t.indexOf(
													'rules/rules') > -1 ||
												this.t
												.indexOf('profile/profile') > -1) {
												uni.switchTab({
													url: '/' + this.t
												})
											} else {
												let urlStr = ''
												for (var key in this.options) {
													if (key != 't') {
														urlStr += '&' + key + '=' + this
															.options[key]
													}
												}
												urlStr = '?' + urlStr.substring(1)
												if (this.t.indexOf('index/login') > -1 || this
													.t
													.indexOf('page_register/msg_tips') > -1) {
													uni.switchTab({
														url: '/pages/devices/devices'
													})
												} else {
													uni.redirectTo({
														url: '/' + this.t + urlStr
													})
												}
											}
										} else {
											// uni.switchTab({
											// 	url: '/pages/home/home'
											// })
											uni.switchTab({
												url: '/pages/devices/devices'
											})
										}
									}, 500)
								} else {
									if (this.options.code) {
										let urlStr = ''
										for (var key in this.options) {
											if (key != 't') {
												urlStr += '&' + key + '=' + this.options[key]
											}
										}
										urlStr = '?' + urlStr.substring(1)
										uni.redirectTo({
											url: '/' + this.t + urlStr
										})
									} else {
										uni.navigateTo({
											url: '/page_register/choose_addorg?isAppReg=true'
										})
									}
								}
							}

						}
					}).catch(err => {
						this.isLoading = false;
						this.setMsgTop(err)
					})
				})
			},
			submit() {

				this.$refs.loginForm.validate().then(res => {
					if (this.loginForm.rememberMe) {
						setStorageKey("username", this.loginForm.username);
						setStorageKey("password", encrypt(this.loginForm.password));
						setStorageKey("rememberMe", this.loginForm.rememberMe);
					} else {
						removeStorageKey("username");
						removeStorageKey("password");
						removeStorageKey("rememberMe");
					}
					this.isLoading = true
					this.$store
						.dispatch("Login", this.loginForm)
						.then(async (res) => {
							this.$refs.promptMsg.open('Login succeeded', 1000) //提示信息组件
							await this.$store.dispatch('GetInfo')
							// let rsp = await getUserInfo({
							// 	id: 0
							// })
							// console.log(rsp,'用户信息getUserInfo');
							// this.$store.commit('SET_ROLES', rsp.data.user.roleIds)
							// console.log(this.$store.state.user.roles, 'this.$store.state.user.roles');
							// await this.$store.dispatch('getUserInfo')
							this.$store.commit('orgLis/SET_ORG_LIST', null)
							let list = await this.$store.dispatch("orgLis/setOrgList");
							if (list && list.length > 0) {
								setTimeout(() => {
									if (this.t) {
										if (this.t.indexOf('crm/crm') > -1 || this.t.indexOf(
												'devices/devices') > -
											1 ||
											this.t
											.indexOf('home/home') > -1 || this.t.indexOf(
												'message/message') > -1 ||
											this.t
											.indexOf('profile/profile') > -1) {
											uni.switchTab({
												url: '/' + this.t
											})
										} else {
											let urlStr = ''
											for (var key in this.options) {
												if (key != 't') {
													urlStr += '&' + key + '=' + this.options[key]
												}
											}
											urlStr = '?' + urlStr.substring(1)
											if (this.t.indexOf('index/login') > -1 || this.t
												.indexOf('page_register/msg_tips') > -1) {
												uni.switchTab({
													url: '/pages/devices/devices'
												})
											} else {
												uni.redirectTo({
													url: '/' + this.t + urlStr
												})
											}
										}
									} else {
										// uni.switchTab({
										// 	url: '/pages/home/home'
										// })
										uni.switchTab({
											url: '/pages/devices/devices'
										})
									}
								}, 1000)
							} else {
								if (this.options.code) {
									let urlStr = ''
									for (var key in this.options) {
										if (key != 't') {
											urlStr += '&' + key + '=' + this.options[key]
										}
									}
									urlStr = '?' + urlStr.substring(1)
									uni.redirectTo({
										url: '/' + this.t + urlStr
									})
								} else {
									uni.navigateTo({
										url: '/page_register/choose_addorg?isAppReg=true'
									})
								}
							}

						})
						.catch(async (e) => {
							this.isLoading = false;

							if (e.code == 6) {
								//跳转至邮箱注册
								setToken(e.data.token)
								setRefreshToken(e.data.refresh_token)
								if (this.loginForm.username.indexOf('@') > 0) {
									let rsp = await getUserInfo({
										id: 0
									})
									let infoRsp = rsp.data.user
									if (!infoRsp.EmailActive) {
										this.openActivePop()
									}
									return
								}
							} else {
								this.getCode();
								// if (e.message) {
								// 	this.$refs.promptMsg.open(e.message, 2000) //提示信息组件
								// }
								this.setMsgTop(e) //提示
							}
						});
				})
			}
		}
	}
</script>

<style lang="less" scoped>
	.active {
		font-size: 36rpx;
		color: #fff;
	}

	.login_tab {
		display: flex;
		color: #fff;
		margin-bottom: 72rpx;
		font-size: 32rpx;
		color: rgba(255, 255, 255, .4);
	}

	.ForgotPassword {
		margin-left: auto;
		color: rgba(153, 153, 153, 1);
		font-size: 28rpx;
		text-decoration: underline;
	}

	// .message_con {
	// 	max-width: 450rpx;
	// 	line-height: 48rpx;
	// 	border-radius: 10rpx;
	// }
	.message_con {
		width: 100%;
		background-color: rgba(22, 26, 38, 1);
		display: flex;
		justify-content: space-between;
		align-items: center;
		border-radius: 10rpx;
		border: 1rpx solid rgba(255, 255, 255, 0.20);

		.message_line {
			width: 1rpx;
			height: 30rpx;
			background-color: rgba(193, 193, 193, 0.20);
		}

		.message_input {
			width: 469rpx;
		}

		/deep/.is-input-border {
			border: none !important;
		}

		.code_btn_con {
			width: 200rpx;
			text-align: center;
			font-size: 28rpx;
			color: rgba(255, 53, 53, 1);

			.btn_text {
				font-size: 32rpx;
			}

			.daoji_text {
				color: rgba(193, 193, 193, 1);
			}
		}
	}
</style>