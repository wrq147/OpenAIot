<template>
	<view class="schedule_reminder_con">
		<view class="schedule_reminder">
			<view class="line_title">
				<view class="left_text">
					<text>Schedule reminder</text>
				</view>
				<!-- <view class="plain">
					<custom-icons iconsName="icon-tianjia" iconsSize="18rpx"></custom-icons>
					<text class="text">Add plan</text>
				</view> -->
				<view class="line"></view>
			</view>
			<view class="empty">
				<custom-icons iconsName="icon-zanwujihua" iconsSize="54rpx" iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
				<view class="empty_text">
					No plan for the day
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	export default {
		name:"follow-swiper",//搜索主键
		data() {
			return {
				//轮播图相关属性
				swiperDotIndex: 0,
				info: [{}, {}, {}],
				mode: 'dot',
				dotsStyles: {
					bottom: 10,
					backgroundColor: 'rgba(120, 130, 157, 1)',
					border: '1px rgba(120, 130, 157, 1) solid',
					color: '#fff',
					selectedBackgroundColor: 'rgba(255, 53, 53, 1)',
					selectedBorder: '1px rgba(255, 53, 53, 1) solid'
				},
				indicatorDots: true,
				autoplay: false,
				interval: 2000,
				duration: 500,
				current: 0,
				//轮播图相关属性
			};
		},
		methods:{
		}
	}
</script>

<style lang="less" scoped>
	.schedule_reminder_con{
		width: 100%;
		// height: 302rpx;
		padding: 20rpx 20rpx 0 20rpx;
		box-sizing: border-box;
		.schedule_reminder{
			width: 100%;
			height: 234rpx;
			border: 1rpx solid rgba(255, 255, 255, 0.20);
			border-radius: 10rpx;
			box-sizing: border-box;
			padding: 30rpx;
			position: relative;
			.empty{
				margin-top: 28rpx;
			}
		}
	}
	
</style>