<template>
	<view style="min-height: 100vh;background-color: #ffffff;">
		<top title=" " leftWidth="157rpx" leftIcon="icon-fanhui" :isleftBack="true"
			backgroundColor="#ffffff"></top>
		<view class="register_con">
			<!-- <view class="title_text">欢迎来到{{appName}}!</view> -->
			<view class="text_tips" style="margin-top: 154rpx;">{{isPhoneRegister?'请输入您的电话号码':'请输入您的邮箱地址'}}</view>
			<view class="input_li">
				<!-- <view class="li_label">
					<view class="label_text">USA</view>
					<view class="label_icon">
						<custom-icons iconsName="icon-xialajiantou" iconsSize="12rpx"
							iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
					</view>
				</view>
				<view class="line"></view>
				<view class="input_con">
					<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx" inputHeight="88rpx"
						:styles="styles" type="text" v-model="usaNum"
						placeholder="请输入您的电话号码" contentFontSize="32rpx"
						primaryColor="rgba(255, 255, 255, 0.5)" :inputBorder="false"/>
				</view> -->
				<uni-easyinput placeholderStyle="color:#999999;font-size:32rpx" inputHeight="88rpx"
					:styles="styles" type="text" v-model="usaNum"
					:placeholder="isPhoneRegister?'请输入您的电话号码':'请输入您的邮箱地址'"
					contentFontSize="32rpx" primaryColor="#2371FF" />
			</view>
			<view class="remenber">
				<view class="left" @click="changeReadAgreement">
					<!-- <view v-if="readAgreement" class="icon t-icon-gouxuan t-icon">
					</view>
					<view v-else class="icon t-icon-weixuanzhong t-icon">
					</view> -->
					<view v-if="readAgreement" class="icon t-icon-gouxuan1 t-icon">
					</view>
					<custom-icons v-else iconsName="icon-weixuanzhong" iconsSize="32rpx" iconsColor="rgba(153, 153, 153, 1)"></custom-icons>
					<view class="txt" style="">
						我已阅读并同意 <view class="red">《服务协议》</view>
					</view>
				</view>
			</view>
			<button class="submit_button" @click="nextClick" :disabled="isLoading" :style="{'opacity':isLoading?0.6:1}"
				:loading="isLoading">
				下一步
			</button>
			<view class="qiehuan" @click="changeRegisterFun" v-if="isAppReg">
				<view class="qiehuan_text">
					{{isPhoneRegister?'切换到邮箱认证':'切换到手机认证'}}
				</view>
				<view class="qiehuan_icon">
					<custom-icons iconsName="icon-qiehuan" iconsSize="30rpx"
						iconsColor="#2371FF"></custom-icons>
				</view>
			</view>
		</view>
		<uni-popup ref="codePopup" type="center" :mask-click="false" :zIndex="999">
			<view class="notice_con">
				<view class="notice_cont">
					<view class="title">Verify</view>
					<view class="content cusConten">
						操作过于频繁,请验证并重试
					</view>
					<view class="input_con">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="imgCode"
							placeholder="请输入验证码" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" :inputBorder="true" />
					</view>
					<view class="img_con">
						<image class="code_img" :src="codeUrl" mode="" @click.stop="getCode"></image>
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
		<msg-prompt ref="promptMsg" @confirm="msgConfirm"></msg-prompt>
	</view>
</template>

