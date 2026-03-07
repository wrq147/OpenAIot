<template>
	<view style="min-height: 100vh;background-color: #ffffff;">
		<!-- <view class="logo_con login_logo onlylogo">
			<image class="logo" :src="appLogo" mode="heightFix"></image>
			<view class="logo_text">
				{{appName}}
			</view>
		</view> -->
		<!-- <view class="logo_bg" :style="{'height':Number(statusBarHeight*2)+512+'rpx'}"></view> -->
		<uni-forms ref="loginForm" :modelValue="loginForm" :rules="rules" labelWidth='80' label-position="top"
			v-if="loginWay==1">
			<view class="form_con login_form" :style="{'top':Number(statusBarHeight*2)+128+'rpx'}">
				<view class="login_tab">
					<view class="tab_li" :class="{'active':loginWay==2}" @click="setLoginWay(2)">
						手机号登录
					</view>
					<view class="tab_li" :class="{'active':loginWay==1}" @click="setLoginWay(1)">
						账号登录
					</view>
				</view>
				<uni-forms-item label="账号" required name="username" id="username_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="username" class="form_li">
						<uni-easyinput :trim="true" placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="loginForm.username"
							placeholder="请输入账号" contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="密码" required name="password" id="password_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="password" class="form_li">
						<uni-easyinput :trim="true" placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="password" v-model="loginForm.password"
							placeholder="请输入密码" contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="验证码" required name="code" id="code_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5" v-if="captchaOnOff">
					<view id="code" class="form_li">
						<view class="code_con_left">
							<uni-easyinput :trim="true" placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
								inputHeight="88rpx" :styles="styles" type="text" v-model="loginForm.code"
								placeholder="请输入验证码" contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)"
								@confirm="submit" />
						</view>
						<view class="code_con_right">
							<image class="code_img" :src="codeUrl" mode="" @click.stop="getCode"></image>
						</view>
					</view>
				</uni-forms-item>
				<view class="remenber">
					<view class="left" @click="changeRemember">
						<view v-if="loginForm.rememberMe" class="icon t-icon-gouxuan1 t-icon">
						</view>
						<custom-icons v-else iconsName="icon-weixuanzhong" iconsSize="32rpx"
							iconsColor="rgba(153, 153, 153, 1)"></custom-icons>
						<view class="txt">
							记住密码
						</view>
					</view>
					<view class="ForgotPassword" @click="handForget(2,100)">
						忘记密码？
					</view>

				</view>
				<button class="submit_button" @click="submit" :disabled="isLoading" :style="{'opacity':isLoading?0.6:1}"
					:loading="isLoading">
					登录
				</button>
				<!-- <button class="submit_button" @click="toRegister">
				 Register
				</button> -->
				<view class="zhece_tips" @click.stop="toRegister" v-if="register">
					还没有账号？<view class="link_re">
						立即注册
					</view>
				</view>
			</view>
		</uni-forms>
		<uni-forms ref="messageLogin" :modelValue="messageLogin" :rules="mesRules" labelWidth='80' label-position="top"
			v-if="loginWay==2">
			<view class="form_con login_form" :style="{'top':Number(statusBarHeight*2)+128+'rpx'}">
				<view class="login_tab">
					<view class="tab_li" :class="{'active':loginWay==2}" @click="setLoginWay(2)">
						手机号登录
					</view>
					<view class="tab_li" :class="{'active':loginWay==1}" @click="setLoginWay(1)">
						账号登录
					</view>
				</view>
				<uni-forms-item label="手机号" required name="tel" id="tel_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="tel" class="form_li">
						<uni-easyinput :trim="true" placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="messageLogin.tel"
							placeholder="请输入手机号" contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="图形验证码" name="imgCode" id="imgCode_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5" v-if="showMesImgCode">
					<view id="imgCode" class="form_li">
						<view class="code_con_left">
							<uni-easyinput :trim="true" placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
								inputHeight="88rpx" :styles="styles" type="text" v-model="mesImgCode"
								placeholder="请输入图形验证码" contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)" />
						</view>
						<view class="code_con_right">
							<image class="code_img" :src="mesImgCodeUrl" mode="" @click.stop="getMesImgCode"></image>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="短信验证码" required name="code" id="code_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="code" class="form_li">
						<view class="message_con">
							<view class="message_input">
								<uni-easyinput :trim="true" placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
									inputHeight="88rpx" :styles="styles" type="text" v-model="messageLogin.code"
									placeholder="请输入短信验证码" contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)"
									@confirm="messageLoginFun" />
							</view>
							<view class="message_line"></view>
							<view class="code_btn_con">
								<view class="btn_text" v-if="messageDaoji<=0" @click="getMessageCode">
									获取验证码
								</view>
								<view class="daoji_text" v-if="messageDaoji>0">
									{{messageDaoji}}s
								</view>
							</view>
						</view>
					</view>
				</uni-forms-item>
				<view class="remenber">
				</view>
				<button class="submit_button" @click="messageLoginFun" :disabled="isLoading"
					:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
					登录
				</button>
				<!-- <button class="submit_button" @click="toRegister">
				 Register
				</button> -->
				<view class="zhece_tips" @click.stop="toRegister" v-if="register">
					还没有账号？<view class="link_re">
						立即注册
					</view>
				</view>
			</view>
		</uni-forms>
		<uni-popup ref="activePopup" type="center" :mask-click="false" :zIndex="999">
			<view class="notice_con">
				<view class="notice_cont">
					<view class="title">验证</view>
					<view class="content">
						请登录邮箱
						<text class="red_text"> {{this.loginForm.username}} </text>激活
					</view>
					<button class="jump_button botton_mar" @click.stop="reSendEmail" v-if="!isDaoji">
						重新发送激活邮件
					</button>
					<button class="submit_button yellow_btn" v-if="isDaoji">
						重新发送激活邮件 {{daojiNum}}s
					</button>
					<button class="submit_button" @click.stop="confirm">
						确认
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
	// #ifdef H5
	import {
		encrypt,
		decrypt
	} from "@/common/jsencrypt.js";
	// #endif
	// const app = getApp(); // 获取 App 实例
	import {
		getCodeImg,
		getUserInfo,
		sendEmailCode,
		sendMobileCode,
		getSMSCodeImg,
		fromTelLogin,
		logout
	} from "@/api/login.js";
	import {
		getConfigKey
	} from '@/api/config.js'

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
				messageLogin: {
					tel: '',
					code: ''
				},
				mesImgCode: '', //手机号登录图形验证码
				mesImgid: '', //手机号登录图形验证码id
				mesImgCodeUrl: '', //手机号登录图形验证码
				showMesImgCode: false,
				messageDaoji: 0,
				rules: {
					username: {
						rules: [{
							required: true,
							errorMessage: '请输入账号',
						}]
					},
					password: {
						rules: [{
							required: true,
							errorMessage: '请输入密码',
						}]
					},
					code: {
						rules: [{
							required: true,
							errorMessage: '请输入验证码',
						}]
					},
				},
				mesRules: {
					tel: {
						rules: [{
							required: true,
							errorMessage: '请输入手机号',
						}]
					},
					code: {
						rules: [{
							required: true,
							errorMessage: '请输入短信验证码',
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
				// 验证码开关
				captchaOnOff: false,
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
				loginWay: 2,
				paramType: -1,
			}
		},
		async onLoad(options) {
			console.log("登录页面");
			uni.showLoading({
				title: '加载中'
			})
			try {
				// await logout()//进入登录页就表示离线了
				if (getStorageKey('serverUrl') && getStorageKey('serverUrl') != 'http://125.124.98.180:7080'&& getStorageKey('serverUrl') != 'http://iot.wookongcloud.com') {
					serverUrl.setServerUrl(getStorageKey('serverUrl'))
				}
				this.$store.commit('SET_MESSAGE_INFO', true)
				if (getToken() && getRefreshToken() && !options.noFirt) { //&&options.noFirt有值表示并非第一次进到登录
					if (this.$store.state.user.roles && this.$store.state.user.roles.length > 0) {} else {
						let rsp = await getUserInfo({
							id: 0
						})
						await this.$store.dispatch('GetInfo')
						this.$store.commit('SET_ROLES', rsp.data.user.roleIds)
					}
					uni.switchTab({
						url: this.$store.state.homejumpurl?this.$store.state.homejumpurl:'/pages/index/index'
					})
					uni.hideLoading()
					return
				} else {
					uni.hideLoading()
				}
				this.setRegShow()
				// this.$nextTick(()=>{
				// 	this.$refs.popup.open()
				// })
				// getApp().open('err', 10)
				// this.getCode();
				this.getCookie();
				if (options.t) {
					this.t = decodeURIComponent(options.t)
				}
				if (options) {
					this.options = JSON.parse(JSON.stringify(options))
				}

			} catch (e) {
				//TODO handle the exception
				uni.hideLoading()
				this.setRegShow()
				// this.$nextTick(()=>{
				// 	this.$refs.popup.open()
				// })
				// getApp().open('err', 10)
				// this.getCode();
				this.getCookie();
				if (options.t) {
					this.t = decodeURIComponent(options.t)
				}
				if (options) {
					this.options = JSON.parse(JSON.stringify(options))
				}
			}

		},
		// onShow() {
		// 	console.log(this.loginWay,'iiiuuuuuuuu');
		// 	this.loginWay=this.loginWay
		// },
		// onHide() {
		// 	console.log(this.loginWay,'eeeeeeeeeee');
		// },
		onShow() {
			// 当应用显示时，尝试从storage恢复数据
			if (uni.getStorageSync('myloginWay')) {
				this.loginWay = uni.getStorageSync('myloginWay');
				uni.removeStorageSync('myloginWay');
			}
			if (uni.getStorageSync('mymessageLoginForm')) {
				this.messageLogin = uni.getStorageSync('mymessageLoginForm');
				uni.removeStorageSync('mymessageLoginForm');
			}
		},
		onHide() {
			// 当应用隐藏时，将数据保存到storage
			uni.setStorageSync('myloginWay', this.loginWay);
			uni.setStorageSync('mymessageLoginForm', this.messageLogin);
			
		},
		computed: {
			appLogo() {
				return this.$store.state.appLogoUrl
			},
			appName() {
				return this.$store.state.appNameText
			}
		},
		methods: {
			getMesImgCode() {
				//获取短信验证的图形验证码
				getSMSCodeImg().then(res => {
					this.mesImgid = res.data.uuid
					this.mesImgCodeUrl = res.data.base64
				})
			},
			getMessageCode() {
				//获取手机验证码

				if (!this.messageLogin.tel || this.messageLogin.tel == '') {
					uni.showToast({
						title: '请输入手机号',
						icon: 'none'
					})
					return
				}
				let mobForm = {
					phone: this.messageLogin.tel
				}
				if (this.mesImgid && this.mesImgid != '') {
					if (!this.mesImgCode && this.mesImgCode == '') {
						uni.showToast({
							title: '请输入图形验证码',
							icon: 'none'
						})
						return
					}
					mobForm.imgid = this.mesImgid
					mobForm.code = this.mesImgCode
				}
				uni.showLoading({
					title: '发送中'
				})
				sendMobileCode(mobForm).then(res => {
					uni.showToast({
						title: '短信发送成功'
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
					uni.hideLoading()
					this.setMsgTop(err)
					if (err.code == 2) {
						this.showMesImgCode = true
						this.getMesImgCode()
					}
				})
			},
			setLoginWay(type) {
				this.loginWay = type
				this.paramType = -1
			},
			handForget(type, param) {
				this.loginWay = type;
				this.paramType = param;
			},
			setRegShow() {
				getConfigKey("sys.account.registerUser").then(res => {
					// console.log(res,'是否有注册功能');
					this.register = res.data;
					if (res.data == 'true') {
						this.register = true
					}
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
				this.$refs.promptMsg.loadingOpen('发送中...')
				sendEmailCode({
					email: this.loginForm.username
				}).then(res => {
					this.$refs.promptMsg.loadingColse()
					this.isDaoji = true
					this.$refs.promptMsg.open('发送成功!', 2000) //提示信息组件
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
					this.$refs.promptMsg.open('邮箱还未激活，请先激活邮箱！', 2000)
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
								.indexOf('index/index') > -1 || this.t.indexOf('rules/rules') > -1 ||
								this.t
								.indexOf('profile/profile') > -1||this.t.indexOf('index/index2') > -1) {
								if(this.$store.state.homejumpurl&&this.t.indexOf('index/index') > -1){
									uni.switchTab({
										url: '/pages/index/index2'
									})
								}else{
									uni.switchTab({
										url: '/' + this.t
									})
								}
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
										url: this.$store.state.homejumpurl?this.$store.state.homejumpurl:'/pages/index/index'
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
								url: this.$store.state.homejumpurl?this.$store.state.homejumpurl:'/pages/index/index'
							})
						}
					} else {
						if (this.options.code) { //员工邀请时的code传参
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
					// #ifdef H5
					setStorageKey("password", encrypt(this.loginForm.password));
					// #endif
					// #ifndef H5
					setStorageKey("password", this.loginForm.password);
					// #endif
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
					this.captchaOnOff = res.data.captchaOnOff === undefined ? true : res.data.captchaOnOff;
					if (this.captchaOnOff) {
						this.codeUrl = "data:image/gif;base64," + res.data.img;
						this.loginForm.uuid = res.data.uuid;
					}
				});
			},
			getCookie() {
				const username = getStorageKey("username");
				const password = getStorageKey("password");
				const rememberMe = getStorageKey("rememberMe");
				// #ifdef H5
				this.loginForm = {
					username: username === undefined || !username ? this.loginForm.username : username,
					password: password === undefined || !password ? this.loginForm.password : decrypt(password),
					rememberMe: rememberMe === undefined ? false : Boolean(rememberMe)
				};
				// #endif
				// #ifndef H5
				this.loginForm = {
					username: username === undefined || !username ? this.loginForm.username : username,
					password: password === undefined || !password ? this.loginForm.password : password,
					rememberMe: rememberMe === undefined ? false : Boolean(rememberMe)
				};
				// #endif
				
			},
			messageLoginFun() {
				//短信登录
				this.$refs.messageLogin.validate().then(res => {
					this.isLoading = true
					fromTelLogin(this.messageLogin).then(async res => {
						setToken(res.data.token)
						setRefreshToken(res.data.refresh_token)
						await this.$store.dispatch('GetInfo')
						this.$refs.promptMsg.open('登录成功', 1000) //提示信息组件
						if (this.paramType == 100) {
							uni.navigateTo({
								url: '/page_register/forgetPass?UpdatePasswordCode=' + res.data.ext_info
									.extObj.UpdatePasswordCode
							})
						} else {
							this.$store.commit('orgLis/SET_ORG_LIST', null)
							let list = await this.$store.dispatch("orgLis/setOrgList");
							if (list && list.length > 0) {
								setTimeout(() => {
									if (this.t) {
										if (this.t.indexOf('crm/crm') > -1 || this.t.indexOf(
												'devices/devices') > -1 || this.t.indexOf(
												'index/index') > -1 || this.t.indexOf(
												'rules/rules') > -1 || this.t.indexOf(
												'profile/profile') > -1||this.t.indexOf('index/index2') > -1) {
											if(this.$store.state.homejumpurl&&this.t.indexOf('index/index') > -1){
												uni.switchTab({
													url: '/pages/index/index2'
												})
											}else{
												uni.switchTab({
													url: '/' + this.t
												})
											}
										} else {
											let urlStr = ''
											for (var key in this.options) {
												if (key != 't') {
													urlStr += '&' + key + '=' + this.options[
														key]
												}
											}
											urlStr = '?' + urlStr.substring(1)
											if (this.t.indexOf('index/login') > -1 || this.t
												.indexOf('page_register/msg_tips') > -1) {
												uni.switchTab({
													url: this.$store.state.homejumpurl?this.$store.state.homejumpurl:'/pages/index/index'
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
											url: this.$store.state.homejumpurl?this.$store.state.homejumpurl:'/pages/index/index'
										})
									}
								}, 500)
							} else {
								if (this.options.code) { //员工邀请
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

					}).catch(err => {
						this.isLoading = false;
						this.setMsgTop(err)
					})
				})
			},
			submit() {//账号密码登录
				this.$refs.loginForm.validate().then(res => {
					if (this.loginForm.rememberMe) {
						setStorageKey("username", this.loginForm.username);
						// #ifdef H5
						setStorageKey("password", encrypt(this.loginForm.password));
						// #endif
						// #ifndef H5
						setStorageKey("password", this.loginForm.password);
						// #endif
						// setStorageKey("password", encrypt(this.loginForm.password));
						setStorageKey("rememberMe", this.loginForm.rememberMe);
					} else {
						removeStorageKey("username");
						removeStorageKey("password");
						removeStorageKey("rememberMe");
					}
					this.isLoading = true
					this.$store.dispatch("Login", this.loginForm)
						.then(async (res) => {
							await this.$store.dispatch('GetInfo')
							this.$refs.promptMsg.open('登录成功', 1000) //提示信息组件
							this.$store.commit('orgLis/SET_ORG_LIST', null)
							let list = await this.$store.dispatch("orgLis/setOrgList");
							if (list && list.length > 0) {
								setTimeout(() => {
									if (this.t) {
										if (this.t.indexOf('crm/crm') > -1 || this.t.indexOf('devices/devices') > -1 ||this.t.indexOf('index/index') > -1 || this.t.indexOf('rules/rules') > -1 ||this.t.indexOf('profile/profile') > -1||this.t.indexOf('index/index2') > -1) {
											if(this.$store.state.homejumpurl&&this.t.indexOf('index/index') > -1){
												uni.switchTab({
													url: '/pages/index/index2'
												})
											}else{
												uni.switchTab({
													url: '/' + this.t
												})
											}
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
													url: this.$store.state.homejumpurl?this.$store.state.homejumpurl:'/pages/index/index'
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
											url: this.$store.state.homejumpurl?this.$store.state.homejumpurl:'/pages/index/index'
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
								// if (e.message) {
								// 	this.$refs.promptMsg.open(e.message, 2000) //提示信息组件
								// }
								this.setMsgTop(e) //提示
								if(e.code==888){
								  this.captchaOnOff=true
								  this.getCode();
								}else if (this.captchaOnOff) {
								  this.getCode();
								}
							}
						});
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