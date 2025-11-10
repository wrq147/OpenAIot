<template>
	<view class="register_con">
		<top title=" " leftWidth="157rpx" leftIcon="none" :isleftBack="false" backgroundColor="#ffffff"></top>
		<!-- <view class="title_text">欢迎来到{{appName}}!</view> -->
		<view class="touxiang" @click="getAvatarAfter">
			<view class="image t-icon-morentouxiang" v-if="!basicInfo.avatar"></view>
			<image class="image" :src="basicInfo.avatar" mode="aspectFit" v-if="basicInfo.avatar"></image>
			<view class="up_icons t-icon-shangchuantouxiang" @click.stop="uploadMul"></view>
		</view>
		<butongAvatar ref="cusAvatar" @getAvatar="getAvatar" :avatar="avatarGroup"></butongAvatar>
		<view class="register_form_con">
			<uni-forms ref="basicInfo" :modelValue="basicInfo" :rules="rules" label-position="top">
				<view class="form_con">
					<uni-forms-item label="昵称" required name="realName" id="realName_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5">
						<view id="realName" class="form_li">
							<uni-easyinput placeholderStyle="color:#999999;font-size:32rpx" inputHeight="88rpx"
								:styles="styles" type="text" v-model="basicInfo.realName" placeholder="请输入昵称"
								contentFontSize="32rpx" primaryColor="#2371FF" />
						</view>
					</uni-forms-item>
					<button class="submit_button" @click="nextClick" :disabled="isLoading"
						:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
						下一步
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
	} from '@/common/image-tools.js'
	import butongAvatar from '@/page_register/uni_modules/butong-avatar/components/butong-avatar/butong-avatar.vue'
	export default {
		components: {
			butongAvatar
		},
		data() {
			return {
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
							errorMessage: '请输入用户名',
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
				isFirstLoad: true
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
					let basicInfo = JSON.parse(JSON.stringify(this.basicInfo))
					delete basicInfo.code
					uni.navigateTo({
						url: '/page_register/register2?register=' + JSON.stringify(basicInfo) + str,
						success: (res) => {
							// 通过eventChannel向被打开页面传送数据
							res.eventChannel.emit('acceptDataFromOpenerPage', {
								avatar: this.basicInfo.avatar,
								code: this.basicInfo.code,
								isAppReg: this.isAppReg
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
		margin-top: 154rpx;

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
	}

	.register_form_con {
		margin-top: 80rpx;
	}
</style>