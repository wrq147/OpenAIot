<template>
	<view>
		<top title="Details" leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" rightWidth="157rpx"
			:isleftBack="true" backgroundColor="#161A26" :key="keyDate">
			<template v-slot:top_right>
				<view class="right_top right1" @click="UnbindDevice" v-if="isDeviceUser">
					<custom-icons iconsName="icon-jiebang" iconsSize="36rpx"></custom-icons>
				</view>
				<view class="right_top" @click="editDeviceInfo">
					<custom-icons iconsName="icon-bianji" iconsSize="36rpx"></custom-icons>
				</view>
			</template>
		</top>

		<uni-nav-bar :status-bar="false" :fixed="true" :border="false" :height="zhanweiHei" backgroundColor="#161A26">
			<template v-slot:allslot>
				<view class="details_top_con" id="detail_top" ref="detail_top">
					<view class="device_top">
						<view class="info_left" @click.stop="preViewImg(deviceBasicInfo.PhotoUrl)"
							v-if="deviceBasicInfo.PhotoUrl">
							<image class="image" :src="deviceBasicInfo.PhotoUrl+'?wh=500x500'" mode="aspectFill">
							</image>
						</view>
						<view class="info_left" @click.stop="preViewImg('../../static/device_default.png')" v-else>
							<image class="image" src="../../static/device_default.png" mode="aspectFill"></image>
						</view>
						<view class="info_right">
							<view class="info_title">
								{{deviceBasicInfo.Name}}
							</view>
							<view class="info_type">
								{{deviceBasicInfo.GroupName}}
							</view>
							<view class="info_status"
								:class="{'offline':deviceBasicInfo.Online==0,'unKnow':deviceBasicInfo.Online==2}">
								<view class="dot"></view>
								<view class="status_text">{{getOnlineInfo(deviceBasicInfo.Online)}}</view>
							</view>
						</view>
					</view>
					<view class="alarm_cont" v-if="isAlarm">
						<view class="alarm_left">
							<custom-icons iconsName="icon-baojing" iconsSize="28rpx"
								iconsColor="#FF3535"></custom-icons>
							<view class="text">
								<view class="text_top">
									Mechanical failure
								</view>
								<view class="text_bottom">
									2023-09-20 20:00:00
								</view>
							</view>
						</view>
						<view class="alarm_right">
							<custom-icons iconsName="icon-a-youjiantouhong" iconsSize="20rpx"
								iconsColor="#FF9A9A"></custom-icons>
						</view>
					</view>
					<view class="line_bor" v-if="!isAlarm"></view>
					<!-- <view class="margin_class"></view> -->
				</view>
			</template>
		</uni-nav-bar>
		<v-tabs v-model="current" :scroll="false" :tabs="tabArr" color="rgba(255, 255, 255, 0.5)"
			activeColor="rgba(255, 255, 255, 1)" :fixed="true" @change="changeTab" :lineScale="0.16" fontSize="28rpx"
			activeFontSize="28rpx" paddingItem="0" lineHeight="4rpx" :hasBorder="false" height="72rpx" padding="0"
			bgColor="rgba(22, 26, 38, 1)" lineColor="rgba(255, 255, 255, 1)" :zIndex="996" scrollPadding="0 0"
			scrollBgColor="rgba(22, 26, 38, 1)"></v-tabs>

		<view class="info_con">
			<device-basic v-show="current==0" :deviceBasicInfoObj="deviceBasicInfoObj" :deviceBasic="deviceBasicInfo"></device-basic>
			<view class="empty_con"
				v-if="current==0&&deviceBasicInfoObj.basic.value&&deviceBasicInfoObj.Tag.value&&deviceBasicInfoObj.basic.value.length==0&&deviceBasicInfoObj.Tag.value.length==0">
				<image class="image" src="/static/no_data.png" mode=""></image>
				<view class="text">There is currently no data available!</view>
			</view>
			<device-iot v-show="current==1" @setDeviceOnline='setDeviceOnline' :deviceId="deviceBasicInfo.DeviceId"
				:deviceBasic="deviceBasicInfo"></device-iot>
			<device-func v-show="current==2" :id="deviceId" :deviceBasic="deviceBasicInfo" :deviceId="deviceBasicInfo.DeviceId"></device-func>
			<device-alarm :ararmList="alarmList" v-show="current==3" :isShowDevice="false"
				:status="alarmStatus"></device-alarm>
		</view>
		<msg-prompt ref="promptMsg" @confirm="confirmUnbind"></msg-prompt>
		<!-- 提示信息组件 -->
	</view>
</template>

