<template>
	<view>
		<top title="Details" leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" rightWidth="157rpx" :isleftBack="true"
			backgroundColor="#161A26">
		</top>
		<uni-forms ref="deviceEditInfo" :modelValue="deviceEditInfo" :rules="deviceRules" label-position="top"
			:labelWidth="200">
			<view class="form_con">
				<uni-forms-item required name="photoUrl" id="photoUrl_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5" :showLabel="false" errorMsgLeft="calc(50% - 80rpx)">
					<view class="touxiang_con">
						<view class="touxiang">
							<image class="image" src="../static/device_default.png" mode="aspectFill"
								v-if="!deviceEditInfo.photoUrl"></image>
							<image class="image" :src="deviceEditInfo.photoUrl+'?wh=500x500'" mode="aspectFit"
								v-if="deviceEditInfo.photoUrl"></image>
							<view class="up_icons t-icon-shangchuantouxiang1" @click.stop="uploadMul"></view>
						</view>
					</view>
					<!-- <view class="upload_li" @click.stop="uploadMul">
						上传logo
					</view> -->
				</uni-forms-item>
				<uni-forms-item label="Device Name" required name="name" id="name_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="name" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx" inputHeight="88rpx"
							:styles="styles" type="text" v-model="deviceEditInfo.name" placeholder="请输入企业名称"
							contentFontSize="32rpx" primaryColor="rgba(255, 255, 255, 0.5)"/>
					</view>
				</uni-forms-item>
				<uni-forms-item label="notes" name="remark" id="remark_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="remark" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx" inputHeight="70rpx"
							:styles="styles" type="textarea" v-model="deviceEditInfo.remark" placeholder="请输入备注"
							contentFontSize="32rpx" autoHeight :isCustom="true" primaryColor="rgba(255, 255, 255, 0.5)"/>
					</view>
				</uni-forms-item>
				<button class="submit_button" @click="submit" :disabled="isLoading"
					:style="{'opacity':isLoading?0.6:1,'margin-top': '60rpx'}" :loading="isLoading">
					保存
				</button>
			</view>
		</uni-forms>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		crmDeviceInfo,
		editDevice
	} from '@/api/device.js'
	import {
		uploadPhoto,
		delPhoto
	} from '@/api/user.js'
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				isLoading: false,
				deviceEditInfo: {
					name: '',
					photoUrl: '',
					remark: ''
				},
				deviceId: 0,
				deviceRules: {
					name: {
						rules: [{
							required: true,
							errorMessage: "Please enter the device name"
						}]
					},
					photoUrl: {
						rules: [{
							required: true,
							errorMessage: "Please upload device images"
						}]
					},
				},
				styles: {
					color: '#fff',
					backgroundColor: '#161A26',
					disableColor: 'rgba(28, 34, 50, 1)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
			};
		},
		onLoad(options) {
			if (options.id) {
				this.deviceId = options.id
				this.getDeviceInfo()
			}
		},
		methods: {
			submit() {
				this.isLoading=true
				editDevice(this.deviceEditInfo).then(res => {
					console.log(res, '修改成功');
					this.$refs.promptMsg.open('Modified successfully', 1500)
					this.$store.commit('SET_DEVICE_LIST', true)
					setTimeout(() => {
						setPagesParam('getDeviceInfo', 'edit')
					}, 1500)
				}).catch(err => {
					this.setMsgTop(err)
					this.isLoading=false
				})
			},
			async getDeviceInfo() {
				//获取设备基本信息
				try {
					let res = await crmDeviceInfo({
						id: this.deviceId
					})
					console.log("设备基本信息", res);
					this.deviceEditInfo = {
						id:res.data.Id,
						name: res.data.Name,
						photoUrl: res.data.PhotoUrl,
						remark: res.data.Remark
					}

				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}

			},
			uploadMul(type) {
				//手动上传图片
				uni.chooseImage({
					count: 1,
					sizeType: ['compressed'], //可以指定是原图还是压缩图，默认二者都有
					success: async (res) => {
						this.$refs.promptMsg.loadingOpen('Uploading...')
						if (res.tempFilePaths.length == 0) {
							return;
						}
						for (let i = 0; i < res.tempFiles.length; i++) {
							if (res.tempFiles[i].size > 10 * 1024 * 1024) {
								this.$refs.promptMsg.open('The uploaded image cannot exceed 10MB', 2000)
								return;
							}
							try {
								let paths = res.tempFilePaths[i];
								let rsp = await uploadPhoto(paths)
								if (this.deviceEditInfo.photoUrl) {
									let rsp2 = await delPhoto(this.deviceEditInfo.photoUrl)
									// console.log("删除图片", rsp2);
								}
								this.deviceEditInfo.photoUrl = rsp
								this.$refs.promptMsg.loadingColse()
								this.$forceUpdate()
							} catch (e) {
								//TODO handle the exception
								this.$refs.promptMsg.loadingColse()
								// console.log(e);
								this.setMsgTop(e)
							}
						}
					}
				})
			},
		}
	}
</script>

<style lang="less">
	.touxiang_con {
		display: flex;
		justify-content: center;

		.touxiang {
			position: relative;
			// margin-top: 80rpx;
			display: flex;
			justify-content: center;
			width: 146rpx;
			height: 140rpx;
			border-radius: 10rpx;

			.image {
				width: 146rpx;
				height: 140rpx;
				border-radius: 10rpx;
			}

			.up_icons {
				width: 50rpx;
				height: 50rpx;
				position: absolute;
				right: -25rpx;
				bottom: -25rpx;
			}

		}
	}

</style>