<script>
	import {
		getSMSCodeImg,
		emailReg,
		sendMobileCode
	} from "@/api/login";
	export default {
		data() {
			return {
				codeUrl: '',
				imgCode: '',
				uuid: '',
				fixedHeight: '100vh',
				cusMask: false,
				isPhoneRegister: true,
				isLoading: false,
				usaNum: '', //手机号码或者邮箱
				readAgreement: false,
				styles: {
					color: '#333333',
					backgroundColor: '#F8F8F8',
					disableColor: 'rgba(28, 34, 50, 0.5)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				basicInfo: {},
				yaoqingId: null,
				str: '',
				isAppReg: true, //是否是app直接注册
			};
		},
		computed:{
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
			confirm() {
				this.getPhoneCode()
			},
			getCode() {
				getSMSCodeImg().then(res => {
					// console.log("res", res);
					this.codeUrl = res.data.base64;
					this.uuid = res.data.uuid;
				});
			},
			changeRegisterFun() {
				//切换注册方式
				this.isPhoneRegister = !this.isPhoneRegister
			},
			changeReadAgreement() {
				//设置协议已读
				this.readAgreement = !this.readAgreement
			},
			msgConfirm() {
				this.readAgreement = true
			},
			async getPhoneCode() {
				//获取手机验证码
				if (this.uuid && this.uuid != '') {
					//如果有图形验证码需走需输入图形验证码的那条路
					try {
						let res = await sendMobileCode({
							phone: Number(this.usaNum),
							code: this.imgCode,
							imgid: this.uuid
						});
						if (res.code == 0) {
							this.basicInfo.mobile = Number(this.usaNum)
							uni.navigateTo({
								url: '/page_register/register3?register=' + JSON.stringify(this.basicInfo) +
									this.str,
								success: (res1) => {
									// 通过eventChannel向被打开页面传送数据
									res1.eventChannel.emit('acceptDataFromOpenerPage', {
										avatar: this.basicInfo.avatar,
										code:this.basicInfo.code,
										isAppReg: this.isAppReg
									})
								}
							})
						}
					} catch (error) {
						// console.log("手机验证码错误1", error);
						this.setMsgTop(error)
						if (error.code == 2) {
							this.getCode();
							this.openCodePop()
						}
					}
				} else {
					//如果没有图形验证码只需要传手机号就可以获取验证码
					try {
						let res = await sendMobileCode({
							phone: Number(this.usaNum),
							code: '',
							imgid: ''
						});
						// console.log("短信验证码2", res);
						if (res.code == 0) {
							this.basicInfo.mobile = Number(this.usaNum)
							let basicInfo=JSON.parse(JSON.stringify(this.basicInfo))
							delete basicInfo.code
							uni.navigateTo({
								url: '/page_register/register3?register=' + JSON.stringify(basicInfo) +
									this.str,
								success: (res1) => {
									// 通过eventChannel向被打开页面传送数据
									res1.eventChannel.emit('acceptDataFromOpenerPage', {
										avatar: this.basicInfo.avatar,
										code:this.basicInfo.code,
										isAppReg: this.isAppReg
									})
								}
							})
						}
					} catch (error) {
						// console.log("手机验证码错误", error);
						this.setMsgTop(error)
						if (error.code == 2) {
							this.getCode();
							this.openCodePop()
							return;
						}
					}
				}
			},
			nextClick() {
				//跳转下一步
				if (!this.isPhoneRegister) {
					if (this.usaNum) {
						if (!this.readAgreement) {
							this.$refs.promptMsg.noticeOpen('您是否阅读并同意服务提示', 'notice', true)
						} else {
							this.basicInfo.email = this.usaNum
							this.basicInfo.isEmailReg = true
							let basicInfo=JSON.parse(JSON.stringify(this.basicInfo))
							delete basicInfo.code
							uni.navigateTo({
								url: '/page_register/register4?register=' + JSON.stringify(basicInfo) + this
									.str,
								success: (res1) => {
									// 通过eventChannel向被打开页面传送数据
									res1.eventChannel.emit('acceptDataFromOpenerPage', {
										avatar: this.basicInfo.avatar,
										code:this.basicInfo.code,
										isAppReg: this.isAppReg
									})
								}
							})
						}
					} else {
						this.$refs.promptMsg.open('请输入您的邮箱地址', 2000)
					}
				} else {
					if (this.usaNum) {
						if (!this.readAgreement) {
							this.$refs.promptMsg.noticeOpen('您是否阅读并同意服务提示', '提示', true)
						} else {
							this.getPhoneCode()
							this.basicInfo.isEmailReg = false
						}

					} else {
						this.$refs.promptMsg.open('请输入你的手机号', 2000)
					}
				}


			}
		}
	}
</script>

<style lang="less">

</style>