<script>
	import deviceBasic from '@/pages_device/device_info/components/device-basic.vue'
	import deviceIot from '@/pages_device/device_info/components/device-iot.vue'
	import deviceAlarm from '@/pages_device/device_info/components/device-alarm.vue'
	import deviceFunc from '@/pages_device/device_info/components/device-func.vue'
	import {
		crmDeviceInfo,
		crmDeviceTagInfo,
		crmDeviceProductInfo,
		warningList,
		UnUseDevice
	} from '@/api/device.js'
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		components: {
			deviceBasic,
			deviceIot,
			deviceAlarm,
			deviceFunc
		},
		data() {
			return {
				isAlarm: false,
				tabArr: ['Info', 'State', 'func', 'Alarm'],
				current: 0,
				zhanweiHei: 0,
				alarmList: ['Ordinary'],
				// ararmList: ['Ordinary', 'Urgent', 'Warning', 'Ordinary', 'Urgent', 'Warning'],
				zhanweiHeiNumber: 0,
				deviceId: 0, //设备id
				deviceProduct: 0, //设备产品id
				deviceBasicList: [],
				deviceBasicInfo: {},
				deviceBasicInfoObj: {
					basic: {
						name: 'Basic information',
						value: []
					},
					Tag: {
						name: 'Tag information',
						value: []
					}
				}, //设备基本信息整合
				alarmStatus: 'loading',
				alarmQuery: {
					pageNum: 1,
					pageSize: 30
				},
				isDeviceUser: false, //是否是设备使用者
				keyDate: '11111'
			};
		},
		async onLoad(options) {
			this.keyDate = new Date().getTime()
			if (this.$store.state.user && this.$store.state.user.uid) {} else {
				await this.$store.dispatch('GetInfo')
			}

			// this.$store.dispatch('GetInfo').then(() => {})
			this.$nextTick(() => {
				this.getTopNavHei()
			})
			if (options.id) {
				//设备id
				this.deviceId = options.id
				uni.showLoading({
					title: 'loading'
				})
				await this.getDeviceInfo()
				this.getDeviceTagInfo()
				this.getDeviceAlarm()
				uni.hideLoading()
			}
		},
		onReachBottom() { //上拉触底
			// console.log("现在的状态是什么", this.status);
			if (this.current == 3) {
				if (this.alarmStatus != 'noMore') {
					this.alarmQuery.pageNum++;
					this.alarmStatus = "loading";
					this.getWarningList();
					// console.log("现在是第几页", this.page);
				}
			}
		},
		methods: {
			editDeviceInfo() {
				//跳转至编辑页面
				uni.navigateTo({
					url: '/pages_device/edit_device?id=' + this.deviceId
				})
			},
			preViewImg(url) {
				let list = []
				list.push(url)
				uni.previewImage({
					current: 0,
					urls: list
				});

			},
			UnbindDevice() {
				//解绑设备
				if (this.isDeviceUser) {
					this.$refs.promptMsg.noticeOpen(
						'After unbinding, you will not be able to view the information of the device. Are you sure to unbind?'
					)
				}
			},
			confirmUnbind() {
				//确认解绑
				UnUseDevice({
					id: this.deviceBasicInfo.Id
				}).then(res => {
					// console.log("res设备使用解绑",res);
					this.$refs.promptMsg.open('Unbind successful', 2000)
					setTimeout(() => {
						setPagesParam('getDeviceList', 'unbind')
					}, 2050)
				}).catch(err => {
					this.setMsgTop(err)
				})
			},
			getDeviceAlarm() {
				//获取设备报警列表
				this.alarmQuery.DeviceId = this.deviceId;
				if (this.alarmQuery.pageNum == 1) {
					this.alarmList = [];
				}
				warningList(this.alarmQuery).then(
					async res => {
						// console.log("查询到的报警列表", res);
						this.alarmList = [...this.alarmList, ...res.data.List];
						if (res.data.List.length < this.alarmQuery.pageSize) {
							this.alarmStatus = 'noMore';
						} else {
							this.alarmStatus = 'more';
						}
					}
				);
			},
			getOnlineInfo(val) {
				//获取在线信息
				if (val == 0) {
					return 'offline'
				}
				if (val == 1) {
					return 'Online'
				}
				if (val == 2) {
					return 'unKnow'
				}
			},
			setDeviceOnline(val) {
				//设置设备在线信息
				this.deviceBasicInfo.Online = val
			},
			async getDeviceInfo() {
				//获取设备基本信息
				try {
					let res = await crmDeviceInfo({
						id: this.deviceId
					})
					// console.log("设备基本信息", res);

					this.deviceProduct = res.data.ProductId
					this.deviceBasicInfo = res.data
					if (!this.$store.state.user.orgId || !this.$store.state.user.uid) {
						await this.$store.dispatch('GetInfo')
					}
					if (this.$store.state.user.orgId == res.data.UseOrgId || this.$store.state.user.uid == res.data
						.UseUserId) {
						this.isDeviceUser = true
					} else {
						this.isDeviceUser = false
					}
					this.keyDate = new Date().getTime()
					this.getDeviceProductInfo()
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}

			},
			async getDeviceProductInfo() {
				//获取设备产品信息
				try {
					let rsp = await crmDeviceProductInfo({
						id: this.deviceBasicInfo.ProductId,
						notsl: true
					})
					// console.log("设备基本信息", rsp);
					let LastOnlinetime = ''
					if (this.deviceBasicInfo.Online == 0) {
						LastOnlinetime = this.deviceBasicInfo.LastOnline
					} else {
						if (this.deviceBasicInfo.Online == 1) {
							LastOnlinetime = 'Online'
						}
					}
					this.deviceBasicList = [{
						Name: 'Machine ID',
						DisplayValue: this.deviceBasicInfo.DeviceNumber
					}, {
						Name: 'Machine coding',
						DisplayValue: this.deviceBasicInfo.DeviceId
					}, {
						Name: 'Machine group',
						DisplayValue: this.deviceBasicInfo.GroupName
					}, {
						Name: 'Product name',
						DisplayValue: rsp.data.Name
					}, {
						Name: 'Product category',
						DisplayValue: rsp.data.ClassName
					}, {
						Name: 'Date of manufacture',
						DisplayValue: this.deviceBasicInfo.PD
					}, {
						Name: 'Creation time',
						DisplayValue: this.deviceBasicInfo.CreateOn
					}, {
						Name: 'Offline time',
						DisplayValue: LastOnlinetime,
					}, {
						Name: 'notes',
						DisplayValue: this.deviceBasicInfo.Remark,
					}]
					this.deviceBasicInfoObj.basic = {
						name: 'Basic information',
						type:1,
						value: this.deviceBasicList
					}
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}

			},
			getDeviceTagInfo() {
				//获取标签信息
				crmDeviceTagInfo({
					id: this.deviceId
				}).then(res => {
					// console.log("标签信息", res);
					this.deviceBasicInfoObj.Tag = {
						name: 'Tag information',
						type:2,
						value: res.data
					}
				}).catch(e => {
					this.setMsgTop(e)
				})
			},
			changeTab() {
				// tab切换

			},
			getTopNavHei() {
				//获取页面头部导航的高度
				// console.log("获取高度");

				// let dataHeight=this.$refs.detail_top.$el.clientHeight
				// // console.log("获取高度",this.$refs.detail_top,dataHeight * 2 + 'rpx');
				// this.zhanweiHei = dataHeight 
				// this.zhanweiHeiNumber = dataHeight * 2
				const query = uni.createSelectorQuery().in(this);
				query.select('#detail_top').boundingClientRect(data => {
					// console.log("得到布局位置信息", data, data.height - 10);
					data.height = data.height
					this.zhanweiHei = data.height
					this.zhanweiHeiNumber = data.height * 2
					// console.log("this.zhanweiHei", this.zhanweiHei, this.zhanweiHeiNumber);
					this.$forceUpdate()
				}).exec();

			},
		}
	}
