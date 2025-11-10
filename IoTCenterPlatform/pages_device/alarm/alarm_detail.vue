<template>
	<view>
		<top title="Details" leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" rightWidth="157rpx"
			:isleftBack="true" backgroundColor="#161A26"></top>
		<view class="detail_con">
			<view class="alarm_list_con basic_info_list">
				<view class="alarm_li">
					<view class="alarm_name">
						<view class="dot" v-if="alarmDetail.Status==0"></view>
						<view class="text">{{alarmDetail.Name}}</view>
					</view>
					<view class="time_level">
						<view class="times">{{alarmDetail.CreateOn}}</view>
						<view class="level">Alarm level: {{setLevel(alarmDetail.Level)}}</view>
					</view>
					<view class="alarm_device">
						<view class="device_left">
							<image class="images" :src="returnImgUrl()" mode=""></image>
						</view>
						<view class="device_right">
							<view class="device_name">{{alarmDetail.DeviceName}}</view>
							<view class="device_num">{{alarmDetail.DeviceId}}</view>
						</view>
					</view>
					<view class="alarm_content" :class="{'ordinary_ararm':alarmDetail.Status==1}" v-if="alarmDetail.Description">
						<view class="cont_icons">
							<custom-icons iconsName="icon-baojing" iconsSize="28rpx"
								:iconsColor="alarmDetail.Status==1?'rgba(255, 255, 255, 0.5)':'#FF3535'"></custom-icons>
						</view>
						<view class="detail_content">
							{{alarmDetail.Description}}
						</view>
					</view>
				</view>
				<view class="basic_info_li">
					<view class="li_label">EventID</view>
					<view class="li_val">{{alarmDetail.Code}}</view>
				</view>
				<!-- <view class="basic_info_li" v-if="alarmDetail.Description">
					<view class="li_label">Message content</view>
					<view class="li_val">{{alarmDetail.Description}}</view>
				</view> -->
			</view>
			<view class="basic_info_list" v-if="alarmDetail.Status==1&&alarmDetail.ClearUser">
				<view class="basic_info_li" style="border-top: none;">
					<view class="li_label">Processed by</view>
					<view class="li_val">
						<view class="icon_avatar">
							<image class="image" :src="alarmDetail.ClearUser.Avatar" mode=""></image>
						</view>
						<view class="val_text">
							{{alarmDetail.ClearUser.RealName}}
						</view>
					</view>
				</view>
				<view class="basic_info_li">
					<view class="li_label">Processing time</view>
					<view class="li_val">{{alarmDetail.ClearOn}}</view>
				</view>
				<view class="basic_info_li" v-if="alarmDetail.ClearRemark">
					<view class="li_label">Notes</view>
					<view class="li_val">{{alarmDetail.ClearRemark}}</view>
				</view>
			</view>
			<button class="submit_button" @click.stop="handAlarm" v-if="alarmDetail.Status==0">
				Handling alarms
			</button>
		</view>
		<uni-popup ref="clearAlarmPopup" type="center" :mask-click="false" :zIndex="999">
			<view class="notice_con">
				<view class="notice_cont">
					<view class="title">Handling alarms</view>
					<view class="title_content">
						Whether to filter the EventID {{alarmDetail.Name}}
					</view>
					<view class="title_content2">
						(Reminder: If filtering event identifiers, all alarms
						for that event identifier will be processed)
					</view>
					<view class="conten_icon_con">
						<view class="conten_icon t-icon-kaibeifen" v-if="clearForm.isFilterFun"
							@click.stop="setFunFilter(false)"></view>
						<view class="conten_icon t-icon-guan" v-if="!clearForm.isFilterFun"
							@click.stop="setFunFilter(true)"></view>
					</view>
					<view class="title_content" style="margin-top: 0;">
						Whether to filter the equipment {{alarmDetail.ProductName}}
					</view>
					<view class="title_content2">
						(Reminder: If the filtering device will process all
						alarms for that device)
					</view>
					<view class="conten_icon_con">
						<view class="conten_icon t-icon-kaibeifen" v-if="clearForm.isFilterDevice"
							@click.stop="setDeviceFilter(false)"></view>
						<view class="conten_icon t-icon-guan" v-if="!clearForm.isFilterDevice"
							@click.stop="setDeviceFilter(true)"></view>
					</view>
					<view class="title_content label">
						Notes
					</view>
					<view class="textarea_con">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="textarea" v-model="clearForm.mark"
							placeholder="Please enter the notes" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" :errorMessage="errorMessage" @focus="textFocus" />
					</view>
					<button class="submit_button" @click.stop="getWarnMsg">
						Confirm
					</button>
					<view class="close_icon" @click="noticeColse">
						<custom-icons iconsName="icon-guanbidanchuang" iconsSize="36rpx"
							iconsColor="rgba(255, 255, 255, 0.2)"></custom-icons>
					</view>
				</view>
			</view>
		</uni-popup>
		<msg-prompt ref="promptMsg" @confirm="confirm"></msg-prompt>
	</view>
