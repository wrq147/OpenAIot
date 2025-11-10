<template>
	<view class="register_con">
		<view class="title_text">Welcome!</view>
		<view class="touxiang" @click="getAvatarAfter">
			<view class="image t-icon-morentouxiang" v-if="!basicInfo.avatar"></view>
			<image class="image" :src="basicInfo.avatar" mode="aspectFit" v-if="basicInfo.avatar"></image>
			<view class="up_icons t-icon-shangchuantouxiang" @click.stop="uploadMul"></view>
		</view>
		<butong-avatar ref="cusAvatar" @getAvatar="getAvatar" :avatar="avatarGroup"></butong-avatar>
		<view class="register_form_con">
			<uni-forms ref="basicInfo" :modelValue="basicInfo" :rules="rules" label-position="top">
				<view class="form_con">
					<uni-forms-item label="username" required name="realName" id="realName_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5">
						<view id="realName" class="form_li">
							<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
								inputHeight="88rpx" :styles="styles" type="text" v-model="basicInfo.realName"
								placeholder="Please enter your username" contentFontSize="32rpx"
								primaryColor="rgba(255, 255, 255, 0.5)" />
						</view>
					</uni-forms-item>
					<button class="submit_button" @click="nextClick" :disabled="isLoading"
						:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
						Next
					</button>
				</view>

			</uni-forms>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		pathToBase64,
		base64ToPath
	} from 'image-tools'
	export default {
		data() {
			return {
				localdata: [],
				basicInfo: {
					realName: "",
					password: "",
					readAgreement: [], //验证是否同意服务协议
					mobileCode: "",
					deptId: 0,
					mobile: "",
					company: "",
					avatar: "",
					postName: "",
					code: undefined //邀请码
				},
				avatarGroup: [],
				isLoading: false,
				rules: {
					realName: {
						rules: [{
							required: true,
							errorMessage: 'Please enter your username',
						}]
					},
				},
				styles: {
					color: '#fff',
					backgroundColor: '#161A26',
					disableColor: '#F7F6F6',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				yaoqingId: null,
				isAppReg: false, //是否是app直接注册
				isFirstLoad: true
			};
		},
		onLoad(options) {
			this.$nextTick(() => {
				this.isFirstLoad = true
				this.getAvatarAfter()
			})
			if (options.yaoqingId) {
				this.yaoqingId = options.yaoqingId
			}
			if (options.code) {
				this.basicInfo.code = options.code
			}
			if (options.isAppReg) {
				this.isAppReg = true
			}
		},
		methods: {
			nextClick() {
				//跳转下一步
				let str = ''
				if (this.yaoqingId) {
					str += "&yaoqingId=" + this.yaoqingId
				}
				if (this.isAppReg) {
					str += '&isAppReg=' + this.isAppReg
				}
				this.$refs.basicInfo.validate().then(res => {
					let basicInfo=JSON.parse(JSON.stringify(this.basicInfo))
					delete basicInfo.code
					uni.navigateTo({
						url: '/page_register/register2?register=' + JSON.stringify(basicInfo) + str,
						success: (res)=>{
							// 通过eventChannel向被打开页面传送数据
							res.eventChannel.emit('acceptDataFromOpenerPage', {
								avatar: this.basicInfo.avatar,
								code:this.basicInfo.code,
								isAppReg:this.isAppReg
							})
						}
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
	}

	.register_form_con {
		margin-top: 80rpx;
	}
</style>