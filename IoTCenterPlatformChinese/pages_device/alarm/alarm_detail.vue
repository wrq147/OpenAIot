<template>
	<view class="pages_bgcon">
		<top title="报警详情" leftWidth="157rpx" leftIcon="icon-fanhui" rightWidth="157rpx"
			:isleftBack="true" backgroundColor="#FFFFFF"></top>
			<uni-nav-bar :status-bar="false" :fixed="true" :border="false" :height="zhanweiHei" backgroundColor="rgba(255, 255, 255, 1)">
				<template v-slot:allslot>
					<view class="details_top_con" id="detail_top" style="padding-bottom: 30rpx;">
						<view class="device_top">
							<view class="info_left" @click.stop="preViewImg(deviceBasicInfo.PhotoUrl)" v-if="deviceBasicInfo.PhotoUrl">
								<image class="image" :src="deviceBasicInfo.PhotoUrl+'?wh=500x500'" mode="aspectFill"></image>
							</view>
							<view class="info_left" @click.stop="preViewImg(getSerVerUrl()+'/appimg/device_default.png')" v-else>
								<image v-if="getSerVerUrl()" class="image" :src="getSerVerUrl()+'/appimg/device_default.png'" mode="aspectFill"></image>
							</view>
							<view class="info_right">
								<view class="info_title">
									{{deviceBasicInfo.Name}}
								</view>
								<view class="info_type">
									<text class="group_name" v-if="deviceBasicInfo.GroupName">
										{{deviceBasicInfo.GroupName}}
									</text>
									<view class="line" v-if="deviceBasicInfo.GroupName&&deviceBasicInfo.ProductName"></view>
									<text class="group_name" v-if="deviceBasicInfo.ProductName">
										{{deviceBasicInfo.ProductName}}
									</text>
								</view>
								<view class="info_status" :class="{'offline':deviceBasicInfo.Online==0,'unKnow':deviceBasicInfo.Online==2}">
									<view class="dot" v-if="deviceBasicInfo.Online!=2"></view>
									<view class="status_text">{{getOnlineInfo(deviceBasicInfo.Online)}}</view>
								</view>
							</view>
						</view>
					</view>
				</template>
			</uni-nav-bar>
		<view class="detail_con" style="padding-top: 10rpx;">
			<view class="alarm_list_con basic_info_list">
				<view class="alarm_li">
					<view class="alarm_name">
						<view class="dot" v-if="alarmDetail.Status==0"></view>
						<view class="text">{{alarmDetail.Name}}</view>
					</view>
					<view class="time_level" style="padding-left: 0;">
						<view class="times">{{alarmDetail.CreateOn}}</view>
						<view class="level">报警等级: {{setLevel(alarmDetail.Level)}}</view>
					</view>
					<!-- <view class="alarm_device">
						<view class="device_left">
							<image class="images" :src="returnImgUrl()" mode=""></image>
						</view>
						<view class="device_right">
							<view class="device_name">{{alarmDetail.DeviceName}}</view>
							<view class="device_num">{{alarmDetail.DeviceId}}</view>
						</view>
					</view> -->
					<view class="alarm_content" :class="{'ordinary_ararm':alarmDetail.Status==1}" v-if="alarmDetail.Description">
						<view class="cont_icons">
							<custom-icons iconsName="icon-baojing" iconsSize="28rpx"
								:iconsColor="alarmDetail.Status==1?'#999999':'#FF3535'"></custom-icons>
						</view>
						<view class="detail_content">
							{{alarmDetail.Description}}
						</view>
					</view>
				</view>
				<view class="basic_info_li">
					<view class="li_label">事件标识</view>
					<view class="li_val">{{alarmDetail.Code}}</view>
				</view>
				<view class="basic_info_li">
					<view class="li_label">工单编号</view>
					<view class="li_val">{{alarmDetail.WarnNumber}}</view>
				</view>
				<!-- <view class="basic_info_li" v-if="alarmDetail.Description">
					<view class="li_label">Message content</view>
					<view class="li_val">{{alarmDetail.Description}}</view>
				</view> -->
			</view>
			<view class="basic_info_list" v-if="alarmDetail.Status==1&&alarmDetail.ClearUser">
				<view class="basic_info_li" style="border-top: none;">
					<view class="li_label">处理人</view>
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
					<view class="li_label">处理时间</view>
					<view class="li_val">{{alarmDetail.ClearOn}}</view>
				</view>
				<view class="basic_info_li" v-if="alarmDetail.ClearRemark">
					<view class="li_label">备注</view>
					<view class="li_val">{{alarmDetail.ClearRemark}}</view>
				</view>
			</view>
			<button class="submit_button" @click.stop="handAlarm" v-if="alarmDetail.Status==0">
				处理报警
			</button>
		</view>
		<uni-popup ref="clearAlarmPopup" type="center" :mask-click="false" :zIndex="999">
			<view class="notice_con">
				<view class="notice_cont">
					<view class="title">处理报警</view>
					<view class="title_content">
						是否过滤事件标识 {{alarmDetail.Name}}
					</view>
					<view class="title_content2">
						(提示：如果过滤事件标识，会清除该事件标识的所有报警信息)
					</view>
					<view class="conten_icon_con">
						<view class="conten_icon t-icon-kai" v-if="clearForm.isFilterFun"
							@click.stop="setFunFilter(false)"></view>
						<view class="conten_icon t-icon-guan1" v-if="!clearForm.isFilterFun"
							@click.stop="setFunFilter(true)"></view>
					</view>
					<view class="title_content" style="margin-top: 0;">
						是否过滤设备 {{alarmDetail.ProductName}}
					</view>
					<view class="title_content2">
						(提示：如果过滤设备，会清除该设备的所有报警信息)
					</view>
					<view class="conten_icon_con">
						<view class="conten_icon t-icon-kai" v-if="clearForm.isFilterDevice"
							@click.stop="setDeviceFilter(false)"></view>
						<view class="conten_icon t-icon-guan1" v-if="!clearForm.isFilterDevice"
							@click.stop="setDeviceFilter(true)"></view>
					</view>
					<view class="title_content label">
						备注
					</view>
					<view class="textarea_con">
						<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="textarea" v-model="clearForm.mark"
							placeholder="请输入备注内容" contentFontSize="32rpx"
							primaryColor="#2371FF" :errorMessage="errorMessage" @focus="textFocus" />
					</view>
					<button class="submit_button" @click.stop="getWarnMsg">
						确定
					</button>
					<view class="close_icon" @click="noticeColse">
						<custom-icons iconsName="icon-guanbidanchuang" iconsSize="36rpx"
							iconsColor="#999999"></custom-icons>
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
		clearAllWarning,
		crmDeviceInfo
	} from '@/api/device.js'
	import {
		setPagesParam
	} from '@/common/utillib.js'
	// #ifdef H5
	import serverUrl1 from '@/common/constVar.js'
	// #endif
	// #ifndef H5
	import serverUrl from '@/common/constVar.js'
	// #endif
	export default {
		data() {
			return {
				zhanweiHei:0,
				alarmDetail: {},
				deviceBasicInfo:{},
				styles: {
					color: '#333333',
					backgroundColor: '#F8F8F8',
					disableColor: 'rgba(28, 34, 50, 1)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				clearForm: {
					isFilterFun: false,
					isFilterDevice: false,
					mark: ""
				},
				warnFilter: {},
				errorMessage: '',
				deviceId:''
			}
		},
		onLoad(options) {
			if (options.detail) {
				this.alarmDetail = JSON.parse(options.detail)
				this.deviceId=this.alarmDetail.DeviceId
				this.getDeviceInfo()
			}
			setTimeout(() => {
				this.getTopNavHei()
			},300)
		},
		methods: {
			preViewImg(url) {//预览图片
				let list = []
				list.push(url)
				uni.previewImage({
					current: 0,
					urls: list
				});
			
			},
			getOnlineInfo(val) {
				//获取在线信息
				if (val == 0) {
					return '离线'
				}
				if (val == 1) {
					return '在线'
				}
				if (val == 2) {
					return '未知'
				}
			},
			async getDeviceInfo() {
				//获取设备基本信息
				try {
					let res = await crmDeviceInfo({
						id: this.deviceId
					})
					// console.log("设备基本信息", res);
					this.deviceBasicInfo = res.data
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}
			
			},
			getTopNavHei() {
				//获取页面头部导航的高度
				// console.log("获取高度");
				const query = uni.createSelectorQuery().in(this);
				query.select('#detail_top').boundingClientRect(data => {
					// console.log("得到布局位置信息", data, data.height - 10);
					data.height = data.height - 10
					this.zhanweiHei = data.height * 2 + 'rpx'
					this.zhanweiHeiNumber = data.height * 2
					// console.log("this.zhanweiHei", this.zhanweiHei, this.zhanweiHeiNumber);
					this.$forceUpdate()
				}).exec();
			
			},
			returnImgUrl(){
				let ser=''
				// #ifdef H5
				ser=serverUrl1.getServerUrl()
				// #endif
				// #ifndef H5
				ser=serverUrl.getServerUrl()
				// #endif
				return ser+this.alarmDetail.DevicePhotoUrl
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
					this.$refs.promptMsg.open('请输入备注内容', 2000)
					this.errorMessage = '请输入备注内容'
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
					this.$refs.promptMsg.noticeOpen('你确定要处理这个警报吗?', '系统提示', true)
				} else {
					let text = ''
					if (filterParam.devId && filterParam.code) {
						text = '是否确认处理所有与设备' + this.alarmDetail.DeviceName +
							'和事件标识' + this.alarmDetail.Name + '有关的报警'
					} else if (filterParam.devId) {
						text = '确定要处理与设备' + this.alarmDetail.DeviceName+'相关的所有警报吗?'
					} else if (filterParam.code) {
						text = '确定要处理所有与事件标识' + this.alarmDetail.Name+'相关的报警吗?'
					}
					this.$refs.promptMsg.noticeOpen(text + '?', '系统提示', true)
				}
			},
			noticeColse() {
				this.$refs.clearAlarmPopup.close()
			},
			setLevel(val) {
				//设置报警级别
				if (val == 0) {
					return '普通'
				}
				if (val == 1) {
					return '警告'
				}
				if (val == 2) {
					return '紧急'
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