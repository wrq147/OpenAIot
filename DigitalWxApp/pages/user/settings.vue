<template>
	<view class="wrap">
		<view class="row" style="margin: 30rpx auto;">
			<navigator url="account_manage">
				<text>账号管理</text>
				<uni-icons type="right" size="20" color="#333333"></uni-icons>
			</navigator>
		</view>
		<view class="row" style="display: none;">
			<navigator url="">
				<text>用户协议</text>
				<uni-icons type="right" size="20" color="#333333"></uni-icons>
			</navigator>
		</view>
		<view class="row" style="display: none;">
			<navigator url="">
				<text>隐私政策</text>
				<uni-icons type="right" size="20" color="#333333"></uni-icons>
			</navigator>
		</view>

	</view>
	<!-- <tabBar :pagePath="'/pages/user'"></tabBar> -->
</template>

<script>
	export default {
		data() {
			return {
				
			}
		},
		methods: {
			
		}
	}
</script>

<style lang="scss">
	.wrap{
		width: 100%;
		
		.row{
			width: 100%;
			background-color: #FFFFFF;
			border-bottom: 1px solid #F6F6F6;
			
			navigator{
				width: 100%;
				height: 120rpx;
				display: flex;
				justify-content: space-between;
				align-items: center;
				padding: 0 30rpx;
				font-size: 30rpx;
				color: #666666;
				box-sizing: border-box;
			}
		}
	}
</style>
