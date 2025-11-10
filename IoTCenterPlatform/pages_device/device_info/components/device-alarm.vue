<template>
	<view>
		<view class="alarm_list_con">
			<view class="alarm_li" v-for="row in ararmList" @click="toAlarmDetail(row)">
				<view class="alarm_name">
					<view class="dot" v-if="row.Status==0"></view>
					<view class="text">{{row.Name}}</view>
				</view>
				<view class="time_level">
					<view class="times">{{row.CreateOn}}</view>
					<view class="level">Alarm level: {{setLevel(row.Level)}}</view>
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
							:iconsColor="row.Status==1?'rgba(255, 255, 255, 0.5)':'#FF3535'"></custom-icons>
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
	import serverUrl from '@/common/constVar.js'
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
			isShowDevice: {
				type: Boolean,
				default: true
			},
			status: {
				type: String,
				default: "loading"
			}
		},
		data() {
			return {

			};
		},
		watch: {},
		mounted() {},
		methods: {
			returnImgUrl(row){
				return serverUrl.getServerUrl()+row.DevicePhotoUrl
			},
			toAlarmDetail(item){
				//跳转至报警详情
				uni.navigateTo({
					url:'/pages_device/alarm/alarm_detail?detail='+JSON.stringify(item)
				})
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

<style lang="scss" scoped>
	
</style>