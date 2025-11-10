<template>
	<view>
		<top title=" " leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" :isleftBack="true"
			backgroundColor="#161A26"></top>
		<view class="register_con">
			<view class="title_text">Welcome!</view>
			<view class="text_tips">{{isPhoneRegister?'Enter your phone number':'Enter your e-mail'}}</view>
			<view class="input_li">
				<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx" inputHeight="88rpx"
					:styles="styles" type="text" v-model="usaNum"
					:placeholder="isPhoneRegister?'Please enter your phone':'Please Enter your e-mail'"
					contentFontSize="32rpx" primaryColor="rgba(255, 255, 255, 0.5)" :inputBorder="false" />
			</view>
			<view class="remenber">
				<view class="left" @click="changeReadAgreement">
					<view v-if="readAgreement" class="icon t-icon-gouxuan t-icon">
					</view>
					<view v-else class="icon t-icon-weixuanzhong t-icon">
					</view>
					<view class="txt">
						I have read and agree to the <view class="red">Service Agreement</view>
					</view>
				</view>
			</view>
			<button class="submit_button" @click="nextClick" :disabled="isLoading" :style="{'opacity':isLoading?0.6:1}"
				:loading="isLoading">
				Next
			</button>
			<view class="qiehuan" @click="changeRegisterFun" v-if="isAppReg">
				<view class="qiehuan_text">
					{{isPhoneRegister?'Switch to email verification':'Switch to phone number verification'}}
				</view>
				<view class="qiehuan_icon">
					<custom-icons iconsName="icon-qiehuan" iconsSize="30rpx"
						iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
				</view>
			</view>
		</view>
		<uni-popup ref="codePopup" type="center" :mask-click="false" :zIndex="999">
			<view class="notice_con">
				<view class="notice_cont">
					<view class="title">Verify</view>
					<view class="content cusConten">
						The operation is too frequent. Please verify and try again
					</view>
					<view class="input_con">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="imgCode"
							placeholder="Please enter the verification code" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" :inputBorder="true" />
					</view>
					<view class="img_con">
						<image class="code_img" :src="codeUrl" mode="" @click.stop="getCode"></image>
					</view>
					<button class="submit_button" @click.stop="confirm">
						Confirm
					</button>
					<view class="close_icon" @click="closeCodePop">
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
		<msg-prompt ref="promptMsg" @confirm="msgConfirm"></msg-prompt>
	</view>
</template>

<script>
	import {
		getCodeImg,
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
				isPhoneRegister: false,
				isLoading: false,
				usaNum: '', //手机号码或者邮箱
				readAgreement: false,
				styles: {
					color: '#fff',
					backgroundColor: '#161A26',
					disableColor: 'rgba(28, 34, 50, 1)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				basicInfo: {},
				yaoqingId: null,
				str: '',
				isAppReg: false, //是否是app直接注册
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
			openCodePop() {
				//打开验证码弹窗
				this.cusMask = true
				this.getCode()
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
				getCodeImg().then(res => {
					// console.log("res", res);
					this.codeUrl = "data:image/gif;base64," + res.data.img;
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
							this.$refs.promptMsg.noticeOpen('Do you read and agree to the service prompt', 'notice', true)
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
						this.$refs.promptMsg.open('Please Enter your e-mail', 2000)
					}
				} else {
					if (this.usaNum) {
						if (!this.readAgreement) {
							this.$refs.promptMsg.noticeOpen('Do you read and agree to the service prompt', 'notice', true)
						} else {
							this.getPhoneCode()
							this.basicInfo.isEmailReg = false
						}

					} else {
						this.$refs.promptMsg.open('Please Enter your phone', 2000)
					}
				}


			}
		}
	}
</script>

<style lang="less">

</style>