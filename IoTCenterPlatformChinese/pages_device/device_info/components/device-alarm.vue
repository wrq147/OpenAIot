<template>
	<view>
		<view class="alarm_list_con">
			<view class="errlist_con" @click="jumpErrList" v-if="isHasIotData&&!isShowDevice">
				<view class="errli_left">
					<custom-icons iconsName="icon-yichangbaogao" iconsSize="28rpx"
						iconsColor="#333"></custom-icons>
					<view class="text">
						异常报告
					</view>
				</view>
				<view class="err_right">
					<custom-icons iconsName="icon-a-youjiantouhong" iconsSize="20rpx"
						iconsColor="#999999"></custom-icons>
				</view>
			</view>
			<view class="alarm_li" v-for="row in ararmList" @click="toAlarmDetail(row)">
				<view class="alarm_name">
					<view class="dot" v-if="row.Status==0"></view>
					<view class="text">{{row.Name}}</view>
				</view>
				<view class="time_level">
					<view class="times">{{row.CreateOn}}</view>
					<view class="level">报警级别: {{setLevel(row.Level)}}</view>
				</view>
				<view class="alarm_device" v-if="isShowDevice">
					<view class="device_left">
						<image class="images" :src="returnImgUrl(row)" mode=""></image>
					</view>
					<view class="device_right">
						<view class="device_name">{{row.DeviceName}}</view>
						<view class="device_num">{{row.DeviceId}}</view>
					</view>
				</view>
				<view class="alarm_content" :class="{'ordinary_ararm':row.Status==1}" v-if="row.Description">
					<view class="cont_icons">
						<custom-icons iconsName="icon-baojing" iconsSize="28rpx"
							:iconsColor="row.Status==1?'#999999':'#FF3535'"></custom-icons>
					</view>
					<view class="content">
						{{row.Description}}
					</view>
				</view>
			</view>
			<uni-load-more iconType="circle" :status="status" v-if="status" />
		</view>
	</view>
</template>

<script>
	// #ifdef H5
	import serverUrl1 from '@/common/constVar.js'
	// #endif
	// #ifndef H5
	import serverUrl from '@/common/constVar.js'
	// #endif
	export default {
		name: "device-alarm",
		props: {
			lineColor: {
				type: String,
				default: "#FF3535"
			},
			value: {
				type: Number,
				default: 40
			},
			ararmList: {
				type: Array,
				default: () => {
					return []
				}
			},
			properties: {
				type: Array,
				default: () => {
					return []
				}
			},
			isShowDevice: {
				type: Boolean,
				default: true
			},
			isHasIotData: {
				type: Boolean,
				default: false
			},
			status: {
				type: String,
				default: "loading"
			},
			id: {
				type: [String,Number],
				default: ""
			},
		},
		data() {
			return {

			};
		},
		watch: {},
		mounted() {},
		methods: {
			jumpErrList(){
				//跳转至异常数据列表
				uni.navigateTo({
					url: '/pages_device/abnormal_data?id='+this.id,
					success: (res1) => {
						// 通过eventChannel向被打开页面传送数据
						res1.eventChannel.emit('acceptDataFromOpenerPage', {
							properties: this.properties,
						})
					}
				})
			},
			returnImgUrl(row) {
				let ser = ''
				// #ifdef H5
				ser = serverUrl1.getServerUrl()
				// #endif
				// #ifndef H5
				ser = serverUrl.getServerUrl()
				// #endif
				return ser + row.DevicePhotoUrl
			},
			toAlarmDetail(item) {
				//跳转至报警详情
				uni.navigateTo({
					url: '/pages_device/alarm/alarm_detail?detail=' + JSON.stringify(item)
				})
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

<style lang="scss" scoped>

</style>