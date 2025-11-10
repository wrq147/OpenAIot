<template>
	<view>
		<top title=" " leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" :isleftBack="true"
			backgroundColor="#161A26"></top>
		<view class="register_con">
			<view class="title_text">Welcome!</view>
			<view class="text_tips">
				<view class="">
					We have sent the verification code to:
				</view>
				<view class="">
					{{basicInfo.mobile}}
				</view>
			</view>
			<view class="code_con">
				<wolf-phone-input ref="wolfPhoneInput" :inputType="1" :inputCount="6" :rowSpaceWidth="4"
					:inputHeight="50" :inputCircleWidth="1" :inputCircleRadius="6" :numFontSize="22"
					:inputCircleDefaultCol="'rgba(255, 255, 255, 0.20)'" :inputCircleSelectedCol="'#2E9DFF'"
					:inputContentCol="'rgba(255, 255, 255, 1)'" @completeInput="completeInput" />
			</view>
			<view class="daojishi" v-if="!isReSend">
				<view class="">
					Retrieve verification code after
				</view>
				<view class="num">
					{{countdownNum+'s'}}
				</view>
			</view>
			<view class="resend_text" v-if="isReSend" @click="getPhoneCode">
				Resend verification
			</view>
			<button class="submit_button" @click="nextClick" :disabled="isLoading" :style="{'opacity':isLoading?0.6:1}"
				:loading="isLoading">
				Next
			</button>
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
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		getCodeImg,
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
				isLoading: false,
				isReSend: false,
				inTimer: null,
				countdownNum: 60,
				basicInfo: {},
				styles: {
					color: '#fff',
					backgroundColor: '#161A26',
					disableColor: 'rgba(28, 34, 50, 1)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				yaoqingId: null, //代理商邀请码
				str: '' //链接上加上代理商邀请码
			};
		},
		onLoad(options) {
			this.str=''
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
				this.setCountdown()
			}
			if (options.yaoqingId) {
				this.yaoqingId = options.yaoqingId
				this.str += "&yaoqingId=" + this.yaoqingId
			}
		},
		onUnload() {
			clearInterval(this.inTimer)
		},
		destroyed() {
			clearInterval(this.inTimer)
		},
		methods: {
			setDaoji() {
				this.countdownNum = 0
			},
			setCodeNull() {
				//设置验证码为空
				this.basicInfo.mobileCode = null
			},
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
						// console.log("传值",{
						//   phone: this.inviteRegisterForm.mobile,
						//   code: this.imgCode,
						//   imgid: this.uuid
						// });
						if (res.code == 0) {
							this.reSendCode()
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
							this.reSendCode()
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
			reSendCode() {
				//重新发送验证码
				this.countdownNum = 60
				this.setCountdown()
			},
			setCountdown() {
				//设置倒计时
				this.isReSend = false
				this.inTimer = setInterval(() => {
					console.log("执行倒计时");
					if (this.countdownNum > 1) {
						this.countdownNum--
					} else {
						clearInterval(this.inTimer)
						this.isReSend = true
					}
				}, 1000)
			},
			completeInput(value) {
				console.log("输入的值", value);
				if (value && value.length == 6) {
					this.basicInfo.mobileCode = value
				} else {
					this.$refs.promptMsg.open('Please enter a six digit verification code', 2000)
					this.$refs.wolfPhoneInput.openKeyBoard()
				}
			},
			nextClick() {
				//跳转下一步
				console.log("weee");
				if (this.basicInfo.mobileCode) {
					let basicInfo=JSON.parse(JSON.stringify(this.basicInfo))
					delete basicInfo.code
					uni.navigateTo({
						url: '/page_register/register4?register=' + JSON.stringify(basicInfo) + this.str,
						success: (res)=>{
							// 通过eventChannel向被打开页面传送数据
							res.eventChannel.emit('acceptDataFromOpenerPage', {
								avatar: this.basicInfo.avatar,
								code:this.basicInfo.code,
								isAppReg:this.isAppReg
							})
						}
					})
				}
			}
		}
	}
</script>

<style lang="less" scoped>
	.code_con {
		width: 100%;
		margin-top: 28rpx;
	}

	.daojishi {
		font-size: 28rpx;
		color: rgba(255, 255, 255, 0.5);
		display: flex;
		align-items: center;
		justify-content: center;
		margin-top: 40rpx;

		.num {
			color: rgba(255, 255, 255, 1);
			margin-left: 10rpx;
		}
	}

	.resend_text {
		font-size: 28rpx;
		color: rgba(239, 169, 2, 1);
		text-decoration: underline;
		margin-top: 40rpx;
	}
</style>