<template>
	<view class="error2_con container">
		<view class="error2_title">Network distribution failed, automatically entering WiFi configuration AP mode</view>
		<view class="error2_tips">
			<text>1.Open a hotspot on your phone with the name <text style="color: red;font-size: 34rpx;margin-left: 10rpx;margin-right: 10rpx;">setmode</text> (all lowercase) and any password;</text>
		</view>
		<view class="error2_tips hasimg">
			<image class="images" src="/static/images/wifi_contact.png" mode=""></image>
		</view>
		<view class="error2_tips">
			<text>2.Restart the device and wait for the hotspot of <text style="color: red;font-size: 34rpx;margin-left: 10rpx;margin-right: 10rpx;">enter_set</text> to appear (manually viewing the mobile wifi list);</text>
		</view>
		<view class="error2_tips">
			<text>3.Close personal <text style="color: red;font-size: 34rpx;margin-left: 10rpx;margin-right: 10rpx;">setmode</text> hotspot, click retry.</text>
		</view>
		<btn-group :buttons="[{ btnText: 'Retry', type: 'primary', id: 'restart' }]"
			@onBottomButtonClick="onBottomButtonClick" :fixed-bottom="true" />
	</view>
</template>

<script>
	import btnGroup from '@/components/distributionNetwork/btn-group.vue'
	export default {
		name: "error-view",
		components: {
			btnGroup,
		},
		data() {
			return {

			};
		},
		methods: {
			onBottomButtonClick() {
				//配网失败重试
				this.$emit('tryAgain')
			}
		}
	}
</script>

<style lang="less" scoped>
	.error2_con{
		display: flex;
		flex-direction: column;
		flex: 1;
		background-color: #fff;
		padding: 72rpx 30rpx 152rpx;
		width: 100%;
		box-sizing: border-box;
		 .error2_title{
			 padding: 0 10rpx;
			 font-size: 40rpx;
			 font-weight: bold;
			 line-height: 40rpx;
		 }
		 .error2_tips{
			 margin-top: 21rpx;
			 padding: 0 10rpx;
			 font-size: 30rpx;
			 line-height: 48rpx;
			 &.hasimg{
				 padding: 0;
				 .images{
					 width:690rpx;
					 height: 660rpx;
				 }
			 }
		 }
	}
</style>