<template>
	<view class="register_con">
		<top title="" leftWidth="157rpx" :isleftBack="false" backgroundColor="#fff"></top>
		<view class="register_title_text">注册账号</view>
		<view class="touxiang" @click="getAvatarAfter">
			<view class="image t-icon-morentouxiang" v-if="!basicInfo.avatar"></view>
			<image class="image" :src="basicInfo.avatar" mode="aspectFit" v-if="basicInfo.avatar"></image>
			<view class="up_icons t-icon-shangchuantouxiang1" @click.stop="uploadMul"></view>
		</view>
		<butongAvatar ref="cusAvatar" @getAvatar="getAvatar" :avatar="avatarGroup"></butongAvatar>
		<view class="register_form_con">
			<uni-forms ref="basicInfo" :modelValue="basicInfo" :rules="rules" label-position="top" labelWidth="200rpx">
				<view class="form_con">
					<uni-forms-item label="姓名" required name="realName" id="realName_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5" :isDis="true" :isfirstTop="true">
						<view id="realName" class="form_li">
							<uni-easyinput placeholderStyle="color:#999999;font-size:32rpx" inputHeight="88rpx"
								:styles="styles" type="text" v-model="basicInfo.realName" placeholder="请输入姓名"
								contentFontSize="32rpx" primaryColor="#2371FF" />
						</view>
					</uni-forms-item>
					<uni-forms-item v-if="!basicInfo.isEmailReg" label="手机号" required name="mobile" id="mobile_form"
						labelFont="32rpx" contentFont="32rpx" :requireOpacity="0.5" :isDis="true" :isfirstTop="true">
						<view id="mobile" class="form_li">
							<uni-easyinput placeholderStyle="color:#999999;font-size:32rpx" inputHeight="88rpx"
								:styles="styles" type="text" v-model="basicInfo.mobile" placeholder="请输入您的电话号码"
								contentFontSize="32rpx" primaryColor="#2371FF" />
						</view>
					</uni-forms-item>
					<uni-forms-item v-if="basicInfo.isEmailReg" label="邮箱" required name="email" id="email_form"
						labelFont="32rpx" contentFont="32rpx" :requireOpacity="0.5" :isDis="true" :isfirstTop="true">
						<view id="email" class="form_li">
							<uni-easyinput placeholderStyle="color:#999999;font-size:32rpx" inputHeight="88rpx"
								:styles="styles" type="text" v-model="basicInfo.email" placeholder="请输入您的邮箱"
								contentFontSize="32rpx" primaryColor="#2371FF" />
						</view>
					</uni-forms-item>
					<uni-forms-item v-if="!hasNode" label="短信验证码" required name="mobileCode" id="mobileCode_form"
						labelFont="32rpx" contentFont="32rpx" :requireOpacity="0.5">
						<view id="mobileCode" class="form_li">
							<view class="message_con">
								<view class="message_input">
									<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
										inputHeight="88rpx" :styles="styles" type="text" v-model="basicInfo.mobileCode"
										placeholder="请输入短信验证码" contentFontSize="32rpx"
										primaryColor="rgba(35, 113, 255, 1)" @confirm="messageLoginFun" />
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
					<uni-forms-item label="密码" required name="password" id="password_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5" :isDis="true" :isfirstTop="true">
						<view id="password" class="form_li">
							<uni-easyinput placeholderStyle="color:#999999;font-size:32rpx" inputHeight="88rpx"
								:styles="styles" type="password" v-model="basicInfo.password" placeholder="请输入密码"
								contentFontSize="32rpx" primaryColor="#2371FF" />
						</view>
					</uni-forms-item>
					<view class="remenber">
						<view class="left" @click="changeReadAgreement">
							<!-- <view v-if="readAgreement" class="icon t-icon-gouxuan t-icon">
							</view>
							<view v-else class="icon t-icon-weixuanzhong t-icon">
							</view> -->
							<view v-if="readAgreement" class="icon t-icon-gouxuan1 t-icon">
							</view>
							<custom-icons v-else iconsName="icon-weixuanzhong" iconsSize="32rpx"
								iconsColor="rgba(153, 153, 153, 1)"></custom-icons>
							<view class="txt" style="">
								我已阅读并同意<uni-link style="color: rgba(35, 113, 255, 1);" class="red" href="static/agreement.html">《服务协议》</uni-link>
							</view>
						</view>
					</view>
					<button class="submit_button" @click="nextClick" :disabled="isLoading"
						:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
						{{hasNode?'接受邀请':'注册账号'}}
					</button>
				</view>

			</uni-forms>
		</view>
		<uni-popup ref="codePopup" type="center" :mask-click="false" :zIndex="999">
			<view class="notice_con">
				<view class="notice_cont">
					<view class="title">验证</view>
					<view class="content cusConten">
						操作过于频繁,请验证并重试
					</view>
					<view class="input_con">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="mesImgCode" placeholder="请输入验证码"
							contentFontSize="32rpx" primaryColor="rgba(255, 255, 255, 0.5)" :inputBorder="true" />
					</view>
					<view class="img_con">
						<image class="code_img" :src="mesImgCodeUrl" mode="" @click.stop="getMesImgCode()"></image>
					</view>
					<button class="submit_button" @click.stop="confirm">
						确认
					</button>
					<view class="close_icon" @click="closeCodePop">
						<custom-icons iconsName="icon-guanbidanchuang" iconsSize="36rpx"
							iconsColor="rgba(248, 248, 248, 1)"></custom-icons>
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
		pathToBase64,
		base64ToPath
	} from '@/common/image-tools.js'
	import {
		getCodeImg,
		sendMobileCode,
		emailReg,
		phoneReg,
		agentInviteInfo,
		agentInviteReg
	} from "@/api/login";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	import {
		setToken,
		setRefreshToken,
	} from '@/common/auth.js'
	import butongAvatar from '@/page_register/uni_modules/butong-avatar/components/butong-avatar/butong-avatar.vue'
	export default {
		components:{butongAvatar},
		data() {
			return {
				fixedHeight: '100vh',
				localdata: [{
						text: "测试",
						value: 0
					},
					{
						text: "开发",
						value: 1
					}
				],
				basicInfo: {
					realName: "",
					password: "",
					readAgreement: [], //验证是否同意服务协议
					mobileCode: "",
					deptId: 0,
					mobile: "",
					email: '',
					company: "",
					avatar: "",
					postName: "",
					code: undefined //邀请码
				},
				avatarGroup: [],
				isLoading: false,
				mesImgCode: '', //手机号登录图形验证码
				mesImgid: '', //手机号登录图形验证码id
				mesImgCodeUrl: '', //手机号登录图形验证码
				cusMask: false,
				showMesImgCode: false,
				messageDaoji: 0,
				rules: {
					realName: {
						rules: [{
							required: true,
							errorMessage: '请输入用户名',
						}]
					},
					mobile: {
						rules: [{
							required: true,
							errorMessage: '请输入手机号',
						}]
					},
					mobileCode: {
						rules: [{
							required: true,
							errorMessage: '请输入验证码',
						}]
					},
					email: {
						rules: [{
							required: true,
							errorMessage: '请输入邮箱',
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
					color: '#333333',
					backgroundColor: '#F8F8F8',
					disableColor: '#F7F6F6',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				yaoqingId: null,
				isAppReg: false, //是否是app直接注册
				isFirstLoad: true,
				readAgreement: false, //是否
				inviteInfo: null, //邀请信息
				hasNode: false,
			};
		},
		computed: {
			appName() {
				return this.$store.state.appNameText
			},
		},
		onLoad(options) {
			this.$nextTick(() => {
				this.isFirstLoad = true
				this.getAvatarAfter()
			})
			if (options.node) {
				this.hasNode = true
				this.basicInfo.mobileCode = options.node
			} else {
				this.hasNode = false
			}
			if (options.yaoqingId) {
				this.yaoqingId = options.yaoqingId
				this.loadYaoqingInfo(this.yaoqingId)
			}

			if (options.isAppReg) {
				this.isAppReg = true
			}
		},
		methods: {
			confirm() {
				this.getMessageCode()
			},
			isEmail(str) { //判断是否是邮箱
				const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
				return emailRegex.test(str);
			},
			loadYaoqingInfo(id) {
				agentInviteInfo({
					id: id
				}).then(res => {
					console.log("邀请信息", res);
					let data = res.data
					if (data) {
						this.inviteInfo = data
						this.basicInfo.realName = data.ContactName

						if (this.node) {
							this.basicInfo.isEmailReg = false
							this.basicInfo.mobile = data.Tel
						} else {
							if (this.isEmail(data.Tel)) {
								this.basicInfo.isEmailReg = true
								this.basicInfo.email = data.Tel
							} else {
								this.basicInfo.isEmailReg = false
								this.basicInfo.mobile = data.Tel
							}
						}
					} else {
						this.hasNode = false
						this.basicInfo.mobileCode = ''
					}

				})
			},
			changeReadAgreement() {
				//设置协议已读
				this.readAgreement = !this.readAgreement
			},
			getMesImgCode() {
				//获取短信验证的图形验证码
				getCodeImg().then(res => {
					this.mesImgid = res.data.uuid
					this.mesImgCodeUrl = "data:image/gif;base64," +res.data.img
				})
			},
			openCodePop() {
				//打开验证码弹窗
				this.cusMask = true
				// this.getCode()
				this.$refs.codePopup.open()
			},
			closeCodePop() {
				//关闭验证码弹窗
				this.cusMask = false
				this.$refs.codePopup.close()
			},
			getMessageCode() {
				//获取手机验证码

				if (!this.basicInfo.mobile || this.basicInfo.mobile == '') {
					uni.showToast({
						title: '请输入手机号',
						icon: 'none'
					})
					return
				}
				let mobForm = {
					phone: this.basicInfo.mobile
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
					this.cusMask = false
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
					// this.setMsgTop(err)
					if (err.code == 2) {
						this.openCodePop()
						this.getMesImgCode()
					}else{
						this.setMsgTop(err)
					}
				})
			},
			nextClick() {
				// console.log("The passwords filled in twice need to be consistent");
				if (this.hasNode) {
					this.$refs.basicInfo.validate().then(res => {
						if(!this.readAgreement){
							this.setMsgTop({message:'请确认服务协议'})
							return
						}
						this.isLoading = true
						agentInviteReg({
							avatar: this.basicInfo.avatar,
							realName: this.basicInfo.realName,
							mobile: this.basicInfo.mobile,
							password: this.basicInfo.password,
							deptId: 0,
							email: "",
							postName: "",
							mobileCode: this.basicInfo.mobileCode,
							yqCode: this.yaoqingId
						}).then(async res => {
							if (res.code == 0) {
								this.$modal.msgSuccess("注册成功");
								setToken(res.data.token);
								setRefreshToken(res.data.refresh_token);
								uni.reLaunch({
									url: '/page_register/daili_register3?yaoqingId=' + this.yaoqingId
								})
							}
						}).catch(err => {
							console.log("注册错误", err);
							this.isLoading = false
							this.setMsgTop(err)
							// if (err.code == 104 || err.code == 102) {
							// 	uni.navigateBack()
							// }
						})
					})
				} else {
					if (this.basicInfo.isEmailReg) {
						this.setEmailReg()
					} else {
						this.setPhoneReg()
					}
				}


			},
			setPhoneReg() {
				// 手机号注册
				this.$refs.basicInfo.validate().then(res => {
					if(!this.readAgreement){
						this.setMsgTop({message:'请确认服务协议'})
						return
					}
					this.isLoading = true
					phoneReg({
						code: undefined,
						avatar: this.basicInfo.avatar,
						realName: this.basicInfo.realName,
						mobile: this.basicInfo.mobile,
						email: '',
						password: this.basicInfo.password,
						deptId: this.basicInfo.deptId,
						postName: this.basicInfo.postName,
						mobileCode: this.basicInfo.mobileCode,
					}).then(async res => {
						// console.log("手机号注册完成", res);
						setToken(res.data.token);
						setRefreshToken(res.data.refresh_token);
						this.$refs.promptMsg.succossOpen()
						if (this.yaoqingId) {
							uni.reLaunch({
								url: '/page_register/daili_register3?yaoqingId=' + this
									.yaoqingId
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
									if (this.t.indexOf('crm/crm') > -1 || this.t.indexOf(
											'devices/devices') > -1 ||
										this.t
										.indexOf('index/index') > -1 || this.t.indexOf('rules/rules') >
										-1 ||
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
										url: this.$store.state.homejumpurl ? this.$store.state
											.homejumpurl : '/pages/index/index'
									})
								}
							} else {
								uni.navigateTo({
									url: '/page_register/choose_addorg?isAppReg=true'
								})
							}
						}

					}).catch(err => {
						console.log("注册错误", err);
						this.isLoading = false
						this.setMsgTop(err)
						// if (err.code == 104 || err.code == 102) {
						// 	uni.navigateBack()
						// }
					})
				})
			},
			setEmailReg() {
				// 邮箱注册
				this.$refs.basicInfo.validate().then(res => {
					if(!this.readAgreement){
						this.setMsgTop({message:'请确认服务协议'})
						return
					}
					this.isLoading = true
					emailReg({
						code: undefined,
						avatar: this.basicInfo.avatar,
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
									url: '/page_register/email_active?yaoqingId=' + this
										.yaoqingId
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
									url: '/page_register/email_active?code=' + this
										.basicInfo.code
								})
							}
						}, 2000)
					}).catch(err => {
						this.isLoading = false
						this.setMsgTop(err)
					})
				})
			},
			uploadMul(type) {
				//手动上传图片
				uni.chooseImage({
					count: 1,
					sizeType: ['compressed'], //可以指定是原图还是压缩图，默认二者都有
					success: async (res) => {
						// console.log("上传图片", res);
						if (res.tempFilePaths.length == 0) {
							return;
						}
						for (let i = 0; i < res.tempFiles.length; i++) {
							if (res.tempFiles[i].size > 10 * 1024 * 1024) {
								this.$refs.promptMsg.open('上传图片不能超过10M', 2000)
								return;
							}
							try {
								let paths = res.tempFilePaths[i];
								pathToBase64(paths).then(base64 => {
										// console.log(base64)
										this.basicInfo.avatar = base64
										this.$forceUpdate()
									})
									.catch(error => {
										// console.error(error)
									})

							} catch (e) {
								//TODO handle the exception
								// console.log(e);
							}
						}
					}
				})
			},
			getAvatarAfter() {
				//确定头像
				this.$refs.cusAvatar.randAvatarDelay()
				this.$refs.promptMsg.loadingOpen()
				setTimeout(() => {
					this.$refs.cusAvatar.saveImg()
					this.$forceUpdate()
					if (!this.isFirstLoad) {
						this.$refs.promptMsg.loadingColse()
					}

				}, 1000)
			},
			getAvatar(img) {
				// console.log("头像信息", img);
				this.avatarGroup = img.group

				// this.basicInfo.avatar = img.img
				pathToBase64(img.img).then(base64 => {
						// console.log(base64)
						if (!this.isFirstLoad) {
							this.basicInfo.avatar = base64
						}
						if (this.isFirstLoad) {
							this.getAvatarAfter()
						}
						this.$forceUpdate()
						this.isFirstLoad = false
					})
					.catch(error => {
						console.error(error)
					})

			},
		}
	}
</script>

<style lang="less" scoped>
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

	.title_text {
		color: #fff;
		font-size: 48rpx;
		margin-top: 128rpx;
	}

	.touxiang {
		position: relative;
		margin-top: 80rpx;

		.image {
			width: 160rpx;
			height: 160rpx;
			border-radius: 50%;
		}

		.up_icons {
			position: absolute;
			right: 0;
			bottom: 0;
			width: 50rpx;
			height: 50rpx;
		}
	}

	.company {
		font-size: 32rpx;
		color: rgba(255, 255, 255, 0.5);
		display: flex;
		justify-content: flex-start;
		align-items: center;
		margin-top: 40rpx;

		.name {
			margin-left: 10rpx;
		}
	}

	.register_con {
		padding: 0;
		min-height: 100vh;
		background-color: #FFFFFF;

		.register_title_text {
			padding-left: 40rpx;
			margin-top: 40rpx;
		}
	}

	.register_form_con {
		margin-top: 80rpx;
	}
</style>