</script>

<style lang="less" scoped>
	.empty_con {
		display: flex;
		justify-content: center;
		align-items: center;
		flex-direction: column;
		margin-top: 200rpx;

		.image {
			width: 246rpx;
			height: 200rpx;
		}

		.text {
			font-size: 28rpx;
			color: rgba(255, 255, 255, 0.5);
			margin-top: 60rpx;
		}
	}

	.details_top_con {
		padding: 0 20rpx;
		width: 100%;
		box-sizing: border-box;
		padding-top: 30rpx;
		background-color: #161A26;

		.device_top {
			// margin-top: 30rpx;
			display: flex;
			justify-content: flex-start;
			align-items: flex-start;
			color: #fff;

			.info_left {
				width: 150rpx;
				height: 150rpx;
				margin-right: 30rpx;

				.image {
					width: 150rpx;
					height: 150rpx;
				}
			}

			.info_right {
				.info_title {
					font-size: 36rpx;
					line-height: 48rpx;

				}

				.info_type {
					margin-top: 6rpx;
					font-size: 28rpx;
					line-height: 36rpx;
					color: rgba(255, 255, 255, 0.5);
				}

				.info_status {
					display: flex;
					justify-content: flex-start;
					align-items: center;
					font-size: 24rpx;

					.dot {
						width: 12rpx;
						height: 12rpx;
						border-radius: 7rpx;
						background-color: rgba(239, 169, 2, 1);
						margin-right: 10rpx;
					}

				}

				.info_status.offline {
					color: rgba(255, 255, 255, 0.5);

					.dot {
						background-color: rgba(255, 255, 255, 1);
					}
				}

				.info_status.unKnow {
					color: rgba(255, 255, 255, 0.2);

					.dot {
						background-color: rgba(99, 101, 122, 1);
					}
				}
			}

		}

		.alarm_cont {
			margin-top: 20rpx;
		}

		.line_bor {
			width: 100%;
			height: 1rpx;
			border-top: 1rpx solid rgba(255, 255, 255, 0.2);
			margin-top: 20rpx;
		}

		.margin_class {
			width: 100%;
			height: 20rpx;
			background-color: #161A26;
		}
	}

	.info_con {
		width: 100%;
		padding: 0 20rpx;
		box-sizing: border-box;
		padding-bottom: 50rpx;
		margin-top: 20rpx;
	}
</style>