</template>

<script>
	import {
		clearEachWarning,
		clearAllWarning
	} from '@/api/device.js'
	import {
		setPagesParam
	} from '@/common/utillib.js'
	import serverUrl from '@/common/constVar.js'
	export default {
		data() {
			return {
				alarmDetail: {},
				styles: {
					color: '#fff',
					backgroundColor: '#161A26',
					disableColor: 'rgba(28, 34, 50, 1)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				clearForm: {
					isFilterFun: false,
					isFilterDevice: false,
					mark: ""
				},
				warnFilter: {},
				errorMessage: ''
			}
		},
		onLoad(options) {
			if (options.detail) {
				// console.log("报警详情", JSON.parse(options.detail));
				this.alarmDetail = JSON.parse(options.detail)
			}
		},
		methods: {
			returnImgUrl(){
				return serverUrl.getServerUrl()+this.alarmDetail.DevicePhotoUrl
			},
			setFunFilter(val) {
				//是否过滤事件
				this.clearForm.isFilterFun = val
			},
			setDeviceFilter(val) {
				//是否过滤设备
				this.clearForm.isFilterDevice = val
			},
			handAlarm() {
				//处理报警
				this.$refs.clearAlarmPopup.open()
			},
			textFocus() {
				this.errorMessage = ''
			},
			getWarnMsg() {
				if (!this.clearForm.mark || this.clearForm.mark == null || this.clearForm.mark == '') {
					this.$refs.promptMsg.open('Please enter the notes', 2000)
					this.errorMessage = 'Please enter the notes'
					return
				}
				if (this.clearForm.isFilterDevice && this.clearForm.isFilterFun) {
					this.warnFilter = {
						devId: this.alarmDetail.DeviceId,
						code: this.alarmDetail.Code,
						remark: this.clearForm.mark
					};
					this.clearPartWarn(this.warnFilter);
				} else if (this.clearForm.isFilterDevice && !this.clearForm.isFilterFun) {
					this.warnFilter = {
						devId: this.alarmDetail.DeviceId,
						remark: this.clearForm.mark
					};
					this.clearPartWarn(this.warnFilter);
				} else if (!this.clearForm.isFilterDevice && this.clearForm.isFilterFun) {
					this.warnFilter = {
						code: this.alarmDetail.Code,
						remark: this.clearForm.mark
					};
					this.clearPartWarn(this.warnFilter);
				} else {
					this.warnFilter = {
						id: this.alarmDetail.Id,
						remark: this.clearForm.mark
					};
					this.clearPartWarn(this.warnFilter);
				}

			},
			confirm() {
				//处理提示确认
				this.$refs.clearAlarmPopup.close()
				if (this.warnFilter.id) {
					clearEachWarning(this.warnFilter).then(res => {
						// console.log(res, '处理报警');
						this.$refs.promptMsg.succossOpen()
						setPagesParam('getWarningList', 'detail')
					}).catch((err => {
						this.setMsgTop(err)
					}))
				} else {
					clearAllWarning(this.warnFilter).then(res => {
						// console.log(res, '处理报警');
						this.$refs.promptMsg.succossOpen()
						setPagesParam('getWarningList', 'detail')
					}).catch((err => {
						this.setMsgTop(err)
					}))
				}
			},
			clearPartWarn(filterParam) {
				//处理部分报警列表
				if (filterParam.id) {
					this.$refs.promptMsg.noticeOpen('Are you sure to process this alarm?', 'system prompt', true)
				} else {
					let text = ''
					if (filterParam.devId && filterParam.code) {
						text = 'Are all alarms related to processing equipment ' + this.alarmDetail.DeviceName +
							' and identifiers ' + this.alarmDetail.Name + ' confirmed'
					} else if (filterParam.devId) {
						text = 'Are you sure to handle all alarms related to the equipment ' + this.alarmDetail.DeviceName
					} else if (filterParam.code) {
						text = 'Are you sure to handle all alarms related to identifier ' + this.alarmDetail.Name
					}
					this.$refs.promptMsg.noticeOpen(text + '?', 'system prompt', true)
				}
			},
			noticeColse() {
				this.$refs.clearAlarmPopup.close()
			},
			setLevel(val) {
				//设置报警级别
				if (val == 0) {
					return 'Ordinary'
				}
				if (val == 1) {
					return 'Warning'
				}
				if (val == 2) {
					return 'Urgent'
				}
			}
		}
	}
</script>

<style lang="less" scoped>
	

	.alarm_list_con {
		padding: 0 30rpx;

		.alarm_li {
			padding: 30rpx 0;
		}
	}

	.submit_button {
		margin-top: 60rpx;
	}
</style>