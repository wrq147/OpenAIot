<template>
	<view class="pages_bgcon">
		<top :title="topTitle" leftWidth="157rpx" leftIcon="icon-fanhui" rightWidth="157rpx" :isleftBack="true"
			backgroundColor="rgba(255, 255, 255, 1)">
		</top>
		<uni-forms ref="deviceEditInfo" :modelValue="deviceEditInfo" :rules="deviceRules" label-position="top"
			:labelWidth="200">
			<view class="form_con">
				<uni-forms-item required name="photoUrl" id="photoUrl_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5" :showLabel="false" errorMsgLeft="calc(50% - 80rpx)">
					<view class="touxiang_con">
						<view class="touxiang">
							<image class="image" :src="getSerVerUrl()+'/appimg/device_default.png'" mode="aspectFill"
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
				<uni-forms-item label="设备名称" required name="name" id="name_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="name" class="form_li">
						<uni-easyinput placeholderStyle="color:#c1c1c1;font-size:32rpx" inputHeight="88rpx"
							:styles="styles" type="text" v-model="deviceEditInfo.name" placeholder="请输入企业名称"
							contentFontSize="32rpx" primaryColor="#2371FF" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="设备位置" name="AddressName" id="AddressName_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="AddressName" class="form_li" @click="mapChooseLocation()">
						<view class="form_input"
							:class="{'placeholder_input':!deviceEditInfo.AddressName||deviceEditInfo.AddressName==''}">
							{{deviceEditInfo.AddressName?deviceEditInfo.AddressName:'请选择位置'}}
						</view>
						<view class="form_sel_icon">
							<custom-icons iconsName="icon-weizhi" iconsSize="30rpx"
								iconsColor="#FF3535"></custom-icons>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="备注" name="remark" id="remark_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="remark" class="form_li">
						<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="70rpx"
							:styles="styles" type="textarea" v-model="deviceEditInfo.remark" placeholder="请输入备注"
							contentFontSize="32rpx" autoHeight :isCustom="true" />
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
	import {
		getGeocoder
	} from '@/api/weather.js'
	import serverUrl from '@/common/constVar.js'
	export default {
		data() {
			return {
				topTitle: '设备详情',
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
							errorMessage: "请输入设备名称"
						}]
					},
					photoUrl: {
						rules: [{
							required: true,
							errorMessage: "请上传设备图片"
						}]
					},
				},
				styles: {
					color: '#333',
					backgroundColor: '#F8F8F8',
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
			mapChooseLocation() {
				// #ifndef H5
				uni.chooseLocation({
					success: res => {
						// console.log("选择地址",res);
						let val=res
						this.deviceEditInfo.lat = val.latitude
						this.deviceEditInfo.lng = val.longitude,
						this.deviceEditInfo.AddressName = val.address
					},
					fail(err) {
						console.log("失败了",err);
					}
				})
				// #endif
				// #ifdef H5
				uni.navigateTo({
					url: '/pages_device/choose_address'
				})
				// #endif
			},
			confirm(val) {
				// console.log("已经选中的设备", val);
				this.deviceEditInfo.lat = val.location.lat
				this.deviceEditInfo.lng = val.location.lng,
				this.deviceEditInfo.AddressName = val.address
			},
			submit() {
				this.isLoading = true
				let submitForm=JSON.parse(JSON.stringify(this.deviceEditInfo))
				delete submitForm.AddressName
				editDevice(submitForm).then(res => {
					this.$refs.promptMsg.open('修改成功', 1500)
					this.$store.commit('SET_DEVICE_LIST', true)
					setTimeout(() => {
						setPagesParam('getDeviceInfo', 'edit')
					}, 1500)
				}).catch(err => {
					this.setMsgTop(err)
					this.isLoading = false
				})
			},
			async getDeviceInfo() {
				//获取设备基本信息
				try {
					let res = await crmDeviceInfo({
						id: this.deviceId
					})
					this.topTitle = res.data.DeviceNumber
					this.deviceEditInfo = {
						id: res.data.Id,
						name: res.data.Name,
						photoUrl: res.data.PhotoUrl,
						remark: res.data.Remark,
						lat: res.data.Lat,
						lng: res.data.Lng,
						AddressName: ''
					}
					if (this.deviceEditInfo.lat && this.deviceEditInfo.lng) {
						let abs = this.wgs84_to_bd09(this.deviceEditInfo.lat, this.deviceEditInfo.lng)
						let coderes = await getGeocoder({
							location: abs.bdLat + ',' + abs.bdLng,
							maptype: 'bd'
						})
						this.deviceEditInfo.AddressName = coderes.data.AddressName
					}


				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}

			},
			wgs84_to_bd09(lat, lng) {
				var x_pi = 3.14159265358979324 * 3000.0 / 180.0;
				var x = lng - 0.0065;
				var y = lat - 0.006;
				var z = Math.sqrt(x * x + y * y) - 0.00002 * Math.sin(y * x_pi);
				var theta = Math.atan2(y, x) - 0.000003 * Math.cos(x * x_pi);
				var bd_lng = z * Math.cos(theta) + 0.0065;
				var bd_lat = z * Math.sin(theta) + 0.006;
				return {
					bdLat: bd_lat,
					bdLng: bd_lng
				};
			},
			uploadMul(type) {
				//手动上传图片
				uni.chooseImage({
					count: 1,
					sizeType: ['compressed'], //可以指定是原图还是压缩图，默认二者都有
					success: async (res) => {
						this.$refs.promptMsg.loadingOpen('上传中...')
						if (res.tempFilePaths.length == 0) {
							return;
						}
						for (let i = 0; i < res.tempFiles.length; i++) {
							if (res.tempFiles[i].size > 10 * 1024 * 1024) {
								this.$refs.promptMsg.open('上传的图片不能超过10MB', 2000)
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
	.pages_bgcon {
		background-color: #FFFFFF;
	}

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


	.upload_li {
		font-size: 32rpx;
		color: #2371FF;
		text-decoration: underline;
		margin-top: 20rpx;
		margin-bottom: 20rpx;
		text-align: center;
	}
</style>