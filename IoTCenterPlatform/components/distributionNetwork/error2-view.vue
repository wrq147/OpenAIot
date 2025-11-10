<template>
	<view class="error2_con container">
		<view class="error2_title">配网失败，自动进入Wifi配置ap模式</view>
		<view class="error2_tips">
			<text>1.使用手机开个热点,名称为<text style="color: red;font-size: 34rpx;margin-left: 10rpx;margin-right: 10rpx;">setmode</text>(全小写)，密码任意；</text>
		</view>
		<view class="error2_tips hasimg">
			<image class="images" src="/static/images/wifi_contact.png" mode=""></image>
		</view>
		<view class="error2_tips">
			<text>2.重启设备，等待出现<text style="color: red;font-size: 34rpx;margin-left: 10rpx;margin-right: 10rpx;">enter_set</text>的热点(手动查看手机wifi列表)</text>
		</view>
		<view class="error2_tips">
			<text>3.关闭个人<text style="color: red;font-size: 34rpx;margin-left: 10rpx;margin-right: 10rpx;">setmode</text>热点，点击重试</text>
		</view>
		<btn-group :buttons="[{ btnText: '重试', type: 'primary', id: 'restart' }]"
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